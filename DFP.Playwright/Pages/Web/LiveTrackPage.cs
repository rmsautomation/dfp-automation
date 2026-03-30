using Microsoft.Playwright;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    /// <summary>
    /// Page object for the LiveTrack external portal (https://tracking.magaya.com/#livetrack).
    /// This is a separate ExtJS-based application, not the DFP portal.
    /// </summary>
    public sealed class LiveTrackPage : BasePage
    {
        private const string LiveTrackUrl = "https://tracking.magaya.com/#livetrack";
        private const string DefaultPassword = "M@g@y@33166";

        public LiveTrackPage(IPage page) : base(page)
        {
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // ExtJS dynamic id pattern: input whose id matches the 'for' attr of the label
        // XPath: //input[@id=(//label[.='Network ID']/@for)]
        private const string NetworkIdInputXPath = "//input[@id=(//label[.='Network ID']/@for)]";
        private const string UsernameInputXPath   = "//input[@id=(//label[.='Username']/@for)]";
        private const string PasswordInputXPath   = "//input[@id=(//label[.='Password']/@for)]";

        // LOGIN button: span with class x-btn-inner containing "LOGIN"
        private const string LoginButtonXPath = "//span[contains(@class,'x-btn-inner') and .//div[normalize-space()='LOGIN']]";

        // Skip button: span#button-1028-btnInnerEl with text "Skip"
        private const string SkipButtonXPath = "//span[contains(@class,'x-btn-inner') and normalize-space()='Skip']";

        // Invoices menu item: second strong with text 'Invoices'
        private const string InvoicesMenuXPath = "(//strong[text()='Invoices'])[2]";

        // Filter button: splitbutton with fa-filter icon
        private const string FilterButtonXPath =
            "//a[.//span[contains(@class,'fa-filter')]]";
        // Number input inside the filter form — label-for pattern (same as login fields)
        private const string FilterNumberInputXPath =
            "//input[@id=(//label[.='Number:']/@for)]";

        // OK button
        private const string OkButtonXPath = "//span[contains(@class,'x-btn-inner') and normalize-space()='OK']";

        // Refresh button (has fa-refresh icon inside)
        private const string RefreshButtonXPath = "//button[.//*[contains(@class,'fa-refresh')]]";

        // Approve Invoice context-menu item
        private const string ApproveInvoiceXPath = "//span[contains(normalize-space(),'Approve Invoice')]";

        // Comments textarea
        private const string CommentsTextareaXPath = "//textarea[@id=(//label[.='Comments:']/@for)]";

        // Submit button
        private const string SubmitButtonXPath = "//span[normalize-space(text())='Submit']";

        // ── Methods ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to LiveTrack, fills Network ID, Username and Password, clicks LOGIN,
        /// then clicks Skip when enabled.
        /// If password is empty, defaults to M@g@y@33166.
        /// </summary>
        public async Task LoginAsync(string username, string networkId, string password)
        {
            if (string.IsNullOrEmpty(password))
                password = DefaultPassword;

            await Page.GotoAsync(LiveTrackUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Network ID
            var networkInput = Page.Locator(NetworkIdInputXPath).First;
            await WaitForEnabledAsync(networkInput, timeoutMs: 15000);
            await networkInput.ClearAsync();
            await TypeAsync(networkInput, networkId);

            // Username
            var usernameInput = Page.Locator(UsernameInputXPath).First;
            await WaitForEnabledAsync(usernameInput, timeoutMs: 10000);
            await usernameInput.ClearAsync();
            await TypeAsync(usernameInput, username);

            // Password
            var passwordInput = Page.Locator(PasswordInputXPath).First;
            await WaitForEnabledAsync(passwordInput, timeoutMs: 10000);
            await passwordInput.ClearAsync();
            await TypeAsync(passwordInput, password);

            // LOGIN button
            var loginBtn = Page.Locator(LoginButtonXPath).First;
            await WaitForEnabledAsync(loginBtn, timeoutMs: 10000);
            await loginBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Skip button (optional — appears after login on some flows)
            var skipBtn = Page.Locator(SkipButtonXPath).First;
            try
            {
                await skipBtn.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 8000
                });
                await WaitForEnabledAsync(skipBtn, timeoutMs: 8000);
                await skipBtn.ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            catch (TimeoutException)
            {
                // Skip button did not appear — login went directly to the app, that's fine
            }
        }

        /// <summary>
        /// Clicks the second "Invoices" strong element in the side menu.
        /// Waits up to 30 s for it to become clickable (page may be slow to load).
        /// </summary>
        public async Task GoToInvoicesAsync()
        {
            var invoicesMenu = Page.Locator(InvoicesMenuXPath).First;
            await WaitForEnabledAsync(invoicesMenu, timeoutMs: 30000);
            await invoicesMenu.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Filters the invoice grid by number.
        /// 1. Waits for the grid to appear.
        /// 2. Clicks the picker trigger to open the field selector.
        /// 3. Clicks the Filter (splitbutton) to open the text input.
        /// 4. Types the number into the text field.
        /// </summary>
        public async Task FilterByNumberAsync(string number)
        {
            // Wait for any ExtJS loading mask to disappear before interacting
            var loadingMask = Page.Locator("div.x-mask").First;
            try
            {
                await loadingMask.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = 20000
                });
            }
            catch (TimeoutException) { /* no mask present — proceed */ }

            // Click the Filter button (fa-filter icon)
            var filterButton = Page.Locator(FilterButtonXPath).First;
            await WaitForEnabledAsync(filterButton, timeoutMs: 15000);
            await filterButton.ClickAsync();

            // Wait for the Number: input to become visible and enabled (label-for pattern)
            var numberInput = Page.Locator(FilterNumberInputXPath).First;
            await WaitForEnabledAsync(numberInput, timeoutMs: 15000);
            await numberInput.ClearAsync();
            await TypeAsync(numberInput, number);
        }

        /// <summary>
        /// Clicks the OK button in any LiveTrack dialog.
        /// </summary>
        public async Task ClickOkButtonAsync()
        {
            var okBtn = Page.Locator(OkButtonXPath).First;
            await WaitForEnabledAsync(okBtn, timeoutMs: 10000);
            await okBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the Refresh button (fa-refresh icon) to reload the current grid view.
        /// </summary>
        public async Task RefreshViewAsync()
        {
            var refreshBtn = Page.Locator(RefreshButtonXPath).First;
            await WaitForEnabledAsync(refreshBtn, timeoutMs: 10000);
            await refreshBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Right-clicks the invoice row matching <paramref name="name"/>,
        /// finds the "Approve Invoice" context-menu item (retries up to 15 times),
        /// fills in the comment, clicks Submit, then waits 2 s.
        /// </summary>
        public async Task ApproveInvoiceAsync(string name, string comment)
        {
            // Right-click the invoice row
            var rowLocator = Page.Locator($"//div[.='{name}']").First;
            await WaitForEnabledAsync(rowLocator, timeoutMs: 15000);
            await rowLocator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });

            // Wait for the "Approve Invoice" context menu item to appear (loop up to 15 times)
            ILocator? approveItem = null;
            for (int i = 1; i <= 15; i++)
            {
                var candidates = Page.Locator(ApproveInvoiceXPath);
                var count = await candidates.CountAsync();
                for (int j = 0; j < count; j++)
                {
                    var candidate = candidates.Nth(j);
                    if (await candidate.IsVisibleAsync())
                    {
                        approveItem = candidate;
                        break;
                    }
                }
                if (approveItem != null) break;
                await Task.Delay(500);
            }

            if (approveItem == null)
                throw new InvalidOperationException($"'Approve Invoice' context menu item was not visible after 15 attempts for invoice '{name}'.");

            await approveItem.ClickAsync();

            // Fill comments textarea
            var commentsInput = Page.Locator(CommentsTextareaXPath).First;
            await WaitForEnabledAsync(commentsInput, timeoutMs: 10000);
            await commentsInput.ClearAsync();
            await TypeAsync(commentsInput, comment);

            // Click Submit
            var submitBtn = Page.Locator(SubmitButtonXPath).First;
            await WaitForEnabledAsync(submitBtn, timeoutMs: 10000);
            await submitBtn.ClickAsync();

            // Wait 2 seconds for the approval to be processed
            await Task.Delay(2000);
        }
    }
}
