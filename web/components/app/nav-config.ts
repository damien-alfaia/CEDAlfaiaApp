import {
  LayoutDashboard,
  Users,
  FileText,
  Receipt,
  Truck,
  CalendarDays,
  Wrench,
  Calculator,
  Settings,
  type LucideIcon,
} from "@/components/icons";
import type { UserRole } from "@/lib/auth";

export type NavItem = {
  label: string;
  href: string;
  icon: LucideIcon;
  /** If undefined, all roles see the item. Otherwise restricted to listed roles. */
  roles?: UserRole[];
};

export type NavSection = {
  label?: string;
  items: NavItem[];
};

export const navSections: NavSection[] = [
  {
    items: [{ label: "Tableau de bord", href: "/", icon: LayoutDashboard }],
  },
  {
    label: "Activité",
    items: [
      { label: "Clients", href: "/clients", icon: Users },
      { label: "Devis", href: "/devis", icon: FileText },
      { label: "Factures", href: "/factures", icon: Receipt },
      { label: "Fournisseurs", href: "/fournisseurs", icon: Truck },
      { label: "Agenda", href: "/agenda", icon: CalendarDays },
    ],
  },
  {
    label: "Administration",
    items: [
      {
        label: "Salariés",
        href: "/salaries",
        icon: Wrench,
        roles: ["super_admin", "administrateur"],
      },
      {
        label: "Comptabilité",
        href: "/comptabilite",
        icon: Calculator,
        roles: ["super_admin", "administrateur"],
      },
      {
        label: "Paramétrage",
        href: "/parametrage",
        icon: Settings,
        roles: ["super_admin", "administrateur"],
      },
    ],
  },
];

export function visibleSections(role: UserRole): NavSection[] {
  return navSections
    .map((section) => ({
      ...section,
      items: section.items.filter((item) => !item.roles || item.roles.includes(role)),
    }))
    .filter((section) => section.items.length > 0);
}
