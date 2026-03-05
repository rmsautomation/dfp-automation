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
    }
}
