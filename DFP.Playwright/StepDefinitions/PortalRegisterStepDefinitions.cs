using System;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class PortalRegisterStepDefinitions
    {
        private readonly PortalRegisterPage _registerPage;
        private readonly YopmailPage _yopmailPage;
        private readonly ScenarioContext _scenarioContext;

        public PortalRegisterStepDefinitions(
            PortalRegisterPage registerPage,
            YopmailPage yopmailPage,
            ScenarioContext scenarioContext)
        {
            _registerPage = registerPage;
            _yopmailPage = yopmailPage;
            _scenarioContext = scenarioContext;
        }

        private string GetNow() =>
            _scenarioContext.TryGetValue("Now", out var v) && v is string s && !string.IsNullOrWhiteSpace(s)
                ? s
                : DateTime.Now.ToString("yyyyMMddHHmmss");

        // ── Register button ───────────────────────────────────────────────────────

        [Then("I click on Register button")]
        [When("I click on Register button")]
        public async Task ThenIClickOnRegisterButton()
            => await _registerPage.ClickRegisterLinkAsync();

        // ── Create your account page ──────────────────────────────────────────────

        [Then("I should see the create your account page in the Portal")]
        public async Task ThenIShouldSeeTheCreateYourAccountPageInThePortal()
            => await _registerPage.VerifyCreateAccountPageAsync();

        // ── Step 1: Personal info ─────────────────────────────────────────────────

        [Then("I enter the Full Name {string}")]
        [When("I enter the Full Name {string}")]
        public async Task ThenIEnterTheFullName(string name)
        {
            string resolved = string.IsNullOrEmpty(name)
                ? $"{GetNow()}Register"
                : name;
            _scenarioContext["FullName"] = resolved;
            Console.WriteLine($"[TC272] FullName stored: {resolved}");
            await _registerPage.EnterFullNameAsync(resolved);
        }

        [Then("I enter the email {string}")]
        [When("I enter the email {string}")]
        public async Task ThenIEnterTheEmail(string email)
        {
            string resolved = string.IsNullOrEmpty(email)
                ? _scenarioContext["ContactEmail"]?.ToString() ?? ""
                : email;
            Console.WriteLine($"[TC272] Email: {resolved}");
            await _registerPage.EnterEmailAsync(resolved);
        }

        [Then("I enter the password {string}")]
        [When("I enter the password {string}")]
        public async Task ThenIEnterThePassword(string password)
        {
            string resolved = string.IsNullOrEmpty(password)
                ? Environment.GetEnvironmentVariable(Constants.PORTAL_PASSWORD) ?? ""
                : password;
            // Store for later portal login step "I enter the password "" in the Portal"
            _scenarioContext["PortalPassword"] = resolved;
            await _registerPage.EnterPasswordAsync(resolved);
        }

        [When("I click on continue button to register the user")]
        public async Task WhenIClickOnContinueButtonToRegisterTheUser()
            => await _registerPage.ClickContinueButtonAsync();

        // ── Step 2: Company info ──────────────────────────────────────────────────

        [Then("I enter the company name {string}")]
        [When("I enter the company name {string}")]
        public async Task ThenIEnterTheCompanyName(string company)
            => await _registerPage.EnterCompanyNameAsync(company);

        [Then("I accept the terms")]
        [When("I accept the terms")]
        public async Task ThenIAcceptTheTerms()
            => await _registerPage.AcceptTermsAsync();

        [When("I click on create your account button in the Portal")]
        public async Task WhenIClickOnCreateYourAccountButtonInThePortal()
            => await _registerPage.ClickCreateAccountButtonAsync();

        // ── Account created confirmation ──────────────────────────────────────────

        [Then("I should see the created account page")]
        public async Task ThenIShouldSeeTheCreatedAccountPage()
            => await _registerPage.VerifyAccountCreatedAsync();

        // ── Email confirmation ────────────────────────────────────────────────────

        [When("I confirm the email")]
        [Then("I confirm the email")]
        public async Task WhenIConfirmTheEmail()
            => await _yopmailPage.ClickLinkInEmailAsync("Confirm your email");

        [Then("I should see confirmation successfull")]
        public async Task ThenIShouldSeeConfirmationSuccessfull()
            => await _registerPage.VerifyEmailConfirmedAsync();

        // ── Welcome text verification ─────────────────────────────────────────────

        [Given("I should see Welcome text {string}")]
        [When("I should see Welcome text {string}")]
        [Then("I should see Welcome text {string}")]
        public async Task IShouldSeeWelcomeText(string expectedText)
            => await _registerPage.VerifyWelcomeTextAsync(expectedText);

        // ── Email address fields (INT portal registration) ────────────────────────

        [Given("I provide an email address {string}")]
        [When("I provide an email address {string}")]
        [Then("I provide an email address {string}")]
        public async Task IProvideAnEmailAddress(string email)
        {
            string resolved = string.IsNullOrEmpty(email)
                ? _scenarioContext["ContactEmail"]?.ToString() ?? ""
                : email;
            Console.WriteLine($"[TC1275] Email address: {resolved}");
            await _registerPage.EnterEmailAddressAsync(resolved);
        }

        [Given("I confirm the provide email address {string} second time.")]
        [When("I confirm the provide email address {string} second time.")]
        [Then("I confirm the provide email address {string} second time.")]
        public async Task IConfirmTheProvideEmailAddress(string email)
        {
            string resolved = string.IsNullOrEmpty(email)
                ? _scenarioContext["ContactEmail"]?.ToString() ?? ""
                : email;
            Console.WriteLine($"[TC1275] Confirm email address: {resolved}");
            await _registerPage.ConfirmEmailAddressAsync(resolved);
        }

        // ── Hub notification verification ─────────────────────────────────────────

        [Then("I verify the notification on the Dashboard page")]
        public async Task ThenIVerifyTheNotificationOnTheDashboardPage()
        {
            var fullName = _scenarioContext.TryGetValue("FullName", out var v) ? v?.ToString() ?? "" : "";
            await _registerPage.VerifyNotificationContainsFullNameAsync(fullName);
        }
    }
}
