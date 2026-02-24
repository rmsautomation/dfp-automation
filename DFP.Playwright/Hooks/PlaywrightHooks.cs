using Microsoft.Playwright;
using DFP.Playwright.Support;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Reqnroll;

namespace DFP.Playwright.Hooks
{
    [Binding]
    public sealed class PlaywrightHooks
    {
        private readonly Support.TestContext _tc;
        private readonly FeatureContext _fc;

        public PlaywrightHooks(Support.TestContext tc, FeatureContext fc)
        {
            _tc = tc;
            _fc = fc;
        }

        [BeforeTestRun]
        public static void LoadEnv()
        {
            EnvLoader.LoadEnvironment();
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var browserName = Environment.GetEnvironmentVariable(Constants.BROWSER) ?? "chromium";
            var headlessEnv = Environment.GetEnvironmentVariable(Constants.HEADLESS) ?? "true";

            var headless = !headlessEnv.Equals("false", StringComparison.OrdinalIgnoreCase);

            _tc.Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            IBrowserType browserType = browserName.ToLower() switch
            {
                "firefox" => _tc.Playwright.Firefox,
                "webkit" => _tc.Playwright.Webkit,
                _ => _tc.Playwright.Chromium
            };

            _tc.Browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = headless ? 0 : 50 // optional but GREAT for debugging
            });

            _tc.Context = await _tc.Browser.NewContextAsync();
            _tc.Page = await _tc.Context.NewPageAsync();

            Console.WriteLine($"Browser: {browserName}");
            Console.WriteLine($"Headless: {headless}");

            var featureTitle = _fc?.FeatureInfo?.Title ?? "";
            if (!featureTitle.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Auto-login: starting (feature != Login)");
                await EnsureLoggedInAsync();
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_tc.Context != null)
                await _tc.Context.CloseAsync();
            if (_tc.Browser != null)
                await _tc.Browser.CloseAsync();
            _tc.Playwright?.Dispose();
        }

        private async Task EnsureLoggedInAsync()
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BASE_URL is null/empty. Provide it via config/env.");

            var username = Environment.GetEnvironmentVariable(Constants.DFP_USERNAME);
            var password = Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("DFP_USERNAME/DFP_PASSWORD are required for auto-login.");

            var login = new LoginPage(_tc.Page!, baseUrl);

            await login.NavigateAsync();

            // Always attempt login for non-Login features if the form is visible.
            if (await login.IsUsernameInputVisibleAsync())
            {
                await login.LoginToDFPAsync(username, password);
            }

            var start = DateTime.UtcNow;
            var loggedIn = false;
            while ((DateTime.UtcNow - start).TotalMilliseconds < 15000)
            {
                if (await login.IsLoggedInAsync() || await login.IsDashboardVisibleAsync())
                {
                    loggedIn = true;
                    break;
                }
                await Task.Delay(500);
            }

            if (!loggedIn)
                throw new TimeoutException("Auto-login did not reach a logged-in state within 15s.");
        }
    }
}

