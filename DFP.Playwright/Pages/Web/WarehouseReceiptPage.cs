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
        /// Verifies the WR number does NOT appear in the Warehouse Receipt list.
        /// Verified live: WR items render as list > listitem > link (not table rows).
        /// </summary>
        public async Task TheWarehouseReceiptShouldNotAppearInResultsAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var literal = ToXPathLiteral(_warehouseReceiptName);
            var wrInResults = await TryFindLocatorAsync(new[]
            {
                // WR numbers appear as links inside list items
                $"//a[normalize-space()={literal}]",
                $"//a[contains(normalize-space(),{literal})]",
                $"internal:role=link[name=\"{_warehouseReceiptName}\"i]"
            }, timeoutMs: 4000);
            Assert.IsNull(wrInResults,
                $"Warehouse Receipt '{_warehouseReceiptName}' was found in the list but it should not appear (Exclude from Tracking = True). URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the WR number does NOT appear in the Cargo Detail results after search.
        /// Waits for NetworkIdle first since the Cargo Detail results can take time to load.
        /// </summary>
        public async Task TheWarehouseReceiptShouldNotBeDisplayedInCargoDetailAsync()
        {
            // Cargo Detail can take a moment to render results after search
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(1000);

            var literal = ToXPathLiteral(_warehouseReceiptName);
            var wrInCargo = await TryFindLocatorAsync(new[]
            {
                $"//a[normalize-space()={literal}]",
                $"//a[contains(normalize-space(),{literal})]",
                $"internal:role=link[name=\"{_warehouseReceiptName}\"i]",
                $"//*[contains(normalize-space(),{literal})]"
            }, timeoutMs: 5000);
            Assert.IsNull(wrInCargo,
                $"Warehouse Receipt '{_warehouseReceiptName}' was found on the Cargo Detail page but it should not appear (Exclude from Tracking = True). URL: {Page.Url}");
        }
    }
}
