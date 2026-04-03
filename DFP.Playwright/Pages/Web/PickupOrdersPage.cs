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
        /// <summary>
        /// Da click en el item de la lista que contiene el número indicado.
        /// HTML: li[qwyk-pickup-orders-index-list-item]
        /// </summary>
        /// <summary>
        /// Verifica que un archivo subido sea visible en la lista de adjuntos.
        /// HTML: div con el nombre del archivo
        /// </summary>
        public async Task VerifyUploadedFileAsync(string fileName)
        {
            var fileItem = Page.Locator($"//div[normalize-space(text())='{fileName}']").First;
            await fileItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await fileItem.IsVisibleAsync(),
                $"Archivo '{fileName}' no encontrado en los adjuntos. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifica label/valor en el summary del Pickup Order.
        /// HTML: label.small.font-weight-bold seguido de div con el valor.
        /// XPath: //label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyPickupOrderDetailLabelAsync(string label, string expectedValue)
        {
            var valueEl = Page.Locator(
                $"//label[normalize-space()='{label}']/following-sibling::div[1]"
            ).First;
            await valueEl.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var actual = (await valueEl.InnerTextAsync()).Trim();
            Assert.IsTrue(actual.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{label}': expected '{expectedValue}' but got '{actual}'. URL: {Page.Url}");
        }

        public async Task VerifyPickupOrderDetailsAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (label, value) in pairs)
                await VerifyPickupOrderDetailLabelAsync(label, value);
        }

        /// <summary>
        /// Verifica que el heading "Pickup Order {number}" sea visible en la página de detalles.
        /// HTML: h3.font-weight-normal "Pickup Order TC1875_1880"
        /// </summary>
        public async Task VerifyPickupOrderDetailsPageAsync(string number)
        {
            var heading = Page.Locator("h3.font-weight-normal")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Pickup Order details heading with '{number}' not visible. URL: {Page.Url}");
        }

        public async Task ClickPickupOrderInListAsync(string number)
        {
            var item = Page.Locator("li[qwyk-pickup-orders-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(item, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(item);
        }

        public async Task VerifyPickupOrderVisibleInListAsync(string number)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var item = Page.Locator("li[qwyk-pickup-orders-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Pickup Order '{number}' did not appear in list after 3 minutes. URL: {Page.Url}");

                if (await item.CountAsync() > 0 && await item.IsVisibleAsync())
                    return;

                var searchBtn = Page.Locator("button")
                    .Filter(new LocatorFilterOptions { HasText = "Search" })
                    .First;
                if (await searchBtn.CountAsync() > 0 && await searchBtn.IsVisibleAsync())
                    await ClickAndWaitForNetworkAsync(searchBtn);

                await Page.WaitForTimeoutAsync(retryIntervalMs);
            }
        }
    }
}
