using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ReportsPage : BasePage
    {
        public ReportsPage(IPage page) : base(page)
        {
        }

        private static string GetPortalBaseUrl()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");
            return baseUrl;
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // <a href="/my-portal/reports/shipments">Shipments</a>
        private static readonly string[] ShipmentsReportNavSelectors =
        {
            "a[href='/my-portal/reports/shipments']",
            "//a[@href='/my-portal/reports/shipments']",
            "//a[contains(@href,'/reports/shipments')]"
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
            var baseUrl = GetPortalBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/reports");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IClickOnShipmentsOption()
        {
            var shipmentsLink = await FindLocatorAsync(ShipmentsReportNavSelectors);
            await ClickAndWaitForNetworkAsync(shipmentsLink);
        }

        public async Task IShouldSeeText(string expectedText)
        {
            var textEl = await TryFindLocatorAsync(new[]
            {
                $"//*[normalize-space()='{expectedText}']",
                $"internal:text=\"{expectedText}\"i",
                $"//*[contains(normalize-space(),'{expectedText}')]"
            }, timeoutMs: 15000);
            Assert.IsNotNull(textEl,
                $"Expected to see text '{expectedText}' on the page but it was not found. URL: {Page.Url}");
        }

        public async Task IShouldNotSeeText(string unexpectedText)
        {
            var textEl = await TryFindLocatorAsync(new[]
            {
                $"//*[normalize-space()='{unexpectedText}']",
                $"internal:text=\"{unexpectedText}\"i",
                $"//*[contains(normalize-space(),'{unexpectedText}')]"
            }, timeoutMs: 4000);
            Assert.IsNull(textEl,
                $"Expected text '{unexpectedText}' to NOT be on the page but it was found. URL: {Page.Url}");
        }

        public async Task ISelectPredefinedRangeWithText(string rangeText)
        {
            var select = await FindLocatorAsync(DateRangeSelectSelectors);
            var value = rangeText.Trim().ToLowerInvariant() switch
            {
                "last 7 days" => "lastweek",
                "last 30 days" => "lastmonth",
                "last 90 days" or "last quarter" => "lastquarter",
                "custom" => "custom",
                _ => rangeText.Trim()
            };
            await select.SelectOptionAsync(value);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IShouldSelectCustomOption()
        {
            var select = await FindLocatorAsync(DateRangeSelectSelectors);
            await select.SelectOptionAsync("custom");
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
            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");

            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/reports/shipments");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var searchButton = await FindLocatorAsync(SearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }
    }
}
