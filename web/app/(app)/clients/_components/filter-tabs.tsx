import Link from "next/link";
import { cn } from "@/lib/utils";

type Props = {
  current: string;
  prospect: string;
  q?: string;
};

const filterTabs: { key: string; label: string }[] = [
  { key: "actif", label: "Actifs" },
  { key: "supprime", label: "Supprimés" },
  { key: "all", label: "Tous" },
];

const prospectTabs: { key: string; label: string }[] = [
  { key: "tous", label: "Tous" },
  { key: "prospect", label: "Prospects" },
  { key: "client", label: "Clients" },
];

function buildHref({ filter, prospect, q }: { filter: string; prospect: string; q?: string }) {
  const sp = new URLSearchParams();
  if (q) sp.set("q", q);
  if (filter !== "actif") sp.set("filter", filter);
  if (prospect !== "tous") sp.set("prospect", prospect);
  const qs = sp.toString();
  return qs ? `/clients?${qs}` : "/clients";
}

export function FilterTabs({ current, prospect, q }: Props) {
  return (
    <div className="flex flex-wrap items-center gap-2 text-sm">
      <ButtonGroup label="Statut">
        {filterTabs.map((t) => (
          <Link
            key={t.key}
            href={buildHref({ filter: t.key, prospect, q })}
            className={cn(
              "rounded-md px-3 py-1.5",
              current === t.key
                ? "bg-primary text-primary-foreground"
                : "text-foreground hover:bg-muted",
            )}
          >
            {t.label}
          </Link>
        ))}
      </ButtonGroup>
      <ButtonGroup label="Type">
        {prospectTabs.map((t) => (
          <Link
            key={t.key}
            href={buildHref({ filter: current, prospect: t.key, q })}
            className={cn(
              "rounded-md px-3 py-1.5",
              prospect === t.key
                ? "bg-primary text-primary-foreground"
                : "text-foreground hover:bg-muted",
            )}
          >
            {t.label}
          </Link>
        ))}
      </ButtonGroup>
    </div>
  );
}

function ButtonGroup({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex items-center gap-1 rounded-md border bg-card p-1">
      <span className="px-2 text-xs uppercase tracking-wider text-muted-foreground">{label}</span>
      {children}
    </div>
  );
}
