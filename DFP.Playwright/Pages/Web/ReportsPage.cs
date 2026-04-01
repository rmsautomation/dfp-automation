using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ReportsPage : BasePage
    {
        private readonly TestContext _tc;

        public ReportsPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Selectors ─────────────────────────────────────────────────────────────

        // <a href="/my-portal/reports/shipments">Shipments</a>
        private static readonly string[] ShipmentsReportNavSelectors =
        {
            "a[href='/my-portal/reports/shipments']",
            "//a[@href='/my-portal/reports/shipments']",
            "//a[contains(@href,'/reports/shipments')]"
        };

        // <a href="/my-portal/reports/warehouse-receipts">Warehouse Receipts</a>
        // Verified live via MCP: sidebar link href="/my-portal/reports/warehouse-receipts"
        private static readonly string[] WarehouseReceiptsReportNavSelectors =
        {
            "a[href='/my-portal/reports/warehouse-receipts']",
            "//a[@href='/my-portal/reports/warehouse-receipts']",
            "//a[contains(@href,'/reports/warehouse-receipts')]",
            "internal:role=link[name=\"Warehouse Receipts\"i]"
        };

        // <select name="dateRange" class="custom-select">
        private static readonly string[] DateRangeSelectSelectors =
        {
            "select[name='dateRange']",
            "//select[@name='dateRange']",
            "select.custom-select"
        };

        // <button type="button"><svg data-icon="calendar">
        private static readonly string[] CalendarButtonSelectors =
        {
            "button:has(svg[data-icon='calendar'])",
            "//button[.//*[@data-icon='calendar']]",
            "button[type='button']:has(svg[data-icon='calendar'])"
        };

        // <button><span class="p-button-label">Today</span></button>
        private static readonly string[] TodayButtonSelectors =
        {
            "button:has(span.p-button-label:text('Today'))",
            "//button[.//span[contains(@class,'p-button-label') and normalize-space()='Today']]",
            "button:has-text('Today')"
        };

        // <a href="/my-portal/reports/invoices">Invoices</a>
        private static readonly string[] InvoicesReportNavSelectors =
        {
            "a[href='/my-portal/reports/invoices']",
            "//a[@href='/my-portal/reports/invoices']",
            "//a[contains(@href,'/reports/invoices')]"
        };

        // <button class="btn btn-primary">Search</button>
        private static readonly string[] SearchButtonSelectors =
        {
            "button.btn-primary:has-text('Search')",
            "//button[contains(@class,'btn-primary') and normalize-space()='Search']",
            "button.btn.btn-primary:has-text('Search')"
        };

        // <button class="btn btn-outline-secondary btn-sm mr-2"><svg data-icon="floppy-disk">
        private static readonly string[] SaveReportButtonSelectors =
        {
            "button:has(svg[data-icon='floppy-disk'])",
            "//button[.//*[@data-icon='floppy-disk']]",
            "button.btn-outline-secondary:has(svg[data-icon='floppy-disk'])"
        };

        // ── Page methods ──────────────────────────────────────────────────────────

        public async Task IAmOnTheReportsPage()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/reports");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IClickOnShipmentsOption()
        {
            var shipmentsLink = await FindLocatorAsync(ShipmentsReportNavSelectors);
            await ClickAndWaitForNetworkAsync(shipmentsLink);
        }

        public async Task IClickOnWarehouseReceiptsOption()
        {
            var warehouseLink = await FindLocatorAsync(WarehouseReceiptsReportNavSelectors);
            await ClickAndWaitForNetworkAsync(warehouseLink);
        }

        // Navigates directly to the Warehouse Receipts report URL — verified live via MCP
        public async Task NavigateToWarehouseReceiptsReportAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/reports/warehouse-receipts");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IShouldSeeText(string expectedText)
        {
            var literal = ToXPathLiteral(expectedText);
            var textEl = await TryFindLocatorAsync(new[]
            {
                "internal:role=cell[name=\"We couldn't find any matching\"i]",
                "internal:text=/We couldn.?t find any matching report/i",
                $"//*[normalize-space()={literal}]",
                $"internal:text=\"{expectedText}\"i",
                $"//*[contains(normalize-space(),{literal})]"
            }, timeoutMs: 15000);
            Assert.IsNotNull(textEl,
                $"Expected to see text '{expectedText}' on the page but it was not found. URL: {Page.Url}");
        }

        public async Task IShouldNotSeeText(string unexpectedText)
        {
            var literal = ToXPathLiteral(unexpectedText);
            var textEl = await TryFindLocatorAsync(new[]
            {
                "internal:role=cell[name=\"We couldn't find any matching\"i]",
                "internal:text=/We couldn.?t find any matching report/i",
                $"//*[normalize-space()={literal}]",
                $"internal:text=\"{unexpectedText}\"i",
                $"//*[contains(normalize-space(),{literal})]"
            }, timeoutMs: 4000);
            Assert.IsNull(textEl,
                $"Expected text '{unexpectedText}' to NOT be on the page but it was found. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a given name does NOT appear anywhere in the report results table.
        /// Reused for both Shipments and Warehouse Receipts reports.
        /// </summary>
        public async Task TheNameShouldNotAppearInResults(string name, string entityLabel = "Record")
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var literal = ToXPathLiteral(name);
            var inResults = await TryFindLocatorAsync(new[]
            {
                $"//table//td[contains(normalize-space(),{literal})]",
                $"//*[contains(@class,'p-datatable')]//td[contains(normalize-space(),{literal})]",
                $"//*[contains(@class,'p-datatable-row')]//td[contains(normalize-space(),{literal})]",
                $"td:has-text('{name}')",
            }, timeoutMs: 4000);
            Assert.IsNull(inResults,
                $"{entityLabel} '{name}' was found in the report results but it should not appear (Exclude from Tracking = True). URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the shipment name does NOT appear anywhere in the report results table.
        /// More robust than checking for the "no results" message, which can vary or be flaky.
        /// </summary>
        public async Task TheShipmentNameShouldNotAppearInResults(string shipmentName)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var literal = ToXPathLiteral(shipmentName);
            var shipmentInResults = await TryFindLocatorAsync(new[]
            {
                $"//table//td[contains(normalize-space(),{literal})]",
                $"//*[contains(@class,'p-datatable')]//td[contains(normalize-space(),{literal})]",
                $"//*[contains(@class,'p-datatable-row')]//td[contains(normalize-space(),{literal})]",
                $"td:has-text('{shipmentName}')",
            }, timeoutMs: 4000);
            Assert.IsNull(shipmentInResults,
                $"Shipment '{shipmentName}' was found in the report results but it should not appear (shipment is hidden). URL: {Page.Url}");
        }

        public async Task ISelectPredefinedRangeWithText(string rangeText)
        {
            var select = await FindLocatorAsync(DateRangeSelectSelectors);
            // Values match the <option value="..."> attributes in the dateRange <select>
            var value = rangeText.Trim().ToLowerInvariant() switch
            {
                "last 7 days"  => "lastweek",
                "last 30 days" => "last30",
                "last 90 days" => "last90",
                "this month"   => "thismonth",
                "this year"    => "thisyear",
                "last month"   => "lastmonth",
                "last year"    => "lastyear",
                "custom"       => "custom",
                _              => rangeText.Trim()
            };
            await SelectOptionAsync(select, value);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IShouldSelectCustomOption()
        {
            var select = await FindLocatorAsync(DateRangeSelectSelectors);
            await SelectOptionAsync(select, "custom");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task ISelectTheCalendar()
        {
            var calendarButton = await FindLocatorAsync(CalendarButtonSelectors);
            await ClickAsync(calendarButton);
            await Page.WaitForTimeoutAsync(500);
        }

        public async Task IClickOnTodayOption()
        {
            var todayButton = await FindLocatorAsync(TodayButtonSelectors);
            await ClickAndWaitForNetworkAsync(todayButton);
        }

        public async Task IClickOnSearchButton()
        {
            var searchButton = await FindLocatorAsync(SearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }

        public async Task IShouldSeeTheSaveReportButton()
        {
            var saveButton = await TryFindLocatorAsync(SaveReportButtonSelectors, timeoutMs: 15000);
            Assert.IsNotNull(saveButton,
                $"Save report button (floppy-disk icon) was not found on the page. URL: {Page.Url}");
        }

        /// <summary>
        /// Navigates to the Reports Shipments page and clicks Search directly,
        /// representing the state where a search has already been executed.
        /// </summary>
        public async Task IAlreadyClickOnSearchButtonInReportsSection()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/reports/shipments");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var searchButton = await FindLocatorAsync(SearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }

        public async Task IClickOnInvoicesOption()
        {
            var invoicesLink = await FindLocatorAsync(InvoicesReportNavSelectors);
            await ClickAndWaitForNetworkAsync(invoicesLink);
        }

        /// <summary>
        /// Clicks the ng-select "Saved reports" dropdown and selects the option matching the given name.
        /// HTML: ng-select[name='selectedReport'] — opens options panel, clicks matching .ng-option
        /// </summary>
        public async Task SelectSavedReportAsync(string name)
        {
            var arrow = Page.Locator("ng-select[name='selectedReport'] .ng-arrow-wrapper");
            await arrow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(arrow, timeoutMs: 10000);
            await Page.WaitForTimeoutAsync(1000);
            await arrow.ClickAsync();

            var option = Page.Locator(".ng-option")
                .Filter(new LocatorFilterOptions { HasText = name })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await option.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Waits for at least one result row to appear in the report results table.
        /// HTML: table tbody tr (data rows, not header)
        /// </summary>
        public async Task ShouldSeeInvoicesInReportResultsAsync()
        {
            var firstRow = Page.Locator("table tbody tr").First;
            await firstRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            var count = await Page.Locator("table tbody tr").CountAsync();
            Assert.IsTrue(count > 0,
                $"Expected invoice rows in report results but none found. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks a button by text. For non-dialog buttons (Search, etc.) in Reports.
        /// </summary>
        public async Task ClickButtonAsync(string buttonText)
        {
            var btn = Page.Locator("button")
                .Filter(new LocatorFilterOptions { HasText = buttonText })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Clicks the "Download to Excel" button and captures the file download.
        /// Uses Playwright RunAndWaitForDownloadAsync — same pattern as HubRadarPage.
        /// </summary>
        public async Task<IDownload> ClickDownloadToExcelAsync()
        {
            var btn = Page.Locator("button")
                .Filter(new LocatorFilterOptions { HasText = "Download to Excel" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            return await Page.RunAndWaitForDownloadAsync(() => btn.ClickAsync());
        }

        /// <summary>
        /// Saves the download to a temp file, reads the xlsx via System.IO.Compression,
        /// counts all rows in sheet1 (including header), and asserts the count equals expectedRows.
        /// No extra NuGet needed — xlsx is a ZIP with XML inside.
        /// </summary>
        public static async Task VerifyExcelRowCountAsync(IDownload download, int expectedRows)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), download.SuggestedFilename);
            await download.SaveAsAsync(tempPath);

            try
            {
                using var archive = ZipFile.OpenRead(tempPath);
                var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml")
                    ?? throw new InvalidOperationException("sheet1.xml not found in downloaded xlsx.");

                using var stream = sheetEntry.Open();
                var doc = XDocument.Load(stream);
                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                var rowCount = doc.Descendants(ns + "row").Count();

                Assert.AreEqual(expectedRows, rowCount,
                    $"Expected {expectedRows} rows in Excel but found {rowCount}. File: {download.SuggestedFilename}");
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
