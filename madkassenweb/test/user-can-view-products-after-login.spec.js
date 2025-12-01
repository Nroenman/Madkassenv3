import { test, expect } from '@playwright/test';

test('user can log in and load products from API', async ({ page }) => {
    // 1) Go to login page
    await page.goto('http://localhost:3000/login');

    // 2) Fill login form
    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');
    await page.click('button[type="submit"]');

    // 3) Wait for redirect after login
    await page.waitForURL('**/AboutPage');

    // 4) Confirm token stored in localStorage
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();

    // 5) Click "PRODUKTER" and wait for Product API call
    const [productResponse] = await Promise.all([
        page.waitForResponse(response =>
            response.url().includes('/Product') && response.ok()
        ),
        page.getByRole('link', { name: 'PRODUKTER' }).click()
    ]);

    // 6) Read JSON and assert we actually got products
    const products = await productResponse.json();

    console.log('Number of products from API:', products.length);

    expect(Array.isArray(products)).toBe(true);
    expect(products.length).toBeGreaterThan(0);

    // Optional: sanity check – we expect at least "Mælk" to exist
    const names = products.map(p => p.productName);
    expect(names).toContain('Mælk');
});
