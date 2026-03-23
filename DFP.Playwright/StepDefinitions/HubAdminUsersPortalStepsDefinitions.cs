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

        // ── TC1483: Portal login (individual steps) ────────────────────────────────

        [Given("I should see the login page")]
        [When("I should see the login page")]
        [Then("I should see the login page")]
        public async Task IShouldSeeTheLoginPage()
        {
            await _hubAdminUsersPortalPage.ShouldSeeLoginPageAsync();
        }

        [Given("I enter the created username {string} in the Portal")]
        [When("I enter the created username {string} in the Portal")]
        [Then("I enter the created username {string} in the Portal")]
        public async Task IEnterTheCreatedUsernameInThePortal(string username)
        {
            string resolved = string.IsNullOrEmpty(username)
                ? _scenarioContext["usernamePortal"]?.ToString() ?? ""
                : username;
            Console.WriteLine($"[TC1483] Portal username: {resolved}");
            await _hubAdminUsersPortalPage.FillPortalUsernameAsync(resolved);
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
            await _hubAdminUsersPortalPage.FillPortalPasswordAsync(resolved);
        }

        [Given("click on Sign in button")]
        [When("click on Sign in button")]
        [Then("click on Sign in button")]
        public async Task IClickOnSignInButton()
        {
            await _hubAdminUsersPortalPage.ClickPortalSignInAsync();
        }

        [Given("I login to Portal as the created user")]
        [When("I login to Portal as the created user")]
        [Then("I login to Portal as the created user")]
        public async Task ILoginToPortalAsTheCreatedUser()
        {
            var email    = _scenarioContext["ContactEmail"]?.ToString()   ?? "";
            var password = _scenarioContext["PortalPassword"]?.ToString() ?? "";
            Console.WriteLine($"[TC1483] Logging in to Portal as: {email}");
            await _hubAdminUsersPortalPage.LoginToPortalAsCreatedUserAsync(email, password);
        }
    }
}
