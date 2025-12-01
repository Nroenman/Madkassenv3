import { test, expect } from '@playwright/test';

test('user can add a product to cart after login', async ({ page }) => {
    // --- LOGIN ---
    await page.goto('http://localhost:3000/login');

    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');
    await page.click('button[type="submit"]');

    await page.waitForURL('**/AboutPage');

    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();

    // --- CHECK CURRENT CART SIZE (BEFORE) ---
    await page.locator('a[href="/cart"]').click();
    await page.waitForURL('**/cart');

    const cartRowsBefore = await page.locator('table tbody tr').count();
    console.log('Cart rows BEFORE:', cartRowsBefore);

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
    const productName = await firstProductLink.innerText();
    console.log('Clicking product:', productName.trim());
    await firstProductLink.click();

    // --- ADD TO CART ---
    const addToCartButton = page.getByRole('button', { name: /Tilføj til kurv/i });
    await expect(addToCartButton).toBeVisible();
    await addToCartButton.click();

    await page.waitForTimeout(500);

    // --- OPEN CART AGAIN ---
    await page.locator('a[href="/cart"]').click();
    await page.waitForURL('**/cart');

    const cartRowsAfter = await page.locator('table tbody tr').count();
    console.log('Cart rows AFTER:', cartRowsAfter);

    // --- ASSERT CART SIZE INCREASED ---
    expect(cartRowsAfter).toBeGreaterThan(cartRowsBefore);
});
