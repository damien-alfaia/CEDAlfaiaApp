"use client";

/**
 * Barrel re-export "use client" pour lucide-react.
 *
 * Pourquoi ce fichier existe :
 * Les icônes lucide sont des composants `forwardRef`. En Next 16 / React 19,
 * un Server Component qui crée un élément JSX comme <Send /> et le passe
 * comme prop à un Client Component (par ex. via `icon: ReactNode`)
 * provoque une erreur de sérialisation RSC :
 *   "Functions cannot be passed directly to Client Components"
 *   { $$typeof, render: function, displayName }   ← le `render` du forwardRef.
 *
 * En re-exportant les icônes depuis un module marqué `"use client"`, Next
 * les traite comme des "client references" sérialisables. Importer les
 * icônes depuis `@/components/icons` au lieu de `lucide-react` règle le
 * problème pour tous les Server Components.
 *
 * Les Client Components peuvent continuer à importer directement
 * `lucide-react` (aucune frontière franchie), mais on uniformise tout vers
 * ce barrel pour éviter la confusion.
 */

export {
  // Navigation principale
  LayoutDashboard,
  Users,
  FileText,
  Receipt,
  Truck,
  CalendarDays,
  Wrench,
  Calculator,
  Settings,
  // Actions / décors UI
  ArchiveRestore,
  ArrowLeft,
  ArrowRight,
  Check,
  ChevronLeft,
  ChevronRight,
  Cog,
  Construction,
  Database,
  FileDown,
  LogOut,
  Mail,
  Menu,
  Pencil,
  Plus,
  RotateCcw,
  Search,
  Send,
  Star,
  Sun,
  Moon,
  Monitor,
  Trash2,
  X,
} from "lucide-react";

export type { LucideIcon } from "lucide-react";
