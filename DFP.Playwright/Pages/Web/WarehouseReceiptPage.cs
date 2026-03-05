using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class WarehouseReceiptPage : BasePage
    {
        private string _warehouseReceiptName = string.Empty;

        public WarehouseReceiptPage(IPage page) : base(page)
        {
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // WR number search input on the WR list page — verified live: label "Warehouse Receipt #"
        // URL: /my-portal/warehouse-receipts/list
        private static readonly string[] WrReferenceInputSelectors =
        {
            "internal:role=textbox[name=\"Warehouse Receipt #\"i]",
            "input[placeholder*='Warehouse Receipt' i]"
        };

        // Search button — verified live: role=button name="Search"
        private static readonly string[] SearchButtonSelectors =
        {
            "internal:role=button[name=\"Search\"i]",
            "button:has-text('Search')"
        };

        // Cargo Detail page heading — verified from HTML: <h3 class="font-weight-normal m-0">Your cargo detail</h3>
        private static readonly string[] CargoDetailHeadingSelectors =
        {
            "h3.font-weight-normal:has-text('Your cargo detail')",
            "//h3[normalize-space()='Your cargo detail']",
            "internal:role=heading[name=\"Your cargo detail\"i]"
        };

        // Cargo Detail WR search input — verified from HTML:
        // <input formcontrolname="warehouse_receipt_number" id="warehouse_receipt_number"
        //        placeholder="Warehouse receipt" class="p-inputtext p-component">
        private static readonly string[] CargoDetailWrInputSelectors =
        {
            "input#warehouse_receipt_number",
            "input[formcontrolname='warehouse_receipt_number']",
            "input[name='warehouse_receipt_number']",
            "input[placeholder='Warehouse receipt']"
        };

        // ── Navigation methods ────────────────────────────────────────────────────

        public async Task NavigateToWarehouseReceiptsListAsync()
        {
            // Verified URL: /my-portal/warehouse-receipts (redirects to /list)
            var origin = new Uri(Page.Url).GetLeftPart(UriPartial.Authority);
            await Page.GotoAsync(origin + "/my-portal/warehouse-receipts");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task NavigateToCargoDetailPageAsync()
        {
            // Verified URL: /my-portal/cargo-detail (Warehouse > Cargo Detail submenu)
            var origin = new Uri(Page.Url).GetLeftPart(UriPartial.Authority);
            await Page.GotoAsync(origin + "/my-portal/cargo-detail");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── State helpers ─────────────────────────────────────────────────────────

        public void SetWarehouseReceiptName(string name)
        {
            _warehouseReceiptName = name;
        }

        public string GetWarehouseReceiptName() => _warehouseReceiptName;

        // ── Search methods ────────────────────────────────────────────────────────

        public async Task EnterWarehouseReceiptNameInSearchFieldAsync()
        {
            var input = await FindLocatorAsync(WrReferenceInputSelectors);
            await TypeAsync(input, _warehouseReceiptName);
        }

        /// <summary>
        /// Waits for the "Your cargo detail" heading, then waits for the WR input
        /// to become enabled (it starts disabled while the page loads its default view),
        /// and finally types the WR name into it.
        /// HTML verified: input#warehouse_receipt_number placeholder="Warehouse receipt"
        /// </summary>
        public async Task EnterWarehouseReceiptNameInCargoDetailSearchFieldAsync()
        {
            // 1. Wait for page heading to confirm we are on Cargo Detail
            var heading = await FindLocatorAsync(CargoDetailHeadingSelectors, timeoutMs: 15000);
            await heading.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15000
            });

            // 2. Wait for the input to become enabled (disabled while "Loading default view...")
            var input = await FindLocatorAsync(CargoDetailWrInputSelectors, timeoutMs: 15000);
            await WaitForEnabledAsync(input, timeoutMs: 15000);

            // 3. Type the WR name
            await TypeAsync(input, _warehouseReceiptName);
        }

        public async Task ClickSearchButtonAsync()
        {
            var searchButton = await FindLocatorAsync(SearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }

        // ── Assertion methods ─────────────────────────────────────────────────────

        /// <summary>
        /// Verifies no WR is returned after searching by this WR number.
        /// When a WR has Exclude from Tracking = True, the list shows a "No warehouse receipts found" heading.
        /// NOTE: WR names are truncated in the UI (e.g. "TC39…") so we cannot check for the name as text.
        /// Verified live via MCP on the STG portal.
        /// </summary>
        public async Task TheWarehouseReceiptShouldNotAppearInResultsAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var noResults = await TryFindLocatorAsync(new[]
            {
                "//h5[contains(normalize-space(),'No warehouse receipts found')]",
                "internal:role=heading[name=\"No warehouse receipts found\"i]",
                "text=No warehouse receipts found"
            }, timeoutMs: 8000);
            Assert.IsNotNull(noResults,
                $"Expected 'No warehouse receipts found' heading in WR list after searching for '{_warehouseReceiptName}' " +
                $"(Exclude from Tracking = True), but results were returned. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies no cargo items are returned in Cargo Detail after searching by this WR number.
        /// When a WR has Exclude from Tracking = True, the search should return zero results,
        /// which renders a "Nothing found" row in the table.
        /// Verified live via MCP on the STG portal.
        /// </summary>
        public async Task TheWarehouseReceiptShouldNotBeDisplayedInCargoDetailAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(1500);

            // When no cargo items match the search, the table shows a "Nothing found" cell.
            // Verified live: rowgroup > row "Nothing found Clear filters" > cell > generic "Nothing found"
            var nothingFound = await TryFindLocatorAsync(new[]
            {
                "//table//td[contains(normalize-space(),'Nothing found')]",
                "//*[normalize-space()='Nothing found']",
                "text=Nothing found"
            }, timeoutMs: 18000);
            Assert.IsNotNull(nothingFound,
                $"Expected 'Nothing found' in Cargo Detail table after searching for WR '{_warehouseReceiptName}' " +
                $"(Exclude from Tracking = True), but results were returned. URL: {Page.Url}");
        }
    }
}
