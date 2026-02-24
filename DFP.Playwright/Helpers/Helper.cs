using Microsoft.Playwright;
using DFP.Playwright.Pages.Web.BasePages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DFP.Playwright.Helpers
{
    public static class Helper
    {
        public static async Task ClickFirstByTextAsync(IPage page, string text)
        {
            await page.GetByText(text).First.ClickAsync();
        }
        public static async Task ClickFirstByTextOnIframeAsync(IPage page, string text, string iframeLocator, string tagName = "*")
        {
            var locator = page.FrameLocator($"#{iframeLocator}").Locator($"//{tagName}[text()='{text}']").First;
            await locator.ClickAsync();
        }
        public static async Task ClickLastByTextOnIframeAsync(IPage page, string text, string iframeLocator, string tagName = "*")
        {
            var locator = page.FrameLocator($"#{iframeLocator}").Locator($"//{tagName}[text()='{text}']").Last;
            await locator.ClickAsync();
        }
        public static async Task ForceClickFirstByTextOnIframeAsync(IPage page, string text, string iframeLocator, string tagName = "*")
        {
            await page.FrameLocator($"#{iframeLocator}").Locator($"//{tagName}[text()='{text}']").First.EvaluateAsync("el => el.click()");
        }
        public static async Task<bool> IsElementVisibleByTextAsync(IPage page, string text, string tagName = "*", int timeoutMs = 1000)
        {
            try
            {
                await page.Locator($"//{tagName}[text()='{text}']").First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeoutMs
                });
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
        public static async Task<bool> IsElementVisibleOnIframeByTextAsync(IPage page, string text, string iframeLocator, string tagName = "*", int timeoutMs = 1000)
        {
            try
            {
                await page.FrameLocator($"#{iframeLocator}").Locator($"//{tagName}[text()='{text}']").First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = timeoutMs
                });
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }
}

