import { test, expect } from '@playwright/test';

test('user can log in and see products', async ({ page }) => {
    await page.goto('http://localhost:3000/login');

    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');
    await page.click('button[type="submit"]');

    await page.waitForURL('**/AboutPage');

    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();

    // 👉 set up wait for the product API *before* clicking the link
    const productsResponsePromise = page.waitForResponse(response =>
        response.url().includes('/api/Product') && response.ok()
    );

    // Click "PRODUKTER" in navbar
    await page.getByRole('link', { name: 'PRODUKTER' }).click();

    await productsResponsePromise;

    const productCards = page.locator('.product-card');
    await expect(productCards.first()).toBeVisible();
});
