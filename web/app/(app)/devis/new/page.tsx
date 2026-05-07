import Link from "next/link";
import { ArrowLeft } from "@/components/icons";
import { Button } from "@/components/ui/button";
import { PieceVenteForm } from "../../_components/piece-vente-form";
import { createPieceVenteAction } from "../../_actions/pieces-vente";
import { listClientsForSelect, listFournisseursActifs } from "@/lib/data/pieces-vente";

export default async function NewDevisPage() {
  const [clients, fournisseurs] = await Promise.all([
    listClientsForSelect(),
    listFournisseursActifs(),
  ]);
  const fournisseursOpt = fournisseurs.map((f) => ({ id: f.id, label: f.nom }));

  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href="/devis">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour aux devis
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Nouveau devis</h1>
        <p className="text-sm text-neutral-500">
          Le numéro de devis sera généré automatiquement à l&apos;enregistrement.
        </p>
      </div>
      <PieceVenteForm
        mode="devis"
        clients={clients}
        fournisseurs={fournisseursOpt}
        action={createPieceVenteAction}
        submitLabel="Créer le devis"
      />
    </div>
  );
}
