using CucumberExpressions.Ast;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFP.Playwright.Pages.Web.BasePages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(IPage page)
        {
            Page = page;
        }

        //
        /*   REGULAR METHODS   */
        //

        protected async Task ClickAsync(ILocator locator)
        {
            // Playwright already auto-waits for element to be actionable
            await locator.ClickAsync();
        }

        // Use after clicks that trigger a full page navigation (Angular router change).
        // DOMContentLoaded resolves immediately if the page already reached that state — NOT a hard wait.
        //Use this method when change URL/route Angular      ClickAndWaitForNavigationAsync

        protected async Task ClickAndWaitForNavigationAsync(ILocator locator)
        {
            await locator.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        // Use after clicks that submit data to the API (save, confirm, send booking).
        // NetworkIdle waits until there are no pending network requests for 500ms.
        //  When call API and update data   ClickAndWaitForNetworkAsyncUse this method when you expect API calls to be made and want to wait for them to complete before proceeding.      ClickAndWaitForNetworkAsync
        protected async Task ClickAndWaitForNetworkAsync(ILocator locator)
        {
            await locator.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        protected async Task ClickForceAsync(ILocator locator)
        {
            // Playwright already auto-waits for element to be actionable
            await locator.ClickAsync(new() { Force = true });
        }

        protected async Task ClearAsync(ILocator locator)
        {
            // Playwright already auto-waits for element to be actionable
            await locator.ClearAsync();
        }

        protected async Task TypeAsync(ILocator locator, string textToWrite, bool clearFirst = true)
        {
            if (clearFirst)
                await locator.FillAsync(textToWrite);
            else
                await locator.PressSequentiallyAsync(textToWrite);
        }

        protected async Task<bool> IsVisibleAsync(ILocator locator, int timeoutMs = 1000)
        {
            try
            {
                await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = timeoutMs });
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        protected async Task<string?> GetAttributeAsync(ILocator locator, string attributeName)
            => await locator.GetAttributeAsync(attributeName);

        protected async Task<string> GetTextAsync(ILocator locator)
            => await locator.InnerTextAsync();

        protected async Task<string> GetInputTextAsync(ILocator locator)
            => await locator.InputValueAsync();

        public async Task<string> GetSelectButtonTextAsync(ILocator locator)
        {
            return await locator
                .Locator("option:checked")
                .InnerTextAsync();
        }
        public async Task<string> GetFirstOptionTextAsync(ILocator select)
        {
            await select.WaitForAsync(); // ensure select exists
            return await select.Locator("option").First.InnerTextAsync();
        }

        protected async Task<IReadOnlyList<IElementHandle>> GetElementsAsync(ILocator locator)
            => await locator.ElementHandlesAsync();

        //
        /*   SPECIFIC METHODS   */
        //

        protected async Task ClickLastAsync(ILocator locator)
        {
            // Equivalent to FindElements(locator).Last().Click()
            await locator.Last.ClickAsync();
        }

        protected async Task ScrollUntilVisibleAsync(ILocator locator, int maxScrolls = 5, int scrollPx = 600)
        {
            for (int i = 0; i < maxScrolls; i++)
            {
                if (await IsVisibleAsync(locator, timeoutMs: 250))
                    return;

                // alternate direction if you want; here: mostly scroll down
                var direction = (i % 2 == 0) ? 1 : -1;
                await Page.Mouse.WheelAsync(0, scrollPx * direction);
                await Page.WaitForTimeoutAsync(150);
            }

            if (!await IsVisibleAsync(locator, timeoutMs: 500))
                throw new PlaywrightException($"Element was not found after {maxScrolls} scrolls.");
        }

        protected async Task ScrollThenClickAsync(ILocator locator, int maxScrolls = 5)
        {
            await ScrollUntilVisibleAsync(locator, maxScrolls);
            await locator.ClickAsync();
        }

        protected static string ToXPathLiteral(string value)
        {
            if (value == null)
                return "''";

            if (!value.Contains("'"))
                return $"'{value}'";

            if (!value.Contains("\""))
                return $"\"{value}\"";

            var parts = value.Split('\'');
            var sb = new StringBuilder("concat(");
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                    sb.Append(", \"'\", ");
                sb.Append('\'').Append(parts[i]).Append('\'');
            }
            sb.Append(')');
            return sb.ToString();
        }

        protected ILocator GetElementByTextAsync(string textToSearch)
        {
            var literal = ToXPathLiteral(textToSearch);
            var locator = Page.Locator($"//*[contains(text(),{literal})]");
            return locator;
        }

        protected async Task ClickElementByTextAsync(string textToSearch, string elementTag = "*")
        {
            var literal = ToXPathLiteral(textToSearch);
            var locator = Page.Locator($"//{elementTag}[contains(text(),{literal})]");
            await locator.Last.ClickAsync();
        }

        protected async Task ScrollThenClickLastAsync(ILocator locator, int maxScrolls = 5)
        {
            await ScrollUntilVisibleAsync(locator, maxScrolls);
            await locator.Last.ClickAsync();
        }

        protected async Task HoverAsync(ILocator locator)
        {
            await locator.HoverAsync();
        }

        protected async Task<ILocator> FindLocatorAsync(string[] selectors, int timeoutMs = 30000)
        {
            var start = DateTime.UtcNow;

            while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
            {
                foreach (var selector in selectors)
                {
                    var direct = Page.Locator(selector);
                    if (await direct.CountAsync() > 0)
                        return direct.First;

                    foreach (var frame in Page.Frames)
                    {
                        var inFrame = frame.Locator(selector);
                        if (await inFrame.CountAsync() > 0)
                            return inFrame.First;
                    }
                }

                await Task.Delay(250);
            }

            throw new TimeoutException($"Selector not found within {timeoutMs}ms. Tried: {string.Join(", ", selectors)}");
        }

        protected async Task<ILocator?> TryFindLocatorAsync(string[] selectors, int timeoutMs = 3000)
        {
            try
            {
                return await FindLocatorAsync(selectors, timeoutMs);
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public async Task<string> GetAlertTextAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            EventHandler<IDialog>? handler = null;
            handler = async (_, dialog) =>
            {
                Page.Dialog -= handler;
                tcs.TrySetResult(dialog.Message);
                await dialog.AcceptAsync();
            };
            Page.Dialog += handler;

            return await tcs.Task;
        }


    }
}

