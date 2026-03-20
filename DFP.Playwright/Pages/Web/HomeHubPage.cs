using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class HomeHubPage : BasePage
    {
        public HomeHubPage(IPage page) : base(page)
        {
        }

        private static string GetHubBaseUrl()
        {
            var url = Environment.GetEnvironmentVariable("HUB_BASE_URL")
                      ?? Environment.GetEnvironmentVariable("BASE_URL")
                      ?? "";
            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            return url;
        }

        // ── TC280: Hub Home ────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to the Hub home page (root URL).
        /// </summary>
        public async Task NavigateToHomeInHubAsync()
        {
            var baseUrl = GetHubBaseUrl().TrimEnd('/');
            await Page.GotoAsync(baseUrl + "/");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Click map: feature param → actual h6 link text on the home page
        private static readonly Dictionary<string, string> ClickMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "View Quotations",  "View Quotations"    },
                { "View Shipments",   "View Shipments"     },
                { "portal users",     "Manage your Portal" },  // "Manage your Portal" → /administration → /administration/portal-users
                { "profile",          "View your Profile"  },
            };

        // Page map: feature param → (url fragment, heading text)
        // Keys must match exactly what the feature file passes to "I should see the {string} page in the Hub"
        private static readonly Dictionary<string, (string UrlPart, string HeadingText)> PageMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "quotations",    ("quotations/list",             "Quotations")   },
                { "shipments",     ("shipments/list",              "Shipments")    },
                { "Portal Users",  ("administration/portal-users", "Portal Users") },
                { "About",         ("user/profile",                "About")        },
            };

        /// <summary>
        /// Clicks a quick-action link on the Hub home page by its visible text (or alias).
        /// These links live inside h6 headings. e.g. buttonText = "View Quotations", "portal users"
        /// ClickMap resolves aliases like "portal users" → "Manage your Portal".
        /// </summary>
        public async Task ClickQuickActionButtonInHubAsync(string buttonText)
        {
            var actualText = ClickMap.TryGetValue(buttonText, out var mapped) ? mapped : buttonText;
            var link = Page.Locator("h6").Filter(new LocatorFilterOptions { HasText = actualText }).Locator("a").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await link.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the expected Hub page is displayed by checking URL fragment and heading text.
        /// Uses PageMap to resolve pageName → (urlPart, headingText).
        /// Supported: "quotations", "shipments", "Portal Users", "About"
        /// </summary>
        public async Task ShouldSeePageInHubAsync(string pageName)
        {
            if (!PageMap.TryGetValue(pageName, out var page))
                throw new ArgumentException($"Unknown page '{pageName}'. Add it to PageMap in HomeHubPage.");

            await Page.WaitForURLAsync($"**/{page.UrlPart}**", new PageWaitForURLOptions { Timeout = 15000 });
            Assert.IsTrue(Page.Url.Contains(page.UrlPart),
                $"Expected URL to contain '{page.UrlPart}'. Actual URL: {Page.Url}");

            var heading = Page.Locator("h1, h2, h3, h4, h5, h6").Filter(new LocatorFilterOptions { HasText = page.HeadingText });
            await heading.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await heading.First.IsVisibleAsync(),
                $"Expected heading '{page.HeadingText}' to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies a section header is visible on the Hub home page.
        /// Supports both h5-based (User Approvals, Your Sites) and link-based (Quotation Requests, Recent Notifications) headers.
        /// </summary>
        public async Task ShouldSeeSectionHeaderInHubAsync(string header)
        {
            // Covers h3 (Portal Customers, Quotations…), h5 (User Approvals…), a[href] (Quotation Requests…)
            var el = Page.Locator("h1, h2, h3, h4, h5, h6, a[href]").Filter(new LocatorFilterOptions { HasText = header }).First;
            await el.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await el.IsVisibleAsync(),
                $"Expected section header '{header}' to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the list under the given section header is not empty.
        /// Polls with WaitForFunctionAsync until at least one li appears (up to 10s), avoiding
        /// race conditions when list items load after the heading becomes visible.
        /// Works for all four home page sections regardless of nesting depth.
        /// </summary>
        public async Task SectionListShouldNotBeEmptyInHubAsync(string section)
        {
            // Poll until the section heading exists AND its parent has at least one li
            // Heading lives in div.card-header (parentElement).
            // The list lives in the grandparent widget element (parentElement.parentElement).
            await Page.WaitForFunctionAsync(
                @"section => {
                    const allEls = Array.from(document.querySelectorAll('h5, a[href]'));
                    const heading = allEls.find(el => el.textContent.trim() === section);
                    if (!heading) return false;
                    const widget = heading.parentElement?.parentElement;
                    return widget ? widget.querySelectorAll('ul > li').length > 0 : false;
                }",
                section,
                new PageWaitForFunctionOptions { Timeout = 10000 });

            // Confirm count for meaningful assertion message
            var count = await Page.EvaluateAsync<int>(
                @"section => {
                    const allEls = Array.from(document.querySelectorAll('h5, a[href]'));
                    const heading = allEls.find(el => el.textContent.trim() === section);
                    if (!heading) return 0;
                    const widget = heading.parentElement?.parentElement;
                    return widget ? widget.querySelectorAll('ul > li').length : 0;
                }",
                section);
            Assert.IsTrue(count > 0,
                $"Expected section '{section}' list to not be empty. Found {count} items.");
        }

        /// <summary>
        /// Clicks the first "View" button in the Recent Notifications section.
        /// Verified from HTML: buttons with text "View Shipment" or "View Quotation" inside the notifications list.
        /// </summary>
        public async Task ClickFirstViewButtonInNotificationsInHubAsync()
        {
            // Scope to the Recent Notifications container
            var notificationsSection = Page.Locator("div").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator("a").Filter(new LocatorFilterOptions { HasText = "Recent Notifications" })
            }).First;
            var viewBtn = notificationsSection.Locator("button").Filter(new LocatorFilterOptions { HasText = "View" }).First;
            await viewBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await viewBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the current page has the Qwyk breadcrumb link (a[href="/"]) indicating a valid detail page.
        /// Verified from HTML: &lt;a class="text-muted ng-star-inserted" href="/"&gt; Qwyk &lt;/a&gt;
        /// </summary>
        public async Task ShouldSeeQwykBreadcrumbInHubAsync()
        {
            var qwykLink = Page.Locator("a[href='/']").Filter(new LocatorFilterOptions { HasText = "Qwyk" });
            await qwykLink.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await qwykLink.First.IsVisibleAsync(),
                $"Expected Qwyk breadcrumb link to be visible. URL: {Page.Url}");
        }
    }
}
