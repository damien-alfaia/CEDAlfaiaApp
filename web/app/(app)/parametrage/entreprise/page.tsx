import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { requireRole } from "@/lib/auth";
import { getParametrage } from "@/lib/data/pieces-vente";
import { ParametrageForm } from "./parametrage-form";
import { saveParametrageAction } from "./actions";

export default async function ParametrageEntreprisePage() {
  await requireRole(["super_admin", "administrateur"]);
  const parametrage = await getParametrage();
  return (
    <div className="space-y-6">
      <div>
        <Button asChild variant="ghost" size="sm">
          <Link href="/parametrage">
            <ArrowLeft className="mr-1 h-4 w-4" />
            Retour
          </Link>
        </Button>
      </div>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Entreprise &amp; SMTP</h1>
        <p className="text-sm text-neutral-500">
          Ces paramètres sont utilisés en entête des PDFs de devis et factures.
        </p>
      </div>
      <ParametrageForm parametrage={parametrage} action={saveParametrageAction} />
    </div>
  );
}
