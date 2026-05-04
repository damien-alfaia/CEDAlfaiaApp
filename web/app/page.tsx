export default function Home() {
  const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL;
  const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY;

  const envOk = Boolean(supabaseUrl && supabaseAnonKey);

  return (
    <main className="flex min-h-screen items-center justify-center p-8">
      <div className="max-w-xl space-y-6 rounded-lg border p-8 shadow-sm">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">CEDAlfaiaApp v3</h1>
          <p className="mt-2 text-sm text-neutral-500">
            Gestion garage automobile — Next.js + Supabase
          </p>
        </div>

        <div className="space-y-2">
          <h2 className="text-sm font-semibold uppercase tracking-wider text-neutral-500">
            Statut environnement
          </h2>
          <ul className="space-y-1 text-sm">
            <li>
              <span className={envOk ? "text-green-600" : "text-red-600"}>{envOk ? "✓" : "✗"}</span>{" "}
              Variables Supabase{" "}
              <code className="rounded bg-neutral-100 px-1.5 py-0.5 text-xs">
                NEXT_PUBLIC_SUPABASE_URL
              </code>{" "}
              +{" "}
              <code className="rounded bg-neutral-100 px-1.5 py-0.5 text-xs">
                NEXT_PUBLIC_SUPABASE_ANON_KEY
              </code>
            </li>
          </ul>
        </div>

        {!envOk && (
          <div className="rounded border border-amber-300 bg-amber-50 p-3 text-sm text-amber-900">
            Configurez <code>.env.local</code> à partir de <code>.env.example</code> avant de
            connecter à Supabase.
          </div>
        )}

        <p className="text-xs text-neutral-400">
          Phase 2 — setup. Auth et navigation arrivent en Phase 3.
        </p>
      </div>
    </main>
  );
}
