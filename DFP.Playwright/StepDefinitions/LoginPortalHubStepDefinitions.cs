using System;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class LoginPortalHubStepDefinitions(DFP.Playwright.Support.TestContext tc, ScenarioContext scenarioContext)
    {
        private readonly DFP.Playwright.Support.TestContext _tc = tc;
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private readonly LoginPage _loginPage = new LoginPage(tc.Page!,
            Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
            ?? Environment.GetEnvironmentVariable("BASE_URL") ?? "");

        [Given("I login to Portal")]
        public async Task GivenILoginToPortal()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            var username = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_USERNAME)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_PASSWORD)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");

            var login = new LoginPage(_tc.Page!, baseUrl);
            await login.NavigateAsync();
            await login.LoginToDFPAsync(username, password, searchLoginModal: true);
            await login.WaitForDashboardAsync();
        }

        [Given("I login to Portal with integration")]
        public async Task GivenILoginToPortalWithIntegration()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_INT_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            var username = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_INT_USERNAME) ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_INT_PASSWORD) ?? "");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_INT_BASE_URL (or PORTAL_BASE_URL/BASE_URL) is required.");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("PORTAL_INT_USERNAME/PORTAL_INT_PASSWORD are required.");

            var login = new LoginPage(_tc.Page!, baseUrl);
            await login.NavigateAsync();
            await login.LogoutIfLoggedInAsync();
            await login.LoginToDFPAsync(username, password, searchLoginModal: false);
            await login.WaitForDashboardAsync();
        }

        [Given("I login to Hub")]
        public async Task GivenILoginToHub()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            var username = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.HUB_USERNAME)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.HUB_PASSWORD)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");

            var login = new LoginPage(_tc.Page!, baseUrl);
            await login.NavigateAsync();
            await login.LogoutIfLoggedInAsync();
            await login.LoginToDFPAsync(username, password, searchLoginModal: false);
            await login.WaitForDashboardAsync();
        }

        [Given("I login to Hub with integration")]
        public async Task GivenILoginToHubWithIntegration()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            var username = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.HUB_INT_USERNAME) ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.HUB_INT_PASSWORD) ?? "");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("HUB_INT_USERNAME/HUB_INT_PASSWORD are required.");

            var login = new LoginPage(_tc.Page!, baseUrl);
            await login.NavigateAsync();
            await login.LoginToDFPAsync(username, password, searchLoginModal: false);
            await login.WaitForDashboardAsync();
        }

        [Then("the login dashboard should be visible")]
        public async Task ThenTheLoginDashboardShouldBeVisible()
        {
            var login = CreateLoginPage(Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "");
            await login.WaitForDashboardAsync();
        }

        [When("I log out")]
        [When("I log out from Portal")]
        [Then("I log out from Portal")]
        public async Task WhenILogOut()
        {
            var login = CreateLoginPage(Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "");
            await login.LogoutAsync();
        }

        [Then("I should be in login page")]
        public async Task ThenIShouldBeInLoginPage()
        {
            // Wait for any pending navigation after logout to settle.
            try { await _tc.Page!.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle, new Microsoft.Playwright.PageWaitForLoadStateOptions { Timeout = 10000 }); } catch { }

            var login = CreateLoginPage(Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "");

            // Portal after logout lands on the homepage (Sign-in nav link) — not directly on the
            // login form. WaitForLoginAsync looks for a heading that only appears after clicking
            // the nav Sign-in link, so we allow it to time out here without failing.
            try { await login.WaitForLoginAsync(5000); } catch { /* homepage with nav Sign-in link is fine */ }

            var loginPageVisible = await login.IsUsernameInputVisibleAsync();
            if (!loginPageVisible)
            {
                // Portal homepage: verify "Sign in" nav link is visible (confirms user is logged out).
                var signInLink = _tc.Page!.Locator("a:has-text('Sign in'), .nav-link:has-text('Sign in')").First;
                var linkVisible = await signInLink.IsVisibleAsync();
                Assert.IsTrue(linkVisible, "Neither username input nor 'Sign in' link is visible — user may not be logged out.");
                return;
            }

            Assert.IsTrue(loginPageVisible, "Username input is not visible, user might not be on login screen.");
        }

        // ── Hub logout flow ───────────────────────────────────────────────────────

        [Given("I log out from Hub")]
        [When("I log out from Hub")]
        [Then("I log out from Hub")]
        public async Task GivenILogOutFromHub()
        {
            await WhenIClickOnProfileButtonInTheHub();
            await WhenIClickOnLogOutOptionInTheHub();
        }

        [When("I click on the profile button in the hub")]
        [Then("I click on the profile button in the hub")]
        public async Task WhenIClickOnProfileButtonInTheHub()
        {
            // Verified from HTML: button#hubNavbarProfileItemButton
            var btn = _tc.Page!.Locator("button#hubNavbarProfileItemButton");
            await btn.WaitForAsync(new Microsoft.Playwright.LocatorWaitForOptions
            {
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 10000
            });
            await btn.ClickAsync();
        }

        [When("I click on Log out option in the hub")]
        [Then("I click on Log out option in the hub")]
        public async Task WhenIClickOnLogOutOptionInTheHub()
        {
            // Verified from HTML: span.ng-tns-* with text "Log out" inside the profile dropdown
            var logoutOption = _tc.Page!.Locator("span:has-text('Log out')").First;
            await logoutOption.WaitForAsync(new Microsoft.Playwright.LocatorWaitForOptions
            {
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await logoutOption.ClickAsync();
        }

        [Then("I should be in login page in the hub")]
        public async Task ThenIShouldBeInLoginPageInTheHub()
        {
            // Verified from HTML: Auth0 login page shows input#username (Email address field)
            var emailInput = _tc.Page!.Locator("input#username");
            await emailInput.WaitForAsync(new Microsoft.Playwright.LocatorWaitForOptions
            {
                State = Microsoft.Playwright.WaitForSelectorState.Visible,
                Timeout = 15000
            });
            Assert.IsTrue(await emailInput.IsVisibleAsync(),
                $"Auth0 login page not reached after Hub logout. URL: {_tc.Page.Url}");
        }

        [Given(@"I login to Hub as user ""?([^""]+)""?")]
        public async Task GivenILoginToHubAs(string userType)
        {
            var isIntegration = IsIntegration(userType);
            if (isIntegration)
            {
                await GivenILoginToHubWithIntegration();
                return;
            }

            await GivenILoginToHub();
        }

        [Given(@"I login to Portal as user ""?([^""]+)""?")]
        public async Task GivenILoginToPortalAs(string userType)
        {
            var isIntegration = IsIntegration(userType);
            if (isIntegration)
            {
                await GivenILoginToPortalWithIntegration();
                return;
            }

            if (IsWithoutIntegration(userType))
            {
                await GivenILoginToPortal();
                return;
            }

            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");

            var username = userType.Trim();
            var password = ResolvePasswordForUsername(username);

            var login = new LoginPage(_tc.Page!, baseUrl);
            await login.NavigateAsync();
            await login.LogoutIfLoggedInAsync();
            await login.LoginToDFPAsync(username, password, searchLoginModal: true);
            await login.WaitForDashboardAsync();
        }

        // ── URL navigation steps (no login — assumes session is already active) ───

        [When(@"I open the portal URL ""?([^""]+)""?")]
        public async Task WhenIOpenThePortalURL(string portalType)
        {
            string baseUrl;
            if (IsIntegration(portalType))
            {
                baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_INT_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "https://38442-dfpstag-magayaprod-auto.next.qwykportals.com/";
            }
            else
            {
                baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "https://magaya-qa.next.qwykportals.com/";
            }

            await _tc.Page!.GotoAsync(baseUrl);
            await _tc.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        }

        [When("I open the hub URL")]
        public async Task WhenIOpenTheHubURL()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? "https://hub.next.qwykportals.com/";

            await _tc.Page!.GotoAsync(baseUrl);
            await _tc.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        }

        private LoginPage CreateLoginPage(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseUrl is null/empty. Provide it via env.");
            return new LoginPage(_tc.Page!, baseUrl);
        }

        private static bool IsIntegration(string userType)
        {
            if (string.IsNullOrWhiteSpace(userType))
                return false;

            var t = userType.Trim().ToLowerInvariant();
            return t == "integration"
                   || t == "int"
                   || t == "with int"
                   || t == "with-int";
        }

        private static bool IsWithoutIntegration(string userType)
        {
            if (string.IsNullOrWhiteSpace(userType))
                return false;

            var t = userType.Trim().ToLowerInvariant();
            return t == "without int"
                   || t == "without-int"
                   || t == "no int"
                   || t == "no-int"
                   || t == "noint"
                   || t == "no integration"
                   || t == "without integration";
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

        private static string ResolvePasswordForUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Username is required.");

            var explicitKey = $"PORTAL_PASSWORD_FOR_{username}";
            var explicitValue = Environment.GetEnvironmentVariable(explicitKey);
            if (!string.IsNullOrWhiteSpace(explicitValue))
                return ResolveEnvValue(explicitValue);

            var direct = Environment.GetEnvironmentVariable(username);
            if (!string.IsNullOrWhiteSpace(direct))
                return ResolveEnvValue(direct);

            var key = System.Text.RegularExpressions.Regex.Replace(username.Trim().ToUpperInvariant(), "[^A-Z0-9]+", "_");
            var byKey = Environment.GetEnvironmentVariable($"{key}_PASSWORD");
            if (!string.IsNullOrWhiteSpace(byKey))
                return ResolveEnvValue(byKey);

            var fallback = Environment.GetEnvironmentVariable(Constants.PORTAL_PASSWORD)
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "";
            if (string.IsNullOrWhiteSpace(fallback))
                throw new InvalidOperationException("PORTAL_PASSWORD (or DFP_PASSWORD) is required as fallback.");

            return ResolveEnvValue(fallback);
        }

        // ── TC1483: Portal login (individual steps) ────────────────────────────────

        [Given("I should see the login page")]
        [When("I should see the login page")]
        [Then("I should see the login page")]
        public async Task IShouldSeeTheLoginPage()
        {
            await _loginPage.ShouldSeeLoginPageAsync();
        }

        [Given("I enter the created username {string} in the Portal")]
        [When("I enter the created username {string} in the Portal")]
        [Then("I enter the created username {string} in the Portal")]
        public async Task IEnterTheCreatedUsernameInThePortal(string username)
        {
            string resolved;
            if (!string.IsNullOrEmpty(username))
            {
                resolved = username;
            }
            else if (_scenarioContext.TryGetValue("UserName", out var un) && un is string uname && !string.IsNullOrWhiteSpace(uname))
            {
                // INT env: username entered in Hub (And I enter the username "")
                resolved = uname;
            }
            else if (_scenarioContext.TryGetValue("Now", out var now) && now is string nowStr && !string.IsNullOrWhiteSpace(nowStr))
            {
                // INT env (TC329/TC580): Magaya contact username = Now var (no @yopmail.com)
                resolved = nowStr;
            }
            else
            {
                resolved = _scenarioContext["usernamePortal"]?.ToString() ?? "";
            }
            Console.WriteLine($"[TC1483/TC1275/TC329] Portal username: {resolved}");
            await _loginPage.FillPortalUsernameAsync(resolved);
        }

        [Given("I enter the password {string} in the Portal")]
        [When("I enter the password {string} in the Portal")]
        [Then("I enter the password {string} in the Portal")]
        public async Task IEnterThePasswordInThePortal(string password)
        {
            string resolved = string.IsNullOrEmpty(password)
                ? (_scenarioContext.TryGetValue("PortalPassword", out var v) ? v?.ToString() ?? "" : "")
                : password;
            Console.WriteLine($"[TC1483] Portal password: {resolved}");
            await _loginPage.FillPortalPasswordAsync(resolved);
        }

        [Given("click on Sign in button")]
        [When("click on Sign in button")]
        [Then("click on Sign in button")]
        public async Task IClickOnSignInButton()
        {
            await _loginPage.ClickPortalSignInAsync();
        }

        [Given("I login to Portal as the created user")]
        [When("I login to Portal as the created user")]
        [Then("I login to Portal as the created user")]
        public async Task ILoginToPortalAsTheCreatedUser()
        {
            var email    = _scenarioContext["ContactEmail"]?.ToString()   ?? "";
            var password = _scenarioContext["PortalPassword"]?.ToString() ?? "";
            Console.WriteLine($"[TC1483] Logging in to Portal as: {email}");
            await _loginPage.LoginToPortalAsCreatedUserAsync(email, password);
        }
    }
}
