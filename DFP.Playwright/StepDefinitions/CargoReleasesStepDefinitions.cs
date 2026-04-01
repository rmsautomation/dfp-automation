using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class CargoReleasesStepDefinitions
    {
        private readonly CargoReleasesPage _cargoReleasesPage;

        public CargoReleasesStepDefinitions(CargoReleasesPage cargoReleasesPage)
        {
            _cargoReleasesPage = cargoReleasesPage;
        }

        [Given("I am on the Cargo Releases page")]
        [When("I am on the Cargo Releases page")]
        [Then("I am on the Cargo Releases page")]
        public async Task IAmOnTheCargoReleasesPage()
            => await _cargoReleasesPage.NavigateToCargoReleasesPageAsync();

        [Given("the Cargo Releases page should be visible")]
        [When("the Cargo Releases page should be visible")]
        [Then("the Cargo Releases page should be visible")]
        public async Task TheCargoReleasesPageShouldBeVisible()
            => await _cargoReleasesPage.VerifyCargoReleasesPageVisibleAsync();

        [Given("I search for the Cargo Release with value {string}")]
        [When("I search for the Cargo Release with value {string}")]
        [Then("I search for the Cargo Release with value {string}")]
        public async Task ISearchForTheCargoRelease(string value)
            => await _cargoReleasesPage.SearchCargoReleaseAsync(value);

        // Scoped to CargoReleases — generic click without dialog-open wait
        [Given("I click on {string} button"), Scope(Feature = "CargoReleases")]
        [When("I click on {string} button"), Scope(Feature = "CargoReleases")]
        [Then("I click on {string} button"), Scope(Feature = "CargoReleases")]
        public async Task IClickOnButtonCargoReleases(string buttonText)
            => await _cargoReleasesPage.ClickButtonAsync(buttonText);

        [Given("the Cargo Release should be visible in the List with text {string}")]
        [When("the Cargo Release should be visible in the List with text {string}")]
        [Then("the Cargo Release should be visible in the List with text {string}")]
        public async Task TheCargoReleaseShouldBeVisibleInTheList(string text)
            => await _cargoReleasesPage.VerifyCargoReleaseVisibleInListAsync(text);

        [Given("I select the Cargo Release from the list with text {string}")]
        [When("I select the Cargo Release from the list with text {string}")]
        [Then("I select the Cargo Release from the list with text {string}")]
        public async Task ISelectTheCargoReleaseFromTheList(string text)
            => await _cargoReleasesPage.SelectCargoReleaseFromListAsync(text);

        [Given("I should see the Cargo Release details page")]
        [When("I should see the Cargo Release details page")]
        [Then("I should see the Cargo Release details page")]
        public async Task IShouldSeeTheCargoReleaseDetailsPage()
            => await _cargoReleasesPage.VerifyCargoReleaseDetailsPageAsync();

        [Given("I should verify the CR INFO")]
        [When("I should verify the CR INFO")]
        [Then("I should verify the CR INFO")]
        public async Task IShouldVerifyTheCRInfo(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (label: r[0], value: r[1]));
            await _cargoReleasesPage.VerifyCRDetailsAsync(pairs);
        }

        [Given("I should see the uploaded file {string}"), Scope(Feature = "CargoReleases")]
        [When("I should see the uploaded file {string}"), Scope(Feature = "CargoReleases")]
        [Then("I should see the uploaded file {string}"), Scope(Feature = "CargoReleases")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _cargoReleasesPage.VerifyUploadedFileAsync(fileName);

        [Given("I should verify the WH {string} is linked to CR")]
        [When("I should verify the WH {string} is linked to CR")]
        [Then("I should verify the WH {string} is linked to CR")]
        public async Task IShouldVerifyTheWHIsLinkedToCR(string whText)
            => await _cargoReleasesPage.VerifyWHLinkedToCRAsync(whText);

        [Given("I click on the WH Link {string} in the cargo details page")]
        [When("I click on the WH Link {string} in the cargo details page")]
        [Then("I click on the WH Link {string} in the cargo details page")]
        public async Task IClickOnTheWHLinkInTheCargoDetailsPage(string whText)
            => await _cargoReleasesPage.ClickWHLinkInCargoDetailsAsync(whText);
    }
}
