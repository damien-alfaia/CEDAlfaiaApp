import { requireProfile, displayName, initials, type Profile } from "@/lib/auth";
import { visibleSections } from "@/components/app/nav-config";
import { SidebarNav } from "@/components/app/sidebar-nav";
import { MobileSidebar } from "@/components/app/mobile-sidebar";
import { UserMenu } from "@/components/app/user-menu";
import { ThemeToggle } from "@/components/theme-toggle";

export default async function AppLayout({ children }: { children: React.ReactNode }) {
  const profile: Profile = await requireProfile();
  const sections = visibleSections(profile.role);

  return (
    <div className="flex min-h-screen bg-background text-foreground">
      {/* Sidebar desktop */}
      <aside className="hidden w-64 shrink-0 border-r border-border bg-card md:flex md:flex-col">
        <div className="flex h-14 items-center border-b border-border px-4">
          <span className="text-lg font-semibold tracking-tight">
            <span className="text-primary">TONI</span>
            <span>Auto</span>
            <span className="text-accent">App</span>
          </span>
        </div>
        <div className="flex-1 overflow-y-auto p-4">
          <SidebarNav sections={sections} />
        </div>
      </aside>

      {/* Main */}
      <div className="flex flex-1 flex-col">
        {/* Topbar */}
        <header className="flex h-14 items-center justify-between border-b border-border bg-card px-4 md:px-6">
          <div className="flex items-center gap-2">
            <MobileSidebar sections={sections} />
            <span className="text-base font-semibold tracking-tight md:hidden">
              <span className="text-primary">TONI</span>Auto
              <span className="text-accent">App</span>
            </span>
          </div>
          <div className="flex items-center gap-1">
            <ThemeToggle />
            <UserMenu
              name={displayName(profile)}
              email={profile.email}
              role={profile.role}
              initials={initials(profile)}
            />
          </div>
        </header>

        {/* Content */}
        <main className="flex-1 overflow-y-auto p-4 md:p-6">{children}</main>
      </div>
    </div>
  );
}
