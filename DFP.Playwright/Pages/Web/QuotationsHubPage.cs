using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class QuotationsHubPage : BasePage
    {
        private string _quoteId = string.Empty;

        public QuotationsHubPage(IPage page) : base(page)
        {
        }

        private static string GetHubBaseUrl()
        {
            var url = Environment.GetEnvironmentVariable("HUB_BASE_URL")
                      ?? Environment.GetEnvironmentVariable("BASE_URL")
                      ?? "";
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            return url;
        }

        // ── TC129: Hub Create Quotation (Full Load-Ocean) ─────────────────────────

        /// <summary>
        /// Clicks the "Create quotation" button on the Hub quotations list page.
        /// Verified from HTML: button.btn-outline-primary with text "Create quotation"
        /// </summary>
        public async Task ClickCreateQuotationButtonInHubAsync()
        {
            var btn = Page.Locator("button.btn-outline-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create quotation" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the Hub create quotation page heading is visible.
        /// Verified from HTML: h3 "Create quotation"
        /// </summary>
        public async Task ShouldSeeCreateQuotationPageInHubAsync()
        {
            var heading = Page.Locator("h3").Filter(new LocatorFilterOptions { HasText = "Create quotation" });
            await heading.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await heading.First.IsVisibleAsync(),
                $"Expected 'Create quotation' heading to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Selects a customer from the first ng-select on the Hub create quotation form.
        /// Waits for the placeholder "Select..." to be visible before typing to ensure the input is ready.
        /// </summary>
        public async Task SelectCustomerInHubAsync(string customer)
        {
            var ngSelect = Page.Locator("ng-select").First;
            var placeholder = ngSelect.Locator(".ng-placeholder");
            await placeholder.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var combobox = ngSelect.Locator("[role='combobox']");
            await combobox.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);
            await combobox.FillAsync(customer);
            await Page.WaitForTimeoutAsync(2000);
            await combobox.PressAsync("Enter");
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Selects a load type from the second ng-select on the Hub create quotation form.
        /// Clicks the ng-value-container to open the dropdown, then selects the matching option (no typing needed).
        /// </summary>
        public async Task SelectLoadTypeInHubAsync(string loadType)
        {
            var ngSelect = Page.Locator("ng-select").Nth(1);
            var placeholder = ngSelect.Locator(".ng-placeholder");
            await placeholder.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ngSelect.Locator(".ng-value-container").ClickAsync();
            await Page.WaitForTimeoutAsync(400);
            var option = Page.Locator(".ng-option").Filter(new LocatorFilterOptions { HasText = loadType });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Selects a modality from the third ng-select on the Hub create quotation form.
        /// Verified from HTML: ng-select for modality — clicks to open and selects option.
        /// </summary>
        public async Task SelectModalityInHubAsync(string modality)
        {
            var combobox = Page.Locator("ng-select").Nth(2).Locator("[role='combobox']");
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await combobox.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
            var option = Page.Locator(".ng-option").Filter(new LocatorFilterOptions { HasText = modality });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Enters the origin location via the lta-button.
        /// Clicks the div[title='Origin'] container, then types in the revealed input[placeholder='Origin'].
        /// </summary>
        public async Task EnterOriginInHubAsync(string origin)
        {
            var btn = Page.Locator("div[title='Origin']");
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            var input = Page.Locator("input[placeholder='Origin']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await input.FillAsync(origin);
            await Page.WaitForTimeoutAsync(1000);
            var suggestion = Page.Locator(".lta-suggestion-item").First;
            await suggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await suggestion.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Enters the destination location via the lta-button.
        /// Clicks the div[title='Destination'] container, then types in the revealed input[placeholder='Destination'].
        /// </summary>
        public async Task EnterDestinationInHubAsync(string destination)
        {
            var btn = Page.Locator("div[title='Destination']");
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            var input = Page.Locator("input[placeholder='Destination']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await input.FillAsync(destination);
            await Page.WaitForTimeoutAsync(1000);
            var suggestion = Page.Locator(".lta-suggestion-item").First;
            await suggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await suggestion.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Clicks the Continue button on the Hub create quotation form (step 1 → step 2).
        /// Verified from HTML: button.btn-primary with text "Continue"
        /// </summary>
        public async Task ClickContinueButtonInHubAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Continue" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the commodity selection section is visible on step 2.
        /// Verified from HTML: div.ng-placeholder "Select a commodity from the list" inside ng-select.
        /// </summary>
        public async Task ShouldSeeCommoditySectionInHubAsync()
        {
            var placeholder = Page.Locator(".ng-placeholder").Filter(new LocatorFilterOptions { HasText = "Select a commodity from the list" });
            await placeholder.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await placeholder.First.IsVisibleAsync(),
                $"Expected commodity dropdown to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Selects a commodity from the ng-select dropdown on Hub step 2.
        /// Finds the ng-select by its .ng-placeholder text, clicks the combobox input, types, and selects matching option.
        /// Verified from HTML: div.ng-placeholder "Select a commodity from the list", span.ng-option-label contains commodity text.
        /// </summary>
        public async Task SelectCommodityInHubAsync(string commodity)
        {
            var ngSelect = Page.Locator("ng-select").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-placeholder", new PageLocatorOptions { HasText = "Select a commodity from the list" })
            });
            var combobox = ngSelect.Locator("[role='combobox']");
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await combobox.ClickAsync();
            await combobox.FillAsync(commodity);
            await Page.WaitForTimeoutAsync(500);
            var option = Page.Locator("span.ng-option-label").Filter(new LocatorFilterOptions { HasText = commodity });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Verifies the quotation details page is visible after navigation by checking the "Additionals" heading.
        /// Verified from HTML: h5.font-weight-bold "Additionals"
        /// </summary>
        public async Task ShouldSeeQuotationDetailsPageInHubAsync()
        {
            var heading = Page.Locator("h5.font-weight-bold").Filter(new LocatorFilterOptions { HasText = "Additionals" });
            await heading.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await heading.First.IsVisibleAsync(),
                $"Expected 'Additionals' heading to be visible after navigation. URL: {Page.Url}");
        }

        /// <summary>
        /// Selects a currency from the ng-select currency dropdown on Hub step 2.
        /// Verified from HTML: ng-select[formcontrolname='currency'] [role='combobox'] — types and selects.
        /// </summary>
        public async Task SelectCurrencyInHubAsync(string currency)
        {
            var combobox = Page.Locator("ng-select[formcontrolname='currency'] [role='combobox']");
            var count = await combobox.CountAsync();
            if (count == 0)
                combobox = Page.Locator("[role='combobox']").Nth(1);

            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await combobox.ClickAsync();
            await combobox.FillAsync(currency);
            await Page.WaitForTimeoutAsync(500);
            var option = Page.Locator(".ng-option").Filter(new LocatorFilterOptions { HasText = currency });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Selects a container size from the ng-select dropdown on Hub step 2.
        /// Finds the ng-select by its .ng-placeholder "Container Size", types the size, and selects matching option.
        /// Verified from HTML: div.ng-placeholder "Container Size", input[role='combobox'] inside.
        /// </summary>
        public async Task SelectContainerSizeInHubAsync(string size)
        {
            var ngSelect = Page.Locator("ng-select").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-placeholder", new PageLocatorOptions { HasText = "Container Size" })
            });
            var combobox = ngSelect.Locator("[role='combobox']");
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await combobox.ClickAsync();
            await combobox.FillAsync(size);
            await Page.WaitForTimeoutAsync(500);
            var option = Page.Locator("span.ng-option-label").Filter(new LocatorFilterOptions { HasText = size });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Toggles an additional option by clicking its label (custom-switch checkbox).
        /// Verified from HTML: label.custom-control-label with the text of the additional (e.g. "Require origin charges").
        /// </summary>
        public async Task SelectAdditionalInHubAsync(string additional)
        {
            var label = Page.Locator("label.custom-control-label")
                .Filter(new LocatorFilterOptions { HasText = additional });
            await label.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await label.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Verifies the quotation status badge matches the expected status (parametrizable).
        /// The CSS class matches the lowercase status: "Draft" → span.status-badge.draft, "Open" → span.status-badge.open, etc.
        /// Waits up to 20s to account for page navigation.
        /// </summary>
        public async Task ShouldSeeQuotationInStatusInHubAsync(string status)
        {
            var cssClass = status.ToLower();
            var badge = Page.Locator($"span.status-badge.{cssClass}").Filter(new LocatorFilterOptions { HasText = status });
            await badge.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await badge.First.IsVisibleAsync(),
                $"Expected quotation status '{status}' to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Waits for the "Publish quotation" button to be enabled, then clicks it.
        /// The button is initially rendered as disabled="" until the page finishes loading.
        /// Verified from HTML: button.p-element.btn-outline-primary with text "Publish quotation"
        /// </summary>
        public async Task ClickPublishQuotationInHubAsync()
        {
            var btn = Page.Locator("button.btn-outline-primary")
                .Filter(new LocatorFilterOptions { HasText = "Publish quotation" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            await Assertions.Expect(btn.First).ToBeEnabledAsync(new LocatorAssertionsToBeEnabledOptions { Timeout = 230000 });
            await btn.First.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        /// <summary>
        /// Waits for the Offers tab to appear and verifies the badge count is greater than zero.
        /// Polls until the badge shows a number > 0, up to 60s (offer search can take time after publishing).
        /// Verified from HTML: a.nav-link containing "Offers" text, span.badge with the count.
        /// </summary>
        public async Task ShouldSeeOffersInHubAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var offersLink = Page.Locator("a.nav-link").Filter(new LocatorFilterOptions { HasText = "Offers" });
            await offersLink.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 300000 });
            var deadline = DateTime.UtcNow.AddSeconds(300);
            int count = 0;
            string badgeText = "0";
            while (DateTime.UtcNow < deadline && count == 0)
            {
                badgeText = (await offersLink.First.Locator("span.badge").InnerTextAsync()).Trim();
                if (!int.TryParse(badgeText, out count)) count = 0;
                if (count == 0) await Page.WaitForTimeoutAsync(1000);
            }
            Assert.IsTrue(count > 0, $"Expected Offers count to be greater than 0 but found '{badgeText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the "Yes" confirmation button in the Hub.
        /// Verified from HTML: button.confirm.btn.btn-primary with text "Yes"
        /// </summary>
        public async Task ClickYesButtonInHubAsync()
        {
            var btn = Page.Locator("button.confirm.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Yes" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Reads and stores the quote ID from the quotation detail heading (e.g. "QUO-02524").
        /// Verified from HTML: h3 > span.text-muted containing the quote ID.
        /// </summary>
        public async Task StoreQuoteIdInHubAsync()
        {
            var span = Page.Locator("h3 span.text-muted").Filter(new LocatorFilterOptions { HasText = "QUO-" });
            await span.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            _quoteId = (await span.First.InnerTextAsync()).Trim();
            Console.WriteLine($"[QuotationsHubPage] Quote ID stored: {_quoteId}");
        }

        public string GetQuoteId() => _quoteId;

        /// <summary>
        /// Selects a package type from the ng-select with placeholder "Packaging".
        /// Types the parameter text, then clicks the matching span.ng-option-label.
        /// Verified from HTML: div.ng-placeholder "Packaging", span.ng-option-label "Carton"
        /// </summary>
        public async Task SelectPackageInHubAsync(string package)
        {
            var ngSelect = Page.Locator("ng-select").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-placeholder", new PageLocatorOptions { HasText = "Packaging" })
            });
            var combobox = ngSelect.Locator("[role='combobox']");
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await combobox.ClickAsync();
            await combobox.FillAsync(package);
            await Page.WaitForTimeoutAsync(500);
            var option = Page.Locator("span.ng-option-label").Filter(new LocatorFilterOptions { HasText = package });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Fills cargo dimension and weight fields in the Hub form.
        /// Verified from HTML: input[formcontrolname='unit_weight/unit_length/unit_width/unit_height']
        /// </summary>
        public async Task EnterCargoDetailsInHubAsync(string weight, string length, string width, string height)
        {
            var weightInput = Page.Locator("input[formcontrolname='unit_weight']");
            var lengthInput = Page.Locator("input[formcontrolname='unit_length']");
            var widthInput  = Page.Locator("input[formcontrolname='unit_width']");
            var heightInput = Page.Locator("input[formcontrolname='unit_height']");

            await weightInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await weightInput.ClearAsync();
            await weightInput.FillAsync(weight);

            await lengthInput.ClearAsync();
            await lengthInput.FillAsync(length);

            await widthInput.ClearAsync();
            await widthInput.FillAsync(width);

            await heightInput.ClearAsync();
            await heightInput.FillAsync(height);
        }

        /// <summary>
        /// Clicks the final "Create Quotation" button on Hub step 2 (fa-hand-holding-dollar icon).
        /// Verified from HTML: button.btn-primary.btn-lg containing fa-icon[icon='hand-holding-dollar']
        /// </summary>
        public async Task ClickCreateQuotationFinalInHubAsync()
        {
            var btn = Page.Locator("button.btn-primary.btn-lg");
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
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

        // ── TC162: Requests > Change Status to Closed ─────────────────────────────

        /// <summary>
        /// Selects a status from the PrimeNG p-dropdown filter on the quotations list.
        /// Verified from HTML: p-dropdown[formcontrolname='status'] → li.p-dropdown-item options (Open, Draft, Request, Booked, Closed).
        /// </summary>
        public async Task FilterQuotationsByStatusInHubAsync(string status)
        {
            var dropdown = Page.Locator("p-dropdown[formcontrolname='status']");
            await dropdown.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dropdown.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
            var option = Page.Locator("li.p-dropdown-item").Filter(new LocatorFilterOptions { HasText = status });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Clicks the Search button in the quotations list filter area.
        /// Verified from HTML: button with text "Search" in the filters form.
        /// </summary>
        public async Task ClickSearchButtonInHubAsync()
        {
            var btn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Search" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the first quotation link in the filtered results table.
        /// Verified from HTML: table tbody tr first td a link.
        /// </summary>
        public async Task SelectFirstQuotationInHubAsync()
        {
            var firstLink = Page.Locator("table tbody tr").First.Locator("td a").First;
            await firstLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await firstLink.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Navigates to the Requests tab on a quotation detail page.
        /// Verified from HTML: a[href*='view=requests'] nav link inside the right-side nav.
        /// </summary>
        public async Task NavigateToRequestsTabInHubAsync()
        {
            var tab = Page.Locator("a[href*='view=requests']");
            await tab.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await tab.ClickAsync();
            await Page.WaitForTimeoutAsync(500);
        }

        /// <summary>
        /// Clicks the Close button on the first open request row in the Requests tab.
        /// Verified from HTML: button.btn-outline-danger with text "Close" in the requests table row.
        /// </summary>
        public async Task ClickCloseRequestButtonInHubAsync()
        {
            var btn = Page.Locator("button.btn-outline-danger")
                .Filter(new LocatorFilterOptions { HasText = "Close" });
            await btn.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);
        }

        /// <summary>
        /// Fills the close reason textarea in the "Close Rate Request without Offer" PrimeNG dialog.
        /// Verified from HTML: textarea[placeholder='Close reason...'] inside p-dynamic-dialog.
        /// </summary>
        public async Task EnterCloseReasonInHubAsync(string reason)
        {
            var textarea = Page.Locator("textarea[placeholder='Close reason...']");
            await textarea.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await textarea.FillAsync(reason);
        }

        /// <summary>
        /// Verifies the first request row in the Requests tab has the expected status badge.
        /// Verified from HTML: span.badge inside first tbody tr of the requests table (e.g. span.badge.badge-success "Closed").
        /// </summary>
        public async Task ShouldSeeFirstRequestInStatusInHubAsync(string status)
        {
            var requestsTable = Page.Locator("table").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator("th").Filter(new LocatorFilterOptions { HasText = "Status" })
            });
            var statusBadge = requestsTable.Locator("tbody tr").First
                .Locator("span.badge")
                .Filter(new LocatorFilterOptions { HasText = status });
            await statusBadge.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await statusBadge.First.IsVisibleAsync(),
                $"Expected request status badge '{status}' to be visible. URL: {Page.Url}");
        }
    }
}
