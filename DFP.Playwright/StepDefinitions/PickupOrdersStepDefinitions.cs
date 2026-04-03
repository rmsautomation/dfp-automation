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
    }
}
