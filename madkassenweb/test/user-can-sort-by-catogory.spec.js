import { test, expect } from '@playwright/test';

test('User can browse all categories from API', async ({ page }) => {
    await page.goto('http://localhost:3000/');

    // Click "PRODUKTER" and wait for categories API
    const [categoriesRes] = await Promise.all([
        page.waitForResponse(r =>
            r.url().includes('/api/Category') && r.ok()
        ),
        page.getByRole('link', { name: 'PRODUKTER' }).click()
    ]);

    const categories = await categoriesRes.json();
    console.log('Number of categories from API:', categories.length);
    expect(categories.length).toBeGreaterThan(0);

    // Open category drawer
    const sortButton = page.getByRole('button', { name: /Sorter efter kategori/i });
    await expect(sortButton).toBeVisible();
    await sortButton.click();

    const drawer = page.locator('.MuiDrawer-paper').first();
    await expect(drawer).toBeVisible();

    // For each category from API:
    //  - click the category button in the drawer
    //  - wait a bit for UI update
    //  - assert that the product grid is visible
    for (const category of categories) {
        const { categoryName } = category;
        console.log('Testing category:', categoryName);

        const categoryButton = drawer.getByText(categoryName, { exact: true });
        await expect(categoryButton).toBeVisible();

        await categoryButton.click();

        // Let the frontend fetch & render
        await page.waitForTimeout(500);

        const productGrid = page.locator('div.grid.grid-cols-1');
        await expect(productGrid).toBeVisible();
    }
});
