import Link from "next/link";
import { ChevronRight, Cog, Database } from "@/components/icons";
import { Card, CardContent } from "@/components/ui/card";
import { requireRole } from "@/lib/auth";

export default async function ParametragePage() {
  await requireRole(["super_admin", "administrateur"]);

  const sections = [
    {
      href: "/parametrage/referentiels",
      icon: Database,
      title: "Marques & modèles",
      description: "Catalogue des marques et modèles utilisé pour les voitures clients.",
      ready: true,
    },
    {
      href: "/parametrage/entreprise",
      icon: Cog,
      title: "Entreprise & SMTP",
      description:
        "Coordonnées, TVA, montant horaire, mentions PDF et configuration SMTP de l'envoi.",
      ready: true,
    },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Paramétrage</h1>
        <p className="text-sm text-neutral-500">Configuration globale de l&apos;application.</p>
      </div>

      <div className="grid gap-3 md:grid-cols-2">
        {sections.map((s) => {
          const Icon = s.icon;
          if (!s.ready) {
            return (
              <Card key={s.href} className="opacity-60">
                <CardContent className="flex items-start gap-4 p-4">
                  <Icon className="mt-1 h-5 w-5 text-neutral-400" />
                  <div className="flex-1">
                    <p className="font-medium">{s.title}</p>
                    <p className="text-sm text-neutral-500">{s.description}</p>
                    <p className="mt-1 text-xs text-neutral-400">à venir en Phase 6</p>
                  </div>
                </CardContent>
              </Card>
            );
          }
          return (
            <Link key={s.href} href={s.href} className="group">
              <Card className="transition-colors hover:border-neutral-400">
                <CardContent className="flex items-start gap-4 p-4">
                  <Icon className="mt-1 h-5 w-5" />
                  <div className="flex-1">
                    <p className="font-medium">{s.title}</p>
                    <p className="text-sm text-neutral-500">{s.description}</p>
                  </div>
                  <ChevronRight className="h-4 w-4 self-center text-neutral-400 transition-transform group-hover:translate-x-0.5" />
                </CardContent>
              </Card>
            </Link>
          );
        })}
      </div>
    </div>
  );
}
