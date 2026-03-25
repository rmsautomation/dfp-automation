using Microsoft.Playwright;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class HubAdminUsersPortalPage : BasePage
    {
        public HubAdminUsersPortalPage(IPage page) : base(page)
        {
        }

        // ── TC1483: Hub Customer - Create Portal User ──────────────────────────────

        /// <summary>
        /// Fills the portal user email field on the create-user form.
        /// Verified from HTML: input[formcontrolname="email"] placeholder="Email address"
        /// </summary>
        public async Task EnterEmailForPortalUserAsync(string email)
        {
            var input = Page.Locator("input[formcontrolname='email']");
            await input.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.First.FillAsync(email);
        }

        /// <summary>
        /// Reads the auto-generated password from the 3rd strong element with inline position:relative style.
        /// XPath: (//strong[contains(@style, 'position: relative')])[3]
        /// </summary>
        public async Task<string> ReadPortalUserPasswordAsync()
        {
            // HTML: <input formcontrolname="password" [disabled]> — password is a disabled textbox
            var el = Page.Locator("input[formcontrolname='password']");
            await el.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            return (await el.InputValueAsync()).Trim();
        }

        /// <summary>
        /// Selects a site in the ng-select "Select a site" dropdown.
        /// Verified from HTML: div.ng-placeholder "Select a site" → span.ng-option-label = site
        /// </summary>
        public async Task SelectSiteAsync(string site)
        {
            var dropdown = Page.Locator("ng-select").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-placeholder").Filter(new LocatorFilterOptions { HasText = "Select a site" })
            });
            await dropdown.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dropdown.First.ClickAsync();
            await Page.WaitForTimeoutAsync(300);

            var option = Page.Locator(".ng-option").Filter(new LocatorFilterOptions
            {
                Has = Page.Locator(".ng-option-label").Filter(new LocatorFilterOptions { HasText = site })
            });
            await option.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
            await option.First.ClickAsync();
            await Page.WaitForTimeoutAsync(200);
        }

        /// <summary>
        /// Fills the Full name input on the create-user form.
        /// Verified from HTML: input[formcontrolname="name"] placeholder="Full name"
        /// </summary>
        public async Task EnterUserNameAsync(string resolvedName)
        {
            var input = Page.Locator("input[formcontrolname='name']");
            await input.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.First.FillAsync(resolvedName);
        }

        /// <summary>
        /// Fills the Company name input on the create-user form.
        /// Verified from HTML: input[formcontrolname="company_name"] placeholder="Company name"
        /// </summary>
        public async Task EnterCompanyNameAsync(string resolvedName)
        {
            var input = Page.Locator("input[formcontrolname='company_name']");
            await input.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.First.FillAsync(resolvedName);
        }

        /// <summary>
        /// Clicks the privacy compliance toggle label to confirm the privacy policy.
        /// Verified from HTML: label[for="privacy_compliance"].custom-control-label
        /// </summary>
        public async Task ConfirmPrivacyAsync()
        {
            var label = Page.Locator("label[for='privacy_compliance']");
            await label.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await label.First.ClickAsync();
        }

        /// <summary>
        /// Clicks the primary "Create User" submit button on the create-user form.
        /// Verified from HTML: button.btn-primary containing "Create User"
        /// </summary>
        public async Task ClickCreateUserSubmitButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary").Filter(new LocatorFilterOptions { HasText = "Create User" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── TC1275: Approve portal user access ────────────────────────────────────

        /// <summary>
        /// Clicks the first pending user row (li.list-group-item-action) in the User Approvals widget
        /// whose text contains the given search text (username or email).
        /// Waits for visible and enabled before clicking.
        /// HTML: li.list-group-item-action > h6 (username) / div.text-truncate (email)
        /// </summary>
        public async Task SelectPendingUserByEmailAsync(string searchText)
        {
            var item = Page.Locator("li.list-group-item-action")
                .Filter(new LocatorFilterOptions { HasText = searchText })
                .First;
            await item.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            await WaitForEnabledAsync(item, timeoutMs: 10000);
            await item.ClickAsync();
        }

        /// <summary>
        /// Enters a username in the Hub portal user form.
        /// HTML: input[@name='username']
        /// When empty, caller should pass the Now-based username.
        /// </summary>
        public async Task EnterUsernameAsync(string username)
        {
            var input = Page.Locator("input[name='username']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(username);
        }

        /// <summary>
        /// Clicks the search icon button to open the entity search panel.
        /// HTML: button.btn-outline-primary with text "Search" (magnifying glass icon)
        /// Waits for enabled before clicking.
        /// </summary>
        public async Task ClickSearchIconForEntityAsync()
        {
            var btn = Page.Locator("button.btn-outline-primary")
                .Filter(new LocatorFilterOptions { HasText = "Search" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 10000);
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Fills the entity name search input.
        /// HTML: input#name placeholder="Search by name"
        /// Waits for enabled before filling.
        /// </summary>
        public async Task EnterEntityNameAsync(string entityName)
        {
            var input = Page.Locator("input#name[placeholder='Search by name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(entityName);
        }

        /// <summary>
        /// Clicks the Search button in the entity search page.
        /// HTML: button.btn-primary.ml-2 containing "Search"
        /// </summary>
        public async Task ClickSearchButtonInEntityPageAsync()
        {
            var btn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Search" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Selects an entity from the results table by clicking the td with the matching text.
        /// HTML: td containing entityName
        /// </summary>
        public async Task SelectEntityByNameAsync(string entityName)
        {
            var row = Page.Locator("td")
                .Filter(new LocatorFilterOptions { HasText = entityName }).First;
            await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await row.ClickAsync();
        }

        /// <summary>
        /// Clicks the Continue button in the entity selection page.
        /// HTML: button.btn-primary containing "Continue"
        /// </summary>
        public async Task ClickContinueButtonInEntityPageAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Continue" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the Approve access button in the portal user approval page.
        /// HTML: button.btn-outline-success containing "Approve access"
        /// </summary>
        public async Task ClickApproveAccessButtonAsync()
        {
            var btn = Page.Locator("button.btn-outline-success")
                .Filter(new LocatorFilterOptions { HasText = "Approve access" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 10000);
            await btn.ClickAsync();
            await Page.WaitForTimeoutAsync(2000);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

    }
}
