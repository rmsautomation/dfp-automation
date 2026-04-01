using Reqnroll;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;
namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class WarehouseReceiptStepDefinitions
    {
        private readonly WarehouseReceiptPage _warehouseReceiptPage;
        private readonly DFP.Playwright.Support.TestContext _tc;

        public WarehouseReceiptStepDefinitions(WarehouseReceiptPage warehouseReceiptPage, DFP.Playwright.Support.TestContext tc)
        {
            _warehouseReceiptPage = warehouseReceiptPage;
            _tc = tc;
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
            if (string.IsNullOrEmpty(name) &&
                _tc.Data.TryGetValue("warehouseReceiptName", out var ctxName) &&
                ctxName is string wrName &&
                !string.IsNullOrEmpty(wrName))
            {
                name = wrName;
            }
            _warehouseReceiptPage.SetWarehouseReceiptName(name);
        }

        [Given("I enter the warehouse receipt name in search field")]
        [When("I enter the warehouse receipt name in search field")]
        [Then("I enter the warehouse receipt name in search field")]
        public async Task IEnterTheWarehouseReceiptNameInSearchField()
        {
            await _warehouseReceiptPage.EnterWarehouseReceiptNameInSearchFieldAsync();
        }

        [Given("I enter the warehouse receipt name in search field in Cargo Detail")]
        [When("I enter the warehouse receipt name in search field in Cargo Detail")]
        [Then("I enter the warehouse receipt name in search field in Cargo Detail")]
        public async Task IEnterTheWarehouseReceiptNameInCargoDetailSearchField()
        {
            await _warehouseReceiptPage.EnterWarehouseReceiptNameInCargoDetailSearchFieldAsync();
        }

        // ── Assertion steps ───────────────────────────────────────────────────────

        [Given("the warehouse receipt should appear in the search results")]
        [When("the warehouse receipt should appear in the search results")]
        [Then("the warehouse receipt should appear in the search results")]
        public async Task TheWarehouseReceiptShouldAppearInTheSearchResults()
        {
            await _warehouseReceiptPage.TheWarehouseReceiptShouldAppearInSearchResultsAsync();
        }

        [Given("the warehouse receipt should appear in the search results in the List with text {string}")]
        [When("the warehouse receipt should appear in the search results in the List with text {string}")]
        [Then("the warehouse receipt should appear in the search results in the List with text {string}")]
        public async Task TheWarehouseReceiptShouldAppearInTheSearchResultsInTheList(string text)
        {
            await _warehouseReceiptPage.TheWarehouseReceiptShouldAppearInSearchResultsInListAsync(text);
        }

        [Given("the warehouse receipt should not appear in the search results")]
        [When("the warehouse receipt should not appear in the search results")]
        [Then("the warehouse receipt should not appear in the search results")]
        public async Task TheWarehouseReceiptShouldNotAppearInTheSearchResults()
        {
            await _warehouseReceiptPage.TheWarehouseReceiptShouldNotAppearInResultsAsync();
        }

        [Given("the warehouse receipt should not be displayed in Cargo Detail")]
        [When("the warehouse receipt should not be displayed in Cargo Detail")]
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

        [Given("I select a view to edit")]
        [When("I select a view to edit")]
        [Then("I select a view to edit")]
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

        [Given("I should see the selected columns in the Table View")]
        [When("I should see the selected columns in the Table View")]
        [Then("I should see the selected columns in the Table View")]
        public async Task IShouldSeeTheSelectedColumnsInTheTableView()
        {
            await _warehouseReceiptPage.ShouldSeeSelectedColumnsInTableViewAsync();
        }

        [Given("I check the custom field {string}")]
        [When("I check the custom field {string}")]
        [Then("I check the custom field {string}")]
        public async Task ICheckTheCustomField(string columnName)
        {
            await _warehouseReceiptPage.CheckCustomFieldColumnExistsAsync(columnName);
        }

        [Given("I check the following custom field values in the table view:")]
        [When("I check the following custom field values in the table view:")]
        [Then("I check the following custom field values in the table view:")]
        public async Task ICheckCustomFieldValuesInTableView(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (columnName: r[0], expectedValue: r[1]));
            await _warehouseReceiptPage.CheckCustomFieldValuesInTableViewAsync(pairs);
        }

        // ── TC2244: WH Cargo tab steps ────────────────────────────────────────────

        [Given("I go to cargo tab")]
        [When("I go to cargo tab")]
        [Then("I go to cargo tab")]
        public async Task IGoToCargoTab()
            => await _warehouseReceiptPage.ClickCargoTabAsync();

        [Given("I should see the cargo items page")]
        [When("I should see the cargo items page")]
        [Then("I should see the cargo items page")]
        public async Task IShouldSeeTheCargoItemsPage()
            => await _warehouseReceiptPage.VerifyCargoItemsHeadingAsync();

        [Given("I click on the {string} link in the cargo item details")]
        [When("I click on the {string} link in the cargo item details")]
        [Then("I click on the {string} link in the cargo item details")]
        public async Task IClickOnTheLinkInTheCargoItemDetails(string linkType)
            => await _warehouseReceiptPage.ClickLinkInCargoDetailsAsync(linkType);

        [Given("I select the warehouse receipt in the search results with text {string}")]
        [When("I select the warehouse receipt in the search results with text {string}")]
        [Then("I select the warehouse receipt in the search results with text {string}")]
        public async Task ISelectTheWarehouseReceiptInTheSearchResultsWithText(string text)
            => await _warehouseReceiptPage.SelectWarehouseReceiptByTextAsync(text);

        [Given("the warehouse receipt details should be displayed with the name {string}")]
        [When("the warehouse receipt details should be displayed with the name {string}")]
        [Then("the warehouse receipt details should be displayed with the name {string}")]
        public async Task TheWarehouseReceiptDetailsShouldBeDisplayedWithTheName(string name)
            => await _warehouseReceiptPage.VerifyWarehouseReceiptDetailsHeadingAsync(name);

        [Given("I should verify label header {string} contains {string}")]
        [When("I should verify label header {string} contains {string}")]
        [Then("I should verify label header {string} contains {string}")]
        public async Task IShouldVerifyLabelHeaderContains(string labelText, string expectedValue)
            => await _warehouseReceiptPage.VerifyLabelHeaderContainsAsync(labelText, expectedValue);

        [Given("I should verify custom fields label header {string} contains {string}")]
        [When("I should verify custom fields label header {string} contains {string}")]
        [Then("I should verify custom fields label header {string} contains {string}")]
        public async Task IShouldVerifyCustomFieldsLabelHeaderContains(string labelText, string expectedValue)
            => await _warehouseReceiptPage.VerifyCustomFieldsLabelHeaderContainsAsync(labelText, expectedValue);

        [Given("I should verify the following custom field values in warehouse receipt details in DFP:")]
        [When("I should verify the following custom field values in warehouse receipt details in DFP:")]
        [Then("I should verify the following custom field values in warehouse receipt details in DFP:")]
        public async Task IShouldVerifyTheFollowingCustomFieldValuesInWarehouseReceiptDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (labelText: r[0], expectedValue: r[1]));
            await _warehouseReceiptPage.VerifyCustomFieldsLabelHeadersAsync(pairs);
        }

        [Given("I should verify the following label headers in warehouse receipt details:")]
        [When("I should verify the following label headers in warehouse receipt details:")]
        [Then("I should verify the following label headers in warehouse receipt details:")]
        public async Task IShouldVerifyTheFollowingLabelHeadersInWarehouseReceiptDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (labelText: r[0], expectedValue: r[1]));
            foreach (var (labelText, expectedValue) in pairs)
                await _warehouseReceiptPage.VerifyLabelHeaderContainsAsync(labelText, expectedValue);
        }

        [Given("I should verify the following parties in warehouse receipt details:")]
        [When("I should verify the following parties in warehouse receipt details:")]
        [Then("I should verify the following parties in warehouse receipt details:")]
        public async Task IShouldVerifyTheFollowingPartiesInWarehouseReceiptDetails(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (partyType: r[0], partyName: r[1]));
            await _warehouseReceiptPage.VerifyPartiesAsync(pairs);
        }

        [Given("I should see the amount {string} for the charge {string}")]
        [When("I should see the amount {string} for the charge {string}")]
        [Then("I should see the amount {string} for the charge {string}")]
        public async Task IShouldSeeTheAmountForTheCharge(string amount, string chargeName)
            => await _warehouseReceiptPage.VerifyChargeAmountAsync(amount, chargeName);

        [Given("I select the pagination number {string}")]
        [When("I select the pagination number {string}")]
        [Then("I select the pagination number {string}")]
        public async Task ISelectThePaginationNumber(string number)
            => await _warehouseReceiptPage.SelectPaginationNumberAsync(number);

        [Given("I should see the commodity {string} in cargo details warehouse")]
        [When("I should see the commodity {string} in cargo details warehouse")]
        [Then("I should see the commodity {string} in cargo details warehouse")]
        public async Task IShouldSeeTheCommodityInCargoDetailsWarehouse(string commodity)
            => await _warehouseReceiptPage.VerifyCommodityInCargoDetailsAsync(commodity);

        [Given("I should see the total pieces {string} in cargo details warehouse")]
        [When("I should see the total pieces {string} in cargo details warehouse")]
        [Then("I should see the total pieces {string} in cargo details warehouse")]
        public async Task IShouldSeeTheTotalPiecesInCargoDetailsWarehouse(string text)
            => await _warehouseReceiptPage.VerifyTotalPiecesInCargoDetailsAsync(text);

        [Given("I go to attachments tab")]
        [When("I go to attachments tab")]
        [Then("I go to attachments tab")]
        public async Task IGoToAttachmentsTab()
            => await _warehouseReceiptPage.ClickAttachmentsTabAsync();

        [Given("I should see the uploaded file {string}"), Scope(Feature = "Warehouse Receipts")]
        [When("I should see the uploaded file {string}"), Scope(Feature = "Warehouse Receipts")]
        [Then("I should see the uploaded file {string}"), Scope(Feature = "Warehouse Receipts")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _warehouseReceiptPage.VerifyUploadedFileAsync(fileName);
    }
}
