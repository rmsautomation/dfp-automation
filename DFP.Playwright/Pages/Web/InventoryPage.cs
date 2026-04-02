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
    public sealed class InventoryPage : BasePage
    {
        private readonly TestContext _tc;

        public InventoryPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to /my-portal/warehouse-inventory.
        /// HTML: a[href='/my-portal/warehouse-inventory'] Inventory
        /// </summary>
        public async Task NavigateToInventoryPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/warehouse-inventory");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the inventory page heading is visible.
        /// HTML: h3.font-weight-normal.m-0 "Your warehouse inventory"
        /// </summary>
        public async Task VerifyInventoryPageVisibleAsync()
        {
            var heading = Page.Locator("h3.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = "Your warehouse inventory" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Your warehouse inventory' heading. URL: {Page.Url}");
        }

        // ── Search ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Types a value into the search input matching the given field label.
        /// Maps label text to the formcontrolname/placeholder of the input.
        /// HTML: input[formcontrolname='part_number'] placeholder="Part number"
        /// </summary>
        public async Task SearchInventoryItemAsync(string fieldLabel, string value)
        {
            // Map friendly label to the input's placeholder/id
            var placeholder = fieldLabel; // placeholder matches the label text directly
            var input = await TryFindLocatorAsync(new[]
            {
                $"input[placeholder='{placeholder}']",
                $"input[id='{fieldLabel.ToLower().Replace(" ", "_")}']",
                $"//input[@placeholder='{placeholder}']"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Search input for '{fieldLabel}' not found. URL: {Page.Url}");
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
        /// Waits up to 3 minutes for at least one row to appear in the datatable.
        /// Clicks Search every 2 seconds while no results are visible.
        /// HTML: .p-datatable-tbody tr
        /// </summary>
        public async Task VerifyInventoryItemVisibleInListAsync()
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var rows = Page.Locator(".p-datatable-tbody tr");

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"No inventory items appeared in the list after 3 minutes. URL: {Page.Url}");

                if (await rows.CountAsync() > 0 && await rows.First.IsVisibleAsync())
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
        /// Clicks the row in the datatable containing the given text (case-insensitive).
        /// HTML: tbody tr > td with the item text
        /// </summary>
        public async Task SelectInventoryItemFromListAsync(string text)
        {
            var row = Page.Locator(".p-datatable-tbody tr")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;
            await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await row.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Detail page ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the inventory item detail page is loaded.
        /// HTML: h5.font-weight-normal.m-0 "Definition"
        /// </summary>
        public async Task VerifyInventoryItemDetailsPageAsync()
        {
            var heading = Page.Locator("h5.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = "Definition" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Definition' heading on inventory detail page. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a label/value pair in the inventory item details card.
        /// HTML: label.small.font-weight-bold.m-0 + following-sibling div
        /// XPath: //label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyInventoryDetailLabelAsync(string labelText, string expectedValue)
        {
            var valueDiv = Page.Locator($"//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple label/value pairs in the inventory details card.
        /// </summary>
        public async Task VerifyInventoryDetailsAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (label, value) in pairs)
                await VerifyInventoryDetailLabelAsync(label, value);
        }

        /// <summary>
        /// Verifies the total pieces badge shows the expected count.
        /// HTML: span.p-badge.p-component with text matching expectedCount
        /// </summary>
        public async Task VerifyTotalPiecesAsync(string expectedCount)
        {
            var badge = Page.Locator("span.p-badge.p-component")
                .Filter(new LocatorFilterOptions { HasText = expectedCount })
                .First;
            await badge.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var actualText = (await badge.InnerTextAsync()).Trim();
            Assert.AreEqual(expectedCount, actualText,
                $"Expected total pieces badge to show '{expectedCount}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies that at least one cargo item row (li[qwyk-cargo-items-index-list-item]) contains the given text.
        /// HTML: li.list-group-item[qwyk-cargo-items-index-list-item] > div.row > div.col-*
        /// </summary>
        public async Task VerifyCargoItemsColumnTextAsync(string expectedText)
        {
            var item = Page.Locator("li[qwyk-cargo-items-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = expectedText })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await item.IsVisibleAsync(),
                $"Expected to find cargo item containing '{expectedText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the On Hand p-avatar icon (hand-holding-box).
        /// HTML: p-avatar > div > fa-icon > svg[data-icon='hand-holding-box']
        /// </summary>
        public async Task ClickOnHandIconAsync()
        {
            var avatar = await TryFindLocatorAsync(new[]
            {
                "p-avatar:has(svg[data-icon='hand-holding-box'])",
                "//p-avatar[.//*[@data-icon='hand-holding-box']]",
                "div.p-avatar:has(svg[data-icon='hand-holding-box'])"
            }, timeoutMs: 15000);

            Assert.IsNotNull(avatar, $"On Hand icon (hand-holding-box) not found. URL: {Page.Url}");
            await WaitForEnabledAsync(avatar!, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(avatar!);
        }
    }
}
