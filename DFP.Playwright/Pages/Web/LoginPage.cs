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
            "role=link[name='Dashboard']",
            "a:has-text('Dashboard')"
        };

        private static readonly string[] ProfileButtonSelectors =
        {
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
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // await Page.WaitForResponseAsync(r => r.Url.Contains(MAINAPP_DFP_IMG));
        }

        public async Task LoginToDFPAsync(string email, string password, bool searchLoginModal = false)
        {
            if (searchLoginModal)
            {
                await EnsureLoginFormAsync(30000);
            }
            else
            {
                await WaitForLoginFormAsync(30000);
            }
            var usernameInput = await FindUsernameInputAsync();
            var passwordInput = await FindPasswordInputAsync();

            await usernameInput.FillAsync(email);
            await Page.WaitForTimeoutAsync(500);
            await passwordInput.FillAsync(password);
            await Page.WaitForTimeoutAsync(500);

            // Prefer clicking Sign in if available
            var signInButton = await TryFindLocatorAsync(SignInButtonSelectors, timeoutMs: 2000);
            if (signInButton != null)
            {
                await signInButton.ClickAsync();
            }
            else
            {
                await passwordInput.PressAsync("Enter");
            }
            var logoutAll = await TryFindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 3000);
            if (logoutAll != null && await IsVisibleAsync(logoutAll))
            {
                await CloseAllSessionIfNeeded();
                await Page.WaitForTimeoutAsync(1000);
                await usernameInput.FillAsync(email);
                await Page.WaitForTimeoutAsync(500);
                await passwordInput.FillAsync(password);
                await Page.WaitForTimeoutAsync(500);
                await passwordInput.PressAsync("Enter");
            }
                

            await Page.WaitForTimeoutAsync(1000);
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
            var loginForm = await TryFindLocatorAsync(LoginFormPrimarySelectors, timeoutMs: 2000);
            if (loginForm == null || !await loginForm.IsVisibleAsync())
            {
                var signIn = await TryFindLocatorAsync(SignInButtonSelectors, timeoutMs: 2000);
                if (signIn != null && await IsVisibleAsync(signIn))
                {
                    await ClickAndWaitForNavigationAsync(signIn);
                }
            }

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
            var locator = await TryFindLocatorAsync(DashboardSelectors, timeoutMs: 5000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var profile = await TryFindLocatorAsync(ProfileButtonSelectors, timeoutMs: 2000);
            if (profile != null && await profile.IsVisibleAsync())
                return true;

            var logout = await TryFindLocatorAsync(LogoutButtonSelectors, timeoutMs: 2000);
            return logout != null && await logout.IsVisibleAsync();
        }

        public async Task WaitForDashboardAsync(int timeoutMs = 10000)
        {
            var start = DateTime.UtcNow;
            while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
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

                await Task.Delay(500);
            }

            throw new TimeoutException($"Dashboard or logged-in state not reached within {timeoutMs}ms.");
        }

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
            var profileButton = await FindLocatorAsync(ProfileButtonSelectors, timeoutMs: 5000);
            await profileButton.ClickAsync();

            var logoutButton = await FindLocatorAsync(LogoutButtonSelectors, timeoutMs: 5000);
            await logoutButton.ClickAsync();
        }

        // FindLocatorAsync / TryFindLocatorAsync are defined in BasePage
    }
}







































































































































