import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function ForbiddenPage() {
  return (
    <main className="flex min-h-screen items-center justify-center p-8">
      <div className="max-w-md space-y-4 text-center">
        <p className="text-sm font-semibold uppercase tracking-wider text-amber-600">403</p>
        <h1 className="text-2xl font-bold">Accès refusé</h1>
        <p className="text-sm text-neutral-500">
          Vous n&apos;avez pas les permissions nécessaires pour accéder à cette page.
        </p>
        <Button asChild>
          <Link href="/">Retour au tableau de bord</Link>
        </Button>
      </div>
    </main>
  );
}
