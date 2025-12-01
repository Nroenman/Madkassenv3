import { test, expect } from '@playwright/test';

test('user can buy a product through checkout flow', async ({ page }) => {
    // --- LOGIN ---
    await page.goto('http://localhost:3000/login');

    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');
    await page.click('button[type="submit"]');

    // wait for successful login redirect
    await page.waitForURL('**/AboutPage');

    // --- GO TO PRODUCT PAGE ---
    const [productResponse] = await Promise.all([
        page.waitForResponse(r => r.url().includes('/Product') && r.ok()),
        page.getByRole('link', { name: 'PRODUKTER' }).click()
    ]);

    // optional: sanity that API returned something
    const products = await productResponse.json();
    expect(products.length).toBeGreaterThan(0);

    const productGrid = page.locator('div.grid.grid-cols-1');
    await expect(productGrid).toBeVisible();

    // --- CLICK A PRODUCT FROM MAIN GRID (Ost) ---
    const ostProduct = productGrid.getByRole('link', { name: /Ost/i }).first();
    await expect(ostProduct).toBeVisible();
    await ostProduct.click();

    // --- ADD TO CART ---
    const addToCartButton = page.getByRole('button', { name: /Tilføj til kurv/i });
    await expect(addToCartButton).toBeVisible();
    await addToCartButton.click();

    // small pause so cart updates
    await page.waitForTimeout(500);

    // --- OPEN CART ---
    await page.locator('a[href="/cart"]').click();
    await page.waitForURL('**/cart');

    await expect(page.getByRole('row', { name: /Ost/i })).toBeVisible();

    // --- CLICK "Gennemfør køb!" ---
    const checkoutButton = page.getByRole('button', { name: /Gennemfør køb/i });
    await expect(checkoutButton).toBeVisible();

    // handle alert, but don't assert on its text
    const [dialog] = await Promise.all([
        page.waitForEvent('dialog'),
        checkoutButton.click()
    ]);
    console.log('Order dialog message:', dialog.message());
    await dialog.accept();

    await page.waitForTimeout(5200);
});
