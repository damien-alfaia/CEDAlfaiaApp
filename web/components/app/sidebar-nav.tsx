"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import type { NavSection } from "./nav-config";
import { cn } from "@/lib/utils";

type Props = {
  sections: NavSection[];
  onNavigate?: () => void;
};

export function SidebarNav({ sections, onNavigate }: Props) {
  const pathname = usePathname();

  return (
    <nav className="flex flex-col gap-6">
      {sections.map((section, idx) => (
        <div key={idx} className="space-y-1">
          {section.label && (
            <h3 className="px-3 text-xs font-semibold uppercase tracking-wider text-muted-foreground">
              {section.label}
            </h3>
          )}
          <ul className="space-y-0.5">
            {section.items.map((item) => {
              const Icon = item.icon;
              const isActive =
                item.href === "/" ? pathname === "/" : pathname.startsWith(item.href);

              return (
                <li key={item.href}>
                  <Link
                    href={item.href}
                    onClick={onNavigate}
                    className={cn(
                      "flex items-center gap-3 rounded-md px-3 py-2 text-sm transition-colors",
                      isActive
                        ? "bg-primary text-primary-foreground"
                        : "text-foreground hover:bg-muted",
                    )}
                  >
                    <Icon className="h-4 w-4" />
                    <span>{item.label}</span>
                  </Link>
                </li>
              );
            })}
          </ul>
        </div>
      ))}
    </nav>
  );
}
