"use client";

import { useActionState } from "react";
import { signUp, type SignUpState } from "@/app/auth/actions";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Alert, AlertDescription } from "@/components/ui/alert";

const initial: SignUpState = {};

export function SignUpForm() {
  const [state, formAction, pending] = useActionState(signUp, initial);
  const fe = state.fieldErrors ?? {};

  if (state.success) {
    return (
      <Alert>
        <AlertDescription>
          {state.needsConfirmation
            ? "Compte créé. Un email de confirmation vous a été envoyé — cliquez sur le lien dedans avant de vous connecter."
            : "Compte créé. Vous pouvez maintenant vous connecter."}
        </AlertDescription>
      </Alert>
    );
  }

  return (
    <form action={formAction} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="email">Email</Label>
        <Input
          id="email"
          name="email"
          type="email"
          autoComplete="email"
          required
          disabled={pending}
          aria-invalid={Boolean(fe.email)}
        />
        {fe.email && <p className="text-xs text-red-600">{fe.email}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password">Mot de passe (8 caractères min)</Label>
        <Input
          id="password"
          name="password"
          type="password"
          autoComplete="new-password"
          required
          minLength={8}
          disabled={pending}
          aria-invalid={Boolean(fe.password)}
        />
        {fe.password && <p className="text-xs text-red-600">{fe.password}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="confirmPassword">Confirmer le mot de passe</Label>
        <Input
          id="confirmPassword"
          name="confirmPassword"
          type="password"
          autoComplete="new-password"
          required
          minLength={8}
          disabled={pending}
          aria-invalid={Boolean(fe.confirmPassword)}
        />
        {fe.confirmPassword && <p className="text-xs text-red-600">{fe.confirmPassword}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="code">Code d&apos;inscription</Label>
        <Input
          id="code"
          name="code"
          type="text"
          required
          disabled={pending}
          aria-invalid={Boolean(fe.code)}
          autoComplete="off"
        />
        {fe.code && <p className="text-xs text-red-600">{fe.code}</p>}
        <p className="text-xs text-muted-foreground">
          Le code vous a été communiqué par l&apos;administrateur.
        </p>
      </div>

      {state.error && (
        <Alert variant="destructive">
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      <Button type="submit" className="w-full" disabled={pending}>
        {pending ? "Création du compte…" : "Créer le compte"}
      </Button>
    </form>
  );
}
