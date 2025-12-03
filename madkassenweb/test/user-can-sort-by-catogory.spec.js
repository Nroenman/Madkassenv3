import { test, expect } from '@playwright/test';

test('User can browse all categories from API', async ({ page }) => {
    await page.goto('http://localhost:3000/');

    // Go to product list and wait for categories API
    const [categoriesRes] = await Promise.all([
        page.waitForResponse(r =>
            r.url().includes('/api/Category') && r.ok()
        ),
        page.getByRole('link', { name: /PRODUKTER/i }).click(),
    ]);

    const categories = await categoriesRes.json();
    console.log('Number of categories from API:', categories.length);

    expect(Array.isArray(categories)).toBe(true);
    expect(categories.length).toBeGreaterThan(0);

    const maxCategoriesToTest = Math.min(categories.length, 5);

    for (let i = 0; i < maxCategoriesToTest; i++) {
        const category = categories[i];
        const categoryId = category.categoryId ?? category.id;
        const categoryName = category.categoryName ?? category.name ?? 'Unknown';

        console.log(`Testing category ${categoryId} (${categoryName})`);

        const categoryProductsRes = await page.request.get(
            `http://localhost:5092/api/Product/category/${categoryId}`
        );

        expect(categoryProductsRes.ok()).toBe(true);

        const products = await categoryProductsRes.json();
        console.log(
            `Category ${categoryId} (${categoryName}) product count:`,
            Array.isArray(products) ? products.length : 'not array'
        );

        expect(Array.isArray(products)).toBe(true);
        // It’s possible some categories are empty in DB, so we *don’t*
        // force length > 0 here – just that the response is a valid array.
    }
});
