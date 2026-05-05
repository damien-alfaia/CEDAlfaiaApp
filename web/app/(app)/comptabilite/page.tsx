import { ModuleStub } from "@/components/app/module-stub";
import { requireRole } from "@/lib/auth";

export default async function ComptabilitePage() {
  await requireRole(["super_admin", "administrateur"]);
  return (
    <ModuleStub
      title="Comptabilité"
      description="Balance âgée, CA, marges, KPIs financiers."
      phase="Phase 6"
    />
  );
}
