import { ModuleStub } from "@/components/app/module-stub";
import { requireRole } from "@/lib/auth";

export default async function SalariesPage() {
  await requireRole(["super_admin", "administrateur"]);
  return (
    <ModuleStub
      title="Salariés"
      description="Personnel, contrats, indisponibilités et salaires."
      phase="Phase 6"
    />
  );
}
