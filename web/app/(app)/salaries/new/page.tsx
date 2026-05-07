import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { requireRole } from "@/lib/auth";
import { SalarieForm } from "../_components/salarie-form";
import { createSalarieAction } from "../actions";

export default async function NewSalariePage() {
  await requireRole(["super_admin", "administrateur"]);
  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href="/salaries">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Nouveau salarié</h1>
      </div>
      <SalarieForm action={createSalarieAction} submitLabel="Créer le salarié" />
    </div>
  );
}
