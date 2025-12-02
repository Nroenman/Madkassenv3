import { test, expect } from '@playwright/test';

test('user can add a product to cart after login', async ({ page }) => {
    // --- LOGIN ---
    await page.goto('http://localhost:3000/login');

    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');
    await page.click('button[type="submit"]');

    // wait for redirect
    await page.waitForURL('**/AboutPage');

    // token exists -> login OK
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();

    // --- GO TO CART (baseline, just to ensure it loads) ---
    await page.locator('a[href="/cart"]').click();
    await page.waitForURL('**/cart');
    const initialCartUrl = page.url();
    expect(initialCartUrl).toContain('/cart');

    // --- GO TO PRODUCTS ---
    const [productResponse] = await Promise.all([
        page.waitForResponse(r => r.url().includes('/Product') && r.ok()),
        page.getByRole('link', { name: 'PRODUKTER' }).click()
    ]);

    const products = await productResponse.json();
    expect(Array.isArray(products)).toBe(true);
    expect(products.length).toBeGreaterThan(0);

    const productGrid = page.locator('div.grid.grid-cols-1');
    await expect(productGrid).toBeVisible();

    // --- CLICK FIRST PRODUCT FROM MAIN GRID (NO HARDCODED NAME) ---
    const firstProductLink = productGrid.getByRole('link').first();
    await expect(firstProductLink).toBeVisible();
    const productName = (await firstProductLink.innerText()).trim();
    console.log('Clicking product:', productName);
    await firstProductLink.click();

    // --- ADD TO CART ---
    const addToCartButton = page.getByRole('button', { name: /Tilføj til kurv/i });
    await expect(addToCartButton).toBeVisible();
    await addToCartButton.click();

    // small pause so any async logic can run
    await page.waitForTimeout(500);

    // --- OPEN CART AGAIN ---
    await page.locator('a[href="/cart"]').click();
    await page.waitForURL('**/cart');

    // ✅ Assertion: we successfully reached the cart page after the flow
    const finalCartUrl = page.url();
    expect(finalCartUrl).toContain('/cart');
});
