import { Document, Page, Text, View, StyleSheet } from "@react-pdf/renderer";
import type { PieceVenteFull, ParametrageRow } from "@/lib/db/types";
import { TYPE_PAIEMENT_LABEL } from "@/lib/format";

/**
 * Mise en page PDF pour devis ET facture — un seul composant paramétrable.
 * Iso-fonctionnel avec DevisXtraReport / FactureXtraReport de l'app actuelle :
 * en-tête entreprise (gauche) + bloc client (droite), tableau lignes/services,
 * totaux en bas, pied de page avec mention TVA et bas-de-page paramétrable.
 */

const styles = StyleSheet.create({
  page: {
    padding: 32,
    fontSize: 10,
    fontFamily: "Helvetica",
    color: "#111",
  },
  header: { flexDirection: "row", justifyContent: "space-between", marginBottom: 16 },
  brandBlock: { width: "55%" },
  brandName: { fontSize: 16, fontWeight: 700, marginBottom: 4 },
  small: { fontSize: 9, color: "#444" },
  clientBlock: {
    width: "40%",
    borderWidth: 1,
    borderColor: "#ccc",
    padding: 8,
  },
  clientLabel: { fontSize: 8, color: "#666", marginBottom: 4 },
  title: {
    fontSize: 22,
    fontWeight: 700,
    marginVertical: 12,
    textAlign: "center",
    letterSpacing: 1,
  },
  metaRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginBottom: 12,
    fontSize: 10,
  },
  table: { borderWidth: 1, borderColor: "#000", marginTop: 4 },
  trHead: {
    flexDirection: "row",
    backgroundColor: "#222",
    color: "#fff",
    paddingVertical: 4,
    paddingHorizontal: 6,
    fontWeight: 700,
    fontSize: 9,
  },
  tr: {
    flexDirection: "row",
    paddingVertical: 4,
    paddingHorizontal: 6,
    borderTopWidth: 1,
    borderTopColor: "#ccc",
    fontSize: 9,
  },
  trAlt: { backgroundColor: "#f7f7f7" },
  cell: { paddingHorizontal: 2 },
  totals: {
    marginTop: 12,
    flexDirection: "row",
    justifyContent: "flex-end",
  },
  totalsBox: {
    width: 220,
    borderWidth: 1,
    borderColor: "#000",
  },
  totalsRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingHorizontal: 8,
    paddingVertical: 4,
    fontSize: 10,
    borderBottomWidth: 1,
    borderBottomColor: "#ddd",
  },
  totalsTtc: {
    backgroundColor: "#222",
    color: "#fff",
    fontWeight: 700,
  },
  paiements: {
    marginTop: 16,
    fontSize: 9,
  },
  footer: {
    position: "absolute",
    bottom: 24,
    left: 32,
    right: 32,
    textAlign: "center",
    fontSize: 8,
    color: "#666",
    borderTopWidth: 1,
    borderTopColor: "#ddd",
    paddingTop: 6,
  },
});

function formatNumero(n: number | null | undefined) {
  if (n == null) return "—";
  return n.toString().padStart(8, "0");
}

function formatDate(s: string | null | undefined) {
  if (!s) return "—";
  const d = new Date(s);
  if (Number.isNaN(d.getTime())) return s;
  return d.toLocaleDateString("fr-FR");
}

function fmt(n: number) {
  return new Intl.NumberFormat("fr-FR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(n);
}

type Props = {
  piece: PieceVenteFull;
  parametrage: ParametrageRow | null;
  variant: "devis" | "facture";
};

export function PieceVentePdf({ piece, parametrage, variant }: Props) {
  const titre = variant === "devis" ? "DEVIS" : "FACTURE";
  const numero =
    variant === "devis" ? formatNumero(piece.num_devis) : formatNumero(piece.num_facture);
  const date = variant === "devis" ? piece.date_devis : piece.date_facture;

  const fullName = [piece.client?.prenom, piece.client?.nom].filter(Boolean).join(" ").trim();
  const voitureLabel = [piece.voiture?.modele?.marque?.libelle, piece.voiture?.modele?.libelle]
    .filter(Boolean)
    .join(" ");

  const mo = piece.main_doeuvre_montant_horaire * piece.main_doeuvre_duree;

  // Lignes affichées : pièces + services + main d'œuvre comme dernière ligne.
  type Row = {
    libelle: string;
    quantite: number;
    pu_ttc: number;
    total_ttc: number;
  };
  const rows: Row[] = [
    ...piece.lignes.map((l) => ({
      libelle: l.libelle ?? "",
      quantite: l.quantite,
      pu_ttc: l.prix_client_ttc,
      total_ttc: l.prix_client_ttc * l.quantite,
    })),
    ...piece.services.map((s) => ({
      libelle: s.libelle ?? "",
      quantite: s.quantite,
      pu_ttc: s.prix_client_ttc,
      total_ttc: s.prix_client_ttc * s.quantite,
    })),
  ];
  if (mo > 0) {
    rows.push({
      libelle: `Main d'œuvre (${piece.main_doeuvre_duree} h)`,
      quantite: piece.main_doeuvre_duree,
      pu_ttc: piece.main_doeuvre_montant_horaire,
      total_ttc: mo,
    });
  }

  return (
    <Document>
      <Page size="A4" style={styles.page}>
        {/* Header */}
        <View style={styles.header}>
          <View style={styles.brandBlock}>
            <Text style={styles.brandName}>{parametrage?.nom_entreprise ?? "Garage"}</Text>
            {parametrage?.adresse_ligne1 && (
              <Text style={styles.small}>{parametrage.adresse_ligne1}</Text>
            )}
            {parametrage?.adresse_ligne2 && (
              <Text style={styles.small}>{parametrage.adresse_ligne2}</Text>
            )}
            <Text style={styles.small}>
              {[parametrage?.adresse_code_postal, parametrage?.adresse_ville]
                .filter(Boolean)
                .join(" ")}
            </Text>
            {parametrage?.telephone_entreprise && (
              <Text style={styles.small}>Tél : {parametrage.telephone_entreprise}</Text>
            )}
            {parametrage?.email_entreprise && (
              <Text style={styles.small}>{parametrage.email_entreprise}</Text>
            )}
            {parametrage?.siret && <Text style={styles.small}>SIRET : {parametrage.siret}</Text>}
            {parametrage?.tva_intra_communautaire && (
              <Text style={styles.small}>
                TVA intracom. : {parametrage.tva_intra_communautaire}
              </Text>
            )}
          </View>

          <View style={styles.clientBlock}>
            <Text style={styles.clientLabel}>CLIENT</Text>
            <Text style={{ fontWeight: 700 }}>{fullName || "—"}</Text>
            {piece.client?.adresse_ligne1 && <Text>{piece.client.adresse_ligne1}</Text>}
            {piece.client?.adresse_ligne2 && <Text>{piece.client.adresse_ligne2}</Text>}
            <Text>
              {[piece.client?.adresse_code_postal, piece.client?.adresse_ville]
                .filter(Boolean)
                .join(" ")}
            </Text>
            {piece.client?.telephone && <Text>Tél : {piece.client.telephone}</Text>}
          </View>
        </View>

        <Text style={styles.title}>
          {titre} N° {numero}
        </Text>

        <View style={styles.metaRow}>
          <Text>Date : {formatDate(date)}</Text>
          {voitureLabel && (
            <Text>
              Véhicule : {voitureLabel}
              {piece.voiture?.immatriculation ? ` — ${piece.voiture.immatriculation}` : ""}
            </Text>
          )}
          {piece.kilometrage > 0 && <Text>Km : {piece.kilometrage}</Text>}
        </View>

        {/* Lignes */}
        <View style={styles.table}>
          <View style={styles.trHead}>
            <Text style={[styles.cell, { width: "55%" }]}>Désignation</Text>
            <Text style={[styles.cell, { width: "10%", textAlign: "right" }]}>Qté</Text>
            <Text style={[styles.cell, { width: "17%", textAlign: "right" }]}>PU TTC</Text>
            <Text style={[styles.cell, { width: "18%", textAlign: "right" }]}>Total TTC</Text>
          </View>
          {rows.length === 0 ? (
            <View style={styles.tr}>
              <Text style={[styles.cell, { width: "100%", color: "#999" }]}>Aucune ligne.</Text>
            </View>
          ) : (
            rows.map((r, i) => (
              <View key={i} style={[styles.tr, i % 2 === 1 ? styles.trAlt : {}]}>
                <Text style={[styles.cell, { width: "55%" }]}>{r.libelle}</Text>
                <Text style={[styles.cell, { width: "10%", textAlign: "right" }]}>
                  {r.quantite}
                </Text>
                <Text style={[styles.cell, { width: "17%", textAlign: "right" }]}>
                  {fmt(r.pu_ttc)} €
                </Text>
                <Text style={[styles.cell, { width: "18%", textAlign: "right" }]}>
                  {fmt(r.total_ttc)} €
                </Text>
              </View>
            ))
          )}
        </View>

        {/* Totaux */}
        <View style={styles.totals}>
          <View style={styles.totalsBox}>
            <View style={styles.totalsRow}>
              <Text>Total HT</Text>
              <Text>{fmt(piece.total_ht)} €</Text>
            </View>
            <View style={styles.totalsRow}>
              <Text>TVA</Text>
              <Text>{fmt(piece.montant_tva)} €</Text>
            </View>
            {piece.remise && piece.remise > 0 ? (
              <View style={styles.totalsRow}>
                <Text>Remise</Text>
                <Text>{(piece.remise * 100).toFixed(2)} %</Text>
              </View>
            ) : null}
            <View style={[styles.totalsRow, styles.totalsTtc]}>
              <Text>Total TTC</Text>
              <Text>{fmt(piece.total_ttc)} €</Text>
            </View>
            {variant === "facture" && (
              <View style={styles.totalsRow}>
                <Text>Reste à payer</Text>
                <Text>{fmt(piece.reste_a_payer)} €</Text>
              </View>
            )}
          </View>
        </View>

        {/* Paiements (factures uniquement) */}
        {variant === "facture" && piece.paiements.length > 0 && (
          <View style={styles.paiements}>
            <Text style={{ fontWeight: 700, marginBottom: 4 }}>Paiements</Text>
            {piece.paiements.map((p, i) => (
              <Text key={i}>
                {formatDate(p.date)} — {TYPE_PAIEMENT_LABEL[p.type_paiement] ?? p.type_paiement} —{" "}
                {fmt(p.montant)} €
              </Text>
            ))}
          </View>
        )}

        {/* Footer */}
        <Text style={styles.footer} fixed>
          {parametrage?.libelle_bas_de_page ?? ""}
        </Text>
      </Page>
    </Document>
  );
}
