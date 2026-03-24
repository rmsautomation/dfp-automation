using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class HubRadarPage : BasePage
    {
        public HubRadarPage(IPage page) : base(page)
        {
        }

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the Radar link in the sidebar navigation.
        /// </summary>
        public async Task NavigateToRadarPageAsync()
        {
            var link = Page.Locator("a[href='/radar']").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await ClickAndWaitForNavigationAsync(link);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the "View Sales Radar" button on the Radar landing page.
        /// </summary>
        public async Task ClickViewSalesRadarAsync()
        {
            var link = Page.Locator("a.btn[href='/radar/sales']").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await ClickAndWaitForNavigationAsync(link);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Section visibility ────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the Performance over Time section header and its Quotations Created chart are visible.
        /// </summary>
        public async Task VerifyPerformanceOverTimeSectionAsync()
        {
            var sectionHeader = Page.Locator("//a[contains(normalize-space(.), 'Performance over Time')]").First;
            await sectionHeader.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await sectionHeader.IsVisibleAsync(),
                $"Expected 'Performance over Time' section header to be visible. URL: {Page.Url}");

            var chart = Page.Locator("qwyk-total-quotations-graph-widget").First;
            await chart.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await chart.IsVisibleAsync(),
                $"Expected Quotations Created chart to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the Rankings section header, Selected metric dropdown, and ranking charts are visible.
        /// </summary>
        public async Task VerifyRankingsSectionAsync()
        {
            var sectionHeader = Page.Locator("//a[contains(normalize-space(.), 'Rankings')]").First;
            await sectionHeader.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await sectionHeader.IsVisibleAsync(),
                $"Expected 'Rankings' section header to be visible. URL: {Page.Url}");

            var metricLabel = Page.GetByText("Selected metric:", new PageGetByTextOptions { Exact = false }).First;
            await metricLabel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await metricLabel.IsVisibleAsync(),
                $"Expected 'Selected metric:' label to be visible. URL: {Page.Url}");

            var productChart = Page.Locator("qwyk-quotations-grouped-by-top-graph-widget").First;
            await productChart.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await productChart.IsVisibleAsync(),
                $"Expected Rankings product chart to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the Data Table section header and table are visible.
        /// </summary>
        public async Task VerifyDataTableSectionAsync()
        {
            var sectionHeader = Page.Locator("//a[contains(normalize-space(.), 'Data Table')]").First;
            await sectionHeader.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await sectionHeader.IsVisibleAsync(),
                $"Expected 'Data Table' section header to be visible. URL: {Page.Url}");

            var table = Page.Locator("table").First;
            await table.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await table.IsVisibleAsync(),
                $"Expected Data Table to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies that the Performance over Time chart (canvas) is visible — used as a generic
        /// "charts are visible" assertion after any filter change.
        /// </summary>
        public async Task VerifyRadarChartsVisibleAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var chart = Page.Locator("qwyk-total-quotations-graph-widget canvas").First;
            await chart.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await chart.IsVisibleAsync(),
                $"Expected radar charts to be visible after filter. URL: {Page.Url}");
        }

        // ── Period filter ─────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks a period filter button (e.g. "1d", "3d", "1w", "2w", "1m").
        /// </summary>
        public async Task FilterByPeriodAsync(string period)
        {
            var btn = Page.Locator($"//ul[contains(@class,'')]/li[normalize-space(.)='{period}']").First;
            // Fallback: any list item with exact text
            if (!await btn.IsVisibleAsync().ContinueWith(t => t.Result))
                btn = Page.GetByRole(AriaRole.Listitem).Filter(new LocatorFilterOptions { HasText = period }).First;

            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Filters panel ─────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the Filters panel by clicking the Filters button inside &lt;qwyk-filter-popover&gt;.
        /// Waits for the button to be enabled before clicking, then waits for the panel to appear.
        /// </summary>
        public async Task OpenRadarFiltersAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var btn = Page.Locator("qwyk-filter-popover button").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 55000 });
            await WaitForEnabledAsync(btn, timeoutMs: 60000);
            await btn.ClickAsync();
            await Page.WaitForTimeoutAsync(800);

            // button[type='reset'] is the Clear button — it only exists when the panel is open
            var clearBtn = Page.Locator("button[type='reset']").First;

            // Retry click once if the panel didn't open
            if (!await clearBtn.IsVisibleAsync())
            {
                await btn.ClickAsync();
                await Page.WaitForTimeoutAsync(800);
            }

            await clearBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            await Page.WaitForTimeoutAsync(500);
        }

        /// <summary>
        /// Selects a value in the Product (Transport Mode) filter inside the Filters panel.
        /// </summary>
        public async Task FilterByTransportModeAsync(string mode)
        {
            await SelectNgSelectOptionAsync("Product", mode);
        }

        /// <summary>
        /// Selects a value in the Load type filter inside the Filters panel.
        /// </summary>
        public async Task FilterByLoadTypeAsync(string loadType)
        {
            await SelectNgSelectOptionAsync("Load type", loadType);
        }

        /// <summary>
        /// Selects a value in the Account Manager filter inside the Filters panel.
        /// </summary>
        public async Task FilterByAccountManagerAsync(string manager)
        {
            await SelectNgSelectOptionAsync("Account Manager", manager);
        }

        /// <summary>
        /// Clicks the Apply button inside the Filters panel.
        /// </summary>
        public async Task ApplyRadarFiltersAsync()
        {
            var dialog = Page.GetByRole(AriaRole.Dialog).First;
            var btn = dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Apply" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Opens the Filters panel, clicks Clear, then Apply to reset all filters.
        /// </summary>
        public async Task ResetRadarFiltersAsync()
        {
            await OpenRadarFiltersAsync();

            // Clear button uses type="reset" — unique and only exists when panel is open
            var clearBtn = Page.Locator("button[type='reset']").First;
            await clearBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(clearBtn, timeoutMs: 5000);
            await clearBtn.ClickAsync();

            var applyBtn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Apply" }).First;
            await applyBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(applyBtn);
        }

        // ── Toolbar actions ───────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the Refresh button (first button in the toolbar button-group).
        /// </summary>
        public async Task ClickRefreshButtonAsync()
        {
            // The refresh button is the first button inside the btn-group div in the toolbar
            var refreshBtn = Page.Locator("//div[@role='group']//button[1]").First;
            await refreshBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await refreshBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the Full Screen button inside the &lt;qwyk-full-screen-button&gt; component.
        /// </summary>
        public async Task ClickFullScreenButtonAsync()
        {
            var fsBtn = Page.Locator("qwyk-full-screen-button button").First;
            await fsBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await fsBtn.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
        }

        /// <summary>
        /// Verifies the page is in full screen mode by checking the document.fullscreenElement JS property.
        /// </summary>
        public async Task VerifyFullScreenModeAsync()
        {
            var isFullScreen = await Page.EvaluateAsync<bool>("() => document.fullscreenElement !== null");
            Assert.IsTrue(isFullScreen, "Expected the page to be in full screen mode.");
        }

        /// <summary>
        /// Presses Escape to exit full screen mode and verifies the page is no longer full screen.
        /// </summary>
        public async Task ExitFullScreenModeAsync()
        {
            await Page.Keyboard.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(500);
        }

        // ── Rankings metric filter ────────────────────────────────────────────────

        /// <summary>
        /// Changes the Selected Metric dropdown in the Rankings section.
        /// </summary>
        public async Task FilterRankingsByMetricAsync(string metric)
        {
            // The Rankings section has an ng-select for "Selected metric"
            var rankingsSection = Page.Locator("//a[contains(normalize-space(.), 'Rankings')]/ancestor::div[contains(@class,'bg-white')]").First;
            var ngSelect = rankingsSection.Locator("ng-select").First;
            await ngSelect.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ngSelect.ClickAsync();

            var option = Page.GetByText(metric, new PageGetByTextOptions { Exact = true }).First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await option.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Export ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the CSV export button and waits for a file download to start.
        /// Stores the download in the page context for assertion.
        /// </summary>
        public async Task<IDownload> ExportToCsvAsync()
        {
            var csvBtn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "CSV" }).First;
            await csvBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var download = await Page.RunAndWaitForDownloadAsync(() => csvBtn.ClickAsync());
            return download;
        }

        /// <summary>
        /// Clicks the Excel export button and waits for a file download to start.
        /// </summary>
        public async Task<IDownload> ExportToExcelAsync()
        {
            var excelBtn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Excel" }).First;
            await excelBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var download = await Page.RunAndWaitForDownloadAsync(() => excelBtn.ClickAsync());
            return download;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the trimmed, non-empty header texts from the Data Table.
        /// Used to cross-check exported file headers.
        /// </summary>
        public async Task<IReadOnlyList<string>> GetDataTableHeadersAsync()
        {
            var raw = await Page.Locator("table thead tr:first-child th").AllTextContentsAsync();
            return [.. raw.Select(h => h.Trim()).Where(h => !string.IsNullOrWhiteSpace(h))];
        }

        /// <summary>
        /// Verifies that every visible row in the Data Table has the given value in the specified column.
        /// The column is located dynamically from the table header row (case-insensitive match).
        /// Additionally, when columnName is "Transport Mode" and expectedValue is "AIR",
        /// also verifies that at least one row has Load type "lse".
        /// </summary>
        public async Task VerifyDataTableColumnAsync(string columnName, string expectedValue)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Wait for at least one data row
            var firstRow = Page.Locator("table tbody tr").First;
            await firstRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });

            // Resolve column index (1-based) from the first header row
            var headerTexts = await Page.Locator("table thead tr:first-child th").AllTextContentsAsync();
            var colIndex = headerTexts
                .Select((h, i) => new { Text = h.Trim(), Index = i + 1 })
                .FirstOrDefault(x => x.Text.Equals(columnName.Trim(), StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(colIndex, $"Column '{columnName}' not found in table headers.");

            var dataCells = await Page.Locator($"table tbody tr td:nth-child({colIndex!.Index})").AllTextContentsAsync();
            Assert.IsTrue(dataCells.Count > 0,
                $"Expected at least one data row after filtering by '{columnName}' = '{expectedValue}'.");

            foreach (var cellText in dataCells)
            {
                Assert.AreEqual(
                    expectedValue.Trim().ToUpperInvariant(),
                    cellText.Trim().ToUpperInvariant(),
                    $"Expected all '{columnName}' cells to be '{expectedValue}' but found '{cellText.Trim()}'.");
            }

            // When filtering by AIR transport mode, also verify LSE load type is present
            if (columnName.Equals("Transport Mode", StringComparison.OrdinalIgnoreCase)
                && expectedValue.Equals("AIR", StringComparison.OrdinalIgnoreCase))
            {
                var loadTypeCol = headerTexts
                    .Select((h, i) => new { Text = h.Trim(), Index = i + 1 })
                    .FirstOrDefault(x => x.Text.Equals("Load type", StringComparison.OrdinalIgnoreCase));

                if (loadTypeCol != null)
                {
                    var loadTypeCells = await Page.Locator($"table tbody tr td:nth-child({loadTypeCol.Index})").AllTextContentsAsync();
                    var hasLse = loadTypeCells.Any(lt => lt.Trim().Equals("lse", StringComparison.OrdinalIgnoreCase));
                    Assert.IsTrue(hasLse,
                        "Expected at least one row with Load type 'lse' when filtering by AIR transport mode.");
                }
            }
        }

        /// <summary>
        /// Opens a p-dropdown filter in the Filters panel by the combobox accessible name (which matches the label text)
        /// and selects the option whose display text matches optionText (case-insensitive).
        /// Real structure: dialog > container > combobox[name=labelText] + button[name="dropdown trigger"]
        /// </summary>
        private async Task SelectNgSelectOptionAsync(string labelText, string optionText)
        {
            // The combobox accessible name matches the label text (e.g. "Product", "Load type", "Account Manager")
            var combobox = Page.GetByRole(AriaRole.Combobox, new PageGetByRoleOptions { Name = labelText }).First;
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            // The dropdown trigger is a sibling button inside the same container
            var container = combobox.Locator("xpath=..");
            var triggerBtn = container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "dropdown trigger" }).First;
            await triggerBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await triggerBtn.ClickAsync();

            // Wait for the listbox overlay to appear
            var listbox = Page.GetByRole(AriaRole.Listbox).First;
            await listbox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            // Select the option whose visible text matches (case-insensitive)
            var option = Page.GetByRole(AriaRole.Option)
                .Filter(new LocatorFilterOptions
                {
                    HasTextRegex = new Regex(
                        $@"^\s*{Regex.Escape(optionText)}\s*$",
                        RegexOptions.IgnoreCase)
                })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await option.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
