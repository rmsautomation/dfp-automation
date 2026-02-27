using System;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class LoginPortalHubStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;

        public LoginPortalHubStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

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

        [Then("the dashboard should be visible")]
        public async Task ThenTheDashboardShouldBeVisible()
        {
            var login = CreateLoginPage(Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "");
            await login.WaitForDashboardAsync();
        }

        [When("I log out")]
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
            var login = CreateLoginPage(Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "");
            await login.WaitForLoginAsync();
            var loginPageVisible = await login.IsUsernameInputVisibleAsync();
            Assert.IsTrue(loginPageVisible, "Username input is not visible, user might not be on login screen.");
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
    }
}
