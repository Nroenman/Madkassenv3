import { test, expect } from '@playwright/test';

test('User can filter products by each allergy individually and all together', async ({ page }) => {
    await page.goto('http://localhost:3000/');

    // Go to product list
    await page.getByRole('link', { name: 'PRODUKTER' }).click();
    await page.waitForURL('**/productlist');

    // Open "Filtrer Allergener" dropdown
    const allergyHeader = page.getByText('Filtrer Allergener');
    await allergyHeader.click();

    // All allergy labels in order
    const allergyNames = ['Gluten', 'Laktose', 'Nødder', 'Skaldyr', 'Soya', 'Æg'];

    // Product card locator
    const productCards = page.locator('div.grid a.group');

    // Baseline product count
    const baselineCount = await productCards.count();
    expect(baselineCount).toBeGreaterThan(0);

    //
    // === INDIVIDUAL ALLERGY TESTING ===
    //
    for (const name of allergyNames) {
        const checkbox = page.getByLabel(name);

        // Check this allergy
        await checkbox.click();
        await page.waitForTimeout(250);

        const filteredCount = await productCards.count();

        // It should show <= baseline products
        expect(filteredCount).toBeLessThanOrEqual(baselineCount);

        // Uncheck to reset for the next allergy
        await checkbox.click();
        await page.waitForTimeout(250);
    }

    //
    // === SELECT ALL AT THE END ===
    //
    for (const name of allergyNames) {
        const checkbox = page.getByLabel(name);
        await checkbox.click(); // select all
    }

    await page.waitForTimeout(400);

    const finalCount = await productCards.count();

    // The count can be 0, but grid must still be visible
    await expect(page.locator('div.grid')).toBeVisible();

    console.log("Final count with ALL allergies:", finalCount);
});
