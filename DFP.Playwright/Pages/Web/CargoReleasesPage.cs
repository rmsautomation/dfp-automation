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
        /// Clicks any button by text — generic, no dialog-wait logic.
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
