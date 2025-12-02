import { test, expect } from '@playwright/test';

test('user can successfully log in and is redirected', async ({ page }) => {
    // 1. Go to login page
    await page.goto('http://localhost:3000/login');

    // 2. Fill login form
    await page.fill('input[name="email"]', 'Test1@test.com');
    await page.fill('input[name="password"]', 'Test1@test.com');

    // 3. Click login and inspect the API response
    const [loginResponse] = await Promise.all([
        page.waitForResponse(r =>
            r.url().includes('/api/Auth') &&
            r.request().method() === 'POST'
        ),
        page.click('button[type="submit"]'),
    ]);

    console.log('Login status:', loginResponse.status());
    const bodyText = await loginResponse.text();
    console.log('Login response body:', bodyText);

    // 4. Only then wait for redirect to /AboutPage
    await page.waitForURL('**/AboutPage');

    // 5. Check token exists to confirm login succeeded
    const token = await page.evaluate(() => localStorage.getItem('authToken'));
    expect(token).not.toBeNull();
});
