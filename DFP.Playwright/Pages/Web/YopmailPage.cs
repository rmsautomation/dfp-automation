using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class YopmailPage : BasePage
    {
        public YopmailPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Navigates to the Yopmail home page.
        /// </summary>
        public async Task NavigateAsync()
        {
            // Use DOMContentLoaded — yopmail has ads/trackers and Hub leaves open
            // WebSocket connections that prevent NetworkIdle from being reached.
            await Page.GotoAsync("https://yopmail.com/",
                new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
            try
            {
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle,
                    new PageWaitForLoadStateOptions { Timeout = 5000 });
            }
            catch (TimeoutException)
            {
                // DOMContentLoaded is sufficient to interact with yopmail; ignore lingering connections
            }
        }

        /// <summary>
        /// Opens the Yopmail inbox for the given email address.
        /// Delegates to EmailInboxPage.OpenYopmailInboxAsync which handles the login input.
        /// </summary>
        public async Task OpenInboxAsync(string email)
        {
            var inboxPage = new EmailInboxPage(Page);
            await inboxPage.OpenYopmailInboxAsync(email);
        }

        /// <summary>
        /// Polls the yopmail inbox every 5 s for up to 5 minutes (60 attempts) until at least
        /// one email arrives from today. Returns the bodies found, or an empty list on timeout.
        /// If the email address is NOT a yopmail address the call is skipped (returns empty).
        /// </summary>
        public async Task<IReadOnlyList<string>> WaitForEmailAsync(string email, int maxAttempts = 60, int delayMs = 5000)
        {
            var domain = email.Split('@').LastOrDefault()?.Trim().ToLowerInvariant() ?? "";
            if (!domain.Contains("yopmail"))
            {
                Console.WriteLine($"[YopmailPage.WaitForEmail] '{email}' is not a yopmail address — skipping.");
                return System.Array.Empty<string>();
            }

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                // ── DEBUG BREAKPOINT ─────────────────────────────────────────────
                // Uncomment the next line to pause the browser and open Playwright Inspector:
                // await Page.PauseAsync();
                // ─────────────────────────────────────────────────────────────────

                try
                {
                    await OpenInboxAsync(email);
                    var inboxPage = new EmailInboxPage(Page);
                    IReadOnlyList<string> bodies = await inboxPage.GetLatestYopmailEmailBodiesFromTodayAsync(maxMessages: 3);
                    Console.WriteLine($"[YopmailPage.WaitForEmail] attempt {attempt}/{maxAttempts} — inbox '{email}' — emails found: {bodies.Count}");

                    if (bodies.Count > 0)
                    {
                        Console.WriteLine($"[YopmailPage.WaitForEmail] Email arrived after {attempt * delayMs / 1000}s.");
                        return bodies;
                    }
                }
                catch (Exception ex) when (ex is TimeoutException || ex.GetType().Name.Contains("Playwright"))
                {
                    // Inbox is empty or frame not ready yet — log and keep polling
                    Console.WriteLine($"[YopmailPage.WaitForEmail] attempt {attempt}/{maxAttempts} — inbox empty or not ready ({ex.GetType().Name}). Retrying in {delayMs / 1000}s...");
                }

                if (attempt < maxAttempts)
                    await Task.Delay(delayMs);
            }

            Console.WriteLine($"[YopmailPage.WaitForEmail] No emails found after {maxAttempts * delayMs / 1000}s for '{email}'.");
            return System.Array.Empty<string>();
        }

        /// <summary>
        /// Polls (up to 60s) until an email containing all expectedTexts is found in the inbox.
        /// Uses EmailInboxPage to navigate and read email bodies from today.
        /// </summary>
        public async Task VerifyEmailBodyContainsAsync(string[] expectedTexts, int maxAttempts = 12)
        {
            string? matchedEmail = null;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var inboxPage = new EmailInboxPage(Page);
                IReadOnlyList<string> bodies = await inboxPage.GetLatestYopmailEmailBodiesFromTodayAsync(maxMessages: 3);

                matchedEmail = bodies.FirstOrDefault(body =>
                    expectedTexts.All(text => body.Contains(text, System.StringComparison.OrdinalIgnoreCase)));

                if (!string.IsNullOrWhiteSpace(matchedEmail))
                    return;

                if (attempt < maxAttempts - 1)
                {
                    await Task.Delay(5000);
                    await Page.ReloadAsync();
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }

            Assert.IsNotNull(matchedEmail,
                $"No email from today contained '{string.Join(" | ", expectedTexts)}' after waiting 60 seconds.");
        }

        /// <summary>
        /// Reads the auto-generated portal user password from the yopmail email body.
        /// The email structure is: Portal: [url]  Username: [email]  Password: [PASSWORD]
        /// XPath: (//strong[contains(@style, 'position: relative')])[3] inside #ifmail frame
        /// </summary>
        public async Task<string> ReadPasswordFromEmailAsync()
        {
            var mailFrame = Page.FrameLocator("#ifmail");
            var el = mailFrame.Locator("xpath=(//strong[contains(@style, 'position: relative')])[3]");
            await el.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            var password = (await el.InnerTextAsync()).Trim();
            Console.WriteLine($"[YopmailPage.ReadPasswordFromEmail] password: {password}");
            return password;
        }

        /// <summary>
        /// Clicks a link with the given text inside the yopmail email content frame (#ifmail).
        /// Call after VerifyEmailBodyContainsAsync — the frame is already open.
        /// </summary>
        public async Task ClickLinkInEmailAsync(string linkText)
        {
            var mailFrame = Page.FrameLocator("#ifmail");
            var link = mailFrame.Locator($"xpath=//a[contains(., '{linkText}')]");

            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = 15000 });
            await link.ScrollIntoViewIfNeededAsync();

            // Start listening for a possible new tab BEFORE clicking
            IPage? newPage = null;
            try
            {
                var newPageTask = Page.Context.WaitForPageAsync(new BrowserContextWaitForPageOptions { Timeout = 5000 });
                await link.ClickAsync();
                newPage = await newPageTask;
            }
            catch (TimeoutException)
            {
                // Link navigated in the current tab — nothing extra to do
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return;
            }

            // New tab opened — grab its URL, close it, navigate the shared IPage to it
            // so all page-object references (HubAdminUsersPortalPage, etc.) stay in sync
            await newPage.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var portalUrl = newPage.Url;
            await newPage.CloseAsync();
            await Page.GotoAsync(portalUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
