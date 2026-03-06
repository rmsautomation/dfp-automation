using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class WarehouseReceiptStepDefinitions
    {
        private readonly WarehouseReceiptPage _warehouseReceiptPage;

        public WarehouseReceiptStepDefinitions(WarehouseReceiptPage warehouseReceiptPage)
        {
            _warehouseReceiptPage = warehouseReceiptPage;
        }

        // ── Navigation steps ──────────────────────────────────────────────────────

        [Given("I navigated to Warehouse Receipts List")]
        public async Task INavigatedToWarehouseReceiptsList()
        {
            await _warehouseReceiptPage.NavigateToWarehouseReceiptsListAsync();
        }

        [Given("I navigated to Cargo Detail Page")]
        public async Task INavigatedToCargoDetailPage()
        {
            await _warehouseReceiptPage.NavigateToCargoDetailPageAsync();
        }

        // ── Search steps ──────────────────────────────────────────────────────────

        [Given("I set the warehouse receipt name to {string}")]
        public void ISetTheWarehouseReceiptNameTo(string name)
        {
            _warehouseReceiptPage.SetWarehouseReceiptName(name);
        }

        [Given("I enter the warehouse receipt name in search field")]
        [When("I enter the warehouse receipt name in search field")]
        public async Task IEnterTheWarehouseReceiptNameInSearchField()
        {
            await _warehouseReceiptPage.EnterWarehouseReceiptNameInSearchFieldAsync();
        }

        [Then("I enter the warehouse receipt name in search field in Cargo Detail")]
        [When("I enter the warehouse receipt name in search field in Cargo Detail")]
        public async Task IEnterTheWarehouseReceiptNameInCargoDetailSearchField()
        {
            await _warehouseReceiptPage.EnterWarehouseReceiptNameInCargoDetailSearchFieldAsync();
        }

        // ── Assertion steps ───────────────────────────────────────────────────────

        [Then("the warehouse receipt should not appear in the search results")]
        public async Task TheWarehouseReceiptShouldNotAppearInTheSearchResults()
        {
            await _warehouseReceiptPage.TheWarehouseReceiptShouldNotAppearInResultsAsync();
        }

        [Then("the warehouse receipt should not be displayed in Cargo Detail")]
        public async Task TheWarehouseReceiptShouldNotBeDisplayedInCargoDetail()
        {
            await _warehouseReceiptPage.TheWarehouseReceiptShouldNotBeDisplayedInCargoDetailAsync();
        }

        // ── Table View – Customize steps ──────────────────────────────────────────

        [Given("I click on Table View in WH Receipt List")]
        [When("I click on Table View in WH Receipt List")]
        [Then("I click on Table View in WH Receipt List")]
        public async Task IClickOnTableViewButton()
        {
            await _warehouseReceiptPage.IClickOnTableViewButton();
        }
        [When("I select a view to edit")]
        public async Task ISelectAViewToEdit()
        {
            await _warehouseReceiptPage.ISelectAViewToEdit();
        }

        [Given("I click on Configuration button")]
        [When("I click on Configuration button")]
        [Then("I click on Configuration button")]
        public async Task IClickOnConfigurationButton()
        {
            await _warehouseReceiptPage.ClickConfigurationButtonAsync();
        }

        [Given("I click on Columns tab")]
        [When("I click on Columns tab")]
        [Then("I click on Columns tab")]
        public async Task IClickOnColumnsTab()
        {
            await _warehouseReceiptPage.ClickColumnsTabAsync();
        }

        [Given("I enter the column Name in the field")]
        [When("I enter the column Name in the field")]
        [Then("I enter the column Name in the field")]
        public async Task IEnterTheColumnNameInTheField()
        {
            await _warehouseReceiptPage.EnterColumnNameInFieldAsync();
        }

        [Given("I select the column Name")]
        [When("I select the column Name")]
        [Then("I select the column Name")]
        public async Task ISelectTheColumnName()
        {
            await _warehouseReceiptPage.SelectColumnNameAsync();
        }

        [Given("I close the Customize View")]
        [When("I close the Customize View")]
        [Then("I close the Customize View")]
        public async Task ICloseTheCustomizeView()
        {
            await _warehouseReceiptPage.CloseCustomizeViewAsync();
        }

        [Then("I should see the selected columns in the Table View")]
        public async Task IShouldSeeTheSelectedColumnsInTheTableView()
        {
            await _warehouseReceiptPage.ShouldSeeSelectedColumnsInTableViewAsync();
        }
    }
}
