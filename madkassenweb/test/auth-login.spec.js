import { test, expect } from '@playwright/test';

test('user can successfully log in and is redirected', async ({ page }) => {
    // 1. Go to login page
    await page.goto('http://localhost:3000/login');

    // 2. Fill login form
    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');

    // 3. Submit form
    await page.click('button[type="submit"]');

    // 4. Wait for redirect to /AboutPage (from your useAuth hook)
    await page.waitForURL('**/AboutPage');

    // 5. Check token exists to confirm login succeeded
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();
});
