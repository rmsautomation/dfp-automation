using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class CustomersHubStepDefinitions
    {
        private readonly CustomersHubPage _customersHubPage;
        private readonly ScenarioContext _scenarioContext;

        public CustomersHubStepDefinitions(CustomersHubPage customersHubPage, ScenarioContext scenarioContext)
        {
            _customersHubPage = customersHubPage;
            _scenarioContext = scenarioContext;
        }

        // ── TC1482: Hub Create Customer ────────────────────────────────────────────

        [Given("I go to Portal Customers in the Hub")]
        [When("I go to Portal Customers in the Hub")]
        public async Task IGoToPortalCustomersInTheHub()
        {
            await _customersHubPage.NavigateToPortalCustomersInHubAsync();
        }

        [Given("I click on Create customer button")]
        [When("I click on Create customer button")]
        [Then("I click on Create customer button")]
        public async Task IClickOnCreateCustomerButton()
        {
            await _customersHubPage.ClickCreateCustomerButtonInHubAsync();
        }

        [Given("I enter the customer name {string} in the Hub")]
        [When("I enter the customer name {string} in the Hub")]
        [Then("I enter the customer name {string} in the Hub")]
        public async Task IEnterTheCustomerNameInTheHub(string name)
        {
            string resolvedName;
            if (string.IsNullOrEmpty(name))
            {
                resolvedName = $"AutoTest{DateTime.Now:yyyyMMddHHmmss}";
                _scenarioContext["CustomerName"] = resolvedName;
            }
            else
            {
                resolvedName = name;
            }
            Console.WriteLine($"[TC1482] Customer name: {resolvedName}");
            await _customersHubPage.EnterCustomerNameInHubAsync(resolvedName);
        }

        [Given("I select the type {string} in the Hub")]
        [When("I select the type {string} in the Hub")]
        [Then("I select the type {string} in the Hub")]
        public async Task ISelectTheTypeInTheHub(string type)
        {
            await _customersHubPage.SelectTypeInHubAsync(type);
        }

        [Given("I select the segment {string} in the Hub")]
        [When("I select the segment {string} in the Hub")]
        [Then("I select the segment {string} in the Hub")]
        public async Task ISelectTheSegmentInTheHub(string segment)
        {
            await _customersHubPage.SelectSegmentInHubAsync(segment);
        }

        [Given("I enter the customer name {string} in the search section in the Hub")]
        [When("I enter the customer name {string} in the search section in the Hub")]
        [Then("I enter the customer name {string} in the search section in the Hub")]
        public async Task IEnterTheCustomerNameInTheSearchSectionInTheHub(string name)
        {
            string resolvedName = string.IsNullOrEmpty(name)
                ? _scenarioContext["CustomerName"]?.ToString() ?? ""
                : name;
            await _customersHubPage.EnterCustomerNameInSearchAsync(resolvedName);
        }

        [Given("I click on Search button in Customers Hub")]
        [When("I click on Search button in Customers Hub")]
        [Then("I click on Search button in Customers Hub")]
        public async Task IClickOnSearchButton()
        {
            await _customersHubPage.ClickSearchButtonAsync();
        }

        [Given("I should see the customer name {string} in the results")]
        [When("I should see the customer name {string} in the results")]
        [Then("I should see the customer name {string} in the results")]
        public async Task IShouldSeeTheCustomerNameInTheResults(string name)
        {
            string resolvedName = string.IsNullOrEmpty(name)
                ? _scenarioContext["CustomerName"]?.ToString() ?? ""
                : name;
            await _customersHubPage.ShouldSeeCustomerNameInResultsAsync(resolvedName);
        }
    }
}
