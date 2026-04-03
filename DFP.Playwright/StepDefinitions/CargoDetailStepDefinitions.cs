using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class CargoDetailStepDefinitions
    {
        private readonly CargoDetailPage _cargoDetailPage;
        private readonly CargoReleasesPage _cargoReleasesPage;

        public CargoDetailStepDefinitions(CargoDetailPage cargoDetailPage, CargoReleasesPage cargoReleasesPage)
        {
            _cargoDetailPage = cargoDetailPage;
            _cargoReleasesPage = cargoReleasesPage;
        }

        [Given("I am on the Cargo Detail page")]
        [When("I am on the Cargo Detail page")]
        [Then("I am on the Cargo Detail page")]
        public async Task IAmOnTheCargoDetailPage()
            => await _cargoDetailPage.NavigateToCargoDetailPageAsync();

        [Given("I should see the Cargo Detail page")]
        [When("I should see the Cargo Detail page")]
        [Then("I should see the Cargo Detail page")]
        public async Task IShouldSeeTheCargoDetailPage()
            => await _cargoDetailPage.VerifyCargoDetailPageVisibleAsync();

        [Given("I search for Parent {string} with number {string} in the Cargo Detail page")]
        [When("I search for Parent {string} with number {string} in the Cargo Detail page")]
        [Then("I search for Parent {string} with number {string} in the Cargo Detail page")]
        public async Task ISearchForParentWithNumber(string parentType, string number)
            => await _cargoDetailPage.SearchForParentAsync(parentType, number);

        [Given("I click on {string} button"), Scope(Feature = "Cargo Details")]
        [When("I click on {string} button"), Scope(Feature = "Cargo Details")]
        [Then("I click on {string} button"), Scope(Feature = "Cargo Details")]
        public async Task IClickOnButton(string buttonText)
            => await _cargoDetailPage.ClickButtonAsync(buttonText);

        [Given("I click on {string} button in the Cargo Detail page")]
        [When("I click on {string} button in the Cargo Detail page")]
        [Then("I click on {string} button in the Cargo Detail page")]
        public async Task IClickOnButtonInCargoDetailPage(string buttonText)
            => await _cargoDetailPage.ClickButtonAsync(buttonText);

        [Given("I should see the search result with the description {string} and status {string} in the Cargo Detail page")]
        [When("I should see the search result with the description {string} and status {string} in the Cargo Detail page")]
        [Then("I should see the search result with the description {string} and status {string} in the Cargo Detail page")]
        public async Task IShouldSeeTheSearchResult(string description, string status)
            => await _cargoDetailPage.VerifySearchResultAsync(description, status);

        [Given("I enter the Cargo Release number {string} in the Cargo Releases search section")]
        [When("I enter the Cargo Release number {string} in the Cargo Releases search section")]
        [Then("I enter the Cargo Release number {string} in the Cargo Releases search section")]
        public async Task IEnterTheCargoReleaseNumber(string number)
        {
            var resolved = number.Replace("{cr_id}", _cargoReleasesPage.GetCRId());
            await _cargoDetailPage.EnterCargoReleaseNumberAsync(resolved);
        }

        [Given("I should see the Cargo Release with number {string} in the List")]
        [When("I should see the Cargo Release with number {string} in the List")]
        [Then("I should see the Cargo Release with number {string} in the List")]
        public async Task IShouldSeeTheCargoReleaseWithNumberInTheList(string number)
        {
            var resolved = number.Replace("{cr_id}", _cargoReleasesPage.GetCRId());
            await _cargoDetailPage.VerifyCargoReleaseVisibleInListAsync(resolved);
        }
    }
}
