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
        private string _tableViewColumnName = string.Empty;

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

        // ── Table View – Customize selectors ─────────────────────────────────────

        // View selector dropdown — verified from HTML: span[role="combobox"][aria-label="Default View"]
        private static readonly string[] DropdwonCustomizeWHSelectors =
        {
            "//span[@role='combobox' and @aria-label='Default View']"
        };

        // "AutomationCustomize" option in the view dropdown — verified from HTML: span text="AutomationCustomize"
        private static readonly string[] AutomationCustomizeViewSelectors =
        {
            "//span[text()='AutomationCustomize']",
            "//span[text()='AutomationCustomize']/following::button[1]"
        };

        // Gear/Customize button next to Default View dropdown — verified live: button.btn-primary:has(svg[data-icon='gear'])
        // Only enabled when a custom view is selected (disabled for Default View)
        private static readonly string[] GearCustomizeButtonSelectors =
        {
            "button.btn-primary:has(svg[data-icon='gear'])",
            "//button[contains(@class,'btn-primary') and .//svg[@data-icon='gear']]"
        };

        // "Columns" tab inside the Customize panel
        private static readonly string[] ColumnsTabSelectors =
        {
            "internal:role=tab[name=\"Columns\"i]",
            "//li[@role='tab'][.//text()[normalize-space()='Columns']]",
            "//*[@role='tab'][normalize-space()='Columns']"
        };
        // Table view toggle button: <div class="p-element btn btn-outline-primary"><fa-icon data-icon="table">
        private static readonly string[] TableViewButtonSelectorsWH =
        [
            "//div[contains(@class,'btn-outline-primary') and contains(@class,'p-element')]",
            "div.btn-outline-primary:has(svg[data-icon='table'])",
            "//div[contains(@class,'btn-outline-primary') and .//svg[@data-icon='table']]"
        ];

        // Search input inside the Customize columns panel — placeholder="Search"
        private static readonly string[] ColumnSearchInputSelectors =
        [
            "input[placeholder='Search']",
            "//input[@placeholder='Search']"
        ];

        // Close (X) button of the Customize View sidebar — contains svg.p-sidebar-close-icon
        private static readonly string[] CustomizeViewCloseButtonSelectors =
        [
            "button:has(svg.p-sidebar-close-icon)",
            "//button[.//svg[contains(@class,'p-sidebar-close-icon')]]"
        ];

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

        // ── Table View – Customize methods ────────────────────────────────────────

        public async Task IClickOnTableViewButton()
        {
            var tableViewBtn = await FindLocatorAsync(TableViewButtonSelectorsWH, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(tableViewBtn);
        }
       
        public async Task ISelectAViewToEdit()
        {
            var dropdown = await FindLocatorAsync(DropdwonCustomizeWHSelectors, timeoutMs: 10000);
            await ClickAsync(dropdown);
            await Page.WaitForTimeoutAsync(500);

            var viewName = await FindLocatorAsync(AutomationCustomizeViewSelectors, timeoutMs: 5000);
            await ClickAsync(viewName);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Step: "I click on Configuration button"
        // Verified from HTML: button.btn-primary with fa-icon data-icon="gear"
        public async Task ClickConfigurationButtonAsync()
        {
            var gearBtn = await FindLocatorAsync(GearCustomizeButtonSelectors, timeoutMs: 10000);
            await ClickAsync(gearBtn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Step: "I click on Columns tab"
        // Verified from HTML: <span class="p-tabview-title">Columns</span>
        public async Task ClickColumnsTabAsync()
        {
            var columnsTab = await FindLocatorAsync(ColumnsTabSelectors, timeoutMs: 8000);
            await ClickAsync(columnsTab);
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I enter the column Name in the field"
        // Types "commodities" in the search input and asserts "Commodities Description" row appears.
        // Verified from HTML: input[placeholder="Search"] and <td> Commodities Description </td>
        public async Task EnterColumnNameInFieldAsync()
        {
            _tableViewColumnName = "Commodities Description";

            var searchInput = await FindLocatorAsync(ColumnSearchInputSelectors, timeoutMs: 8000);
            await TypeAsync(searchInput, "commodities");

            var columnRow = Page.Locator("//td[normalize-space()='Commodities Description']");
            await columnRow.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 8000
            });
            Assert.IsTrue(await columnRow.IsVisibleAsync(),
                "Expected 'Commodities Description' row to appear in the Columns search results.");
        }

        // Step: "I select the column Name"
        // Clicks the PrimeNG checkbox for "Commodities Description" only if not already checked.
        // Checked state is indicated by class "p-highlight" on div.p-checkbox-box.
        public async Task SelectColumnNameAsync()
        {
            var row = Page.Locator("//tr[.//td[normalize-space()='Commodities Description']]").First;
            var checkboxBox = row.Locator("div.p-checkbox-box").First;
            var classes = await checkboxBox.GetAttributeAsync("class") ?? "";

            if (!classes.Contains("p-highlight"))
            {
                await checkboxBox.ClickAsync();
                await Page.WaitForTimeoutAsync(300);
            }
        }

        // Step: "I close the Customize View"
        // Verified from HTML: button containing svg.p-sidebar-close-icon
        public async Task CloseCustomizeViewAsync()
        {
            var closeBtn = await FindLocatorAsync(CustomizeViewCloseButtonSelectors, timeoutMs: 8000);
            await ClickAsync(closeBtn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Step: "I should see the selected columns in the Table View"
        // Verified from HTML: th[role="columnheader"] containing "Commodities Description"
        public async Task ShouldSeeSelectedColumnsInTableViewAsync()
        {
            var col = string.IsNullOrEmpty(_tableViewColumnName) ? "Commodities Description" : _tableViewColumnName;

            var colHeader = await TryFindLocatorAsync(new[]
            {
                $"//th[@role='columnheader' and .//*[contains(normalize-space(),'{col}')]]",
                $"//th[contains(normalize-space(),'{col}')]",
                $"th:has-text('{col}')"
            }, timeoutMs: 8000);

            Assert.IsNotNull(colHeader,
                $"Expected column header '{col}' to be visible in the Table View. URL: {Page.Url}");
        }
    }
}
