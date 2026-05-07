import { test, expect } from "@playwright/test";

test.describe("Pages publiques", () => {
  test("La page /login affiche le formulaire de connexion", async ({ page }) => {
    await page.goto("/login");
    await expect(page.getByLabel(/email/i)).toBeVisible();
    await expect(page.getByLabel(/mot de passe/i)).toBeVisible();
    await expect(page.getByRole("button", { name: /se connecter/i })).toBeVisible();
    await expect(page.getByRole("link", { name: /s.inscrire/i })).toBeVisible();
  });

  test("La page /signup affiche le formulaire d'inscription", async ({ page }) => {
    await page.goto("/signup");
    await expect(page.getByLabel(/email/i)).toBeVisible();
    await expect(page.getByLabel(/mot de passe/i).first()).toBeVisible();
    await expect(page.getByLabel(/confirmer/i)).toBeVisible();
    await expect(page.getByLabel(/code d.inscription/i)).toBeVisible();
  });

  test("Login avec mauvaises credentials affiche une erreur", async ({ page }) => {
    await page.goto("/login");
    await page.getByLabel(/email/i).fill("nobody@example.com");
    await page.getByLabel(/mot de passe/i).fill("wrong-password-xyz");
    await page.getByRole("button", { name: /se connecter/i }).click();
    await expect(page.getByText(/email ou mot de passe incorrect/i)).toBeVisible({
      timeout: 10_000,
    });
  });

  test("Signup avec mauvais code affiche une erreur", async ({ page }) => {
    await page.goto("/signup");
    await page.getByLabel(/email/i).fill(`test-${Date.now()}@example.com`);
    await page.getByLabel(/mot de passe \(/i).fill("Test1234!");
    await page.getByLabel(/confirmer/i).fill("Test1234!");
    await page.getByLabel(/code d.inscription/i).fill("definitely-wrong-code");
    await page.getByRole("button", { name: /créer le compte/i }).click();
    await expect(page.getByText(/code/i)).toBeVisible({ timeout: 10_000 });
  });

  test("Une page protégée redirige vers /login si non authentifié", async ({ page }) => {
    const response = await page.goto("/clients");
    // Attendu : 307 vers /login. Playwright suit les redirections, on vérifie l'URL finale.
    await expect(page).toHaveURL(/\/login/);
    expect(response?.status()).toBeLessThan(400);
  });
});
