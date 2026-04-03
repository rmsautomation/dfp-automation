using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class PickupOrdersPage : BasePage
    {
        private readonly TestContext _tc;

        public PickupOrdersPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the Pickup Orders nav link when it becomes enabled.
        /// HTML: a[href='/my-portal/pickup-orders'] Pickup Orders
        /// </summary>
        public async Task NavigateToPickupOrdersPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/pickup-orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Page verification ─────────────────────────────────────────────────────

        /// <summary>
        /// Waits for the pickup orders list to be visible and non-empty.
        /// HTML: qwyk-pickup-orders-index-list ul.list-group li[qwyk-pickup-orders-index-list-item]
        /// </summary>
        public async Task VerifyPickupOrdersListVisibleAsync()
        {
            var listItem = Page.Locator("li[qwyk-pickup-orders-index-list-item]").First;
            await listItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            Assert.IsTrue(await listItem.IsVisibleAsync(),
                $"Expected Pickup Orders list to be visible and non-empty. URL: {Page.Url}");
        }

        // ── Search ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Types into the Pickup Order number search input.
        /// HTML: input[formcontrolname='number'] or input[placeholder='Pickup order #']
        /// </summary>
        public async Task EnterPickupOrderNumberAsync(string number)
        {
            var input = await TryFindLocatorAsync(new[]
            {
                "input[placeholder='Pickup order #']",
                "input[formcontrolname='number'][placeholder='Pickup order #']",
                "input[formcontrolname='number']"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Pickup Order number input not found. URL: {Page.Url}");
            await WaitForEnabledAsync(input!, timeoutMs: 15000);
            await input!.ClearAsync();
            await TypeAsync(input, number);
        }

        /// <summary>
        /// Generic button click for Pickup Orders feature.
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
        /// Waits for a pickup order list item containing the given number to be visible.
        /// HTML: li[qwyk-pickup-orders-index-list-item]
        /// </summary>
        public async Task VerifyPickupOrderVisibleInListAsync(string number)
        {
            var item = Page.Locator("li[qwyk-pickup-orders-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            Assert.IsTrue(await item.IsVisibleAsync(),
                $"Expected Pickup Order '{number}' in list. URL: {Page.Url}");
        }
    }
}
