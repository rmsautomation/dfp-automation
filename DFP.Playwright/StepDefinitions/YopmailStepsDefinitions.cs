using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class YopmailStepsDefinitions
    {
        private readonly YopmailPage _yopmailPage;
        private readonly ScenarioContext _scenarioContext;

        public YopmailStepsDefinitions(YopmailPage yopmailPage, ScenarioContext scenarioContext)
        {
            _yopmailPage = yopmailPage;
            _scenarioContext = scenarioContext;
        }

        [Given("I go to yopmail URL")]
        [When("I go to yopmail URL")]
        [Then("I go to yopmail URL")]
        public async Task IGoToYopmailURL()
        {
            await _yopmailPage.NavigateAsync();
        }

        [Given("I create my yopmail email {string}")]
        [When("I create my yopmail email {string}")]
        [Then("I create my yopmail email {string}")]
        public async Task ICreateMyYopmailEmail(string email)
        {
            string resolvedEmail = string.IsNullOrEmpty(email)
                ? _scenarioContext["ContactEmail"]?.ToString() ?? ""
                : email;

            // Only navigate to yopmail if the address is actually a yopmail inbox.
            // Same domain check as ShipmentTrackingStepDefinitions.RefreshNotificationEmailsAsync.
            var domain = resolvedEmail.Split('@').LastOrDefault()?.Trim().ToLowerInvariant() ?? "";
            if (!domain.Contains("yopmail"))
            {
                Console.WriteLine($"[Yopmail] Skipping inbox navigation — '{resolvedEmail}' is not a yopmail address.");
                return;
            }

            await _yopmailPage.OpenInboxAsync(resolvedEmail);
        }

        [Given(@"I should receive an email with text ""([^""]*)"" in the body")]
        [When(@"I should receive an email with text ""([^""]*)"" in the body")]
        [Then(@"I should receive an email with text ""([^""]*)"" in the body")]
        public async Task IShouldReceiveAnEmailWithTextInTheBody(string textArg)
        {
            var expectedTexts = (textArg ?? string.Empty)
                .Split('|', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            await _yopmailPage.VerifyEmailBodyContainsAsync(expectedTexts);
        }

        [Given("I store the portal user password from the email")]
        [When("I store the portal user password from the email")]
        [Then("I store the portal user password from the email")]
        public async Task IStoreThePortalUserPasswordFromTheEmail()
        {
            var password = await _yopmailPage.ReadPasswordFromEmailAsync();
            _scenarioContext["PortalPassword"] = password;
            Console.WriteLine($"[Yopmail] PortalPassword stored: {password}");
        }

        [Given("I click on Login to Magaya in the email")]
        [When("I click on Login to Magaya in the email")]
        [Then("I click on Login to Magaya in the email")]
        public async Task IClickOnLoginToMagayaInTheEmail()
        {
            await _yopmailPage.ClickLinkInEmailAsync("Login to Magaya");
        }
    }
}
