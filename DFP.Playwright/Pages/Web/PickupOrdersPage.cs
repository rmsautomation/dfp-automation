using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using UglyToad.PdfPig;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class PickupOrdersPage : BasePage
    {
        private readonly TestContext _tc;
        private string _pickupTotalPieces = string.Empty;
        private string _lastDownloadPath = string.Empty;

        public string GetPickupTotalPieces() => _pickupTotalPieces;

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
        /// Navigates to the Pickup Orders list page.
        /// HTML: a[href='/my-portal/pickup-orders']
        /// </summary>
        public async Task NavigateToPickupOrdersPageAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/pickup-orders");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Page verification ─────────────────────────────────────────────────────

        /// <summary>
        /// Waits for the pickup orders list to be visible and non-empty.
        /// HTML: li[qwyk-pickup-orders-index-list-item]
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
        /// HTML: input[placeholder='Pickup order #'] or input[formcontrolname='number']
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
        /// Generic button click scoped to Pickup Orders feature.
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
        /// Waits for a pickup order list item containing the given number. Retries clicking
        /// Search every 2 seconds for up to 3 minutes.
        /// HTML: li[qwyk-pickup-orders-index-list-item]
        /// </summary>
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

        /// <summary>
        /// Clicks the list item that contains the given number and waits for navigation.
        /// HTML: li[qwyk-pickup-orders-index-list-item]
        /// </summary>
        public async Task ClickPickupOrderInListAsync(string number)
        {
            var item = Page.Locator("li[qwyk-pickup-orders-index-list-item]")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(item, timeoutMs: 15000);
            await ClickAndWaitForNavigationAsync(item);
        }

        // ── Details page ──────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies the heading "Pickup Order {number}" is visible on the details page.
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

        /// <summary>
        /// Verifies a label/value pair inside the Pickup Order summary card.
        /// HTML: label.small.font-weight-bold followed by sibling div with the value.
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

        /// <summary>
        /// Verifies all label/value pairs from the data table.
        /// </summary>
        public async Task VerifyPickupOrderDetailsAsync(IEnumerable<(string label, string value)> pairs)
        {
            foreach (var (label, value) in pairs)
                await VerifyPickupOrderDetailLabelAsync(label, value);
        }

        // ── Cargo details ─────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the total pieces from the cargo details section and stores it in _pickupTotalPieces.
        /// HTML: div.col-3.font-weight-bold > div "251 pieces"
        /// </summary>
        public async Task StoreTotalPiecesAsync()
        {
            // Match any div whose text contains "pieces" — same pattern as WarehouseReceiptPage
            var el = Page.Locator("//div[contains(normalize-space(text()),'pieces')]").First;

            await Page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
            await el.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
            await el.ScrollIntoViewIfNeededAsync();

            _pickupTotalPieces = (await el.InnerTextAsync()).Trim();
            Console.WriteLine($"[PickupOrdersPage] Stored pickupTotalPieces: {_pickupTotalPieces}");
        }

        // ── PDF ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the PDF dropdown button.
        /// HTML: qwyk-downloadable-drop-down button.btn-outline-secondary with text "PDF"
        /// </summary>
        public async Task ClickPdfButtonAsync()
        {
            var btn = Page.Locator("qwyk-downloadable-drop-down button.btn-outline-secondary")
                .Filter(new LocatorFilterOptions { HasText = "PDF" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            await btn.ClickAsync();
        }

        /// <summary>
        /// Clicks a PDF dropdown option (e.g. "Download PDF", "Email PDF"),
        /// intercepts the resulting download and saves it to a temp path for later verification.
        /// HTML: button.list-group-item inside ul.list-group-flush
        /// </summary>
        public async Task SelectPdfOptionAsync(string optionText)
        {
            var option = Page.Locator("ul.list-group-flush button.list-group-item")
                .Filter(new LocatorFilterOptions { HasText = optionText })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(option, timeoutMs: 15000);

            // Start intercepting download before the click triggers it
            var downloadTask = Page.WaitForDownloadAsync();
            await option.ClickAsync();
            var download = await downloadTask;

            var tempPath = Path.Combine(Path.GetTempPath(), download.SuggestedFilename);
            await download.SaveAsAsync(tempPath);
            _lastDownloadPath = tempPath;
            Console.WriteLine($"[PickupOrdersPage] PDF saved to: {tempPath}");
        }

        /// <summary>
        /// Extracts text from the last downloaded PDF and verifies it contains
        /// the total pieces value stored by StoreTotalPiecesAsync.
        /// Equivalent to openLastDownloadedFileCommodityQuantity() in TestComplete.
        /// </summary>
        public Task VerifyTotalPiecesInPdfAsync()
        {
            Assert.IsFalse(string.IsNullOrEmpty(_pickupTotalPieces),
                "pickupTotalPieces is empty. Run 'I store the total pieces' before this step.");
            Assert.IsFalse(string.IsNullOrEmpty(_lastDownloadPath),
                "No PDF has been downloaded. Run 'I select Download PDF option' before this step.");
            Assert.IsTrue(File.Exists(_lastDownloadPath),
                $"PDF file not found at: {_lastDownloadPath}");

            // Extract text from PDF using PdfPig
            var sb = new StringBuilder();
            using (var pdfDoc = PdfDocument.Open(_lastDownloadPath))
            {
                foreach (var page in pdfDoc.GetPages())
                    sb.Append(page.Text);
            }

            var pdfText = sb.ToString();
            Console.WriteLine($"[PickupOrdersPage] PDF text (first 500 chars): {pdfText[..Math.Min(500, pdfText.Length)]}");

            // Normalize whitespace before comparing
            var normalized = Regex.Replace(pdfText, @"\s+", " ");
            // Extract just the number part: "251" from "251 pieces"
            var searchValue = _pickupTotalPieces.Split(' ')[0];

            Console.WriteLine($"[PickupOrdersPage] Looking for: '{searchValue}' (stored: '{_pickupTotalPieces}')");

            Assert.IsTrue(normalized.Contains(searchValue, StringComparison.OrdinalIgnoreCase),
                $"PDF does not contain total pieces '{searchValue}' (stored: '{_pickupTotalPieces}'). File: {_lastDownloadPath}");

            try { File.Delete(_lastDownloadPath); } catch { /* ignore cleanup errors */ }
            return Task.CompletedTask;
        }

        // ── Pagination ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the "Last Page" paginator button if it is enabled.
        /// HTML: button.p-paginator-last
        /// </summary>
        public async Task GoToLastPageAsync()
        {
            await DismissCookieBannerIfVisibleAsync();

            var btn = Page.Locator("button.p-paginator-last").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 15000);
            await btn.ScrollIntoViewIfNeededAsync();
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Selects the rows-per-page option from the PrimeNG paginator dropdown.
        /// Uses ScrollIntoViewIfNeeded + JS click to avoid Chrome download bar overlap.
        /// HTML: p-dropdown inside p-paginator, li.p-dropdown-item with the number text.
        /// </summary>
        public async Task SelectPaginationNumberAsync(string number)
        {
            await DismissCookieBannerIfVisibleAsync();

            const int maxWaitMs = 30000;
            var deadline = DateTime.UtcNow.AddMilliseconds(maxWaitMs);

            // Target the chevron trigger inside the rows-per-page dropdown (p-paginator-rpp-options),
            // NOT the jump-to-page dropdown (p-paginator-page-options) which is the first dropdown.
            var trigger = Page.Locator("div.p-paginator-rpp-options div.p-dropdown-trigger[role='button']").First;
            await trigger.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(trigger, timeoutMs: 15000);
            await trigger.ScrollIntoViewIfNeededAsync();

            var option = Page.Locator("p-dropdownitem li.p-dropdown-item")
                .Filter(new LocatorFilterOptions { HasText = number })
                .First;

            // Retry clicking the trigger until the desired option appears in the overlay panel
            while (true)
            {
                if (DateTime.UtcNow >= deadline)
                    Assert.Fail($"Pagination option '{number}' did not appear after 30 seconds. URL: {Page.Url}");

                await trigger.ClickAsync();

                var pollDeadline = DateTime.UtcNow.AddMilliseconds(3000);
                while (DateTime.UtcNow < pollDeadline)
                {
                    if (await option.CountAsync() > 0 && await option.IsVisibleAsync())
                        goto optionFound;
                    await Page.WaitForTimeoutAsync(200);
                }
            }
            optionFound:

            await option.DispatchEventAsync("click");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
        }

        // ── Attachments ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifies that a file name is visible in the attachments list.
        /// HTML: div with the file name as text content
        /// </summary>
        public async Task VerifyUploadedFileAsync(string fileName)
        {
            var fileItem = Page.Locator($"//div[normalize-space(text())='{fileName}']").First;
            await fileItem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await fileItem.IsVisibleAsync(),
                $"File '{fileName}' not found in attachments. URL: {Page.Url}");
        }
    }
}
