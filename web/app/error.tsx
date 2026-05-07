"use client";

import { useEffect } from "react";
import { Button } from "@/components/ui/button";

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // Hook to a real error tracker (Sentry, etc.) when ready.
    console.error(error);
  }, [error]);

  return (
    <main className="flex min-h-screen items-center justify-center p-8">
      <div className="max-w-md space-y-4 text-center">
        <p className="text-sm font-semibold uppercase tracking-wider text-red-600">Erreur</p>
        <h1 className="text-2xl font-bold">Quelque chose s&apos;est mal passé</h1>
        <p className="text-sm text-muted-foreground">
          Une erreur inattendue est survenue. Si le problème persiste, contactez le support.
        </p>
        {error.digest && (
          <p className="font-mono text-xs text-muted-foreground">Réf : {error.digest}</p>
        )}
        <div className="flex justify-center gap-2">
          <Button onClick={reset}>Réessayer</Button>
        </div>
      </div>
    </main>
  );
}
