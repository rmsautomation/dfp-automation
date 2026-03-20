using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class CustomersHubPage : BasePage
    {
        public CustomersHubPage(IPage page) : base(page)
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

        // ── TC1482: Hub Create Customer ────────────────────────────────────────────

        /// <summary>
        /// Navigates to the Portal Customers list page (/administration/portal-teams).
        /// Verified from HTML: a.nav-link[href="/administration/portal-teams"] text "Customers"
        /// </summary>
        public async Task NavigateToPortalCustomersInHubAsync()
        {
            var baseUrl = GetHubBaseUrl().TrimEnd('/');
            await Page.GotoAsync(baseUrl + "/administration/portal-teams");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the visible "Create customer" button.
        /// Used twice: once on the list page (btn-outline-primary) and once on the form (btn-primary/submit).
        /// Both buttons are labeled "Create customer" so the first visible one is clicked each time.
        /// </summary>
        public async Task ClickCreateCustomerButtonInHubAsync()
        {
            var btn = Page.Locator("button").Filter(new LocatorFilterOptions { HasText = "Create customer" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Fills the customer name input on the create-customer form.
        /// Verified from HTML: input[placeholder="Team name"] formcontrolname="name"
        /// The resolved name (after empty-param substitution) is passed from the step definition.
        /// </summary>
        public async Task EnterCustomerNameInHubAsync(string resolvedName)
        {
            var input = Page.Locator("input[placeholder='Team name']");
            await input.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.First.FillAsync(resolvedName);
        }

        /// <summary>
        /// Clicks the radio button for the given type (Company / Department / Other).
        /// Verified from HTML: input[id="typecompany"] + label[for="typecompany"]
        /// Clicks the label so the radio activates regardless of overlays.
        /// </summary>
        public async Task SelectTypeInHubAsync(string type)
        {
            // label text matches the type param: "Company", "Department", "Other"
            var label = Page.Locator("label.custom-control-label")
                .Filter(new LocatorFilterOptions { HasText = type }).First;
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await label.ClickAsync();
        }

        /// <summary>
        /// Selects a segment in the ng-select dropdown on the create-customer form.
        /// Verified from HTML: div.ng-placeholder "Segment" → span.ng-option-label "Customer"
        /// </summary>
        public async Task SelectSegmentInHubAsync(string segment)
        {
            // Open the ng-select that has placeholder "Segment"
            var dropdown = Page.Locator("ng-select").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-placeholder").Filter(new LocatorFilterOptions { HasText = "Segment" })
            });
            await dropdown.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dropdown.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            // Click the matching option
            var option = Page.Locator(".ng-option").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-option-label").Filter(new LocatorFilterOptions { HasText = segment })
            });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(200);
        }

        /// <summary>
        /// Fills the "Search by Name" input on the Portal Customers list page.
        /// Verified from HTML: input[placeholder="Search by Name"] formcontrolname="name"
        /// The resolved name (after empty-param substitution) is passed from the step definition.
        /// </summary>
        public async Task EnterCustomerNameInSearchAsync(string resolvedName)
        {
            var input = Page.Locator("input[placeholder='Search by Name']");
            await input.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.First.FillAsync(resolvedName);
        }

        /// <summary>
        /// Clicks the Search button on the Portal Customers list, waiting for it to be enabled first.
        /// The button starts disabled and becomes enabled after typing in a filter field.
        /// </summary>
        public async Task ClickSearchButtonAsync()
        {
            // Wait until the Search button becomes enabled (not disabled)
            await Page.WaitForFunctionAsync(
                @"() => {
                    const buttons = Array.from(document.querySelectorAll('button'));
                    const btn = buttons.find(b => b.textContent.trim() === 'Search');
                    return btn != null && !btn.disabled;
                }",
                null,
                new PageWaitForFunctionOptions { Timeout = 10000 });

            var btn = Page.Locator("button:not([disabled])").Filter(new LocatorFilterOptions { HasText = "Search" }).First;
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the customer name appears as a link inside a table cell in the search results.
        /// Verified from HTML: td > a href="/administration/portal-teams/..." text = customer name
        /// </summary>
        public async Task ShouldSeeCustomerNameInResultsAsync(string resolvedName)
        {
            var link = Page.Locator("td").GetByText(resolvedName, new LocatorGetByTextOptions { Exact = true });
            await link.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await link.First.IsVisibleAsync(),
                $"Expected customer '{resolvedName}' to be visible in results. URL: {Page.Url}");
        }
    }
}
