using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class PickupOrdersStepDefinitions
    {
        private readonly PickupOrdersPage _pickupOrdersPage;

        public PickupOrdersStepDefinitions(PickupOrdersPage pickupOrdersPage)
        {
            _pickupOrdersPage = pickupOrdersPage;
        }

        [Given("I am on the Pickup Orders page")]
        [When("I am on the Pickup Orders page")]
        [Then("I am on the Pickup Orders page")]
        public async Task IAmOnThePickupOrdersPage()
            => await _pickupOrdersPage.NavigateToPickupOrdersPageAsync();

        [Given("I should see the Pickup Orders list")]
        [When("I should see the Pickup Orders list")]
        [Then("I should see the Pickup Orders list")]
        public async Task IShouldSeeThePickupOrdersList()
            => await _pickupOrdersPage.VerifyPickupOrdersListVisibleAsync();

        [Given("I enter the Pickup Order number {string} in the Pickup Orders section")]
        [When("I enter the Pickup Order number {string} in the Pickup Orders section")]
        [Then("I enter the Pickup Order number {string} in the Pickup Orders section")]
        public async Task IEnterThePickupOrderNumber(string number)
            => await _pickupOrdersPage.EnterPickupOrderNumberAsync(number);

        [Given("I click on {string} button"), Scope(Feature = "Pickup Orders")]
        [When("I click on {string} button"), Scope(Feature = "Pickup Orders")]
        [Then("I click on {string} button"), Scope(Feature = "Pickup Orders")]
        public async Task IClickOnButton(string buttonText)
            => await _pickupOrdersPage.ClickButtonAsync(buttonText);

        [Given("I should see the Pickup Order with number {string} in the List in the Available Pickup Orders section")]
        [When("I should see the Pickup Order with number {string} in the List in the Available Pickup Orders section")]
        [Then("I should see the Pickup Order with number {string} in the List in the Available Pickup Orders section")]
        public async Task IShouldSeeThePickupOrderInList(string number)
            => await _pickupOrdersPage.VerifyPickupOrderVisibleInListAsync(number);

        [Given("I click on the Pickup Order with number {string} in the Available Pickup Orders section")]
        [When("I click on the Pickup Order with number {string} in the Available Pickup Orders section")]
        [Then("I click on the Pickup Order with number {string} in the Available Pickup Orders section")]
        public async Task IClickOnThePickupOrderInList(string number)
            => await _pickupOrdersPage.ClickPickupOrderInListAsync(number);

        [Given("I should see the Pickup Order details page with number {string}")]
        [When("I should see the Pickup Order details page with number {string}")]
        [Then("I should see the Pickup Order details page with number {string}")]
        public async Task IShouldSeeThePickupOrderDetailsPage(string number)
            => await _pickupOrdersPage.VerifyPickupOrderDetailsPageAsync(number);

        [Given("I should see the uploaded file {string}"), Scope(Feature = "Pickup Orders")]
        [When("I should see the uploaded file {string}"), Scope(Feature = "Pickup Orders")]
        [Then("I should see the uploaded file {string}"), Scope(Feature = "Pickup Orders")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _pickupOrdersPage.VerifyUploadedFileAsync(fileName);

        [Given("I should verify the pickup order details")]
        [When("I should verify the pickup order details")]
        [Then("I should verify the pickup order details")]
        public async Task IShouldVerifyThePickupOrderDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (label: r[0], value: r[1]));
            await _pickupOrdersPage.VerifyPickupOrderDetailsAsync(pairs);
        }
    }
}
