import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Construction } from "@/components/icons";

type Props = {
  title: string;
  description?: string;
  phase: string;
};

export function ModuleStub({ title, description, phase }: Props) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{title}</h1>
        {description && <p className="text-sm text-muted-foreground">{description}</p>}
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center gap-3">
            <Construction className="h-5 w-5 text-amber-500" />
            <div>
              <CardTitle>Module à venir</CardTitle>
              <CardDescription>Implémentation prévue en {phase}.</CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          La navigation et l&apos;auth sont en place ; le module métier sera développé
          ultérieurement selon la feuille de route.
        </CardContent>
      </Card>
    </div>
  );
}
