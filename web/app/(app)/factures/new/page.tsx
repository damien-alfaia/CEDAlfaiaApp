import Link from "next/link";
import { ArrowLeft } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { PieceVenteForm } from "../../_components/piece-vente-form";
import { createPieceVenteAction } from "../../_actions/pieces-vente";
import { listClientsForSelect, listFournisseursActifs } from "@/lib/data/pieces-vente";

export default async function NewFacturePage() {
  const [clients, fournisseurs] = await Promise.all([
    listClientsForSelect(),
    listFournisseursActifs(),
  ]);
  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href="/factures">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour aux factures
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Nouvelle facture</h1>
        <p className="text-sm text-muted-foreground">
          Le numéro de facture sera généré automatiquement à l&apos;enregistrement.
        </p>
      </div>
      <PieceVenteForm
        mode="facture"
        clients={clients}
        fournisseurs={fournisseurs.map((f) => ({ id: f.id, label: f.nom }))}
        action={createPieceVenteAction}
        submitLabel="Créer la facture"
      />
    </div>
  );
}
