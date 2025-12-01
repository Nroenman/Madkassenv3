import { test, expect } from '@playwright/test';

test('User can browse all categories from API', async ({ page }) => {
    await page.goto('http://localhost:3000/');

    // Click "PRODUKTER" and wait for categories API
    const [categoriesRes] = await Promise.all([
        page.waitForResponse(r =>
            r.url().includes('/api/Category') && r.ok()
        ),
        page.getByRole('link', { name: 'PRODUKTER' }).click(),
    ]);

    await page.waitForURL('**/productlist');

    const categories = await categoriesRes.json();
    expect(Array.isArray(categories)).toBeTruthy();
    expect(categories.length).toBeGreaterThan(0);

    // Open the "Kategorier" dropdown once
    const categoriesHeader = page.getByText('Kategorier');
    await expect(categoriesHeader).toBeVisible();
    await categoriesHeader.click();

    // Loop through all categories from the API
    for (const category of categories) {
        const categoryId = category.categoryId;
        const categoryName = category.categoryName;

        // Create a relaxed text locator for the category name
        const categoryItem = page.getByText(
            new RegExp(`^\\s*${categoryName}\\s*$`)
        );

        // If for some reason the dropdown closed, open it again
        if (!(await categoryItem.isVisible())) {
            await categoriesHeader.click(); // toggle open
        }

        await expect(categoryItem).toBeVisible();

        const [categoryProductsRes] = await Promise.all([
            page.waitForResponse(r =>
                r.url().includes('/api/Product/category/') &&
                r.url().endsWith(`/${categoryId}`) &&
                r.ok()
            ),
            categoryItem.click(),
        ]);

        const categoryProducts = await categoryProductsRes.json();
        expect(categoryProducts).toBeTruthy();

        // Product grid is still visible
        await expect(page.locator('div.grid')).toBeVisible();
    }
});
