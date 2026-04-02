using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class InventoryStepDefinitions
    {
        private readonly InventoryPage _inventoryPage;

        public InventoryStepDefinitions(InventoryPage inventoryPage)
        {
            _inventoryPage = inventoryPage;
        }

        [Given("I am on the Inventory page")]
        [When("I am on the Inventory page")]
        [Then("I am on the Inventory page")]
        public async Task IAmOnTheInventoryPage()
            => await _inventoryPage.NavigateToInventoryPageAsync();

        [Given("the inventory page should be visible")]
        [When("the inventory page should be visible")]
        [Then("the inventory page should be visible")]
        public async Task TheInventoryPageShouldBeVisible()
            => await _inventoryPage.VerifyInventoryPageVisibleAsync();

        [Given("I search for the inventory item {string} with value {string}")]
        [When("I search for the inventory item {string} with value {string}")]
        [Then("I search for the inventory item {string} with value {string}")]
        public async Task ISearchForTheInventoryItem(string fieldLabel, string value)
            => await _inventoryPage.SearchInventoryItemAsync(fieldLabel, value);

        // Scoped to Inventory — generic click without dialog-open wait
        [Given("I click on {string} button"), Scope(Feature = "Inventory")]
        [When("I click on {string} button"), Scope(Feature = "Inventory")]
        [Then("I click on {string} button"), Scope(Feature = "Inventory")]
        public async Task IClickOnButtonInventory(string buttonText)
            => await _inventoryPage.ClickButtonAsync(buttonText);

        [Given("the inventory item should be visible in the List")]
        [When("the inventory item should be visible in the List")]
        [Then("the inventory item should be visible in the List")]
        public async Task TheInventoryItemShouldBeVisibleInTheList()
            => await _inventoryPage.VerifyInventoryItemVisibleInListAsync();

        [Given("I select the inventory item from the list with text {string}")]
        [When("I select the inventory item from the list with text {string}")]
        [Then("I select the inventory item from the list with text {string}")]
        public async Task ISelectTheInventoryItemFromTheList(string text)
            => await _inventoryPage.SelectInventoryItemFromListAsync(text);

        [Given("I should see the inventory item details page")]
        [When("I should see the inventory item details page")]
        [Then("I should see the inventory item details page")]
        public async Task IShouldSeeTheInventoryItemDetailsPage()
            => await _inventoryPage.VerifyInventoryItemDetailsPageAsync();

        [Given("I should verify the following inventory item details:")]
        [When("I should verify the following inventory item details:")]
        [Then("I should verify the following inventory item details:")]
        public async Task IShouldVerifyTheFollowingInventoryItemDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (label: r[0], value: r[1]));
            await _inventoryPage.VerifyInventoryDetailsAsync(pairs);
        }

        [Given("I should verify the total pieces in the inventory item details page is {string}")]
        [When("I should verify the total pieces in the inventory item details page is {string}")]
        [Then("I should verify the total pieces in the inventory item details page is {string}")]
        [Given("I should verify the total pieces in the cargo details is {string}")]
        [When("I should verify the total pieces in the cargo details is {string}")]
        [Then("I should verify the total pieces in the cargo details is {string}")]
        public async Task IShouldVerifyTheTotalPieces(string expectedCount)
            => await _inventoryPage.VerifyTotalPiecesAsync(expectedCount);

        [Given("I click on On Hand icon")]
        [When("I click on On Hand icon")]
        [Then("I click on On Hand icon")]
        public async Task IClickOnOnHandIcon()
            => await _inventoryPage.ClickOnHandIconAsync();

        [Given("I should see the text {string} in the cargo items column")]
        [When("I should see the text {string} in the cargo items column")]
        [Then("I should see the text {string} in the cargo items column")]
        public async Task IShouldSeeTheTextInTheCargoItemsColumn(string expectedText)
            => await _inventoryPage.VerifyCargoItemsColumnTextAsync(expectedText);
    }
}
