import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ClientForm } from "../_components/client-form";
import { createClientAction } from "../actions";

export default function NewClientPage() {
  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <Button asChild variant="ghost" size="sm">
          <Link href="/clients">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Nouveau client</h1>
        <p className="text-sm text-neutral-500">
          Renseignez au minimum le nom. Les autres champs sont optionnels.
        </p>
      </div>
      <ClientForm action={createClientAction} submitLabel="Créer le client" />
    </div>
  );
}
