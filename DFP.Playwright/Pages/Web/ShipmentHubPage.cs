using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ShipmentHubPage : BasePage
    {
        // Stores the date selected in the milestone calendar for later assertion.
        // Format: "MMMM d, yyyy" (e.g., "March 6, 2026").
        private string _selectedDate = string.Empty;

        public ShipmentHubPage(IPage page) : base(page)
        {
        }

        private static string GetHubBaseUrl()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            return baseUrl;
        }

        private static readonly string[] HubCustomerReferenceInputSelectors =
        [
            "internal:role=textbox[name=\"Customer Reference\"i]",
            "input[placeholder*='customer reference' i]",
            "//input[contains(@placeholder,'customer reference') or contains(@placeholder,'Customer Reference')]"
        ];

        private static readonly string[] HubSearchButtonSelectors =
        [
            "internal:role=button[name=\"Search\"i]",
            "//button[normalize-space(text())='Search']",
            "button[type='submit']:has-text('Search')"
        ];

        /// <summary>
        /// Navigates directly to the Hub Shipments list via URL.
        /// GotoAsync + NetworkIdle is the correct pattern for cross-application navigation.
        /// </summary>
        public async Task INavigatedToShipmentListInTheHub()
        {
            var baseUrl = GetHubBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/shipments");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the Shipment Reference input to focus it.
        /// ClickAsync is correct — this is a simple focus, no navigation or API call.
        /// </summary>
        public async Task IClickOnCustomerReferenceInputFieldInTheHub()
        {
            var customerRef = await FindLocatorAsync(HubCustomerReferenceInputSelectors);
            await ClickAsync(customerRef);
        }

        /// <summary>
        /// Types the shipment name (retrieved from the portal step via the step definition) into the Hub reference field.
        /// </summary>
        public async Task IEnterTheShipmentNameInCustomReferenceFieldInTheHub(string shipmentName)
        {
            var shipmentRefInput = await FindLocatorAsync(HubCustomerReferenceInputSelectors);
            await TypeAsync(shipmentRefInput, shipmentName);
        }

        public async Task OpenShipmentByReferenceAsync(string shipmentReference)
        {
            var refLiteral = ToXPathLiteral(shipmentReference);
            var refLink = await TryFindLocatorAsync(
            [
                $"internal:role=link[name=\"{shipmentReference}\"i]",
                $"//a[contains(@href,'/shipments/') and contains(normalize-space(),{refLiteral})]",
                $"//*[contains(text(),{refLiteral})]/ancestor::a[contains(@href,'/shipments/')]"
            ], timeoutMs: 10000);

            Assert.IsNotNull(refLink, $"Shipment link for reference '{shipmentReference}' was not found in Hub results.");
            await ClickAndWaitForNavigationAsync(refLink!);
        }

        public async Task CheckTheTrackingIsEnabledForTheShipmentAsync()
        {
            var trackingTab = await FindLocatorAsync(
            [
                "internal:role=link[name=\"Tracking\"i]",
                "internal:role=tab[name=\"Tracking\"i]",
                "a[href*='view=tracking']",
                "//*[self::a or self::button][contains(normalize-space(),'Tracking')]"
            ], timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(trackingTab);

            var inProgress = await TryFindLocatorAsync(
            [
                "internal:text=\"In progress\"i",
                "//*[contains(normalize-space(),'Automated tracking')]/following::*[contains(normalize-space(),'In progress')][1]",
                "//*[contains(normalize-space(),'In progress')]"
            ], timeoutMs: 30000);

            Assert.IsNotNull(inProgress, $"Automated Tracking Status was not 'In progress'. URL: {Page.Url}");
        }

        public async Task CheckTheTrackingIsDisabledForTheShipmentAsync()
        {
            var trackingTab = await FindLocatorAsync(
            [
                "internal:role=link[name=\"Tracking\"i]",
                "internal:role=tab[name=\"Tracking\"i]",
                "a[href*='view=tracking']",
                "//*[self::a or self::button][contains(normalize-space(),'Tracking')]"
            ], timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(trackingTab);

            var inProgress = await TryFindLocatorAsync(
            [
                "internal:text=\"In progress\"i",
                "//*[contains(normalize-space(),'Automated tracking')]/following::*[contains(normalize-space(),'In progress')][1]",
                "//*[contains(normalize-space(),'In progress')]"
            ], timeoutMs: 8000);

            Assert.IsNull(inProgress, $"Automated Tracking still appears as 'In progress'. URL: {Page.Url}");

            var notTracked = await TryFindLocatorAsync(
            [
                "internal:text=\"Not Tracked\"i",
                "//*[contains(normalize-space(),'Automated tracking')]/following::*[contains(normalize-space(),'Not Tracked')][1]",
                "//*[contains(normalize-space(),'Not Tracked')]"
            ], timeoutMs: 10000);

            Assert.IsNotNull(notTracked, $"Automated Tracking Status was not 'Not Tracked'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the Search button in the Hub.
        /// ClickAndWaitForNetworkAsync is correct — search triggers an API call.
        /// </summary>
        public async Task IClickOnSearchButtonInTheHub()
        {
            var searchButton = await FindLocatorAsync(HubSearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }

        /// <summary>
        /// Verifies the shipment appears in the Hub search results.
        /// Retries up to 3 times to handle delayed list rendering.
        /// </summary>
        public async Task TheShipmentShouldAppearInTheSearchResultsInTheHub(string shipmentName)
        {
            const int maxRetries = 3;
            const int retryDelayMs = 3000;

            var resultSelectors = new[]
            {
                $"internal:text=\"{shipmentName}\"i",
                $"//*[contains(text(),'{shipmentName}')]"
            };

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var result = await TryFindLocatorAsync(resultSelectors, timeoutMs: 5000);
                if (result != null)
                    return;

                if (attempt < maxRetries)
                    await Page.WaitForTimeoutAsync(retryDelayMs);
            }

            Assert.Fail($"Shipment '{shipmentName}' was not found in the Hub search results after {maxRetries} attempts.");
        }

        /// <summary>
        /// Clicks the Milestones nav tab, waiting until it is visible and then navigating.
        /// Verified from HTML: a.nav-link containing "Milestones" text.
        /// </summary>
        public async Task GoToMilestonesTabAsync()
        {
            // Target specifically the shipment milestones tab by href — avoids matching the
            // "Shipment Milestones" admin nav link that also contains the word "Milestones".
            // Verified from HTML: a.nav-link[href='/shipments/{guid}?view=milestones']
            var tab = Page.Locator("a.nav-link[href*='?view=milestones']");
            await tab.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await tab.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Finds the milestone row by name and clicks its pencil (Edit) button.
        /// After clicking, asserts the "Update {name} milestone" h5 heading is visible.
        /// Verified from HTML: div.row > div.col > div.font-weight-bold contains the name;
        /// first button.btn-link in that row is the pencil edit button.
        /// h5.mb-4 "Update {name} milestone" appears after click.
        /// </summary>
        public async Task ClickEditButtonForMilestoneAsync(string milestoneName)
        {
            // Scope to the specific li that contains the milestone name, then target the pencil
            // button (btn-link WITHOUT text-success) — the checkmark button has text-success.
            // Verified from HTML: li > div.row > div.col > div.font-weight-bold + div.col-auto > button.btn-link (pencil)
            var editBtn = Page.Locator(
                $"(//li[.//div[contains(@class,'font-weight-bold') and contains(normalize-space(),'{milestoneName}')]]//div[contains(@class,'col-auto')]//button[contains(@class,'btn-link') and not(contains(@class,'text-success'))])[1]"
            );
            await editBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await editBtn.ClickAsync();
            await Page.WaitForTimeoutAsync(800);

            var heading = Page.Locator("h5.mb-4");
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected milestone edit heading to be visible after clicking Edit for '{milestoneName}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the calendar button in the milestone edit form.
        /// Verified from HTML: button.btn-outline-primary with fa-calendar icon.
        /// </summary>
        public async Task ClickCalendarButtonInMilestoneAsync()
        {
            // Scoped to the milestone edit form to avoid matching the header dropdown-toggle button.
            // Verified from HTML: form > button[type='button'].btn-outline-primary with fa-calendar icon.
            var calBtn = Page.Locator("form button[type='button'].btn-outline-primary");
            await calBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await calBtn.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        /// <summary>
        /// Selects a date in the open PrimeNG datepicker and stores it for later assertion.
        /// Behavior:
        ///   - If dateParam is "date" (the literal keyword used in the feature) or empty → use today's date.
        ///   - Otherwise → parse dateParam (ISO format "yyyy-MM-dd" or "MM/dd/yyyy").
        /// The stored date is formatted as "MMMM d, yyyy" (e.g., "March 6, 2026").
        /// </summary>
        public async Task SelectDateInCalendarAsync(string dateParam)
        {
            var targetDate = (string.IsNullOrWhiteSpace(dateParam) || dateParam.Equals("date", StringComparison.OrdinalIgnoreCase))
                ? DateTime.Today
                : DateTime.Parse(dateParam);

            // Store the FIRST/original date so GetSelectedDate() always returns this base date.
            _selectedDate = targetDate.ToString("MMMM d, yyyy");
            await ClickCalendarDayAsync(targetDate);
        }

        /// <summary>
        /// Selects the date 7 days ahead (dateParam + 7) in the PrimeNG calendar.
        /// If dateParam is "date" or empty → uses today + 7 (does NOT override _selectedDate).
        /// If the target date falls in a different month, navigates forward one month first.
        /// </summary>
        public async Task SelectNextDateInCalendarAsync(string dateParam)
        {
            var baseDate = (string.IsNullOrWhiteSpace(dateParam) || dateParam.Equals("date", StringComparison.OrdinalIgnoreCase))
                ? DateTime.Today
                : DateTime.Parse(dateParam);

            var targetDate = baseDate.AddDays(7);

            // If the target date is in a different month, navigate the calendar forward.
            if (targetDate.Month != baseDate.Month)
            {
                var nextMonthBtn = Page.Locator(
                    "//div[contains(@class,'p-datepicker')]//button[contains(@class,'p-datepicker-next')]"
                ).First;
                await nextMonthBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                await nextMonthBtn.ClickAsync();
                await Page.WaitForTimeoutAsync(300);
            }

            // Do NOT update _selectedDate here — keep the base date stored by SelectDateInCalendarAsync.
            await ClickCalendarDayAsync(targetDate);
        }

        /// <summary>
        /// Shared helper: clicks a specific day cell in the open PrimeNG datepicker.
        /// Does NOT update _selectedDate — callers are responsible for storing the date when needed.
        /// </summary>
        private async Task ClickCalendarDayAsync(DateTime targetDate)
        {

            var dayNumber = targetDate.Day.ToString();
            // Hub uses PrimeNG p-calendar (older version) — renders as div.p-datepicker (not <p-datepicker> tag).
            var dayCell = Page.Locator(
                $"//div[contains(@class,'p-datepicker')]//td[not(contains(@class,'other-month')) and not(@data-p-other-month='true') and not(contains(@class,'p-disabled'))][.//span[normalize-space()='{dayNumber}']]//span[normalize-space()='{dayNumber}']"
            ).First;
            await dayCell.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await dayCell.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Clicks the "Save changes" submit button when it becomes enabled.
        /// Verified from HTML: button[type='submit'].btn-primary "Save changes".
        /// </summary>
        public async Task ClickSaveChangesButtonAsync()
        {
            var saveBtn = Page.Locator("button[type='submit'].btn-primary");
            await saveBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(saveBtn, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(saveBtn);
            var editBtn = Page.Locator(
                $"(//li[.//div[contains(@class,'font-weight-bold') and contains(normalize-space(),'Container empty to shipper')]]//div[contains(@class,'col-auto')]//button[contains(@class,'btn-link') and not(contains(@class,'text-success'))])[1]"
            );
            await editBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 60000 });
        }

        /// <summary>
        /// Verifies the selected date is visible in the milestone tab after saving.
        /// If dateParam is "date" (the literal keyword), uses the date stored by SelectDateInCalendarAsync.
        /// The date is matched as "MMMM d, yyyy" (e.g., "March 6, 2026") inside any div in the milestone list.
        /// Verified from HTML: div.ng-star-inserted containing "March 6, 2026 7:15 AM".
        /// </summary>
        public async Task ShouldSeeDateInMilestoneTabAsync(string dateParam)
        {
            var expectedDate = (string.IsNullOrWhiteSpace(dateParam) || dateParam.Equals("date", StringComparison.OrdinalIgnoreCase))
                ? _selectedDate
                : DateTime.Parse(dateParam).ToString("MMMM d, yyyy");

            var dateLocator = Page.Locator($"//div[contains(normalize-space(),'{expectedDate}')]").First;
            await dateLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await dateLocator.IsVisibleAsync(),
                $"Expected date '{expectedDate}' to be visible in the milestone tab. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the date 7 days ahead (date + 7) is visible in the milestone tab.
        /// If dateParam is "date"/empty → uses _selectedDate + 7 days.
        /// </summary>
        public async Task ShouldSeeNextDateInMilestoneTabAsync(string dateParam)
        {
            var baseDate = (string.IsNullOrWhiteSpace(dateParam) || dateParam.Equals("date", StringComparison.OrdinalIgnoreCase))
                ? DateTime.Parse(_selectedDate)
                : DateTime.Parse(dateParam);

            var expectedDate = baseDate.AddDays(7).ToString("MMMM d, yyyy");

            var dateLocator = Page.Locator($"//div[contains(normalize-space(),'{expectedDate}')]").First;
            await dateLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await dateLocator.IsVisibleAsync(),
                $"Expected next date '{expectedDate}' to be visible in the milestone tab. URL: {Page.Url}");
        }

        /// <summary>
        /// Returns the date stored by SelectDateInCalendarAsync in "MMMM d, yyyy" format (e.g., "March 6, 2026").
        /// Used by portal steps to verify the same date in the Portal milestone history popup.
        /// </summary>
        public string GetSelectedDate() => _selectedDate;

        /// <summary>
        /// Clicks the shipment link in the Hub search results to open the shipment detail.
        /// Verified from HTML: div.d-flex > a[href='/shipments/{guid}'] — the GUID varies per test run.
        /// </summary>
        public async Task SelectCreatedShipmentAsync()
        {
            // Scoped to td > div.d-flex to avoid matching sidebar/nav links that also contain /shipments/.
            // Verified from HTML: <td><div class="d-flex justify-content-between"><a href="/shipments/{guid}">
            var shipmentLink = Page.Locator("td div.d-flex a[href*='/shipments/']").First;
            await shipmentLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await ClickAndWaitForNetworkAsync(shipmentLink);
        }

        /// <summary>
        /// Verifies the shipment does NOT appear in the Hub search results.
        /// Used to confirm a hidden shipment is invisible in the Hub.
        /// </summary>
        public async Task TheShipmentShouldNotAppearInTheSearchResultsInTheHub(string shipmentName)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var resultSelectors = new[]
            {
                $"internal:text=\"{shipmentName}\"i",
                $"//*[contains(text(),'{shipmentName}')]"
            };

            var result = await TryFindLocatorAsync(resultSelectors, timeoutMs: 4000);
            Assert.IsNull(result,
                $"Shipment '{shipmentName}' was found in the Hub search results but it should not appear (shipment is hidden). URL: {Page.Url}");
        }

        // ── Milestone confirmation ────────────────────────────────────────────────

        /// <summary>
        /// Finds the milestone section row by its name and clicks the circle-check (confirm) button.
        /// </summary>
        public async Task ClickConfirmButtonInSectionAsync(string sectionName)
        {
            // The section row: div.row containing a div with the section name text
            // The confirm button: button.btn-link.text-success containing fa-icon[circle-check]
            var confirmBtn = Page.Locator(
                $"//div[contains(@class,'row') and .//div[contains(normalize-space(),'{sectionName}')]]" +
                $"//button[contains(@class,'text-success') and .//*[local-name()='svg' and @data-icon='circle-check']]")
                .First;
            await confirmBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(confirmBtn, timeoutMs: 5000);
            await confirmBtn.ClickAsync();
        }

        /// <summary>
        /// Verifies the "Confirm confirmation milestone" heading is visible.
        /// </summary>
        public async Task ShouldSeeConfirmationPageAsync()
        {
            var heading = Page.Locator("h5")
                .Filter(new LocatorFilterOptions { HasText = "Confirm confirmation milestone" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Confirm confirmation milestone' heading to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the Expected date field in the confirmation milestone form.
        /// Uses today + 7 days if no date is provided.
        /// </summary>
        public async Task SelectExpectedDateInCalendarAsync(string date)
        {
            var targetDate = string.IsNullOrWhiteSpace(date)
                ? DateTime.Now.AddDays(7)
                : (DateTime.TryParse(date, out var parsed) ? parsed : DateTime.Now.AddDays(7));

            await FillMilestoneDateFieldAsync("Expected date", targetDate);
        }

        /// <summary>
        /// Fills the Actual date field in the confirmation milestone form.
        /// Uses today + 3 days if no date is provided.
        /// </summary>
        public async Task SelectActualDateInCalendarAsync(string date)
        {
            var targetDate = string.IsNullOrWhiteSpace(date)
                ? DateTime.Now.AddDays(3)
                : (DateTime.TryParse(date, out var parsed) ? parsed : DateTime.Now.AddDays(3));

            await FillMilestoneDateFieldAsync("Actual date", targetDate);
        }

        private async Task FillMilestoneDateFieldAsync(string fieldLabel, DateTime targetDate)
        {
            // The id is on the <p-calendar> component element, so scope to the input inside it.
            var dateStr = targetDate.ToString("MM/dd/yyyy HH:mm");
            var inputId = fieldLabel.Contains("Expected", StringComparison.OrdinalIgnoreCase)
                ? "#expected_timestamp input"
                : "#actual_timestamp input";

            var dateInput = Page.Locator(inputId);
            await dateInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dateInput.ClickAsync();
            // Select all existing text and delete it before typing.
            await dateInput.PressAsync("Control+a");
            await dateInput.PressAsync("Delete");
            // PrimeNG p-calendar requires real keystrokes (not FillAsync) to fire Angular
            // change-detection events. FillAsync bypasses them and the value gets cleared on blur.
            await dateInput.PressSequentiallyAsync(dateStr, new LocatorPressSequentiallyOptions { Delay = 50 });
            // Press Escape to close any calendar popup without triggering the blur validator.
            await dateInput.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Verifies the milestone row shows the confirmed state — the duotone green circle-check icon
        /// and "Confirmed" text in the section header.
        /// </summary>
        public async Task ShouldSeeGreenIconAsync()
        {
            // After confirming, the section header changes to "Confirmed"
            var confirmedDiv = Page.Locator("//div[contains(@class,'font-weight-bold') and contains(normalize-space(),'Confirmed')]").First;
            await confirmedDiv.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await confirmedDiv.IsVisibleAsync(),
                $"Expected 'Confirmed' status to be visible in the milestone section. URL: {Page.Url}");
        }

    }
}
