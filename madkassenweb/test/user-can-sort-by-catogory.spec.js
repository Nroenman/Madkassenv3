import { test, expect } from '@playwright/test';

test('User can browse all categories from API', async ({ page }) => {
    await page.goto('http://localhost:3000/');

    // Click "PRODUKTER" and wait for the categories API to respond
    const [categoriesRes] = await Promise.all([
        page.waitForResponse(r =>
            r.url().includes('/api/Category') && r.ok()
        ),
        page.getByRole('link', { name: 'PRODUKTER' }).click()
    ]);

    // Basic API sanity check
    expect(categoriesRes.ok()).toBeTruthy();

    const categories = await categoriesRes.json();
    console.log('Number of categories from API:', categories.length);

    expect(Array.isArray(categories)).toBe(true);
    expect(categories.length).toBeGreaterThan(0);
});
