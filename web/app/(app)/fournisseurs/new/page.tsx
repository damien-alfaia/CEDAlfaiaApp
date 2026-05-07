import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { FournisseurForm } from "../_components/fournisseur-form";
import { createFournisseurAction } from "../actions";

export default function NewFournisseurPage() {
  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href="/fournisseurs">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Nouveau fournisseur</h1>
      </div>
      <FournisseurForm action={createFournisseurAction} submitLabel="Créer le fournisseur" />
    </div>
  );
}
