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
        private readonly ScenarioContext _sc;

        public PlaywrightHooks(Support.TestContext tc, FeatureContext fc, ScenarioContext sc)
        {
            _tc = tc;
            _fc = fc;
            _sc = sc;
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
            var skipAutoLogin = featureTitle.Equals("Login", StringComparison.OrdinalIgnoreCase)
                                || HasTag("Login")
                                || HasTag("API");
            if (!skipAutoLogin)
            {
                Console.WriteLine("Auto-login: starting");
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
            var portalBaseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL) ?? "";
            var portalIntBaseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_INT_BASE_URL) ?? "";
            var baseUrl = portalBaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL is null/empty. Auto-login requires Portal (no integration).");

            var username = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_USERNAME)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_PASSWORD)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("PORTAL_USERNAME/PORTAL_PASSWORD are required for auto-login.");

            var login = new LoginPage(_tc.Page!, baseUrl);

            await login.NavigateAsync();

            await login.LoginToDFPAsync(username, password, searchLoginModal: true);

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

        private static string ResolveEnvValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmed = value.Trim();
            if (trimmed.StartsWith("Env.", StringComparison.OrdinalIgnoreCase))
            {
                var key = trimmed.Substring(4);
                return Environment.GetEnvironmentVariable(key) ?? "";
            }

            var indirect = Environment.GetEnvironmentVariable(trimmed);
            if (!string.IsNullOrWhiteSpace(indirect))
                return indirect;

            return value;
        }

        private bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            var target = tag.Trim();
            var featureTags = _fc?.FeatureInfo?.Tags ?? Array.Empty<string>();
            var scenarioTags = _sc?.ScenarioInfo?.Tags ?? Array.Empty<string>();

            return featureTags.Any(t => t.Equals(target, StringComparison.OrdinalIgnoreCase))
                   || scenarioTags.Any(t => t.Equals(target, StringComparison.OrdinalIgnoreCase));
        }
    }
}

