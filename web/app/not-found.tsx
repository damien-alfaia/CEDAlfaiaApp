import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function NotFound() {
  return (
    <main className="flex min-h-screen items-center justify-center p-8">
      <div className="max-w-md space-y-4 text-center">
        <p className="text-sm font-semibold uppercase tracking-wider text-neutral-500">404</p>
        <h1 className="text-2xl font-bold">Page introuvable</h1>
        <p className="text-sm text-neutral-500">
          La page que vous cherchez n&apos;existe pas ou a été déplacée.
        </p>
        <Button asChild>
          <Link href="/">Retour au tableau de bord</Link>
        </Button>
      </div>
    </main>
  );
}
