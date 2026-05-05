import { ModuleStub } from "@/components/app/module-stub";
import { requireRole } from "@/lib/auth";

export default async function ParametragePage() {
  await requireRole(["super_admin", "administrateur"]);
  return (
    <ModuleStub
      title="Paramétrage"
      description="Entreprise, TVA, main d'œuvre, SMTP, logo."
      phase="Phase 6"
    />
  );
}
