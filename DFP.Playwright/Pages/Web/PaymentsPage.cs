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
    public sealed class PaymentsPage : BasePage
    {
        private readonly TestContext _tc;

        public PaymentsPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to /my-portal/payments.
        /// HTML: a[href='/my-portal/payments'] Payments
        /// </summary>
        public async Task NavigateToPaymentsPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/payments");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the Payments list heading is visible.
        /// HTML: h3.font-weight-normal.m-0 "Your payments"
        /// </summary>
        public async Task VerifyPaymentsListVisibleAsync()
        {
            var heading = Page.Locator("h3.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = "Your payments" })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(heading, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Your payments' heading. URL: {Page.Url}");
        }

        // ── Buttons ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Generic button click — no dialog wait logic.
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

        // ── Search ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Types into the Payment number search input.
        /// HTML: input.payment-number-input[formcontrolname='number'] placeholder="Payment number"
        /// </summary>
        public async Task EnterPaymentNumberAsync(string number)
        {
            var input = await TryFindLocatorAsync(new[]
            {
                "input.payment-number-input",
                "input[formcontrolname='number'][placeholder='Payment number']",
                "input#number[placeholder='Payment number']"
            }, timeoutMs: 15000);

            Assert.IsNotNull(input, $"Payment number input not found. URL: {Page.Url}");
            await WaitForEnabledAsync(input!, timeoutMs: 10000);
            await input!.ClearAsync();
            await TypeAsync(input, number);
        }

        // ── List ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Waits up to 3 minutes for a payment row containing the given text to appear.
        /// Clicks Search every 2 seconds while no matching result is visible.
        /// HTML: li[qwyk-payments-list-view-item]
        /// </summary>
        public async Task VerifyPaymentVisibleInListAsync(string text)
        {
            const int retryIntervalMs = 2000;
            const int maxDurationMs = 180000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxDurationMs);

            var item = Page.Locator("li[qwyk-payments-list-view-item]")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;

            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Payment '{text}' did not appear in the list after 3 minutes. URL: {Page.Url}");

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
        /// Clicks the payment row containing the given text.
        /// HTML: li[qwyk-payments-list-view-item]
        /// </summary>
        public async Task SelectPaymentFromListAsync(string text)
        {
            var item = Page.Locator("li[qwyk-payments-list-view-item]")
                .Filter(new LocatorFilterOptions { HasText = text })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await item.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Detail page ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the Payment detail page heading contains the given number.
        /// HTML: h3.font-weight-normal.m-0 "Payment {number}"
        /// </summary>
        public async Task VerifyPaymentDetailsPageAsync(string number)
        {
            var expected = $"Payment {number}";
            var heading = Page.Locator("h3.font-weight-normal.m-0")
                .Filter(new LocatorFilterOptions { HasText = expected })
                .First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected Payment detail heading '{expected}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a label/value pair in the Payment details card.
        /// Handles both label+h5 (Total amount) and label+div patterns.
        /// XPath: //label[normalize-space()='{label}']/(following-sibling::div|following-sibling::h5)[1]
        /// </summary>
        public async Task VerifyPaymentDetailLabelAsync(string labelText, string expectedValue)
        {
            var valueEl = Page.Locator(
                $"//label[normalize-space()='{labelText}']/following-sibling::*[self::div or self::h5][1]"
            ).First;
            await valueEl.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            var actualText = (await valueEl.InnerTextAsync()).Trim();
            Assert.IsTrue(actualText.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
                $"Label '{labelText}': expected to contain '{expectedValue}' but found '{actualText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple label/value pairs in the Payment details card.
        /// </summary>
        public async Task VerifyPaymentDetailsAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (label, value) in pairs)
                await VerifyPaymentDetailLabelAsync(label, value);
        }

        // ── Invoices section ──────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that at least one invoice row in the Invoices section contains the expected value.
        /// Columns (Invoice number, Amount, Applied) are positional — we match by text presence in the row.
        /// HTML: li[qwyk-payment-invoices-list-view-item]
        /// </summary>
        public async Task VerifyInvoiceSectionRowContainsAsync(string value)
        {
            var row = Page.Locator("li[qwyk-payment-invoices-list-view-item]")
                .Filter(new LocatorFilterOptions { HasText = value })
                .First;
            await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await row.IsVisibleAsync(),
                $"Expected invoice row containing '{value}' in Invoices section. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies multiple values exist in invoice rows in the Invoices section.
        /// The "label" column from the table is ignored — only the value is checked.
        /// </summary>
        public async Task VerifyInvoicesSectionAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (_, value) in pairs)
                await VerifyInvoiceSectionRowContainsAsync(value);
        }

        // ── Attachments ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies a file name is visible in the attachments list.
        /// HTML: div with file name text inside the attachments section.
        /// </summary>
        public async Task VerifyUploadedFileAsync(string fileName)
        {
            var fileItem = Page.Locator($"//div[normalize-space(text())='{fileName}']").First;
            await fileItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await fileItem.IsVisibleAsync(),
                $"Uploaded file '{fileName}' not found in attachments. URL: {Page.Url}");
        }
    }
}
