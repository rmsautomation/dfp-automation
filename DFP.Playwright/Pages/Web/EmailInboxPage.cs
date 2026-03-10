using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFP.Playwright.Pages.Web
{
    public sealed class EmailInboxPage : BasePage
    {
        public EmailInboxPage(IPage page) : base(page)
        {
        }

        public async Task OpenYopmailInboxAsync(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new InvalidOperationException("Email address is required to check inbox.");

            var inbox = NormalizeInbox(emailAddress);
            var inboxUrl = $"https://yopmail.com/en/?login={Uri.EscapeDataString(inbox)}";
            await Page.GotoAsync(inboxUrl);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var visibleInboxInput = Page.Locator("input[name='login']:visible, input[placeholder*='inbox' i]:visible").First;
            try
            {
                await visibleInboxInput.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 4000
                });

                await visibleInboxInput.FillAsync(inbox);
                await visibleInboxInput.PressAsync("Enter");
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            catch (TimeoutException)
            {
                // The inbox can already be opened via the login query parameter.
            }
        }

        public async Task<IReadOnlyList<string>> GetLatestYopmailEmailBodiesFromTodayAsync(int maxMessages = 3)
        {
            List<int> rowIndexes = new();
            IFrame? inboxFrame = null;
            ILocator? rows = null;

            for (var attempt = 0; attempt < 6 && rowIndexes.Count == 0; attempt++)
            {
                inboxFrame = Page.Frame("ifinbox") ?? Page.Frames.FirstOrDefault(f => string.Equals(f.Name, "ifinbox", StringComparison.OrdinalIgnoreCase));
                Assert.IsNotNull(inboxFrame, $"Inbox frame 'ifinbox' was not found. URL: {Page.Url}");

                rows = inboxFrame!.Locator("div.m, tr[id^='m_']");
                await rows.First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 20000
                });

                var count = await rows.CountAsync();
                var scan = Math.Min(count, 25);
                rowIndexes = new List<int>();

                for (var i = 0; i < scan && rowIndexes.Count < maxMessages; i++)
                {
                    var row = rows.Nth(i);
                    var text = (await row.InnerTextAsync()).Trim();
                    if (LooksLikeToday(text))
                        rowIndexes.Add(i);
                }

                if (rowIndexes.Count == 0)
                {
                    var inboxText = ((await inboxFrame!.Locator("body").InnerTextAsync()) ?? string.Empty).Trim();
                    if (inboxText.Contains("today", StringComparison.OrdinalIgnoreCase))
                    {
                        for (var i = 0; i < scan && rowIndexes.Count < maxMessages; i++)
                            rowIndexes.Add(i);
                    }
                }

                if (rowIndexes.Count == 0 && attempt < 5)
                {
                    await Task.Delay(5000);
                    await Page.ReloadAsync();
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }

            if (rowIndexes.Count == 0 || rows == null)
                return Array.Empty<string>();

            var bodies = new List<string>();
            var previousMailText = string.Empty;
            foreach (var index in rowIndexes)
            {
                var row = rows.Nth(index);
                var rowText = (await row.InnerTextAsync()).Trim();
                await row.ClickAsync();

                var bodyFrame = Page.Frame("ifmail") ?? Page.Frames.FirstOrDefault(f => string.Equals(f.Name, "ifmail", StringComparison.OrdinalIgnoreCase));
                if (bodyFrame == null)
                    continue;

                var body = bodyFrame.Locator("body");
                await body.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 15000
                });

                var text = await WaitForYopmailBodyTextAsync(body, previousMailText);
                previousMailText = text;
                var combined = $"{rowText}\n{text}".Trim();
                if (!string.IsNullOrWhiteSpace(combined))
                    bodies.Add(combined);
            }

            return bodies;
        }

        public static async Task<IReadOnlyList<string>> GetLatestImapEmailBodiesFromTodayAsync(
            string emailAddress,
            string username,
            string password,
            int maxMessages = 3)
        {
            var domain = (emailAddress.Split('@').LastOrDefault() ?? string.Empty).ToLowerInvariant();
            var host = ResolveImapHost(domain);
            var port = 993;

            using var client = new ImapClient();
            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(username, password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var fromDate = DateTime.Today;
            var uids = await inbox.SearchAsync(SearchQuery.DeliveredAfter(fromDate.AddDays(-1)));
            var latest = uids.Reverse().Take(50).ToList();

            var result = new List<string>();
            foreach (var uid in latest)
            {
                if (result.Count >= maxMessages)
                    break;

                var message = await inbox.GetMessageAsync(uid);
                var localDate = message.Date.LocalDateTime;
                if (localDate.Date != DateTime.Today)
                    continue;

                var combined = $"{message.Subject}\n{message.TextBody}\n{message.HtmlBody}";
                if (!string.IsNullOrWhiteSpace(combined))
                    result.Add(combined);
            }

            await client.DisconnectAsync(true);
            return result;
        }

        private static string ResolveImapHost(string domain)
        {
            if (domain.Contains("gmail"))
                return "imap.gmail.com";

            // Generic fallback for other providers that follow imap.<domain>.
            return $"imap.{domain}";
        }

        private static string NormalizeInbox(string emailAddress)
        {
            var trimmed = emailAddress.Trim();
            var at = trimmed.IndexOf('@');
            return at > 0 ? trimmed[..at] : trimmed;
        }

        private static bool LooksLikeToday(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var now = DateTime.Now;
            var todayMarkers = new[]
            {
                "today",
                now.ToString("MM/dd"),
                now.ToString("M/d"),
                now.ToString("dd/MM"),
                now.ToString("d/M"),
                now.ToString("yyyy-MM-dd"),
                now.ToString("dd-MM-yyyy")
            };

            return todayMarkers.Any(m => text.Contains(m, StringComparison.OrdinalIgnoreCase));
        }

        private static async Task<string> WaitForYopmailBodyTextAsync(ILocator body, string previousMailText)
        {
            for (var attempt = 0; attempt < 20; attempt++)
            {
                var text = (await body.InnerTextAsync()).Trim();
                if (!string.IsNullOrWhiteSpace(text) && !string.Equals(text, previousMailText, StringComparison.Ordinal))
                    return text;

                await Task.Delay(250);
            }

            return (await body.InnerTextAsync()).Trim();
        }
    }
}
