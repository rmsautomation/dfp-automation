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

        // ── Portal login page assertion ───────────────────────────────────────────

        /// <summary>
        /// Verifies the portal login page is visible by checking the logo image.
        /// HTML: img[alt="Logo"][src*="site_contrast_logo"]
        /// </summary>
        public async Task ShouldSeeLoginPageAsync()
        {
            var logo = Page.Locator("img[alt='Logo'][src*='site_contrast_logo']");
            await logo.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
        }

        // ── Portal login (individual steps for TC1483) ────────────────────────────

        private LoginPage CreateLoginPage() =>
            new LoginPage(Page,
                Environment.GetEnvironmentVariable("PORTAL_BASE_URL")
                ?? Environment.GetEnvironmentVariable("BASE_URL")
                ?? "");

        /// <summary>
        /// Waits for input#email to be visible and enabled, then fills it.
        /// HTML: input#email[name="email"] placeholder="Email address"
        /// </summary>
        public async Task FillPortalUsernameAsync(string username)
        {
            var input = Page.Locator("input#email[name='email']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await Microsoft.Playwright.Assertions.Expect(input).ToBeEnabledAsync(new() { Timeout = 10000 });
            await input.FillAsync(username);
        }

        /// <summary>
        /// Waits for input#password to be visible and enabled, then fills it.
        /// HTML: input#password[name="password"] type="password"
        /// </summary>
        public async Task FillPortalPasswordAsync(string password)
        {
            var input = Page.Locator("input#password[name='password']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await Microsoft.Playwright.Assertions.Expect(input).ToBeEnabledAsync(new() { Timeout = 10000 });
            await input.FillAsync(password);
        }

        /// <summary>
        /// Clicks the Sign in submit button and waits for the dashboard.
        /// HTML: button[type="submit"].btn-primary containing "Sign in"
        /// </summary>
        public async Task ClickPortalSignInAsync()
        {
            var btn = Page.Locator("button[type='submit'].btn-primary").Filter(
                new LocatorFilterOptions { HasText = "Sign in" });
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await btn.ClickAsync();
            await CreateLoginPage().WaitForDashboardAsync();
        }

        /// <summary>Full portal login in one call (used by ILoginToPortalAsTheCreatedUser).</summary>
        public async Task LoginToPortalAsCreatedUserAsync(string email, string password)
        {
            var login = CreateLoginPage();
            await login.NavigateAsync();
            await login.LogoutIfLoggedInAsync();
            await login.LoginToDFPAsync(email, password, searchLoginModal: true);
            await login.WaitForDashboardAsync();
        }
    }
}
