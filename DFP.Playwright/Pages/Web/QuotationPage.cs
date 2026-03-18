using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class QuotationPage : BasePage
    {
        private string _originPort = string.Empty;
        private string _destinationPort = string.Empty;
        private string _firstVessel = string.Empty;
        private string _bookingVessel = string.Empty;
        private string _quoteId = string.Empty;
        private int _initialNotifications = 0;

        public QuotationPage(IPage page) : base(page)
        {
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // "Create Quotation" button — verified from HTML: div.btn-group > button.btn-primary text "Create Quotation"
        private static readonly string[] CreateQuotationButtonSelectors =
        [
            "//div[contains(@class,'btn-group')]//button[contains(@class,'btn-primary') and contains(normalize-space(),'Create')]",
            "button.btn-primary:has-text('Create')"
        ];

        // "Get an Instant Quotation" page heading — verified from HTML: h4.font-weight-normal
        private static readonly string[] QuotationPageHeadingSelectors =
        [
            "h4.font-weight-normal:has-text('Get an Instant Quotation')",
            "//h4[contains(normalize-space(),'Get an Instant Quotation')]"
        ];

        // Transport mode radio label — verified from HTML: label[input[name='transport_mode']] with span text e.g. "Ocean"
        // Used with string.Format to inject mode: Air | Ocean | Truck
        private const string TransportModeLabelXPath =
            "//label[.//input[@name='transport_mode'] and .//span[normalize-space()='{0}']]";

        // Load type radio label — verified from HTML: label[input[name='load_type']] with text e.g. "Full (FCL)"
        // Used with string.Format to inject loadType: Full (FCL) | Partial (LCL)
        private const string LoadTypeLabelXPath =
            "//label[.//input[@name='load_type'] and contains(normalize-space(),'{0}')]";

        // Port trigger input (readonly) — click to open port search panel; two exist: index 0 = origin, 1 = destination
        private const string PortClickToSearchSelector = "input[placeholder='Click to search...']";

        // Port search input inside the autocomplete panel — placeholder="Type to search..."
        private static readonly string[] PortTypeToSearchSelectors =
        [
            "input[placeholder='Type to search...']",
            "//input[@placeholder='Type to search...']"
        ];

        // First autocomplete result item — verified from HTML: li.p-autocomplete-item
        private static readonly string[] PortFirstAutocompleteItemSelectors =
        [
            "(//li[contains(@class,'p-autocomplete-item')])[1]",
            "li.p-autocomplete-item"
        ];

        // Continue button inside the port selection panel — verified from HTML: button[type='submit'].btn-primary
        private static readonly string[] PortContinueButtonSelectors =
        [
            "button[type='submit'].btn-primary",
            "//button[@type='submit' and contains(@class,'btn-primary')]"
        ];

        // "Continue your quote" span on the quotation form — verified from HTML: span text="Continue your quote"
        private static readonly string[] ContinueYourQuoteSelectors =
        [
            "//span[normalize-space()='Continue your quote']",
            "span:has-text('Continue your quote')"
        ];

        // Route summary paragraph — verified from HTML: p.mb-4.font-weight-light containing From/to spans
        private static readonly string[] RouteSummarySelectors =
        [
            "p.mb-4.font-weight-light",
            "//p[contains(@class,'mb-4') and contains(@class,'font-weight-light')]"
        ];

        // Calendar toggle button — verified from HTML: button.btn-outline-secondary.calendar
        private static readonly string[] CalendarButtonSelectors =
        [
            "button.btn-outline-secondary.calendar",
            "//button[contains(@class,'btn-outline-secondary') and contains(@class,'calendar')]"
        ];

        // Today's date cell in the datepicker — verified from HTML: span.p-datepicker-current-day
        private static readonly string[] TodayDateSelectors =
        [
            "span.p-datepicker-current-day",
            "//span[contains(@class,'p-datepicker-current-day')]"
        ];

        // Currency ng-select container — verified from HTML: ng-select[formcontrolname='currency']
        private static readonly string[] CurrencyDropdownSelectors =
        [
            "ng-select[formcontrolname='currency'] .ng-select-container",
            "//ng-select[@formcontrolname='currency']"
        ];

        // ── Navigation methods ────────────────────────────────────────────────────

        public async Task NavigateToQuotationsListAsync()
        {
            var origin = new Uri(Page.Url).GetLeftPart(UriPartial.Authority);
            await Page.GotoAsync(origin + "/my-portal/quotations");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Action methods ────────────────────────────────────────────────────────

        // Step: "I click on Create Quotation button"
        // Waits for the button to be enabled before clicking.
        public async Task ClickCreateQuotationButtonAsync()
        {
            var btn = await FindLocatorAsync(CreateQuotationButtonSelectors, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        // Step: "I should see the create Quotation Page"
        // Verified from HTML: h4.font-weight-normal "Get an Instant Quotation"
        public async Task ShouldSeeCreateQuotationPageAsync()
        {
            var heading = await TryFindLocatorAsync(QuotationPageHeadingSelectors, timeoutMs: 10000);
            Assert.IsNotNull(heading,
                $"Expected 'Get an Instant Quotation' heading to be visible. URL: {Page.Url}");
            Assert.IsTrue(await heading!.IsVisibleAsync(),
                "'Get an Instant Quotation' heading is not visible.");
        }

        // Step: "I click on {string} transport mode"
        // Verified from HTML: label > input[name='transport_mode'] + span with text "Air" | "Ocean" | "Truck"
        public async Task ClickTransportModeAsync(string mode)
        {
            var label = Page.Locator(string.Format(TransportModeLabelXPath, mode));
            await label.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 8000
            });
            await label.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I click on {string} load type"
        // Verified from HTML: label > input[name='load_type'] with text "Full (FCL)" | "Partial (LCL)"
        public async Task ClickLoadTypeAsync(string loadType)
        {
            var label = Page.Locator(string.Format(LoadTypeLabelXPath, loadType));
            await label.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 8000
            });
            await label.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I enter {string} as the Origin Port"
        // Clicks the first readonly port input, types the port name, selects the first autocomplete result,
        // then clicks Continue. Verified from HTML: input[placeholder='Click to search...'] (index 0 = origin)
        public async Task EnterOriginPortAsync(string port)
        {
            _originPort = port;
            var inputs = Page.Locator(PortClickToSearchSelector);
            await inputs.First.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
            await EnterPortSearchAndContinueAsync(port);
        }

        // Step: "I enter {string} as the Destination Port"
        // After origin is filled, its input gains ng-dirty/ng-touched.
        // The destination input is still ng-untouched ng-pristine — target it by those classes.
        // Verified from HTML: input[placeholder='Click to search...'].ng-untouched.ng-pristine
        public async Task EnterDestinationPortAsync(string port)
        {
            _destinationPort = port;
            var destInput = Page.Locator("input[placeholder='Click to search...'].ng-untouched.ng-pristine");
            await destInput.First.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
            await EnterPortSearchAndContinueAsync(port);
        }

        private async Task EnterPortSearchAndContinueAsync(string port)
        {
            var searchInput = await FindLocatorAsync(PortTypeToSearchSelectors, timeoutMs: 8000);
            await TypeAsync(searchInput, port);

            var firstItem = await FindLocatorAsync(PortFirstAutocompleteItemSelectors, timeoutMs: 8000);
            await ClickAsync(firstItem);
            await Page.WaitForTimeoutAsync(300);

            var continueBtn = await FindLocatorAsync(PortContinueButtonSelectors, timeoutMs: 8000);
            await ClickAndWaitForNetworkAsync(continueBtn);
        }

        // Step: "I click on Continue your quote"
        // Verified from HTML: span text="Continue your quote"
        public async Task ClickContinueYourQuoteAsync()
        {
            var btn = await FindLocatorAsync(ContinueYourQuoteSelectors, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        // Step: "I should see the Origin and Destination ports"
        // Verified from HTML: p.mb-4.font-weight-light containing "From" and "to" spans
        public async Task ShouldSeeOriginAndDestinationPortsAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var routeSummary = await TryFindLocatorAsync(RouteSummarySelectors, timeoutMs: 10000);
            Assert.IsNotNull(routeSummary,
                $"Expected route summary (From ... to ...) to be visible. URL: {Page.Url}");
            var text = await routeSummary!.InnerTextAsync();
            Assert.IsTrue(text.Contains("From") && text.Contains("to"),
                $"Expected route summary to contain 'From' and 'to'. Actual: '{text}'. URL: {Page.Url}");
        }

        // Step: "I click on the calendar"
        // Verified from HTML: button.btn-outline-secondary.calendar with fa-icon calendar
        public async Task ClickCalendarAsync()
        {
            var calendar = await FindLocatorAsync(CalendarButtonSelectors, timeoutMs: 8000);
            await ClickAsync(calendar);
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I select the date"
        // Selects today's date dynamically using the p-datepicker-current-day class.
        // Verified from HTML: span.p-datepicker-current-day (today is always pre-highlighted)
        public async Task SelectTodaysDateAsync()
        {
            var today = await FindLocatorAsync(TodayDateSelectors, timeoutMs: 8000);
            await ClickAsync(today);
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I click on currency"
        // Opens the Currency ng-select dropdown.
        // Verified from HTML: ng-select[formcontrolname='currency']
        public async Task ClickCurrencyDropdownAsync()
        {
            var dropdown = await FindLocatorAsync(CurrencyDropdownSelectors, timeoutMs: 8000);
            await ClickAsync(dropdown);
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I select {string} as the currency"
        // Clicks the matching option in the currency dropdown (e.g. "USD").
        // Verified from HTML: div.ng-option with text containing the currency code
        public async Task SelectCurrencyAsync(string currency)
        {
            var option = Page.Locator($"//div[contains(@class,'ng-option') and contains(normalize-space(),'{currency}')]").First;
            await option.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 8000
            });
            await option.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I select the container size {string}"
        // Verified from HTML: span[role='combobox'][aria-label='Container Size'] is the p-dropdown trigger.
        // Two spans share the text "Container Size"; we target the one with role=combobox to avoid strict-mode violation.
        // Parametrizable: "40' Container", "20' Container", etc.
        public async Task SelectContainerSizeAsync(string size)
        {
            // Target the exact combobox trigger (not the static label span)
            var trigger = Page.Locator("span[role='combobox'][aria-label='Container Size']");
            await trigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await trigger.ClickAsync();
            await Page.WaitForTimeoutAsync(500);

            var item = Page.Locator($"//li[contains(@class,'p-dropdown-item') and contains(normalize-space(),\"{size}\")]").First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await item.ScrollIntoViewIfNeededAsync();
            await item.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I select the container type {string}"
        // Verified from HTML: span[role='combobox'][aria-label='Container Type'] is the p-dropdown trigger.
        // Parametrizable: "All Types", "Reefer", etc.
        public async Task SelectContainerTypeAsync(string type)
        {
            var trigger = Page.Locator("span[role='combobox'][aria-label='Container Type']");
            await trigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await trigger.ClickAsync();
            await Page.WaitForTimeoutAsync(500);

            var item = Page.Locator($"//li[contains(@class,'p-dropdown-item') and contains(normalize-space(),\"{type}\")]").First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await item.ScrollIntoViewIfNeededAsync();
            await item.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I click on Commodity"
        // Opens the Commodity ng-select dropdown.
        // Verified from HTML: ng-select[formcontrolname='commodity'] .ng-select-container
        public async Task ClickCommodityDropdownAsync()
        {
            var trigger = Page.Locator("ng-select[formcontrolname='commodity'] .ng-select-container");
            await trigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await trigger.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I select the Commodity {string}"
        // Selects the matching option from the open Commodity dropdown.
        // Verified from HTML: div.ng-option with text e.g. "Freight All Kinds (FAK)"
        // Parametrizable: "Freight All Kinds (FAK)", "Agriculture (Fruits and Vegetables)", etc.
        public async Task SelectCommodityAsync(string commodity)
        {
            var option = Page.Locator($"//div[contains(@class,'ng-option') and contains(normalize-space(),\"{commodity}\")]").First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.ScrollIntoViewIfNeededAsync();
            await option.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // Step: "I click on Create quotation from details"
        // Verified from HTML: button.btn-lg.btn-primary > span "Create quotation"
        public async Task ClickCreateQuotationFromDetailsAsync()
        {
            var btn = Page.Locator("//button[contains(@class,'btn-lg') and contains(@class,'btn-primary') and .//span[normalize-space()='Create quotation']]");
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }

        // Step: "I should see the offers"
        // Waits for the "Schedules" tab to become visible — this is the actual signal that offers finished loading.
        // Carrier API calls can take several minutes, so timeout is 5 min with 2s polling to avoid browser pressure.
        public async Task ShouldSeeOffersAsync()
        {
            // The "Schedules" tab only renders once the carrier API returns results.
            // Using WaitForFunction with polling so we don't hammer the browser during a slow API wait.
            await Page.WaitForFunctionAsync(
                "() => [...document.querySelectorAll('span')].some(s => s.textContent.trim() === 'Schedules')",
                null, new PageWaitForFunctionOptions { Timeout = 300000, PollingInterval = 2000 });

            var schedulesTab = Page.Locator("//span[normalize-space()='Schedules']").First;
            Assert.IsTrue(await schedulesTab.IsVisibleAsync(),
                $"Expected 'Schedules' tab to be visible after offers loaded. URL: {Page.Url}");
        }

        // Step: "I filter By {string}"
        // Clicks "Clear all" to reset carrier filters, then checks the carrier checkbox by name.
        // Verified from HTML: a "Clear all" + label > div with carrier name text.
        public async Task FilterByCarrierAsync(string carrier)
        {
            var clearAll = Page.Locator("a:has-text('Clear all')").Nth(2);
            await clearAll.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await clearAll.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var carrierLabel = Page.Locator($"//label[.//div[contains(normalize-space(),\"{carrier}\")]]");
            await carrierLabel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await carrierLabel.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I select {string} schedules"
        // Clicks the Schedules tab to show schedule results for the filtered carrier.
        // Verified from HTML: span "Schedules"
        public async Task SelectCarrierSchedulesAsync()
        {
            var schedulesTab = Page.Locator("//span[normalize-space()='Schedules']").First;
            await schedulesTab.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await schedulesTab.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        // Step: "I should see the schedules"
        // Asserts "Next departures" span is visible.
        // Verified from HTML: span "Next departures"
        public async Task ShouldSeeSchedulesAsync()
        {
            var nextDep = Page.Locator("//span[normalize-space()='Next departures']");
            await nextDep.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await nextDep.IsVisibleAsync(),
                $"Expected 'Next departures' to be visible in schedules. URL: {Page.Url}");
        }

        // Step: "I select the schedules"
        // Clicks the first "Select" button in the schedules table.
        // Verified from HTML: button.btn-secondary.rounded-pill.btn-sm "Select"
        public async Task SelectScheduleAsync()
        {
            var selectBtn = Page.Locator("button.btn-secondary.rounded-pill.btn-sm").First;
            await selectBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(selectBtn);
        }

        // Step: "I confirm the operation"
        // Verified from HTML: button.confirm.btn-secondary "Confirm"
        public async Task ConfirmOperationAsync()
        {
            var confirmBtn = Page.Locator("button.confirm.btn-secondary");
            await confirmBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(confirmBtn);
        }

        // Step: "I should see the quotation details to send the booking"
        // Waits for the "Transport Instructions" nav tab (active) to appear — confirms quotation details loaded.
        // Verified from HTML: li.nav-item > a.nav-link.active containing "Transport Instructions"
        public async Task ShouldSeeQuotationDetailsAsync()
        {
            var transportTab = Page.Locator("//li[contains(@class,'nav-item')]//a[contains(@class,'nav-link') and contains(@class,'active') and contains(normalize-space(),'Transport Instructions')]");
            await transportTab.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await transportTab.IsVisibleAsync(),
                $"Expected 'Transport Instructions' active tab to be visible. URL: {Page.Url}");
        }

        // Step: "I store the first Vessel"
        // Reads the Vessel/Voyage text from the first row of the schedules table and stores it in _firstVessel.
        // Verified from HTML: td.d-none.d-md-table-cell.border-left in first tbody tr of qwyk-schedules-list table
        public async Task StoreFirstVesselAsync()
        {
            var vesselCell = Page.Locator(
                "(//qwyk-schedules-list//tbody//tr)[1]//td[contains(@class,'border-left') and contains(@class,'d-md-table-cell')]"
            ).First;
            await vesselCell.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            _firstVessel = (await vesselCell.InnerTextAsync()).Trim();
        }

        // Step: "I select first the schedule"
        // Waits for the first Select button to be enabled, then clicks it.
        // Verified from HTML: button.btn-secondary.rounded-pill.btn-sm "Select"
        public async Task SelectFirstScheduleAsync()
        {
            var selectBtn = Page.Locator("button.btn-secondary.rounded-pill.btn-sm").First;
            await selectBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(selectBtn, timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(selectBtn);
        }

        // Step: "I store the Vessel for the booking"
        // Reads the vessel name from the booking form input and stores it in _bookingVessel.
        // Verified from HTML: input[formcontrolname='vessel'][placeholder='Vessel']
        public async Task StoreVesselForBookingAsync()
        {
            var vesselInput = Page.Locator("input[formcontrolname='vessel']");
            await vesselInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            _bookingVessel = (await vesselInput.InputValueAsync()).Trim();
        }

        // Step: "I compare the Vessel with the Vessel schedule"
        // Asserts that the vessel stored from the booking form contains the vessel stored from the schedules table.
        public async Task CompareVesselWithScheduleAsync()
        {
            await Task.CompletedTask;
            Assert.IsTrue(
                _firstVessel.Contains(_bookingVessel, StringComparison.OrdinalIgnoreCase),
                $"Expected schedule vessel '{_firstVessel}' to contain booking vessel '{_bookingVessel}'."
            );
        }

        // ── TC4520: Quote ID ──────────────────────────────────────────────────────

        /// <summary>
        /// Reads the quote ID from the bold span in the quotation header (e.g. "QUO-02463 | ...")
        /// and stores it in _quoteId. Extracts only the "QUO-XXXXX" part before the pipe separator.
        /// Verified from HTML: span.font-weight-bold containing "QUO-NNNNN |"
        /// </summary>
        public async Task StoreQuoteIdAsync()
        {
            var quoteSpan = Page.Locator("span.font-weight-bold")
                .Filter(new LocatorFilterOptions { HasText = "QUO-" })
                .First;
            await quoteSpan.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var rawText = (await quoteSpan.InnerTextAsync()).Trim();
            var match = Regex.Match(rawText, @"QUO-\d+");
            Assert.IsTrue(match.Success, $"Could not extract quote ID from text '{rawText}'. URL: {Page.Url}");
            _quoteId = match.Value;
        }

        /// <summary>Returns the quote ID stored by StoreQuoteIdAsync (e.g. "QUO-02463").</summary>
        public string GetQuoteId() => _quoteId;

        private static string GetHubBaseUrl()
        {
            var url = Environment.GetEnvironmentVariable("HUB_BASE_URL")
                      ?? Environment.GetEnvironmentVariable("BASE_URL")
                      ?? "";
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            return url;
        }

        /// <summary>
        /// Enters the stored quote ID in the "Quotation #" search input and waits for results.
        /// Verified from HTML: input[formcontrolname='friendly_id'][placeholder='Quotation #']
        /// </summary>
        public async Task EnterQuotationIdInSearchAsync()
        {
            var input = Page.Locator("input[formcontrolname='friendly_id']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(_quoteId);
            await input.PressAsync("Enter");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the stored quote ID appears as a link in the results list.
        /// Verified from HTML: li[qwyk-quotations-list-item] > a with quote ID text
        /// </summary>
        public async Task ShouldSeeQuoteIdInResultsAsync()
        {
            var listItem = Page.Locator("li[qwyk-quotations-list-item]")
                .Filter(new LocatorFilterOptions { HasText = _quoteId });
            await listItem.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await listItem.First.IsVisibleAsync(),
                $"Expected quote '{_quoteId}' to appear in the results list. URL: {Page.Url}");
        }

        // ── TC145: Transaction role (Buyer / Seller) ──────────────────────────────

        /// <summary>
        /// Clicks the Buyer/Seller radio button label by its visible span text.
        /// Verified from HTML: label.btn-outline-secondary > input[formcontrolname='transaction'] + span[text]
        /// </summary>
        public async Task ClickTransactionRoleAsync(string role)
        {
            var label = Page.Locator(
                $"//label[contains(@class,'btn-outline-secondary') and .//span[normalize-space()='{role}']]");
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await label.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        // ── TC145: Notifications counter ──────────────────────────────────────────

        /// <summary>
        /// Reads the bell icon counter and stores it for later comparison.
        /// Verified from HTML: button.btn-icon > fa-layers > fa-layers-counter > span.fa-layers-counter
        /// </summary>
        public async Task StoreInitialNotificationsAsync()
        {
            var counter = Page.Locator("span.fa-layers-counter").First;
            await counter.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var text = (await counter.InnerTextAsync()).Trim();
            _initialNotifications = int.TryParse(text, out var n) ? n : 0;
        }

        /// <summary>
        /// Asserts the bell icon counter equals the stored initial value + 1.
        /// Verified from HTML: span.fa-layers-counter
        /// </summary>
        public async Task VerifyFinalNotificationsAsync()
        {
            var counter = Page.Locator("span.fa-layers-counter").First;
            await counter.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var text = (await counter.InnerTextAsync()).Trim();
            var finalCount = int.TryParse(text, out var n) ? n : 0;
            Assert.AreEqual(_initialNotifications + 1, finalCount,
                $"Expected notifications to be {_initialNotifications + 1} but got {finalCount}. URL: {Page.Url}");
        }

        // ── TC145: Portal quote status ────────────────────────────────────────────

        /// <summary>
        /// Verifies the status badge for the stored quote ID shows the expected status.
        /// Verified from HTML: li[qwyk-quotations-list-item] > span.badge with status text
        /// </summary>
        public async Task ShouldSeeQuoteStatusAsync(string status)
        {
            var listItem = Page.Locator("li[qwyk-quotations-list-item]")
                .Filter(new LocatorFilterOptions { HasText = _quoteId });
            await listItem.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var badge = listItem.First.Locator("span.badge")
                .Filter(new LocatorFilterOptions { HasText = status });
            Assert.IsTrue(await badge.IsVisibleAsync(),
                $"Expected quote '{_quoteId}' to have status '{status}'. URL: {Page.Url}");
        }

        // ── TC145: Hub quotation search ───────────────────────────────────────────

        /// <summary>
        /// Navigates to the Hub quotations list page.
        /// Verified from URL: HUB_BASE_URL/quotations/list
        /// </summary>
        public async Task NavigateToQuotationListInHubAsync()
        {
            var baseUrl = GetHubBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/quotations/list");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Waits for and clicks the System ID input in the Hub.
        /// Verified from HTML: input[id='friendly_id'][placeholder='System ID']
        /// </summary>
        public async Task ClickSystemIdInputInHubAsync()
        {
            var input = Page.Locator("input#friendly_id[placeholder='System ID']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClickAsync();
        }

        /// <summary>
        /// Types the stored quote ID into the Hub System ID input field.
        /// Verified from HTML: input[id='friendly_id'][placeholder='System ID']
        /// </summary>
        public async Task EnterQuoteIdInHubAsync()
        {
            var input = Page.Locator("input#friendly_id[placeholder='System ID']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(_quoteId);
        }

        /// <summary>
        /// Verifies the stored quote ID appears as a link in the Hub search results table.
        /// Verified from HTML: tbody > tr > td > a with quote ID text (e.g. "QUO-02487")
        /// </summary>
        public async Task QuoteShouldAppearInHubResultsAsync()
        {
            var link = Page.Locator($"tbody a:has-text('{_quoteId}')");
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await link.IsVisibleAsync(),
                $"Expected quote '{_quoteId}' to appear in the Hub results table. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the status badge in the Hub results row shows the expected status.
        /// Verified from HTML: span.status-badge with text e.g. "Booked"
        /// </summary>
        public async Task HubStatusShouldBeAsync(string status)
        {
            var row = Page.Locator("tbody tr").Filter(new LocatorFilterOptions { HasText = _quoteId });
            await row.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var badge = row.First.Locator("span.status-badge")
                .Filter(new LocatorFilterOptions { HasText = status });
            Assert.IsTrue(await badge.IsVisibleAsync(),
                $"Expected Hub status '{status}' for quote '{_quoteId}'. URL: {Page.Url}");
        }
    }
}
