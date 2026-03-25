using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class PortalRegisterPage : BasePage
    {
        public PortalRegisterPage(IPage page) : base(page) { }

        /// <summary>
        /// Clicks the Register nav link: &lt;a href="/auth/register"&gt;Register&lt;/a&gt;
        /// Waits for it to be enabled before clicking.
        /// </summary>
        public async Task ClickRegisterLinkAsync()
        {
            var link = Page.Locator("a[href='/auth/register']").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(link, timeoutMs: 10000);
            await ClickAndWaitForNavigationAsync(link);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the &lt;h2&gt;Create your account&lt;/h2&gt; heading is visible.
        /// </summary>
        public async Task VerifyCreateAccountPageAsync()
        {
            var heading = Page.Locator("h2").Filter(new LocatorFilterOptions { HasText = "Create your account" }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Create your account' heading. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the Full name input: &lt;input id="name" placeholder="Full name"&gt;
        /// </summary>
        public async Task EnterFullNameAsync(string name)
        {
            var input = Page.Locator("#name").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.FillAsync(name);
        }

        /// <summary>
        /// Fills the Email address input: &lt;input id="email" placeholder="Email address"&gt;
        /// </summary>
        public async Task EnterEmailAsync(string email)
        {
            var input = Page.Locator("#email").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.FillAsync(email);
        }

        /// <summary>
        /// Fills the Password input: &lt;input id="password" type="password"&gt;
        /// </summary>
        public async Task EnterPasswordAsync(string password)
        {
            var input = Page.Locator("#password").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.FillAsync(password);
        }

        /// <summary>
        /// Clicks the Continue button (step 1 of registration):
        /// &lt;button type="button"&gt;&lt;span&gt;Continue&lt;/span&gt;&lt;/button&gt;
        /// </summary>
        public async Task ClickContinueButtonAsync()
        {
            var btn = Page.Locator("button[type='button']")
                .Filter(new LocatorFilterOptions { HasText = "Continue" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Fills the Company name input (step 2). Waits for enabled first:
        /// &lt;input id="company_name" placeholder="Company name"&gt;
        /// </summary>
        public async Task EnterCompanyNameAsync(string company)
        {
            var input = Page.Locator("#company_name").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(company);
        }

        /// <summary>
        /// Checks the Terms checkbox: &lt;input type="checkbox" id="accept_terms"&gt;
        /// </summary>
        public async Task AcceptTermsAsync()
        {
            var checkbox = Page.Locator("#accept_terms").First;
            await checkbox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await checkbox.CheckAsync();
        }

        /// <summary>
        /// Clicks the Create your account submit button (step 2):
        /// &lt;button type="submit"&gt;&lt;span&gt;Create your account&lt;/span&gt;&lt;/button&gt;
        /// </summary>
        public async Task ClickCreateAccountButtonAsync()
        {
            var btn = Page.Locator("button[type='submit']")
                .Filter(new LocatorFilterOptions { HasText = "Create your account" }).First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await btn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Verifies the &lt;h2&gt;Account created&lt;/h2&gt; confirmation heading is visible.
        /// </summary>
        public async Task VerifyAccountCreatedAsync()
        {
            var heading = Page.Locator("h2").Filter(new LocatorFilterOptions { HasText = "Account created" }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Account created' heading. URL: {Page.Url}");
        }

        /// <summary>
        /// Verifies the "Your email address has been confirmed." message after clicking the confirmation link.
        /// &lt;div&gt;Your email address has been confirmed.&lt;/div&gt;
        /// </summary>
        public async Task VerifyEmailConfirmedAsync()
        {
            var msg = Page.GetByText("Your email address has been confirmed.", new PageGetByTextOptions { Exact = false }).First;
            await msg.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await msg.IsVisibleAsync(),
                $"Expected email confirmation message. URL: {Page.Url}");
        }

        // ── Complete your account page (INT portal) ───────────────────────────────

        /// <summary>
        /// Verifies the "Complete your account" h2 heading is visible and enabled.
        /// HTML: h2 containing "Complete your account"
        /// </summary>
        public async Task VerifyCompleteYourAccountPageAsync()
        {
            var heading = Page.Locator("h2").Filter(new LocatorFilterOptions { HasText = "Complete your account" }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Complete your account' heading. URL: {Page.Url}");
        }

        /// <summary>Fills input#username on the Complete your account form.</summary>
        public async Task EnterUsernameToCompleteAccountAsync(string username)
        {
            var input = Page.Locator("input#username[formcontrolname='username']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(username);
        }

        /// <summary>Fills input#password on the Complete your account form.</summary>
        public async Task EnterPasswordToCompleteAccountAsync(string password)
        {
            var input = Page.Locator("input#password[formcontrolname='password']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(password);
        }

        /// <summary>Fills input#password_confirmation on the Complete your account form.</summary>
        public async Task ConfirmPasswordToCompleteAccountAsync(string password)
        {
            var input = Page.Locator("input#password_confirmation[formcontrolname='password_confirmation']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(password);
        }

        /// <summary>Fills input#first_name on the Complete your account form.</summary>
        public async Task EnterFirstNameToCompleteAccountAsync(string firstName)
        {
            var input = Page.Locator("input#first_name[formcontrolname='first_name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(firstName);
        }

        /// <summary>Fills input#email on the Complete your account form.</summary>
        public async Task EnterEmailToCompleteAccountAsync(string email)
        {
            var input = Page.Locator("input#email[formcontrolname='email']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(email);
        }

        /// <summary>Fills input#last_name on the Complete your account form.</summary>
        public async Task EnterLastNameToCompleteAccountAsync(string lastName)
        {
            var input = Page.Locator("input#last_name[formcontrolname='last_name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 10000);
            await input.FillAsync(lastName);
        }

        /// <summary>
        /// Verifies the welcome h4 heading is visible with the given text.
        /// HTML: h4.pt-4.pb-4 containing the welcome text
        /// </summary>
        public async Task VerifyWelcomeTextAsync(string expectedText)
        {
            var heading = Page.Locator("h4").Filter(new LocatorFilterOptions { HasText = expectedText }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected welcome text '{expectedText}'. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the Email address input on the INT portal registration form.
        /// HTML: input#email[type="email"] formcontrolname="email" placeholder="Email address"
        /// </summary>
        public async Task EnterEmailAddressAsync(string email)
        {
            var input = Page.Locator("input#email[type='email']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.FillAsync(email);
        }

        /// <summary>
        /// Fills the Confirm your email address input on the INT portal registration form.
        /// HTML: input#email_confirmation[type="email"] formcontrolname="email_confirmation" placeholder="Confirm your email address"
        /// </summary>
        public async Task ConfirmEmailAddressAsync(string email)
        {
            var input = Page.Locator("input#email_confirmation[type='email']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.FillAsync(email);
        }

        /// <summary>
        /// Verifies that the Recent Notifications widget on the Hub dashboard contains a notification
        /// with the registered user's full name (e.g. "{fullName} from QA Team signed up for an account").
        /// </summary>
        public async Task VerifyNotificationContainsFullNameAsync(string fullName)
        {
            var widget = Page.Locator("qwyk-notifications-list-widget").First;
            await widget.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            var notification = widget.Locator("div").Filter(new LocatorFilterOptions { HasText = fullName }).First;
            await notification.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await notification.IsVisibleAsync(),
                $"Expected notification with '{fullName}' in Recent Notifications. URL: {Page.Url}");
        }
    }
}
