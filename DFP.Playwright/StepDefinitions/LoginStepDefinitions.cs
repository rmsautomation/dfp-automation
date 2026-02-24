using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DFP.Playwright.Pages;
using DFP.Playwright.Support;
using Reqnroll;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class LoginStepDefinitions
    {
        private readonly Support.TestContext _tc;
        private readonly LoginPage _login;

        public LoginStepDefinitions(
            Support.TestContext tc,
            LoginPage login)
        {
            _tc = tc;
            _login = login;
        }

        [Given("the user is on the login page")]
        public async Task GivenTheUserIsOnTheLoginPage()
        {
            await _login.NavigateAsync();
        }

        [When("the user logs in with valid credentials")]
        public async Task WhenTheUserLogsInWithValidCredentials()
        {
            await _login.LoginToDFPAsync(Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)!, Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)!);
        }

        [Then("the dashboard should be visible")]
        public async Task ThenTheDashboardShouldBeVisible()
        {
            await _login.WaitForDashboardAsync();
        }

        [Given("user navigated to the dashboard")]
        public async Task GivenNavigatedToTheDashboard()
        {
            await _login.NavigateAsync();
            await _login.LoginToDFPAsync(Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)!, Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)!);
            await _tc.Page!.WaitForTimeoutAsync(3000);
        }

        [When("user logs out")]
        public async Task WhenUserLogsOut()
        {
            await _login.LogoutAsync();
        }

        [Then("user should be in login page")]
        public async Task ThenUserShouldBeInLoginPage()
        {
            await _login.WaitForLoginAsync();
            var loginPageVisible = await _login.IsUsernameInputVisibleAsync();
            Assert.IsTrue(loginPageVisible, "Username input is not visible, user might not be on login screen.");
        }

    }
}

