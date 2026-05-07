import "server-only";
import { createClient } from "@/lib/supabase/server";

export type Kpis = {
  // Mois en cours
  nbDevisMois: number;
  nbFacturesMois: number;
  caTtcMois: number;
  beneficeTtcMois: number;
  // Année en cours
  caTtcAnnee: number;
  beneficeTtcAnnee: number;
  // Stocks
  resteAPayerTotal: number;
  nbFacturesAEnvoyerComptable: number;
  nbDevisNonEnvoyes: number;
};

export type CaParMois = {
  mois: string; // "2026-05"
  caTtc: number;
  beneficeTtc: number;
  nbFactures: number;
};

export type BalanceAgeeRow = {
  client_id: number;
  client_label: string;
  reste_total: number;
  jours_max: number;
};

function startOfMonth(d: Date) {
  return new Date(d.getFullYear(), d.getMonth(), 1, 0, 0, 0, 0);
}

function startOfYear(d: Date) {
  return new Date(d.getFullYear(), 0, 1, 0, 0, 0, 0);
}

export async function getKpis(): Promise<Kpis> {
  const supabase = await createClient();
  const now = new Date();
  const debutMois = startOfMonth(now).toISOString();
  const debutAnnee = startOfYear(now).toISOString();

  // Devis du mois
  const { count: nbDevisMois } = await supabase
    .from("pieces_vente")
    .select("id", { count: "exact", head: true })
    .is("date_facture", null)
    .gte("date_devis", debutMois);

  // Devis non envoyés
  const { count: nbDevisNonEnvoyes } = await supabase
    .from("pieces_vente")
    .select("id", { count: "exact", head: true })
    .is("date_facture", null)
    .eq("is_devis_envoye", false);

  // Factures non annulées du mois
  const { data: facturesMois } = await supabase
    .from("pieces_vente")
    .select("total_ttc, benefice_ttc")
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .gte("date_facture", debutMois);
  const nbFacturesMois = facturesMois?.length ?? 0;
  const caTtcMois = (facturesMois ?? []).reduce((a, r) => a + (r.total_ttc ?? 0), 0);
  const beneficeTtcMois = (facturesMois ?? []).reduce((a, r) => a + (r.benefice_ttc ?? 0), 0);

  // Année
  const { data: facturesAnnee } = await supabase
    .from("pieces_vente")
    .select("total_ttc, benefice_ttc")
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .gte("date_facture", debutAnnee);
  const caTtcAnnee = (facturesAnnee ?? []).reduce((a, r) => a + (r.total_ttc ?? 0), 0);
  const beneficeTtcAnnee = (facturesAnnee ?? []).reduce((a, r) => a + (r.benefice_ttc ?? 0), 0);

  // Reste à payer global (factures non annulées avec reste > 0)
  const { data: ouvertes } = await supabase
    .from("pieces_vente")
    .select("reste_a_payer")
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .gt("reste_a_payer", 0);
  const resteAPayerTotal = (ouvertes ?? []).reduce((a, r) => a + (r.reste_a_payer ?? 0), 0);

  // Factures à envoyer comptable
  const { count: nbFacturesAEnvoyerComptable } = await supabase
    .from("pieces_vente")
    .select("id", { count: "exact", head: true })
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .eq("is_valide", true)
    .eq("is_envoye_comptable", false);

  return {
    nbDevisMois: nbDevisMois ?? 0,
    nbFacturesMois,
    caTtcMois,
    beneficeTtcMois,
    caTtcAnnee,
    beneficeTtcAnnee,
    resteAPayerTotal,
    nbFacturesAEnvoyerComptable: nbFacturesAEnvoyerComptable ?? 0,
    nbDevisNonEnvoyes: nbDevisNonEnvoyes ?? 0,
  };
}

/**
 * CA TTC + bénéfice par mois sur N derniers mois (N=12 par défaut).
 * Calculé côté serveur en TS — on reste sous le seuil free Supabase et c'est plus
 * lisible qu'un raw SQL. Si la volumétrie augmente, on passera à une vue SQL.
 */
export async function getCaParMois(nbMois = 12): Promise<CaParMois[]> {
  const supabase = await createClient();
  const now = new Date();
  const start = new Date(now.getFullYear(), now.getMonth() - (nbMois - 1), 1);

  const { data, error } = await supabase
    .from("pieces_vente")
    .select("date_facture, total_ttc, benefice_ttc")
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .gte("date_facture", start.toISOString());
  if (error) throw new Error(`getCaParMois: ${error.message}`);

  const buckets: Record<string, CaParMois> = {};
  for (let i = 0; i < nbMois; i++) {
    const d = new Date(start.getFullYear(), start.getMonth() + i, 1);
    const k = `${d.getFullYear()}-${(d.getMonth() + 1).toString().padStart(2, "0")}`;
    buckets[k] = { mois: k, caTtc: 0, beneficeTtc: 0, nbFactures: 0 };
  }
  for (const r of data ?? []) {
    if (!r.date_facture) continue;
    const d = new Date(r.date_facture);
    const k = `${d.getFullYear()}-${(d.getMonth() + 1).toString().padStart(2, "0")}`;
    if (!buckets[k]) continue;
    buckets[k].caTtc += r.total_ttc ?? 0;
    buckets[k].beneficeTtc += r.benefice_ttc ?? 0;
    buckets[k].nbFactures += 1;
  }
  return Object.values(buckets);
}

/**
 * Balance âgée : pour chaque client ayant un reste à payer > 0, total ouvert
 * + ancienneté maximale (en jours) de la facture la plus ancienne non payée.
 */
export async function getBalanceAgee(): Promise<BalanceAgeeRow[]> {
  const supabase = await createClient();
  const { data, error } = await supabase
    .from("pieces_vente")
    .select("client_id, reste_a_payer, date_facture, client:clients ( nom, prenom )")
    .not("date_facture", "is", null)
    .eq("is_facture_annule", false)
    .gt("reste_a_payer", 0);
  if (error) throw new Error(`getBalanceAgee: ${error.message}`);

  type Row = {
    client_id: number | null;
    reste_a_payer: number;
    date_facture: string | null;
    client: { nom: string; prenom: string | null } | null;
  };
  const map = new Map<number, BalanceAgeeRow>();
  const now = Date.now();
  for (const r of (data ?? []) as unknown as Row[]) {
    if (r.client_id == null) continue;
    const days = r.date_facture
      ? Math.floor((now - new Date(r.date_facture).getTime()) / (1000 * 60 * 60 * 24))
      : 0;
    const label =
      [r.client?.prenom, r.client?.nom].filter(Boolean).join(" ").trim() ||
      `Client #${r.client_id}`;
    const cur = map.get(r.client_id);
    if (cur) {
      cur.reste_total += r.reste_a_payer ?? 0;
      cur.jours_max = Math.max(cur.jours_max, days);
    } else {
      map.set(r.client_id, {
        client_id: r.client_id,
        client_label: label,
        reste_total: r.reste_a_payer ?? 0,
        jours_max: days,
      });
    }
  }
  return Array.from(map.values()).sort((a, b) => b.reste_total - a.reste_total);
}
