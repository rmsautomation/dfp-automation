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
    public sealed class CargoReleasesPage : BasePage
    {
        private readonly TestContext _tc;
        private string _crId = string.Empty;

        public CargoReleasesPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to /my-portal/cargo-releases.
        /// HTML: a[href='/my-portal/cargo-releases'] Cargo Releases
        /// </summary>
        public async Task NavigateToCargoReleasesPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/cargo-releases");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the Cargo Releases page heading is visible.
        /// HTML: h3.font-weight-normal.m-0 "Your Cargo Releases"
        /// </summary>
        public async Task VerifyCargoReleasesPageVisibleAsync()
        {
            var heading = Page.Locator("h3.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = "Your Cargo Releases" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Your Cargo Releases' heading. URL: {Page.Url}");
        }

        // ── Search ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Types into the Cargo Release # search input.
        /// HTML: input[formcontrolname='number'] placeholder="Cargo Release #"
        /// </summary>
        public async Task SearchCargoReleaseAsync(string value)
        {
            var input = await TryFindLocatorAsync(new[]
            {
                "input[formcontrolname='number']",
                "input[placeholder='Cargo Release #']",
                "input#number"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Cargo Release search input not found. URL: {Page.Url}");
            await WaitForEnabledAsync(input!, timeoutMs: 10000);
            await input!.ClearAsync();
            await TypeAsync(input, value);
        }

        /// <summary>
        /// Clicks any button by text — waits up to 30s for enabled.
        /// </summary>
        public async Task ClickButtonAsync(string buttonText)
        {
            var btn = Page.Locator("button")
                .Filter(new LocatorFilterOptions { HasText = buttonText })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            await WaitForEnabledAsync(btn, timeoutMs: 30000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Clicks the Next button inside qwyk-create-cargo-release-buttons (Create CR wizard, any step).
        /// HTML: qwyk-create-cargo-release-buttons button.btn-primary span "Next"
        /// </summary>
        public async Task ClickNextInCreateCRAsync()
        {
            var btn = Page.Locator("qwyk-create-cargo-release-buttons button.btn-primary").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            await WaitForEnabledAsync(btn, timeoutMs: 30000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Waits 2s then clicks the Next button in step2 (Angular needs time to process loaded items).
        /// HTML: qwyk-create-cargo-release-buttons button.btn-primary span "Next"
        /// </summary>
        public async Task ClickNextInCreateCRStep2Async()
        {
            await Page.WaitForTimeoutAsync(2000);
            var btn = Page.Locator("qwyk-create-cargo-release-buttons button.btn-primary").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            await WaitForEnabledAsync(btn, timeoutMs: 30000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Clicks the "Send Cargo Release" button inside qwyk-create-cargo-release-buttons (step3).
        /// HTML: qwyk-create-cargo-release-buttons button.btn-primary span "Send Cargo Release"
        /// </summary>
        public async Task ClickSendCargoReleaseAsync()
        {
            var btn = Page.Locator("qwyk-create-cargo-release-buttons button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Send Cargo Release" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            await WaitForEnabledAsync(btn, timeoutMs: 30000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Verifies a row in the step2 selected-items table (tbody.p-datatable-tbody) contains the given text.
        /// HTML: tbody.p-datatable-tbody tr with matching text
        /// </summary>
        public async Task VerifyItemLoadedInStep2Async(string text)
        {
            var row = Page.Locator("tbody.p-datatable-tbody tr")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;
            await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await row.IsVisibleAsync(),
                $"Expected item '{text}' in step2 table. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the "Load selected items" button in the p-datatable-footer.
        /// HTML: div.p-datatable-footer button.btn-secondary:has-text('Load selected items')
        /// </summary>
        public async Task ClickLoadSelectedItemsAsync()
        {
            var btn = Page.Locator("div.p-datatable-footer button.btn-secondary")
                .Filter(new LocatorFilterOptions { HasText = "Load selected items" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        // ── List ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Waits up to 3 minutes for a cargo release row containing the given text to appear.
        /// Clicks Search every 2 seconds while no matching result is visible.
        /// HTML: li[qwyk-cargo-releases-index-list-item]
        /// </summary>
        public async Task VerifyCargoReleaseVisibleInListAsync(string text)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var item = Page.Locator("li[qwyk-cargo-releases-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Cargo release '{text}' did not appear in the list after 3 minutes. URL: {Page.Url}");

                if (await item.CountAsync() > 0 && await item.IsVisibleAsync())
                    return;

                var searchBtn = await TryFindLocatorAsync(new[]
                {
                    "button.btn-primary:has-text('Search')",
                    "//button[contains(@class,'btn-primary') and normalize-space()='Search']",
                    "button:has-text('Search')"
                }, timeoutMs: 3000);

                if (searchBtn != null)
                    await ClickAndWaitForNetworkAsync(searchBtn);

                await Page.WaitForTimeoutAsync(retryIntervalMs);
            }
        }

        /// <summary>
        /// Clicks the cargo release row containing the given text.
        /// HTML: li[qwyk-cargo-releases-index-list-item]
        /// </summary>
        public async Task SelectCargoReleaseFromListAsync(string text)
        {
            var item = Page.Locator("li[qwyk-cargo-releases-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await item.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Detail page ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the Cargo Release detail page is loaded by checking the Summary nav tab.
        /// HTML: a.nav-link[href*='/cargo-releases/'][href*='?view=summary'] "Summary"
        /// </summary>
        public async Task VerifyCargoReleaseDetailsPageAsync()
        {
            var summaryTab = Page.Locator("a.nav-link[href*='/cargo-releases/'][href*='?view=summary']").First;
            await summaryTab.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await summaryTab.IsVisibleAsync(),
                $"Expected Cargo Release Summary tab on detail page. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a label/value pair in the CR summary card.
        /// HTML: label.small.font-weight-bold.m-0 + following-sibling div
        /// XPath: //label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyCRDetailLabelAsync(string labelText, string expectedValue)
        {
            var valueDiv = Page.Locator($"//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple label/value pairs in the CR summary card.
        /// </summary>
        public async Task VerifyCRDetailsAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (label, value) in pairs)
                await VerifyCRDetailLabelAsync(label, value);
        }

        // ── Create CR ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks a dropdown-item button matching the given text (e.g. "New Cargo Release").
        /// HTML: ul.nav button.dropdown-item with matching text
        /// </summary>
        public async Task SelectDropdownOptionAsync(string optionText)
        {
            var btn = Page.Locator("button.dropdown-item")
                .Filter(new LocatorFilterOptions { HasText = optionText })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Verifies the Create CR page heading is visible.
        /// HTML: h3.font-weight-normal.mt-3 with matching text
        /// </summary>
        public async Task VerifyCreateCRPageAsync(string headingText)
        {
            var heading = Page.Locator("h3.font-weight-normal")
                .Filter(new LocatorFilterOptions { HasText = headingText })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected heading '{headingText}' on Create CR page. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the "Now" (or any btn-link) button matching the given text.
        /// HTML: button.btn.btn-link.btn-sm with matching text
        /// </summary>
        public async Task SelectReleaseAtOptionAsync(string optionText)
        {
            var btn = Page.Locator("button.btn-link")
                .Filter(new LocatorFilterOptions { HasText = optionText })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 10000);
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Types into the "Release to name" autocomplete input and clicks the first suggestion matching the text.
        /// HTML: input.p-autocomplete-input[placeholder='Release to name (type to search)']
        ///       li.p-autocomplete-item with matching text
        /// </summary>
        public async Task EnterReleaseToNameAsync(string name)
        {
            var input = await TryFindLocatorAsync(new[]
            {
                "input.p-autocomplete-input[placeholder='Release to name (type to search)']",
                "input.p-autocomplete-dd-input[placeholder='Release to name (type to search)']",
                "//input[@placeholder='Release to name (type to search)']"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Release to name autocomplete input not found. URL: {Page.Url}");
            await WaitForEnabledAsync(input!, timeoutMs: 10000);
            await input!.ClearAsync();
            await TypeAsync(input, name);

            var suggestion = Page.Locator("li.p-autocomplete-item")
                .Filter(new LocatorFilterOptions { HasText = name })
                .First;
            await suggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await suggestion.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Selects a country from the p-dropdown#release_to_country.
        /// HTML: div#release_to_country.p-dropdown → li.p-dropdown-item span matching countryName
        /// </summary>
        public async Task SelectCountryForCRAsync(string countryName)
        {
            var dropdown = Page.Locator("div#release_to_country.p-dropdown").First;
            await dropdown.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(dropdown, timeoutMs: 10000);
            await dropdown.ClickAsync();

            var option = Page.Locator("li.p-dropdown-item")
                .Filter(new LocatorFilterOptions { HasText = countryName })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await option.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Available cargo section ───────────────────────────────────────────────

        /// <summary>
        /// Enters a value into the Available Cargo "Number" search input.
        /// HTML: input[formcontrolname='parent_number'] placeholder="Number"
        /// </summary>
        public async Task EnterAvailableCargoNumberAsync(string value)
        {
            // Scoped to the available cargo form to avoid matching other Number inputs on the page.
            // HTML: form input[formcontrolname='parent_number'] id="parent_number"
            var input = await TryFindLocatorAsync(new[]
            {
                "form input[formcontrolname='parent_number']",
                "input#parent_number",
                "//form//input[@formcontrolname='parent_number']"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Available cargo Number input not found. URL: {Page.Url}");
            await WaitForEnabledAsync(input!, timeoutMs: 15000);
            await input!.ClearAsync();
            await TypeAsync(input, value);
        }

        /// <summary>
        /// Waits up to 3 minutes for a row with the given text to appear in the Available Cargo table.
        /// Clicks Search every 2 seconds while no matching row is visible, then clicks its checkbox.
        /// HTML: tr containing qwyk-lazy-load-value with text → p-tablecheckbox div.p-checkbox-box
        /// </summary>
        public async Task SelectAvailableCargoItemAsync(string text)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var row = Page.Locator("tr")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Available cargo item '{text}' did not appear after 3 minutes. URL: {Page.Url}");

                if (await row.CountAsync() > 0 && await row.IsVisibleAsync())
                    break;

                var searchBtn = await TryFindLocatorAsync(new[]
                {
                    "button.btn-secondary:has-text('Search')",
                    "//button[contains(@class,'btn-secondary') and normalize-space()='Search']",
                    "button:has-text('Search')"
                }, timeoutMs: 3000);

                if (searchBtn != null)
                    await ClickAndWaitForNetworkAsync(searchBtn);

                await Page.WaitForTimeoutAsync(retryIntervalMs);
            }

            await WaitForEnabledAsync(row, timeoutMs: 10000);
            var checkbox = row.Locator("p-tablecheckbox div.p-checkbox-box").First;
            await checkbox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await checkbox.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Confirmation ──────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the confirmation message heading is visible.
        /// HTML: h4.font-weight-normal with matching text
        /// </summary>
        public async Task VerifyConfirmationMessageAsync(string expectedText)
        {
            var heading = Page.Locator("h4.font-weight-normal")
                .Filter(new LocatorFilterOptions { HasText = expectedText })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Confirmation message '{expectedText}' not found. URL: {Page.Url}");
        }

        /// <summary>
        /// Reads the CR number from the confirmation page and stores it in _crId.
        /// HTML: p.text-muted "Cargo Release Number: 721"
        /// </summary>
        public async Task StoreCRNumberAsync()
        {
            var para = Page.Locator("p.text-muted.mb-0.lead")
                .Filter(new LocatorFilterOptions { HasText = "Cargo Release Number:" })
                .First;
            await para.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var text = (await para.InnerTextAsync()).Trim();
            // "Cargo Release Number: 721" → take everything after ":"
            var colonIdx = text.IndexOf(':');
            _crId = colonIdx >= 0 ? text.Substring(colonIdx + 1).Trim() : text;
            Console.WriteLine($"[CargoReleasesPage] CR number stored: {_crId}");
        }

        /// <summary>
        /// Verifies the Pickup Order detail page heading is visible.
        /// HTML: h3.font-weight-normal "Pickup Order {name}"
        /// </summary>
        public async Task VerifyPickupOrderDetailsPageAsync(string name)
        {
            var expected = $"Pickup Order {name}";
            var heading = Page.Locator("h3.font-weight-normal")
                .Filter(new LocatorFilterOptions { HasText = expected })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected Pickup Order detail heading '{expected}'. URL: {Page.Url}");
        }

        public string GetCRId() => _crId;
        public void SetCRId(string id) => _crId = id;

        /// <summary>
        /// Verifies a CR link (svg[data-icon='link'] inside a[href*='/cargo-releases/']) is visible
        /// and contains the given CR number in the link text.
        /// HTML: a[href*='/my-portal/cargo-releases/']:has(svg[data-icon='link']) "Cargo Release: 721"
        /// </summary>
        public async Task VerifyCRLinkedToWHAsync(string crText)
        {
            var link = await TryFindLocatorAsync(new[]
            {
                $"a[href*='/my-portal/cargo-releases/']:has(svg[data-icon='link'])",
                $"//a[contains(@href,'/cargo-releases/') and .//*[@data-icon='link']]"
            }, timeoutMs: 15000);

            Assert.IsNotNull(link, $"CR link not found in cargo details. URL: {Page.Url}");
            var linkText = (await link!.InnerTextAsync()).Trim();
            Assert.IsTrue(linkText.Contains(crText, StringComparison.OrdinalIgnoreCase),
                $"CR link text '{linkText}' does not contain '{crText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the "Continue to cargo release" link on the confirmation page.
        /// HTML: a.btn.btn-primary[href*='/cargo-releases/'] "Continue to cargo release"
        /// </summary>
        public async Task ClickContinueToCargoReleaseAsync()
        {
            var link = Page.Locator("a.btn.btn-primary[href*='/cargo-releases/']")
                .Filter(new LocatorFilterOptions { HasText = "Continue to cargo release" })
                .First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await ClickAndWaitForNetworkAsync(link);
        }

        // ── Status ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the status badge is visible with the given text.
        /// HTML: span.badge.badge-secondary.font-weight-normal with text matching status
        /// </summary>
        public async Task VerifyStatusBadgeAsync(string status)
        {
            var badge = Page.Locator("span.badge.badge-secondary.font-weight-normal")
                .Filter(new LocatorFilterOptions { HasText = status })
                .First;
            await badge.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await badge.IsVisibleAsync(),
                $"Status badge '{status}' not found. URL: {Page.Url}");
        }

        // ── Events ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Waits up to 3 minutes for an event entry containing the given text to appear.
        /// Reloads the page every 2 seconds while the event is not visible.
        /// </summary>
        public async Task VerifyEventVisibleAsync(string eventText)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Event '{eventText}' did not appear after 3 minutes. URL: {Page.Url}");

                var match = Page.GetByText(eventText, new PageGetByTextOptions { Exact = false }).First;
                if (await match.CountAsync() > 0 && await match.IsVisibleAsync())
                    return;

                await Page.ReloadAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Page.WaitForTimeoutAsync(retryIntervalMs);
            }
        }

        // ── Attachments ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies an uploaded file is visible in the attachments list.
        /// HTML: div[normalize-space(text())='{fileName}']
        /// </summary>
        public async Task VerifyUploadedFileAsync(string fileName)
        {
            var fileItem = Page.Locator($"//div[normalize-space(text())='{fileName}']").First;
            await fileItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await fileItem.IsVisibleAsync(),
                $"Uploaded file '{fileName}' not found in the attachments list. URL: {Page.Url}");
        }

        // ── Cargo tab ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies a WH link (svg[data-icon='link'] inside a[href*='/warehouse-receipts/']) is visible
        /// and contains the given text (WH number).
        /// HTML: a[href*='/my-portal/warehouse-receipts/']:has(svg[data-icon='link'])
        /// </summary>
        public async Task VerifyWHLinkedToCRAsync(string whText)
        {
            var link = await TryFindLocatorAsync(new[]
            {
                $"a[href*='/my-portal/warehouse-receipts/']:has(svg[data-icon='link'])",
                $"//a[contains(@href,'/warehouse-receipts/') and .//*[@data-icon='link']]"
            }, timeoutMs: 15000);

            Assert.IsNotNull(link, $"WH link not found in cargo details. URL: {Page.Url}");
            var linkText = (await link!.InnerTextAsync()).Trim();
            Assert.IsTrue(linkText.Contains(whText, StringComparison.OrdinalIgnoreCase),
                $"WH link text '{linkText}' does not contain '{whText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a PK link (svg[data-icon='link'] inside a[href*='/pickup-orders/']) is visible
        /// and contains the given text (PK number).
        /// HTML: a[href*='/my-portal/pickup-orders/']:has(svg[data-icon='link'])
        /// </summary>
        public async Task VerifyPKLinkedToCRAsync(string pkText)
        {
            var link = await TryFindLocatorAsync(new[]
            {
                $"a[href*='/my-portal/pickup-orders/']:has(svg[data-icon='link'])",
                $"//a[contains(@href,'/pickup-orders/') and .//*[@data-icon='link']]"
            }, timeoutMs: 15000);

            Assert.IsNotNull(link, $"PK link not found in cargo details. URL: {Page.Url}");
            var linkText = (await link!.InnerTextAsync()).Trim();
            Assert.IsTrue(linkText.Contains(pkText, StringComparison.OrdinalIgnoreCase),
                $"PK link text '{linkText}' does not contain '{pkText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the PK link (svg[data-icon='link'] inside a[href*='/pickup-orders/']) in cargo details.
        /// HTML: a[href*='/my-portal/pickup-orders/']:has(svg[data-icon='link'])
        /// </summary>
        public async Task ClickPKLinkInCargoDetailsAsync(string pkText)
        {
            var link = await TryFindLocatorAsync(new[]
            {
                $"a[href*='/my-portal/pickup-orders/']:has(svg[data-icon='link'])",
                $"//a[contains(@href,'/pickup-orders/') and .//*[@data-icon='link']]"
            }, timeoutMs: 15000);

            Assert.IsNotNull(link, $"PK link '{pkText}' not found in cargo details. URL: {Page.Url}");
            await ClickAndWaitForNetworkAsync(link!);
        }

        /// <summary>
        /// Clicks the WH link (svg[data-icon='link'] inside a[href*='/warehouse-receipts/']) in cargo details.
        /// HTML: a[href*='/my-portal/warehouse-receipts/']:has(svg[data-icon='link'])
        /// </summary>
        public async Task ClickWHLinkInCargoDetailsAsync(string whText)
        {
            var link = await TryFindLocatorAsync(new[]
            {
                $"a[href*='/my-portal/warehouse-receipts/']:has(svg[data-icon='link'])",
                $"//a[contains(@href,'/warehouse-receipts/') and .//*[@data-icon='link']]"
            }, timeoutMs: 15000);

            Assert.IsNotNull(link, $"WH link '{whText}' not found in cargo details. URL: {Page.Url}");
            await ClickAndWaitForNetworkAsync(link!);
        }
    }
}
