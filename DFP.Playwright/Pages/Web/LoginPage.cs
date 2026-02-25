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
            "internal:role=textbox[name=\"Username\"i]"
        };

        private static readonly string[] PasswordSelectors =
        {
            "internal:role=textbox[name=\"Password\"i]"
        };

        private static readonly string[] SignInButtonSelectors =
        {
            "internal:role=button[name=\"Sign in\"i]"
        };
        // codegen:login-end

        private static readonly string[] LoginHeadingSelectors =
        {
            "internal:role=heading[name=\"Sign in to your account\"i]"
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
            "role=button[name='A']"
        };

        private static readonly string[] LogoutButtonSelectors =
        {
            "role=button[name='Log out']"
        };
        // private const string MAINAPP_DFP_IMG = "/Images/.....";

        private readonly string _baseUrl;


        public async Task NavigateAsync()
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new InvalidOperationException("BaseUrl is null/empty. Provide it via config/env/DI.");

            Console.WriteLine($"navigating to {_baseUrl}");
            await Page.GotoAsync(_baseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.Load });
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            // await Page.WaitForResponseAsync(r => r.Url.Contains(MAINAPP_DFP_IMG));
        }

        public async Task LoginToDFPAsync(string email, string password)
        {
            var usernameInput = await FindLocatorAsync(UsernameSelectors);
            var passwordInput = await FindLocatorAsync(PasswordSelectors);

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

        public async Task<bool> IsLogoutAllSessionButtonVisibleAsync()
        {
            var locator = await TryFindLocatorAsync(LogoutAllSessionSelectors, timeoutMs: 1000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task<bool> IsUsernameInputVisibleAsync()
        {
            var locator = await TryFindLocatorAsync(UsernameSelectors, timeoutMs: 1000);
            return locator != null && await locator.IsVisibleAsync();
        }

        public async Task WaitForLoginAsync(int timeoutMs = 10000)
        {
            var locator = await FindLocatorAsync(LoginHeadingSelectors, timeoutMs);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
        }

        public async Task WaitForLoginFormAsync(int timeoutMs = 10000)
        {
            var locator = await FindLocatorAsync(UsernameSelectors, timeoutMs);
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
            var locator = await FindLocatorAsync(DashboardSelectors, timeoutMs);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs
            });
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










































































