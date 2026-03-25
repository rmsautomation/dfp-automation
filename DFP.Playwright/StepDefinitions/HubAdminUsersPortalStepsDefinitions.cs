using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class HubAdminUsersPortalStepsDefinitions
    {
        private readonly HubAdminUsersPortalPage _hubAdminUsersPortalPage;
        private readonly ScenarioContext _scenarioContext;

        public HubAdminUsersPortalStepsDefinitions(HubAdminUsersPortalPage hubAdminUsersPortalPage, ScenarioContext scenarioContext)
        {
            _hubAdminUsersPortalPage = hubAdminUsersPortalPage;
            _scenarioContext = scenarioContext;
        }

        // ── TC1483: Hub Customer - Create Portal User ──────────────────────────────

        [Given("I store the now var")]
        [When("I store the now var")]
        [Then("I store the now var")]
        public Task IStoreTheNowVar()
        {
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            _scenarioContext["Now"] = now;
            Console.WriteLine($"[TC1483] Now var: {now}");
            return Task.CompletedTask;
        }

        private string GetNow() =>
            _scenarioContext.TryGetValue("Now", out var v) && v is string s && !string.IsNullOrWhiteSpace(s)
                ? s
                : DateTime.Now.ToString("yyyyMMddHHmmss");

        [Given("I store the new contact email {string}")]
        [When("I store the new contact email {string}")]
        [Then("I store the new contact email {string}")]
        public Task IStoreTheNewContactEmail(string email)
        {
            string resolvedEmail = string.IsNullOrEmpty(email)
                ? $"{GetNow()}contact@yopmail.com"
                : email;
            _scenarioContext["ContactEmail"]   = resolvedEmail;
            _scenarioContext["usernamePortal"] = resolvedEmail;   // also stored as usernamePortal
            Console.WriteLine($"[TC1483] ContactEmail / usernamePortal: {resolvedEmail}");
            return Task.CompletedTask;
        }

        [Given("I set the portal password {string}")]
        [When("I set the portal password {string}")]
        [Then("I set the portal password {string}")]
        public Task ISetThePortalPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                // Empty arg → just print whatever is already stored (don't overwrite)
                var current = _scenarioContext.TryGetValue("PortalPassword", out var v) ? v?.ToString() ?? "(empty)" : "(not set)";
                Console.WriteLine($"[TC1483] PortalPassword (current in context): {current}");
            }
            else
            {
                _scenarioContext["PortalPassword"] = password;
                Console.WriteLine($"[TC1483] PortalPassword set manually: {password}");
            }
            return Task.CompletedTask;
        }

        [Given("I enter the email for the portal user in the Hub")]
        [When("I enter the email for the portal user in the Hub")]
        [Then("I enter the email for the portal user in the Hub")]
        public async Task IEnterTheEmailForThePortalUserInTheHub()
        {
            var email = _scenarioContext["ContactEmail"]?.ToString() ?? "";
            await _hubAdminUsersPortalPage.EnterEmailForPortalUserAsync(email);
        }

        [Given("I store the password for the portal user created in the Hub")]
        [When("I store the password for the portal user created in the Hub")]
        [Then("I store the password for the portal user created in the Hub")]
        public async Task IStoreThePasswordForThePortalUserCreatedInTheHub()
        {
            var password = await _hubAdminUsersPortalPage.ReadPortalUserPasswordAsync();
            _scenarioContext["PortalPassword"] = password;
            Console.WriteLine($"[TC1483] PortalPassword: {password}");
        }

        [Given("I select the site {string}")]
        [When("I select the site {string}")]
        [Then("I select the site {string}")]
        public async Task ISelectTheSite(string site)
        {
            await _hubAdminUsersPortalPage.SelectSiteAsync(site);
        }

        [Given("I enter the User Name {string}")]
        [When("I enter the User Name {string}")]
        [Then("I enter the User Name {string}")]
        public async Task IEnterTheUserName(string name)
        {
            string resolvedName = string.IsNullOrEmpty(name)
                ? $"Auto{GetNow()}"
                : name;
            Console.WriteLine($"[TC1483] UserName: {resolvedName}");
            await _hubAdminUsersPortalPage.EnterUserNameAsync(resolvedName);
        }

        [Given("I enter the company name {string} in the Hub")]
        [When("I enter the company name {string} in the Hub")]
        [Then("I enter the company name {string} in the Hub")]
        public async Task IEnterTheCompanyNameInTheHub(string company)
        {
            string resolvedCompany = string.IsNullOrEmpty(company)
                ? $"company{GetNow()}"
                : company;
            Console.WriteLine($"[TC1483] CompanyName: {resolvedCompany}");
            await _hubAdminUsersPortalPage.EnterCompanyNameAsync(resolvedCompany);
        }

        [Given("I confirm the privacy")]
        [When("I confirm the privacy")]
        [Then("I confirm the privacy")]
        public async Task IConfirmThePrivacy()
        {
            await _hubAdminUsersPortalPage.ConfirmPrivacyAsync();
        }

        [Given("I click on create user button in the Hub")]
        [When("I click on create user button in the Hub")]
        [Then("I click on create user button in the Hub")]
        public async Task IClickOnCreateUserButtonInTheHub()
        {
            await _hubAdminUsersPortalPage.ClickCreateUserSubmitButtonAsync();
        }

        // ── TC1275: Hub approval of portal user ───────────────────────────────────

        [Given("I select the created user {string} in the Hub to approve the access")]
        [When("I select the created user {string} in the Hub to approve the access")]
        [Then("I select the created user {string} in the Hub to approve the access")]
        public async Task ISelectTheCreatedUserInTheHubToApproveTheAccess(string email)
        {
            string resolved = string.IsNullOrEmpty(email)
                ? _scenarioContext["ContactEmail"]?.ToString() ?? ""
                : email;
            await _hubAdminUsersPortalPage.SelectPendingUserByEmailAsync(resolved);
        }

        [Given("I enter the username {string}")]
        [When("I enter the username {string}")]
        [Then("I enter the username {string}")]
        public async Task IEnterTheUsername(string username)
        {
            string resolved = string.IsNullOrEmpty(username)
                ? GetNow()
                : username;
            _scenarioContext["UserName"] = resolved;
            Console.WriteLine($"[TC1275] UserName stored: {resolved}");
            await _hubAdminUsersPortalPage.EnterUsernameAsync(resolved);
        }

        [Given("I click on search icon to search the Entity")]
        [When("I click on search icon to search the Entity")]
        [Then("I click on search icon to search the Entity")]
        public async Task IClickOnSearchIconToSearchTheEntity()
        {
            await _hubAdminUsersPortalPage.ClickSearchIconForEntityAsync();
        }

        [Given("I enter the Entity {string}")]
        [When("I enter the Entity {string}")]
        [Then("I enter the Entity {string}")]
        public async Task IEnterTheEntity(string entityName)
        {
            await _hubAdminUsersPortalPage.EnterEntityNameAsync(entityName);
        }

        [Given("I click on search button in the Entity page")]
        [When("I click on search button in the Entity page")]
        [Then("I click on search button in the Entity page")]
        public async Task IClickOnSearchButtonInTheEntityPage()
        {
            await _hubAdminUsersPortalPage.ClickSearchButtonInEntityPageAsync();
        }

        [Given("I select the entity {string}")]
        [When("I select the entity {string}")]
        [Then("I select the entity {string}")]
        public async Task ISelectTheEntity(string entityName)
        {
            await _hubAdminUsersPortalPage.SelectEntityByNameAsync(entityName);
        }

        [Given("I click on Continue button in the entity Page")]
        [When("I click on Continue button in the entity Page")]
        [Then("I click on Continue button in the entity Page")]
        public async Task IClickOnContinueButtonInTheEntityPage()
        {
            await _hubAdminUsersPortalPage.ClickContinueButtonInEntityPageAsync();
        }

        [Given("I click on approve access button")]
        [When("I click on approve access button")]
        [Then("I click on approve access button")]
        public async Task IClickOnApproveAccessButton()
        {
            await _hubAdminUsersPortalPage.ClickApproveAccessButtonAsync();
        }

    }
}
