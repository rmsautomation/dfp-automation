using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class InvoicePage : BasePage
    {
        private string _invoiceName = string.Empty;
        private readonly string _baseUrl;

        public InvoicePage(IPage page, string baseUrl) : base(page)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // Invoices nav link: <a class="nav-link" href="/my-portal/invoices"> Invoices </a>
        private static readonly string[] InvoicesNavSelectors =
        [
            "//a[@href='/my-portal/invoices' and normalize-space()='Invoices']",
            "a.nav-link[href='/my-portal/invoices']"
        ];

        // Invoice number search input: input#number placeholder="Invoice number"
        private static readonly string[] InvoiceNumberInputSelectors =
        [
            "input#number",
            "input[formcontrolname='number']",
            "input.invoice-number-input",
            "input[placeholder='Invoice number']"
        ];

        // Search button
        private static readonly string[] SearchButtonSelectors =
        [
            "internal:role=button[name=\"Search\"i]",
            "button:has-text('Search')"
        ];

        // Charges tab: <a class="nav-link" href="...?view=charges"><svg data-icon="file-invoice"/> Charges </a>
        private static readonly string[] ChargesTabNavSelectors =
        [
            "//a[contains(@href,'?view=charges')][.//svg[@data-icon='file-invoice']]",
            "//a[contains(@href,'?view=charges')]",
            "a.nav-link:has(svg[data-icon='file-invoice'])"
        ];

        // ── State helpers ─────────────────────────────────────────────────────────

        public void SetInvoiceName(string name) => _invoiceName = name;
        public string GetInvoiceName() => _invoiceName;

        // ── Navigation methods ────────────────────────────────────────────────────

        /// <summary>
        /// Navigates directly to the Invoices list page.
        /// URL: /my-portal/invoices
        /// </summary>
        public async Task NavigateToInvoicesListAsync()
        {
            await Page.GotoAsync(_baseUrl + "/my-portal/invoices");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Tab navigation methods ────────────────────────────────────────────────

        /// <summary>
        /// Clicks the Charges tab on the invoice detail page.
        /// HTML: a.nav-link href="...?view=charges" with svg data-icon="file-invoice"
        /// </summary>
        public async Task ClickChargesTabAsync()
        {
            var tab = await FindLocatorAsync(ChargesTabNavSelectors, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(tab);
        }

        // ── Search methods ────────────────────────────────────────────────────────

        /// <summary>
        /// Types the stored invoice name into the Invoice number input.
        /// If param is empty, uses stored _invoiceName.
        /// HTML: input#number placeholder="Invoice number"
        /// </summary>
        public async Task EnterInvoiceNameInSearchFieldAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = _invoiceName;

            var input = await FindLocatorAsync(InvoiceNumberInputSelectors, timeoutMs: 15000);
            await WaitForEnabledAsync(input, timeoutMs: 15000);
            await input.ClearAsync();
            await TypeAsync(input, name);
        }

        public async Task ClickSearchButtonAsync()
        {
            var btn = await FindLocatorAsync(SearchButtonSelectors, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        // ── Assertion methods ─────────────────────────────────────────────────────

        /// <summary>
        /// Waits up to 5s for a div with the given text to appear in the invoice list.
        /// If not found, clicks Search every 2 seconds for up to 3 minutes.
        /// HTML: &lt;div class="small"&gt;Invoice&lt;/div&gt;
        /// </summary>
        public async Task SelectInvoiceInSearchResultsWithTextAsync(string text)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var resultDiv = Page.Locator($"//div[contains(normalize-space(),'{text}')]").First;

            // Check immediately first — only enter the retry loop if not yet visible
            try
            {
                await resultDiv.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                await resultDiv.ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return;
            }
            catch (TimeoutException) { }

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Invoice with text '{text}' not found in search results after 3 minutes. URL: {Page.Url}");

                var searchButton = await TryFindLocatorAsync(SearchButtonSelectors, timeoutMs: 3000);
                if (searchButton != null)
                    await ClickAndWaitForNetworkAsync(searchButton);

                await Page.WaitForTimeoutAsync(retryIntervalMs);

                if (await resultDiv.IsVisibleAsync())
                {
                    await resultDiv.ClickAsync();
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    return;
                }
            }
        }

        /// <summary>
        /// Waits for the invoice detail page heading to be visible and enabled.
        /// If name is empty, falls back to stored _invoiceName.
        /// HTML: &lt;h3 class="font-weight-normal m-0"&gt; Invoice TC1083_1087 &lt;/h3&gt;
        /// </summary>
        public async Task VerifyInvoiceDetailsHeadingAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = _invoiceName;

            var expected = $"Invoice {name}";
            var heading = Page.Locator("h3.font-weight-normal").Filter(new LocatorFilterOptions { HasText = expected }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected invoice heading to contain '{expected}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Waits up to 5s for any element containing the given text to appear in the invoice list.
        /// If not found, clicks Search every 2 seconds for up to 3 minutes. Does NOT click the item.
        /// HTML: &lt;div class="small"&gt;Invoice&lt;/div&gt; inside a list row.
        /// XPath: //div[contains(normalize-space(),'{text}')]
        /// </summary>
        public async Task TheInvoiceShouldAppearInSearchResultsInListAsync(string text)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var resultDiv = Page.Locator($"//div[contains(normalize-space(),'{text}')]").First;

            // Check immediately first — only enter the retry loop if not yet visible
            try
            {
                await resultDiv.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                return;
            }
            catch (TimeoutException) { }

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Invoice with text '{text}' not found in search results after 3 minutes. URL: {Page.Url}");

                var searchButton = await TryFindLocatorAsync(SearchButtonSelectors, timeoutMs: 3000);
                if (searchButton != null)
                    await ClickAndWaitForNetworkAsync(searchButton);

                await Page.WaitForTimeoutAsync(retryIntervalMs);

                if (await resultDiv.IsVisibleAsync())
                    return;
            }
        }

        /// <summary>
        /// Verifies a label/value pair in the invoice Details card.
        /// HTML: &lt;label class="small m-0 font-weight-bold"&gt; Invoice date &lt;/label&gt;
        ///       &lt;div&gt;03/30/2026&lt;/div&gt;
        /// XPath: //label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyInvoiceLabelHeaderContainsAsync(string labelText, string expectedValue)
        {
            var valueDiv = Page.Locator($"//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a custom field inside &lt;qwyk-custom-fields-view&gt; on the invoice detail page.
        /// HTML: label.small.font-weight-bold.m-0 + div sibling
        /// XPath: //qwyk-custom-fields-view//label[normalize-space()='{label}']/following-sibling::div[1]
        /// </summary>
        public async Task VerifyInvoiceCustomFieldContainsAsync(string labelText, string expectedValue)
        {
            var valueDiv = Page.Locator(
                $"//qwyk-custom-fields-view//label[normalize-space()='{labelText}']/following-sibling::div[1]").First;
            await WaitForEnabledAsync(valueDiv, timeoutMs: 15000);
            var actualText = (await valueDiv.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Custom field '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple label/value pairs in the invoice Details card using a data table.
        /// </summary>
        public async Task VerifyInvoiceLabelHeadersAsync(IEnumerable<(string labelText, string expectedValue)> pairs)
        {
            foreach (var (labelText, expectedValue) in pairs)
                await VerifyInvoiceLabelHeaderContainsAsync(labelText, expectedValue);
        }

        /// <summary>
        /// Verifies multiple custom fields inside qwyk-custom-fields-view using a data table.
        /// </summary>
        public async Task VerifyInvoiceCustomFieldsAsync(IEnumerable<(string labelText, string expectedValue)> pairs)
        {
            foreach (var (labelText, expectedValue) in pairs)
                await VerifyInvoiceCustomFieldContainsAsync(labelText, expectedValue);
        }
    }
}
