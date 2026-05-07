import Link from "next/link";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight } from "@/components/icons";

type Props = {
  page: number;
  pageCount: number;
  q?: string;
  filter?: string;
  prospect?: string;
};

function buildHref({
  page,
  q,
  filter,
  prospect,
}: {
  page: number;
  q?: string;
  filter?: string;
  prospect?: string;
}) {
  const sp = new URLSearchParams();
  if (q) sp.set("q", q);
  if (filter && filter !== "actif") sp.set("filter", filter);
  if (prospect && prospect !== "tous") sp.set("prospect", prospect);
  if (page > 1) sp.set("page", String(page));
  const qs = sp.toString();
  return qs ? `/clients?${qs}` : "/clients";
}

export function Pagination({ page, pageCount, q, filter, prospect }: Props) {
  if (pageCount <= 1) return null;
  const prev = Math.max(1, page - 1);
  const next = Math.min(pageCount, page + 1);
  return (
    <div className="flex items-center justify-between gap-2 text-sm">
      <p className="text-muted-foreground">
        Page {page} / {pageCount}
      </p>
      <div className="flex gap-2">
        <Button asChild variant="outline" size="sm" disabled={page === 1}>
          <Link href={buildHref({ page: prev, q, filter, prospect })}>
            <ChevronLeft className="mr-1 h-4 w-4" />
            Précédent
          </Link>
        </Button>
        <Button asChild variant="outline" size="sm" disabled={page === pageCount}>
          <Link href={buildHref({ page: next, q, filter, prospect })}>
            Suivant
            <ChevronRight className="ml-1 h-4 w-4" />
          </Link>
        </Button>
      </div>
    </div>
  );
}
