using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class HomeHubStepDefinitions
    {
        private readonly HomeHubPage _homeHubPage;

        public HomeHubStepDefinitions(HomeHubPage homeHubPage)
        {
            _homeHubPage = homeHubPage;
        }

        // ── TC280: Hub Home ────────────────────────────────────────────────────────

        [Given("I navigate to home in the Hub")]
        [When("I navigate to home in the Hub")]
        [Then("I navigate to home in the Hub")]
        public async Task INavigateToHomeInTheHub()
        {
            await _homeHubPage.NavigateToHomeInHubAsync();
        }

        [Given("I click on {string} button in the Hub")]
        [When("I click on {string} button in the Hub")]
        [Then("I click on {string} button in the Hub")]
        public async Task IClickOnButtonInTheHub(string buttonText)
        {
            await _homeHubPage.ClickQuickActionButtonInHubAsync(buttonText);
        }

        [Then("I should see the {string} page in the Hub")]
        public async Task IShouldSeeThePageInTheHub(string pageName)
        {
            await _homeHubPage.ShouldSeePageInHubAsync(pageName);
        }

        [Given("I should see the section header {string} in the Hub")]
        [When("I should see the section header {string} in the Hub")]
        [Then("I should see the section header {string} in the Hub")]
        public async Task IShouldSeeTheSectionHeaderInTheHub(string header)
        {
            await _homeHubPage.ShouldSeeSectionHeaderInHubAsync(header);
        }

        [Given("the {string} list should not be empty in the Hub")]
        [When("the {string} list should not be empty in the Hub")]
        [Then("the {string} list should not be empty in the Hub")]
        public async Task TheListShouldNotBeEmptyInTheHub(string section)
        {
            await _homeHubPage.SectionListShouldNotBeEmptyInHubAsync(section);
        }

        [Given("I click on the first View button in Recent Notifications in the Hub")]
        [When("I click on the first View button in Recent Notifications in the Hub")]
        [Then("I click on the first View button in Recent Notifications in the Hub")]
        public async Task IClickOnTheFirstViewButtonInRecentNotificationsInTheHub()
        {
            await _homeHubPage.ClickFirstViewButtonInNotificationsInHubAsync();
        }

        [Then("I should be redirected to a page with the Qwyk breadcrumb in the Hub")]
        public async Task IShouldBeRedirectedToAPageWithTheQwykBreadcrumbInTheHub()
        {
            await _homeHubPage.ShouldSeeQwykBreadcrumbInHubAsync();
        }
    }
}
