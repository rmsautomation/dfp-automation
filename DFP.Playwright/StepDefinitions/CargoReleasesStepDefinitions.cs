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
            var pairs = dataTable.Rows.Select(r =>
            {
                var value = r[1].Replace("{cr_id}", _cargoReleasesPage.GetCRId());
                return (label: r[0], value);
            });
            await _cargoReleasesPage.VerifyCRDetailsAsync(pairs);
        }

        [Given("I should select {string} option")]
        [When("I should select {string} option")]
        [Then("I should select {string} option")]
        public async Task IShouldSelectOption(string optionText)
            => await _cargoReleasesPage.SelectDropdownOptionAsync(optionText);

        [Given("I should see the Create Cargo Release page with text {string}")]
        [When("I should see the Create Cargo Release page with text {string}")]
        [Then("I should see the Create Cargo Release page with text {string}")]
        public async Task IShouldSeeTheCreateCargoReleasePage(string headingText)
            => await _cargoReleasesPage.VerifyCreateCRPageAsync(headingText);

        [Given("I should select Release at {string} option")]
        [When("I should select Release at {string} option")]
        [Then("I should select Release at {string} option")]
        public async Task IShouldSelectReleaseAtOption(string optionText)
            => await _cargoReleasesPage.SelectReleaseAtOptionAsync(optionText);

        [Given("I should enter the name {string}")]
        [When("I should enter the name {string}")]
        [Then("I should enter the name {string}")]
        public async Task IShouldEnterTheName(string name)
            => await _cargoReleasesPage.EnterReleaseToNameAsync(name);

        [Given("I click on {string} button in the Create Cargo Release page")]
        [When("I click on {string} button in the Create Cargo Release page")]
        [Then("I click on {string} button in the Create Cargo Release page")]
        public async Task IClickOnButtonInCreateCRPage(string _)
            => await _cargoReleasesPage.ClickNextInCreateCRAsync();

        [Given("the item is loaded with text {string} in the Create Cargo Release page step2")]
        [When("the item is loaded with text {string} in the Create Cargo Release page step2")]
        [Then("the item is loaded with text {string} in the Create Cargo Release page step2")]
        public async Task TheItemIsLoadedInStep2(string text)
            => await _cargoReleasesPage.VerifyItemLoadedInStep2Async(text);

        [Given("I click on {string} button in the Create Cargo Release page step2")]
        [When("I click on {string} button in the Create Cargo Release page step2")]
        [Then("I click on {string} button in the Create Cargo Release page step2")]
        public async Task IClickOnButtonInCreateCRPageStep2(string buttonText)
        {
            if (buttonText.Contains("Load selected items", StringComparison.OrdinalIgnoreCase))
                await _cargoReleasesPage.ClickLoadSelectedItemsAsync();
            else
                await _cargoReleasesPage.ClickNextInCreateCRAsync();
        }

        [Given("I click on Next button in the Create Cargo Release page step2")]
        [When("I click on Next button in the Create Cargo Release page step2")]
        [Then("I click on Next button in the Create Cargo Release page step2")]
        public async Task IClickOnNextButtonInCreateCRPageStep2()
            => await _cargoReleasesPage.ClickNextInCreateCRStep2Async();

        [Given("I click on {string} button in the Create Cargo Release page step3")]
        [When("I click on {string} button in the Create Cargo Release page step3")]
        [Then("I click on {string} button in the Create Cargo Release page step3")]
        public async Task IClickOnButtonInCreateCRPageStep3(string _)
            => await _cargoReleasesPage.ClickSendCargoReleaseAsync();

        [Given("I enter the Warehouse receipt with number {string} in the Available cargo section")]
        [When("I enter the Warehouse receipt with number {string} in the Available cargo section")]
        [Then("I enter the Warehouse receipt with number {string} in the Available cargo section")]
        public async Task IEnterTheWarehouseReceiptInAvailableCargo(string value)
            => await _cargoReleasesPage.EnterAvailableCargoNumberAsync(value);

        [Given("I select the item with number {string} in the List in the Available cargo section")]
        [When("I select the item with number {string} in the List in the Available cargo section")]
        [Then("I select the item with number {string} in the List in the Available cargo section")]
        public async Task ISelectTheItemInAvailableCargo(string text)
            => await _cargoReleasesPage.SelectAvailableCargoItemAsync(text);

        [Given("I should see the text {string} in the confirmation message")]
        [When("I should see the text {string} in the confirmation message")]
        [Then("I should see the text {string} in the confirmation message")]
        public async Task IShouldSeeTheTextInTheConfirmationMessage(string expectedText)
            => await _cargoReleasesPage.VerifyConfirmationMessageAsync(expectedText);

        [Given("I store the Cargo Release number")]
        [When("I store the Cargo Release number")]
        [Then("I store the Cargo Release number")]
        public async Task IStoreTheCargoReleaseNumber()
            => await _cargoReleasesPage.StoreCRNumberAsync();

        [Given("I click on {string} button in the confirmation message")]
        [When("I click on {string} button in the confirmation message")]
        [Then("I click on {string} button in the confirmation message")]
        public async Task IClickOnButtonInConfirmationMessage(string buttonText)
            => await _cargoReleasesPage.ClickContinueToCargoReleaseAsync();

        [Given("I should select the country {string} for the CR")]
        [When("I should select the country {string} for the CR")]
        [Then("I should select the country {string} for the CR")]
        public async Task IShouldSelectTheCountryForTheCR(string countryName)
            => await _cargoReleasesPage.SelectCountryForCRAsync(countryName);

        [Given("I should see the event {string}"), Scope(Feature = "CargoReleases")]
        [When("I should see the event {string}"), Scope(Feature = "CargoReleases")]
        [Then("I should see the event {string}"), Scope(Feature = "CargoReleases")]
        public async Task IShouldSeeTheEvent(string eventText)
            => await _cargoReleasesPage.VerifyEventVisibleAsync(eventText);

        [Given("I should verify the status in {string}")]
        [When("I should verify the status in {string}")]
        [Then("I should verify the status in {string}")]
        public async Task IShouldVerifyTheStatusIn(string status)
            => await _cargoReleasesPage.VerifyStatusBadgeAsync(status);

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

        [Given("I should verify the PK {string} is linked to CR")]
        [When("I should verify the PK {string} is linked to CR")]
        [Then("I should verify the PK {string} is linked to CR")]
        public async Task IShouldVerifyThePKIsLinkedToCR(string pkText)
            => await _cargoReleasesPage.VerifyPKLinkedToCRAsync(pkText);

        [Given("I should see the pickup order details page with the name {string}")]
        [When("I should see the pickup order details page with the name {string}")]
        [Then("I should see the pickup order details page with the name {string}")]
        public async Task IShouldSeeThePickupOrderDetailsPage(string name)
            => await _cargoReleasesPage.VerifyPickupOrderDetailsPageAsync(name);

        [Given("I should verify the CR {string} is linked to WH")]
        [When("I should verify the CR {string} is linked to WH")]
        [Then("I should verify the CR {string} is linked to WH")]
        public async Task IShouldVerifyTheCRIsLinkedToWH(string crText)
        {
            var resolved = crText.Replace("{cr_id}", _cargoReleasesPage.GetCRId());
            await _cargoReleasesPage.VerifyCRLinkedToWHAsync(resolved);
        }

        [Given("I click on the WH Link {string} in the cargo details page")]
        [When("I click on the WH Link {string} in the cargo details page")]
        [Then("I click on the WH Link {string} in the cargo details page")]
        public async Task IClickOnTheWHLinkInTheCargoDetailsPage(string whText)
            => await _cargoReleasesPage.ClickWHLinkInCargoDetailsAsync(whText);

        [Given("I click on the PK Link {string} in the cargo details page")]
        [When("I click on the PK Link {string} in the cargo details page")]
        [Then("I click on the PK Link {string} in the cargo details page")]
        public async Task IClickOnThePKLinkInTheCargoDetailsPage(string pkText)
            => await _cargoReleasesPage.ClickPKLinkInCargoDetailsAsync(pkText);
    }
}
