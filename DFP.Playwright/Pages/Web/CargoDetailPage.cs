using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class CargoDetailPage : BasePage
    {
        private readonly TestContext _tc;

        public CargoDetailPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the Cargo Detail nav link when it becomes enabled.
        /// HTML: a[href='/my-portal/cargo-detail'] Cargo Detail
        /// </summary>
        public async Task NavigateToCargoDetailPageAsync()
        {
            var link = Page.Locator("a[href='/my-portal/cargo-detail']").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(link, timeoutMs: 15000);
            await ClickAndWaitForNetworkAsync(link);
        }

        // ── Page verification ─────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the "Your cargo detail" heading is visible.
        /// HTML: h3.font-weight-normal.m-0 "Your cargo detail"
        /// </summary>
        public async Task VerifyCargoDetailPageVisibleAsync()
        {
            var heading = Page.Locator("h3.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = "Your cargo detail" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Your cargo detail' heading. URL: {Page.Url}");
        }

        // ── Search ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Selects the parent type from the dropdown and types the number.
        /// Waits up to 2 minutes for the dropdown to be enabled (page may load slowly).
        /// HTML: p-dropdown[formcontrolname='parent_type'] + input#parent_number
        /// </summary>
        public async Task SearchForParentAsync(string parentType, string number)
        {
            const int maxWaitMs = 120000;
            const int retryIntervalMs = 2000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxWaitMs);

            var dropdown = Page.Locator("p-dropdown[formcontrolname='parent_type']").First;
            await dropdown.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = maxWaitMs });
            await WaitForEnabledAsync(dropdown, timeoutMs: maxWaitMs);

            var item = Page.Locator("p-dropdownitem li.p-dropdown-item")
                .Filter(new LocatorFilterOptions { HasText = parentType })
                .First;

            // Retry clicking the dropdown until the desired item appears (dropdown may not open on first click)
            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Dropdown item '{parentType}' did not appear after 2 minutes. URL: {Page.Url}");

                await dropdown.ClickAsync();
                await Page.WaitForTimeoutAsync(retryIntervalMs);

                if (await item.CountAsync() > 0 && await item.IsVisibleAsync())
                    break;
            }

            await item.ClickAsync();

            var numberInput = Page.Locator("#parent_number").First;
            await numberInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(numberInput, timeoutMs: 15000);
            await numberInput.ClearAsync();
            await TypeAsync(numberInput, number);
        }

        // ── Button ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Generic button click scoped to Cargo Details feature.
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

        // ── Results ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies a result row in the cargo detail table contains the given description and status.
        /// HTML: tbody[role='rowgroup'] tr — filters by description text and status badge text.
        /// </summary>
        public async Task VerifySearchResultAsync(string description, string status)
        {
            var row = Page.Locator("tbody[role='rowgroup'] tr")
                .Filter(new LocatorFilterOptions { HasText = description })
                .Filter(new LocatorFilterOptions { HasText = status })
                .First;
            await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            Assert.IsTrue(await row.IsVisibleAsync(),
                $"Expected row with description '{description}' and status '{status}'. URL: {Page.Url}");
        }

        // ── Cargo Release search ──────────────────────────────────────────────────

        /// <summary>
        /// Types into the Cargo Release number search input.
        /// HTML: input[formcontrolname='number'][placeholder='Cargo Release #']
        /// </summary>
        public async Task EnterCargoReleaseNumberAsync(string number)
        {
            var input = Page.Locator("input[formcontrolname='number'][placeholder='Cargo Release #']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(input, timeoutMs: 15000);
            await input.ClearAsync();
            await TypeAsync(input, number);
        }

        /// <summary>
        /// Waits for a Cargo Release list item containing the given text to appear.
        /// HTML: li[qwyk-cargo-releases-index-list-item]
        /// </summary>
        public async Task VerifyCargoReleaseVisibleInListAsync(string text)
        {
            var item = Page.Locator("li[qwyk-cargo-releases-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            Assert.IsTrue(await item.IsVisibleAsync(),
                $"Expected Cargo Release list item containing '{text}'. URL: {Page.Url}");
        }
    }
}
