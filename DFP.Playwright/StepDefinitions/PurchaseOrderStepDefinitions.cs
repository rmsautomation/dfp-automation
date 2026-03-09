using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class PurchaseOrderStepDefinitions
    {
        private readonly PurchaseOrderPage _purchaseOrderPage;

        public PurchaseOrderStepDefinitions(PurchaseOrderPage purchaseOrderPage)
        {
            _purchaseOrderPage = purchaseOrderPage;
        }

        [Given("I am on the purchase order list page")]
        public async Task IAmOnThePurchaseOrderListPage()
            => await _purchaseOrderPage.NavigateToPurchaseOrderListAsync();

        [When(@"I click on the ""Create New Purchase Order"" button")]
        public async Task IClickOnTheCreateNewPurchaseOrderButton()
            => await _purchaseOrderPage.ClickCreatePurchaseOrderButtonAsync();

        [Then("I should be on the purchase order creation page")]
        public async Task IShouldBeOnThePurchaseOrderCreationPage()
            => await _purchaseOrderPage.ShouldBeOnPurchaseOrderCreationPageAsync();

        [When("I enter the Purchase Order number")]
        [Then("I enter the Purchase Order number")]
        public async Task IEnterThePurchaseOrderNumber()
            => await _purchaseOrderPage.EnterPurchaseOrderNumberAsync();

        [When("I enter the buyer details")]
        [Then("I enter the buyer details")]
        public async Task IEnterTheBuyerDetails()
            => await _purchaseOrderPage.EnterBuyerDetailsAsync();

        [When("I select the currency")]
        [Then("I select the currency")]
        [Given("I select the currency")]
        public async Task ISelectTheCurrency()
            => await _purchaseOrderPage.SelectCurrencyAsync();

        [When("I enter the supplier details")]
        [Then("I enter the supplier details")]
        public async Task IEnterTheSupplierDetails()
            => await _purchaseOrderPage.EnterSupplierDetailsAsync();

        [Then("I select the Transport Mode {string}")]
        [When("I select the Transport Mode {string}")]
        public async Task ISelectTheTransportMode(string mode)
            => await _purchaseOrderPage.SelectTransportModeAsync(mode);

        [Then("I enter the Cargo Origin {string}")]
        [When("I enter the Cargo Origin {string}")]
        public async Task IEnterTheCargoOrigin(string origin)
            => await _purchaseOrderPage.EnterCargoOriginAsync(origin);

        [Then("I enter the Cargo destination {string}")]
        [When("I enter the Cargo destination {string}")]
        public async Task IEnterTheCargoDestination(string destination)
            => await _purchaseOrderPage.EnterCargoDestinationAsync(destination);

        [When("I click on Save button in the Purchase Order")]
        [Then("I click on Save button in the Purchase Order")]
        public async Task IClickOnSaveButtonInThePurchaseOrder()
            => await _purchaseOrderPage.ClickSaveButtonAsync();

        [Then("I should see the purchase order details")]
        public async Task IShouldSeeThePurchaseOrderDetails()
            => await _purchaseOrderPage.ShouldSeePurchaseOrderDetailsAsync();

        [When("I enter the purchase order number in the search")]
        [Then("I enter the purchase order number in the search")]
        public async Task IEnterThePurchaseOrderNumberInTheSearch()
            => await _purchaseOrderPage.EnterPurchaseOrderNumberInSearchAsync();

        [When("I click on PO search button")]
        [Then("I click on PO search button")]
        public async Task IClickOnSearchButton()
            => await _purchaseOrderPage.ClickSearchButtonAsync();

        [Then("I should see the purchase order number in the list")]
        public async Task IShouldSeeThePurchaseOrderNumberInTheList()
            => await _purchaseOrderPage.ShouldSeePurchaseOrderInListAsync();
    }
}
