using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class WarehouseReceiptPage : BasePage
    {
        private string _warehouseReceiptName = string.Empty;
        private string _tableViewColumnName = string.Empty;
        private readonly TestContext _tc;

        public WarehouseReceiptPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
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
            await Page.WaitForTimeoutAsync(5000);
            await Page.GotoAsync(PortalOrigin() + "/my-portal/warehouse-receipts");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task NavigateToCargoDetailPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/cargo-detail");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

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
        /// <summary>
        /// Verifies the WR row with the stored name is visible in the search results table.
        /// HTML: //tr[contains(@class,'warehouse-receipt-row')]
        /// </summary>
        public async Task TheWarehouseReceiptShouldAppearInSearchResultsAsync()
        {
            // Retry loop: check every 5 seconds for up to 3 minutes.
            // If the WR is not in results, click Search again and check again.
            const int retryIntervalMs = 5000;
            const int maxDurationMs = 180000; // 3 minutes
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            while (true)
            {
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var matchingRow = Page.Locator(
                    $"//tr[contains(@class,'warehouse-receipt-row') and contains(normalize-space(),'{_warehouseReceiptName}')]");
                var isVisible = await matchingRow.IsVisibleAsync();
                if (isVisible)
                    return;

                if (DateTime.UtcNow >= deadline)
                {
                    Assert.Fail(
                        $"Expected warehouse receipt '{_warehouseReceiptName}' to appear in search results after 3 minutes of retries. URL: {Page.Url}");
                }

                // Wait 5 seconds then click Search again.
                await Page.WaitForTimeoutAsync(retryIntervalMs);
                var searchButton = await TryFindLocatorAsync(SearchButtonSelectors, timeoutMs: 5000);
                if (searchButton != null)
                    await ClickAndWaitForNetworkAsync(searchButton);
            }
        }

        /// <summary>
        /// Waits up to 5 seconds for a div containing <paramref name="text"/> to appear in the WR list.
        /// If not found, clicks Search every 2 seconds for up to 3 minutes.
        /// HTML: &lt;div&gt;automation&lt;/div&gt;
        /// Uses contains() to tolerate partial/whitespace matches.
        /// </summary>
        public async Task TheWarehouseReceiptShouldAppearInSearchResultsInListAsync(string text)
        {
            const int initialWaitMs = 5000;
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000; // 3 minutes
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var resultDiv = Page.Locator($"//div[contains(normalize-space(),'{text}')]").First;

            // Initial 5-second wait
            await Page.WaitForTimeoutAsync(initialWaitMs);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            while (true)
            {
                if (await resultDiv.IsVisibleAsync())
                    return;

                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Element with text '{text}' did not appear in WR search results after 3 minutes. URL: {Page.Url}");

                var searchButton = await TryFindLocatorAsync(SearchButtonSelectors, timeoutMs: 3000);
                if (searchButton != null)
                    await ClickAndWaitForNetworkAsync(searchButton);

                await Page.WaitForTimeoutAsync(retryIntervalMs);
            }
        }

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

        // Step: "I check the custom field {string}"
        // Verifies a column with the given name exists in the table header.
        // HTML: <th role="columnheader"><div class="d-flex..."> ColumnName <div...>
        public async Task CheckCustomFieldColumnExistsAsync(string columnName)
        {
            // Multiple p-datatable tables can be on the same page, each with the same column header.
            // Use .First to avoid strict-mode violation — we only need to confirm it appears at least once.
            var header = Page.Locator(
                $"//th[@role='columnheader'][.//div[contains(@class,'d-flex') and contains(normalize-space(),'{columnName}')]]").First;
            await header.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await header.IsVisibleAsync(),
                $"Custom field column '{columnName}' not found in table header. URL: {Page.Url}");
        }

        // Step: "I check the following custom field values in the table view:"
        // Accepts a DataTable: | ColumnName | ExpectedValue |
        // Custom field cells load asynchronously (Angular *ngIf renders after an API call),
        // so we poll each cell with 500ms retries for up to 12s until the value appears.
        // Uses th.cellIndex (native DOM property) + row.cells[N] to guarantee alignment.
        public async Task CheckCustomFieldValuesInTableViewAsync(IEnumerable<(string columnName, string expectedValue)> columnValuePairs)
        {
            // Wait for the first data row to be present.
            var firstRow = Page.Locator("table.p-datatable-table tbody tr.warehouse-receipt-row").First;
            await firstRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });

            foreach (var (columnName, expectedValue) in columnValuePairs)
            {
                var safeCol = columnName.Replace("'", "\\'").ToLower();

                // Find column index via th.cellIndex — the DOM-native column index that
                // always aligns with tr.cells[N] regardless of spacer columns.
                var colIndex = await Page.EvaluateAsync<int>($@"
                    () => {{
                        const table = document.querySelector('table.p-datatable-table');
                        if (!table) return -1;
                        const th = Array.from(table.querySelectorAll('th')).find(h => {{
                            const d = h.querySelector('div.d-flex');
                            const label = d ? (d.childNodes[0]?.textContent ?? '') : (h.textContent ?? '');
                            return label.trim().toLowerCase().includes('{safeCol}');
                        }});
                        return th ? th.cellIndex : -1;
                    }}
                ");

                Assert.IsTrue(colIndex >= 0,
                    $"Column '{columnName}' not found in Table View headers. URL: {Page.Url}");

                // Poll with retries: cells render with empty Angular *ngIf placeholders first,
                // then re-render with actual values once the async API response arrives.
                var cellText = string.Empty;
                var deadline = DateTime.UtcNow.AddSeconds(12);
                while (DateTime.UtcNow < deadline && string.IsNullOrEmpty(cellText))
                {
                    cellText = await Page.EvaluateAsync<string>($@"
                        () => {{
                            const table = document.querySelector('table.p-datatable-table');
                            if (!table) return '';
                            const row = table.querySelector('tbody tr.warehouse-receipt-row');
                            if (!row) return '';
                            const cell = row.cells[{colIndex}];
                            return cell ? (cell.textContent ?? '').trim() : '';
                        }}
                    ");
                    if (string.IsNullOrEmpty(cellText))
                        await Page.WaitForTimeoutAsync(500);
                }

                Assert.IsTrue(cellText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                    $"Column '{columnName}': expected '{expectedValue}' but found '{cellText}'. URL: {Page.Url}");
            }
        }

        // ── TC2244: WH Cargo tab methods ─────────────────────────────────────────

        // Cargo nav tab: <a class="nav-link" href="...?view=cargo-items"><svg data-icon="list"/> Cargo </a>
        private static readonly string[] CargoTabNavSelectors =
        [
            "//a[contains(@href,'?view=cargo-items')][.//svg[@data-icon='list']]",
            "//a[contains(@href,'?view=cargo-items')]",
            "a.nav-link:has(svg[data-icon='list'])"
        ];

        // Attachments nav tab: <a class="nav-link" href="...?view=attachments"><svg data-icon="paperclip"/> Attachments </a>
        private static readonly string[] AttachmentsTabNavSelectors =
        [
            "//a[contains(@href,'?view=attachments')][.//svg[@data-icon='paperclip']]",
            "//a[contains(@href,'?view=attachments')]",
            "a.nav-link:has(svg[data-icon='paperclip'])"
        ];

        // Cargo Items heading: <h5 class="font-weight-normal m-0">Cargo Items</h5>
        private const string CargoItemsHeadingSelector = "h5.font-weight-normal.m-0";

        /// <summary>
        /// Clicks the Cargo nav tab on the WH receipt detail page.
        /// Verified from HTML: a.nav-link with href containing ?view=cargo-items and svg data-icon='list'.
        /// </summary>
        public async Task ClickCargoTabAsync()
        {
            var tab = await FindLocatorAsync(CargoTabNavSelectors, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(tab);
        }

        /// <summary>
        /// Clicks the Attachments nav tab on the WH receipt detail page.
        /// Verified from HTML: a.nav-link with href containing ?view=attachments and svg data-icon='paperclip'.
        /// </summary>
        public async Task ClickAttachmentsTabAsync()
        {
            var tab = await FindLocatorAsync(AttachmentsTabNavSelectors, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(tab);
        }

        /// <summary>
        /// Waits for the "Cargo Items" h5 heading to be enabled and visible.
        /// Verified from HTML: h5.font-weight-normal.m-0 containing "Cargo Items".
        /// </summary>
        public async Task VerifyCargoItemsHeadingAsync()
        {
            var heading = Page.Locator(CargoItemsHeadingSelector)
                .Filter(new LocatorFilterOptions { HasText = "Cargo Items" })
                .First;
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"'Cargo Items' heading not found. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the first link whose href contains '/{linkType}/' in the cargo item details.
        /// Verified from HTML: a[href*='/shipments/'] with fa-icon data-icon='link'.
        /// </summary>
        public async Task ClickLinkInCargoDetailsAsync(string linkType)
        {
            var link = Page.Locator($"//a[contains(@href,'/{linkType}/')]").First;
            await WaitForEnabledAsync(link, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(link);
        }

        // ── WR Search Results / Detail methods ───────────────────────────────────

        /// <summary>
        /// Clicks the WR row in the search results that exactly matches the given text.
        /// HTML: <div class="...">automation</div> inside a WR list item.
        /// Waits for the element to be enabled before clicking.
        /// </summary>
        public async Task SelectWarehouseReceiptByTextAsync(string text)
        {
            var item = Page.Locator($"//div[normalize-space(text())='{text}']").First;
            await WaitForEnabledAsync(item, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(item);
        }

        /// <summary>
        /// Waits for the WR detail page heading and verifies it contains "Warehouse receipt {name}".
        /// If name is empty, falls back to the stored _warehouseReceiptName.
        /// HTML: <h3 class="font-weight-normal"> Warehouse receipt TC923_3373 </h3>
        /// </summary>
        public async Task VerifyWarehouseReceiptDetailsHeadingAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = _warehouseReceiptName;

            var expected = $"Warehouse receipt {name}";
            var heading = Page.Locator("h3.font-weight-normal").Filter(new LocatorFilterOptions { HasText = expected }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected WR detail heading to contain '{expected}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Generic step: finds the label matching <paramref name="labelText"/> and verifies
        /// its sibling div contains <paramref name="expectedValue"/>.
        /// If <paramref name="expectedValue"/> is empty, falls back to the stored _warehouseReceiptName.
        /// XPath pattern: //label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyLabelHeaderContainsAsync(string labelText, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
                expectedValue = _warehouseReceiptName;

            var valueDiv = Page.Locator($"//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a custom field inside &lt;qwyk-custom-fields-view&gt;.
        /// HTML: label.small.font-weight-bold.m-0 + div.ng-star-inserted (sibling).
        /// XPath: //qwyk-custom-fields-view//label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyCustomFieldsLabelHeaderContainsAsync(string labelText, string expectedValue)
        {
            var valueDiv = Page.Locator(
                $"//qwyk-custom-fields-view//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Custom field '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple custom fields inside &lt;qwyk-custom-fields-view&gt; using a data table.
        /// Accepts pairs of (labelText, expectedValue).
        /// </summary>
        public async Task VerifyCustomFieldsLabelHeadersAsync(IEnumerable<(string labelText, string expectedValue)> pairs)
        {
            foreach (var (labelText, expectedValue) in pairs)
                await VerifyCustomFieldsLabelHeaderContainsAsync(labelText, expectedValue);
        }

        /// <summary>
        /// Verifies the total pieces text is visible in the WR cargo details.
        /// HTML: &lt;div&gt;503 pieces&lt;/div&gt;
        /// XPath: //div[normalize-space(text())='{text}']
        /// </summary>
        public async Task VerifyTotalPiecesInCargoDetailsAsync(string text)
        {
            var el = Page.Locator($"//div[normalize-space(text())='{text}']").First;
            await el.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await el.IsVisibleAsync(),
                $"Total pieces '{text}' not found in cargo details. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a commodity text is visible in the WR cargo details table.
        /// HTML: &lt;div class="col-3"&gt; UpdateCommodity &lt;/div&gt; (note surrounding spaces — normalize-space handles it).
        /// XPath: //div[contains(@class,'col-3') and normalize-space()='{commodity}']
        /// </summary>
        public async Task VerifyCommodityInCargoDetailsAsync(string commodity)
        {
            var cell = Page.Locator($"//div[contains(@class,'col-3') and normalize-space()='{commodity}']").First;
            await WaitForEnabledAsync(cell, timeoutMs: 15000);
            Assert.IsTrue(await cell.IsVisibleAsync(),
                $"Commodity '{commodity}' not found in cargo details. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a file name is visible in the attachments list.
        /// HTML: &lt;div&gt;test.jpg&lt;/div&gt;
        /// XPath: //div[normalize-space(text())='{fileName}']
        /// </summary>
        public async Task VerifyUploadedFileAsync(string fileName)
        {
            var fileItem = Page.Locator($"//div[normalize-space(text())='{fileName}']").First;
            await fileItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await fileItem.IsVisibleAsync(),
                $"Uploaded file '{fileName}' not found in the attachments list. URL: {Page.Url}");
        }

        /// <summary>
        /// Selects a rows-per-page number from the paginator dropdown.
        /// Clicks the chevron trigger inside p-paginator-rpp-options, then picks the matching option.
        /// HTML: div.p-paginator-rpp-options > div[role="button"][aria-label="dropdown trigger"]
        ///       li[role="option"] span containing the number text.
        /// </summary>
        public async Task SelectPaginationNumberAsync(string number)
        {
            var trigger = Page.Locator(
                "//div[contains(@class,'p-paginator-rpp-options')]//div[@role='button' and @aria-label='dropdown trigger']").First;
            await WaitForEnabledAsync(trigger, timeoutMs: 10000);
            await trigger.ClickAsync();

            var option = Page.Locator(
                $"//li[@role='option'][.//span[normalize-space()='{number}']]").First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies a charge row contains the expected charge name and amount.
        /// HTML: li.list-group-item > div.row > div.col-4 (name) + div.col-2.text-right (amount)
        /// XPath: //li[.//div[col-4 and name]][.//div[text-right and amount]]
        /// </summary>
        public async Task VerifyChargeAmountAsync(string amount, string chargeName)
        {
            var chargeRow = Page.Locator(
                $"//li[contains(@class,'list-group-item')]" +
                $"[.//div[contains(@class,'col-4') and normalize-space()='{chargeName}']]" +
                $"[.//div[contains(@class,'text-right') and normalize-space()='{amount}']]").First;
            await WaitForEnabledAsync(chargeRow, timeoutMs: 15000);
            Assert.IsTrue(await chargeRow.IsVisibleAsync(),
                $"Charge '{chargeName}' with amount '{amount}' not found. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies parties displayed inside &lt;qwyk-entities-index&gt; on the WR detail page.
        /// HTML structure per party:
        ///   &lt;li class="list-group-item"&gt;
        ///     &lt;div class="small text-muted"&gt; Carrier &lt;/div&gt;   ← party type (has surrounding spaces)
        ///     &lt;div class="text-truncate font-weight-bold"&gt;MSC&lt;/div&gt;  ← party name
        ///   &lt;/li&gt;
        /// </summary>
        public async Task VerifyPartiesAsync(IEnumerable<(string partyType, string partyName)> pairs)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            foreach (var (partyType, partyName) in pairs)
            {
                var partyItem = Page.Locator(
                    $"//li[contains(@class,'list-group-item')]" +
                    $"[.//div[contains(@class,'text-muted') and normalize-space()='{partyType}']]" +
                    $"[.//div[contains(@class,'font-weight-bold') and normalize-space()='{partyName}']]").First;
                await partyItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                Assert.IsTrue(await partyItem.IsVisibleAsync(),
                    $"Party '{partyType}: {partyName}' not found in the Parties section. URL: {Page.Url}");
            }
        }
    }
}
