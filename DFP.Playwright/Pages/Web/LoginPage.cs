using Microsoft.Playwright;
using DFP.Playwright.Pages.Web.BasePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFP.Playwright.Pages.Web
{
    public sealed class LoginPage : BasePage
    {
        public LoginPage(IPage page, string baseUrl) : base(page)
        {
            _baseUrl = baseUrl;
        }

        // codegen:login-start
        private static readonly string[] UsernameSelectors =
        {
            "internal:role=textbox[name=\"Username\"i]",
            "internal:role=textbox[name=\"Email\"i]",
            "internal:role=textbox[name=\"Email address\"i]",
            "internal:role=textbox[name=\"Email address * Password *\"i]",
            "internal:role=textbox[name=\"Email address *\"i]",
            "input[type=\"text\"]",
            "input[type=\"email\"]",
            "input[name*=\"email\"i]",
            "input[placeholder*=\"email\"i]"
        };

        private static readonly string[] PasswordSelectors =
        {
            "internal:role=textbox[name=\"Password\"i]",
            "internal:text=\"Password *\"i",
            "internal:role=textbox[name=\"Password\"s]",
            "input[type=\"password\"]"
        };

        private static readonly string[] LoginHeadingSelectors =
        {
            "internal:role=heading[name=\"Sign in to your account\"i]"
        };

        private static readonly string[] SignInButtonSelectors =
        {
            "internal:role=button[name=\"Sign in\"i]",
            "internal:role=link[name=\"Sign in\"i]",
            "a:has-text(\"Sign in\")",
            "text=Sign in",
            "internal:role=button[name=\"Log in\"i]",
            "internal:role=button[name=\"Login\"i]",
            "internal:role=button[name=\"Continue\"i]",
            "qwyk-quotation-navigation >> internal:text=\"Sign in\"i"
        };
        // codegen:login-end

        private static readonly string[] LoginHubHeadingSelectors =
        {
            "internal:role=heading[name=\"Sign in\"i]",
            "internal:role=heading[name=\"Welcome back\"i]",
            "internal:text=\"Get started with the Digital\"i"
        };

        // Selectors captured by codegen for 'login'
        public static readonly string[] Selectors = new[]
        {
            "a >> internal:has-text=/^Dashboard$/",
            "internal:role=button[name=\"A\"s]",
            "internal:role=button[name=\"Log out\"i]",
            "internal:role=button[name=\"Sign in\"i]",
            "internal:role=heading[name=\"Sign in to your account\"i]",
            "internal:role=textbox[name=\"Password\"i]",
            "internal:role=textbox[name=\"Username\"i]",
            "internal:text=\"Here is a snapshot of what's\"i",
            "internal:text=\"Loading ops data...\"i",
        };


        private static readonly string[] LogoutAllSessionSelectors =
        {
            "#lbtLogOutAllSessions",
            "text=/log out all sessions/i"
        };

        private static readonly string[] DashboardSelectors =
        {
            // These sidebar links ONLY exist when logged into the portal.
            // "a:has-text('Dashboard')" and "role=link[name='Dashboard']" were removed
            // because the public homepage navbar also has an <a>Dashboard</a> link,
            // causing a false-positive that triggered LogoutIfLoggedInAsync on every login.
            "a[href='/my-portal/settings']",
            "a[href='/my-portal/conversations']",
            "a[href='/my-portal/orders']",
        };

        private static readonly string[] ProfileButtonSelectors =
        {
            // Portal: avatar button shows user initial — try by single-char text, class, or role
            "button[class*='avatar']",
            "[class*='user-avatar']",
            "[class*='avatar-btn']",
            "//nav//ul//li//button[string-length(normalize-space())=1]",
            "role=button[name='A']",
            "internal:role=button[name=\"Avatar\"s]",
            "internal:role=button[name=\"Avatar next\"i]"
        };

        private static readonly string[] LogoutButtonSelectors =
        {
            "role=button[name='Log out']",
            "internal:role=menuitem[name=\"Log out\"i] >> a"
        };

        private static readonly string[] LoginFormPrimarySelectors =
        {
            "internal:role=textbox[name=\"Username\"i]",
            "internal:role=textbox[name=\"Email\"i]",
            "internal:role=textbox[name=\"Email address\"i]",
            "internal:role=textbox[name=\"Email address * Password *\"i]",
            "internal:role=textbox[name=\"Email address *\"i]",
            "internal:role=textbox[name=\"Password\"i]",
            "internal:text=\"Password *\"i",
            "internal:role=textbox[name=\"Password\"s]",
            "input[type=\"password\"]"
        };

        private static readonly string[] LoginFormSelectors =
            UsernameSelectors
                .Concat(PasswordSelectors)
                .Concat(SignInButtonSelectors)
                .ToArray();
        // private const string MAINAPP_DFP_IMG = "/Images/.....";

        private readonly string _baseUrl;


        public async Task NavigateAsync()
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new InvalidOperationException("BaseUrl is null/empty. Provide it via config/env/DI.");

            Console.WriteLine($"navigating to {_baseUrl}");
            await Page.GotoAsync(_baseUrl, new PageGotoOptions
            {
                // Commit fires as soon as HTTP response headers arrive — no content wait needed.
                // For searchLoginModal:true, we navigate away to /my-portal immediately after,
                // so waiting for DOMContentLoaded here was wasteful. For Hub login (searchLoginModal:false),
                // WaitForLoginFormAsync (30s timeout) handles waiting for the form.
                WaitUntil = WaitUntilState.Commit,
                Timeout = 60000
            });
            // Duplicate: DOMContentLoaded already waited by GotoAsync above.
            // await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // NetworkIdle always hits the full timeout on this SPA (background connections stay alive).
            // try
            // {
            //     await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 5000 });
            // }
            // catch
            // {
            //     // Portal SPA may keep background connections alive; continue when DOM is ready.
            // }
            // await Page.WaitForResponseAsync(r => r.Url.Contains(MAINAPP_DFP_IMG));
        }

        public async Task LoginToDFPAsync(string email, string password, bool searchLoginModal = false)
        {
            if (searchLoginModal)
            {
                await EnsureLoginFormAsync(30000);

                // Wait for the modal dialog and scope ALL form interactions to it so the background
                // inline form (added to the portal UI recently) is never touched by mistake.
                // The modal is a div[role='dialog'].p-dynamic-dialog — NOT a native <dialog> element.
                // Exclude cookie-consent banners (aria-label="cookieconsent") — use the login dialog only.
                var dialog = Page.Locator("[role='dialog']:not([aria-label='cookieconsent'])").First;
                bool dialogVisible = false;
                try
                {
                    await dialog.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
                    dialogVisible = true;
                }
                catch { /* no dialog — fall through to legacy path */ }

                if (dialogVisible)
                {
                    // password is type="password", button text is "Sign in".
                    var emailInput    = dialog.Locator("input[placeholder='Email address']");
                    var passwordInput = dialog.Locator("input[type='password']");
                    var signInBtn     = dialog.Locator("button:has-text('Sign in')");

                    await emailInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                    await Microsoft.Playwright.Assertions.Expect(emailInput).ToBeEnabledAsync(new() { Timeout = 10000 });
                    await emailInput.FillAsync(email);
                    await passwordInput.FillAsync(password);
                    await signInBtn.ClickAsync();

                    var logoutAllModal = await TryFindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 500);
                    if (logoutAllModal != null && await IsVisibleAsync(logoutAllModal))
                    {
                        await CloseAllSessionIfNeeded();
                        await emailInput.FillAsync(email);
                        await passwordInput.FillAsync(password);
                        await passwordInput.PressAsync("Enter");
                    }
                    return;
                }
            }
            else
            {
                await WaitForLoginFormAsync(30000);
            }

            var usernameInputFallback = await FindUsernameInputAsync();
            var passwordInputFallback = await FindPasswordInputAsync();

            await usernameInputFallback.FillAsync(email);
            await passwordInputFallback.FillAsync(password);

            // Prefer clicking Sign in if available, otherwise submit with Enter.
            var signInButton = await TryFindLocatorAsync(SignInButtonSelectors, timeoutMs: 2000);
            if (signInButton != null)
                await signInButton.ClickAsync();
            else
                await passwordInputFallback.PressAsync("Enter");

            // Only wait 500ms for "logout all sessions" — it rarely appears and 3000ms was wasteful.
            var logoutAll = await TryFindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 500);
            if (logoutAll != null && await IsVisibleAsync(logoutAll))
            {
                await CloseAllSessionIfNeeded();
                await usernameInputFallback.FillAsync(email);
                await passwordInputFallback.FillAsync(password);
                await passwordInputFallback.PressAsync("Enter");
            }
        }

        private async Task<ILocator> FindUsernameInputAsync()
        {
            var locator = await FindInputBySelectorsAsync(UsernameSelectors, preferEmail: true);
            if (locator != null)
                return locator;

            throw new TimeoutException("Username/email input not found for login.");
        }

        private async Task<ILocator> FindPasswordInputAsync()
        {
            var locator = await FindInputBySelectorsAsync(PasswordSelectors, preferEmail: false, requirePasswordType: true, requirePasswordKeyword: true);
            if (locator != null)
                return locator;

            locator = await FindInputBySelectorsAsync(PasswordSelectors, preferEmail: false, requirePasswordType: false, requirePasswordKeyword: true);
            if (locator != null)
                return locator;

            throw new TimeoutException("Password input not found for login.");
        }

        private async Task<ILocator?> FindInputBySelectorsAsync(
            string[] selectors,
            bool preferEmail,
            bool requirePasswordType = false,
            bool requirePasswordKeyword = false)
        {
            foreach (var selector in selectors)
            {
                var direct = Page.Locator(selector);
                var found = await FindMatchingInputAsync(direct, preferEmail, requirePasswordType, requirePasswordKeyword);
                if (found != null)
                    return found;

                foreach (var frame in Page.Frames)
                {
                    var inFrame = frame.Locator(selector);
                    found = await FindMatchingInputAsync(inFrame, preferEmail, requirePasswordType, requirePasswordKeyword);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        private static async Task<ILocator?> FindMatchingInputAsync(
            ILocator candidates,
            bool preferEmail,
            bool requirePasswordType,
            bool requirePasswordKeyword)
        {
            var count = await candidates.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var candidate = candidates.Nth(i);
                var type = (await candidate.GetAttributeAsync("type"))?.Trim().ToLowerInvariant() ?? "";

                if (requirePasswordType && type != "password")
                    continue;
                if (!requirePasswordType && type == "password")
                    continue;

                var aria = (await candidate.GetAttributeAsync("aria-label")) ?? "";
                var name = (await candidate.GetAttributeAsync("name")) ?? "";
                var placeholder = (await candidate.GetAttributeAsync("placeholder")) ?? "";
                var combined = $"{aria} {name} {placeholder}".ToLowerInvariant();

                if (requirePasswordKeyword)
                {
                    if (combined.Contains("password"))
                        return candidate;
                    continue;
                }

                if (!preferEmail)
                    return candidate;

                if (combined.Contains("password"))
                    continue;
                if (combined.Contains("email") || combined.Contains("user") || combined.Contains("username"))
                    return candidate;
            }

            return count > 0 ? candidates.First : null;
        }

        private async Task EnsureLoginFormAsync(int timeoutMs)
        {
            // Navigating to /my-portal when not logged in auto-redirects to /?next=... and
            // opens the login modal automatically — no need to find and click "Sign in".
            var myPortalUrl = _baseUrl.TrimEnd('/') + "/my-portal";
            await Page.GotoAsync(myPortalUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            await WaitForLoginFormAsync(timeoutMs);
        }

        public async Task<bool> IsLogoutAllSessionButtonVisibleAsync()
        {
            var locator = await TryFindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 1000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task<bool> IsUsernameInputVisibleAsync()
        {
            var locator = await TryFindLocatorAsync(UsernameSelectors, timeoutMs: 8000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task WaitForLoginAsync(int timeoutMs = 10000)
        {
            var locator = await FindLocatorAsync(LoginHeadingSelectors, timeoutMs);
            if (locator == null)
            {
                locator = await FindLocatorAsync(LoginHubHeadingSelectors, timeoutMs);
            }
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
        }

        public async Task WaitForLoginFormAsync(int timeoutMs = 10000)
        {
            var locator = await FindLocatorAsync(LoginFormPrimarySelectors, timeoutMs);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
        }

        public async Task<bool> IsDashboardVisibleAsync()
        {
            // 1000ms: fail-fast for fresh browser contexts (no session). The loop in
            // WaitForDashboardAsync and EnsureLoggedInAsync handles retrying.
            var locator = await TryFindLocatorAsync(DashboardSelectors, timeoutMs: 1000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task<bool> IsLoggedInAsync()
        {
            // 500ms each: fail-fast for fresh browser contexts. Callers that need
            // to poll (WaitForDashboardAsync, EnsureLoggedInAsync) retry in their own loop.
            var profile = await TryFindLocatorAsync(ProfileButtonSelectors, timeoutMs: 500);
            if (profile != null && await profile.IsVisibleAsync())
                return true;

            var logout = await TryFindLocatorAsync(LogoutButtonSelectors, timeoutMs: 500);
            return logout != null && await logout.IsVisibleAsync();
        }

        public async Task WaitForDashboardAsync(int timeoutMs = 10000)
        {
            var start = DateTime.UtcNow;
            while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    var locator = await TryFindLocatorAsync(DashboardSelectors, timeoutMs: 1000);
                    if (locator != null && await locator.IsVisibleAsync())
                        return;

                    if (await IsLoggedInAsync())
                        return;

                    var userInput = await TryFindLocatorAsync(UsernameSelectors, timeoutMs: 500);
                    var passInput = await TryFindLocatorAsync(PasswordSelectors, timeoutMs: 500);
                    var loginFormVisible = (userInput != null && await userInput.IsVisibleAsync())
                                           || (passInput != null && await passInput.IsVisibleAsync());
                    if (!loginFormVisible)
                        return;
                }
                catch (Exception ex) when (ex.Message.Contains("Frame was detached") || ex.Message.Contains("frame was detached"))
                {
                    // Frame detached means the SPA triggered a navigation — wait for the new frame to settle.
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    return;
                }

                await Task.Delay(500);
            }

            throw new TimeoutException($"Dashboard or logged-in state not reached within {timeoutMs}ms.");
        }

        // Called after WaitForDashboardAsync confirms the portal is loaded.
        public async Task DismissCookieBannerAsync()
            => await DismissCookieBannerIfVisibleAsync();

        public async Task ClickContinueToDashboardButtonAsync()
        {
            try
            {
                var logoutAll = await FindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 3000);
                // Equivalent to waitFor({ state: 'visible', timeout: 3000 })
                await logoutAll.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 3000
                });

                await logoutAll.ClickAsync();
            }
            catch (TimeoutException)
            {
                // button didn't appear, ignore
            }
        }
        public async Task CloseAllSessionIfNeeded()
        {
                var logoutAll = await FindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 3000);
                await ClickAsync(logoutAll);
        }

        public async Task LogoutAsync()
        {
            // Stabilize the page before looking for the avatar button.
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.EvaluateAsync("window.scrollTo(0, 0)");
            await Page.WaitForTimeoutAsync(500);

            var profileButton = await FindLocatorAsync(ProfileButtonSelectors, timeoutMs: 15000);
            await profileButton.ClickAsync();

            var logoutButton = await FindLocatorAsync(LogoutButtonSelectors, timeoutMs: 10000);
            await logoutButton.ClickAsync();

            // Wait for the page navigation triggered by logout to settle before any further interaction.
            try
            {
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions { Timeout = 15000 });
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 15000 });
            }
            catch
            {
                // Navigation may have already completed; continue.
            }
        }

        public async Task LogoutIfLoggedInAsync()
        {
            var isLoggedIn = await IsLoggedInAsync() || await IsDashboardVisibleAsync();
            if (!isLoggedIn)
                return;

            try
            {
                await LogoutAsync();
                await WaitForLoginAsync(10000);
            }
            catch
            {
                // If logout fails, continue; login flow will handle it.
            }
        }

        // FindLocatorAsync / TryFindLocatorAsync are defined in BasePage
    // ── Portal login page assertion ───────────────────────────────────────────

        /// <summary>
        /// Verifies the portal login page / homepage is visible.
        /// No-int portal: checks logo img[alt="Logo"][src*="site_contrast_logo"]
        /// Int portal: falls back to Sign in nav link a[href="/my-portal"] with text "Sign in"
        /// </summary>
        public async Task ShouldSeeLoginPageAsync()
        {
            var logo = Page.Locator("img[alt='Logo'][src*='site_contrast_logo']");
            try
            {
                await logo.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 8000 });
                return;
            }
            catch { /* int portal may not have the logo — fall through to Sign in button check */ }

            // Int portal: homepage shows a "Sign in" nav link instead of the login form directly
            // HTML: <a class="nav-link btn btn-outline-secondary..." href="/my-portal">Sign in</a>
            var signInBtn = Page.Locator("a[href='/my-portal']")
                .Filter(new LocatorFilterOptions { HasText = "Sign in" })
                .First;
            await signInBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(signInBtn, timeoutMs: 5000);
        }

        // ── Portal login (individual steps for TC1483) ────────────────────────────

        private LoginPage CreateLoginPage() =>
            new LoginPage(Page,
                Environment.GetEnvironmentVariable("PORTAL_BASE_URL")
                ?? Environment.GetEnvironmentVariable("BASE_URL")
                ?? "");

        /// <summary>
        /// Finds the username/email input using the same UsernameSelectors as LoginToDFPAsync.
        /// Works for both no-int (Email address) and int (Username) portals.
        /// </summary>
        public async Task FillPortalUsernameAsync(string username)
        {
            var input = await FindUsernameInputAsync();
            await Microsoft.Playwright.Assertions.Expect(input).ToBeEnabledAsync(new() { Timeout = 10000 });
            await input.FillAsync(username);
        }

        /// <summary>
        /// Finds the password input using the same PasswordSelectors as LoginToDFPAsync.
        /// Works for both no-int and int portals.
        /// </summary>
        public async Task FillPortalPasswordAsync(string password)
        {
            var input = await FindPasswordInputAsync();
            await Microsoft.Playwright.Assertions.Expect(input).ToBeEnabledAsync(new() { Timeout = 10000 });
            await input.FillAsync(password);
        }

        /// <summary>
        /// Clicks the Sign in button using the same SignInButtonSelectors as LoginToDFPAsync.
        /// Works for both no-int and int portals.
        /// </summary>
        public async Task ClickPortalSignInAsync()
        {
            var btn = await FindLocatorAsync(SignInButtonSelectors, 10000);
            await btn.ClickAsync();
            await WaitForDashboardAsync();
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







































































































































