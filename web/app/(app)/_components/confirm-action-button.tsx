"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

type Props = {
  label: string;
  confirmTitle: string;
  confirmDescription: string;
  confirmCta: string;
  /** Server Action déclenchée à la confirmation. */
  action: () => Promise<void>;
  variant?: "default" | "outline" | "ghost" | "secondary" | "destructive";
  destructive?: boolean;
  icon?: React.ReactNode;
  size?: "default" | "sm" | "lg" | "icon";
};

export function ConfirmActionButton({
  label,
  confirmTitle,
  confirmDescription,
  confirmCta,
  action,
  variant = "outline",
  destructive,
  icon,
  size = "sm",
}: Props) {
  const [open, setOpen] = useState(false);
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button
          variant={variant}
          size={size}
          className={destructive ? "text-red-600 hover:text-red-700" : undefined}
        >
          {icon}
          {label}
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>{confirmTitle}</DialogTitle>
          <DialogDescription>{confirmDescription}</DialogDescription>
        </DialogHeader>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <DialogFooter>
          <Button variant="ghost" onClick={() => setOpen(false)} disabled={pending}>
            Annuler
          </Button>
          <Button
            variant={destructive ? "destructive" : "default"}
            disabled={pending}
            onClick={async () => {
              setPending(true);
              setError(null);
              try {
                await action();
                setOpen(false);
              } catch (e) {
                setError(e instanceof Error ? e.message : "Erreur");
              } finally {
                setPending(false);
              }
            }}
          >
            {pending ? "En cours…" : confirmCta}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
