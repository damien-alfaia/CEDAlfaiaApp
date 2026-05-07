import Link from "next/link";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";

type Props = {
  page: number;
  pageCount: number;
  buildHref: (page: number) => string;
};

export function ListPagination({ page, pageCount, buildHref }: Props) {
  if (pageCount <= 1) return null;
  const prev = Math.max(1, page - 1);
  const next = Math.min(pageCount, page + 1);
  return (
    <div className="flex items-center justify-between gap-2 text-sm">
      <p className="text-neutral-500">
        Page {page} / {pageCount}
      </p>
      <div className="flex gap-2">
        <Button asChild variant="outline" size="sm" disabled={page === 1}>
          <Link href={buildHref(prev)}>
            <ChevronLeft className="mr-1 h-4 w-4" />
            Précédent
          </Link>
        </Button>
        <Button asChild variant="outline" size="sm" disabled={page === pageCount}>
          <Link href={buildHref(next)}>
            Suivant
            <ChevronRight className="ml-1 h-4 w-4" />
          </Link>
        </Button>
      </div>
    </div>
  );
}
