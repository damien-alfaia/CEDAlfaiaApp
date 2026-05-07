import { test, expect } from "@playwright/test";

const HAS_AUTH = !!(process.env.E2E_USER_EMAIL && process.env.E2E_USER_PASSWORD);

test.skip(!HAS_AUTH, "E2E_USER_EMAIL / E2E_USER_PASSWORD non définis — flow authentifié sauté.");

/**
 * Login partagé entre les tests de ce fichier (state réutilisé via storageState).
 */
test.beforeEach(async ({ page }) => {
  await page.goto("/login");
  await page.getByLabel(/email/i).fill(process.env.E2E_USER_EMAIL!);
  await page.getByLabel(/mot de passe/i).fill(process.env.E2E_USER_PASSWORD!);
  await page.getByRole("button", { name: /se connecter/i }).click();
  await page.waitForURL(/\/$/, { timeout: 10_000 });
});

test.describe("Flow client (golden path)", () => {
  test("Liste clients accessible et bouton Nouveau client visible", async ({ page }) => {
    await page.goto("/clients");
    await expect(page.getByRole("heading", { name: /clients/i })).toBeVisible();
    await expect(page.getByRole("link", { name: /nouveau client/i })).toBeVisible();
  });

  test("Créer un client puis le supprimer (soft delete)", async ({ page }) => {
    const nom = `E2E-${Date.now()}`;

    // Création
    await page.goto("/clients/new");
    await page.getByLabel(/^nom/i).fill(nom);
    await page.getByLabel(/^prénom/i).fill("Test");
    await page.getByLabel(/téléphone/i).fill("0612345678");
    await page.getByRole("button", { name: /créer le client/i }).click();
    await page.waitForURL(/\/clients\/\d+/, { timeout: 10_000 });

    // Vérification fiche
    await expect(page.getByRole("heading", { name: new RegExp(nom) })).toBeVisible();

    // Suppression (soft)
    await page.getByRole("button", { name: /^supprimer$/i }).first().click();
    // Confirmation dans le dialog
    await page
      .getByRole("button", { name: /confirmer la suppression/i })
      .click();
    await page.waitForURL(/\/clients/, { timeout: 10_000 });

    // Vérification : le client n'apparaît plus dans le filtre Actifs
    await page.goto("/clients?q=" + encodeURIComponent(nom));
    // soit "Aucun client à afficher", soit table vide
    await expect(page.getByText(/aucun client/i)).toBeVisible({ timeout: 5_000 });
  });

  test("Navigation depuis la sidebar fonctionne", async ({ page }) => {
    await page.goto("/");
    // Click sur "Clients" dans la sidebar
    await page.getByRole("link", { name: /^clients$/i }).first().click();
    await expect(page).toHaveURL(/\/clients/);

    await page.getByRole("link", { name: /^devis$/i }).first().click();
    await expect(page).toHaveURL(/\/devis/);

    await page.getByRole("link", { name: /^factures$/i }).first().click();
    await expect(page).toHaveURL(/\/factures/);
  });
});
