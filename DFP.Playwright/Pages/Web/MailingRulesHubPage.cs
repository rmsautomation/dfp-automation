using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class MailingRulesHubPage : BasePage
    {
        public MailingRulesHubPage(IPage page) : base(page)
        {
        }

        // ── Navigation ────────────────────────────────────────────────────────────

        public async Task NavigateToNotificationsAsync()
        {
            var origin = new Uri(Page.Url).GetLeftPart(UriPartial.Authority);
            await Page.GotoAsync(origin + "/administration/notifications");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Mailing Lists ─────────────────────────────────────────────────────────

        public async Task ShouldSeeCreateMailingListButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await btn.IsVisibleAsync(),
                $"Expected 'Create mailing list' button to be visible. URL: {Page.Url}");
        }

        public async Task SearchMailingListByNameAsync(string name)
        {
            var input = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        public async Task ClickSearchButtonAsync()
        {
            var btn = Page.Locator("button[type='submit'].btn-primary.btn-sm")
                .Filter(new LocatorFilterOptions { HasText = "Search" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// If the trash button is visible, clicks it and confirms "Yes". Waits for NetworkIdle after deletion.
        /// </summary>
        public async Task CheckIfListExistsToDeleteItAsync()
        {
            try
            {
                var trashBtn = Page.Locator("button.p-element.btn.btn-outline-danger.ml-1").First;
                await trashBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                await WaitForEnabledAsync(trashBtn, timeoutMs: 5000);
                await trashBtn.ClickAsync();
                var yesBtn = Page.Locator("button.btn-danger.mb-2")
                    .Filter(new LocatorFilterOptions { HasText = "Yes" })
                    .First;
                await yesBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                await ClickAndWaitForNetworkAsync(yesBtn);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            catch (Exception)
            {
                // Trash button not found or not actionable within 10s — nothing to delete, continue
            }
        }

        /// <summary>
        /// Clicks the mailing list link and waits for the detail page to load.
        /// </summary>
        public async Task SelectCreatedMailingListAsync(string name)
        {
            var link = Page.Locator($"//td/a[contains(text(),'{name}')]").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(link);
        }

        public async Task ShouldSeeAvailableMembersListAsync()
        {
            var heading = Page.Locator("h5").Filter(new LocatorFilterOptions { HasText = "Available members" }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Available members' heading to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Types the email with delay so the app registers each character, presses Enter,
        /// then waits for network to settle before the next action.
        /// </summary>
        public async Task EnterEmailToAddMemberAsync(string email)
        {
            var input = Page.Locator("//input[@formcontrolname='email']").Nth(1);
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 5000);
            await input.ClearAsync();
            await input.PressSequentiallyAsync(email, new LocatorPressSequentiallyOptions { Delay = 80 });
            await input.PressAsync("Enter");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the add-member button and waits for the network request to complete.
        /// </summary>
        public async Task AddMemberAsync()
        {
            var btn = Page.Locator("//button[contains(@class,'btn-outline-success') and .//fa-icon]").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Saves the mailing list and waits for the page to reach NetworkIdle before continuing.
        /// </summary>
        public async Task SaveListAsync()
        {
            var btn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Save list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(btn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks "Create mailing list" and waits for navigation to the creation form.
        /// </summary>
        public async Task ClickCreateMailingListButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary:not(.ml-2)")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(btn);
        }

        public async Task EnterMailingNameAsync(string name)
        {
            var input = Page.Locator("input#name[placeholder='Name']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// Waits for the save button to be enabled (avoids hardcoded sleep), then saves.
        /// </summary>
        public async Task ClickCreateMailingListSaveButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(btn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Mailing Rules ─────────────────────────────────────────────────────────

        public async Task GoToMailingRulesAsync()
        {
            var link = Page.Locator("a.nav-link[href*='view=mailing-rules']");
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(link);
        }

        public async Task ShouldSeeMailingRulesAsync()
        {
            var heading = Page.Locator("h6.w-100.text-left");
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected Mailing Rules notice heading to be visible. URL: {Page.Url}");
        }

        public async Task SearchMailingRuleByNameAsync(string name)
        {
            var input = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// If the rule exists, deletes it and waits for the rules list to reload before continuing.
        /// </summary>
        public async Task CheckIfRuleExistsToDeleteItAsync()
        {
            try
            {
                var trashBtn = Page.Locator("button.p-element.btn.btn-outline-danger.ml-1").First;
                await trashBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                await trashBtn.ClickAsync();
                var yesBtn = Page.Locator("button.btn-danger.mb-2")
                    .Filter(new LocatorFilterOptions { HasText = "Yes" })
                    .First;
                await yesBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                await ClickAndWaitForNetworkAsync(yesBtn);
                await Page.WaitForTimeoutAsync(10000);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Wait until we're back on the rules list — the search input is the page-ready indicator.
                // This prevents the next steps from running while the deletion is still in progress.
                var searchInput = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
                await searchInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
                await WaitForEnabledAsync(searchInput, timeoutMs: 8000);
            }
            catch (Exception)
            {
                // Rule not found — a new one will be created
                Console.WriteLine("[MailingRulesHubPage] Rule not found — a new rule will be created.");
            }
        }

        /// <summary>
        /// Waits for the search input to be clickable (page-ready indicator after a delete/reload),
        /// then clicks "Create rule" and navigates to the creation form.
        /// </summary>
        public async Task ClickCreateRuleButtonAsync()
        {
            // Use the search input as a page-ready indicator — it becomes enabled once the list has loaded
            var searchInput = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await searchInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(searchInput, timeoutMs: 8000);

            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create rule" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNavigationAsync(btn);
        }

        public async Task ShouldSeeCreateRuleViewAsync()
        {
            var label = Page.Locator("label[for='template_id']");
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await label.IsVisibleAsync(),
                $"Expected 'Notification Type' label to be visible on create rule page. URL: {Page.Url}");
        }

        public async Task EnterMailingRuleNameAsync(string name)
        {
            var input = Page.Locator("input#name[placeholder='Name']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// Opens the PrimeNG dropdown and clicks the option using page-level GetByText.
        /// Waits for the overlay panel before searching — PrimeNG appends overlays to body.
        /// </summary>
        public async Task SelectNotificationTypeAsync(string type)
        {
            var dropdown = Page.Locator("p-dropdown").First;
            await dropdown.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dropdown.ClickAsync();

            var panel = Page.Locator(".p-dropdown-panel").First;
            await panel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await Page.WaitForTimeoutAsync(500);

            var option = Page.GetByText(type, new PageGetByTextOptions { Exact = true }).First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await option.ClickAsync();
            // Wait for the form to update after selection before next action
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks "Create mailing rule", waits for it to be enabled, then waits for NetworkIdle.
        /// Tries multiple selectors to handle class/text variations.
        /// </summary>
        public async Task ClickCreateMailingRuleButtonAsync()
        {
            // Try GetByRole first (most robust — case-insensitive, partial match)
            var btn = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Create mailing rule" }).First;
            var found = false;
            try
            {
                await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
                found = true;
            }
            catch { /* fallback below */ }

            if (!found)
            {
                // Fallback: any button whose text contains "mailing rule" (case-insensitive via XPath)
                btn = Page.Locator("//button[contains(translate(normalize-space(.),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'mailing rule')]").First;
                await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            }

            await WaitForEnabledAsync(btn, timeoutMs: 8000);
            await ClickAndWaitForNetworkAsync(btn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Recipients ────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the mailing list search panel, searches by name, waits for the result row,
        /// then clicks the circle-plus button.
        /// </summary>
        public async Task SelectMailingListAsync(string mailingListName)
        {
            var addBtn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Add" })
                .First;
            await addBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(addBtn, timeoutMs: 5000);
            await addBtn.ClickAsync();

            var searchInput = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await searchInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await searchInput.FillAsync(mailingListName);
            await searchInput.PressAsync("Enter");

            // Wait for the result row before clicking plus
            var resultRow = Page.Locator($"//td[contains(normalize-space(),'{mailingListName}')]").First;
            await resultRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            var circlePlusBtn = Page.Locator("button.p-element.btn-outline-success.ml-1").First;
            await circlePlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(circlePlusBtn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(circlePlusBtn);
        }

        public async Task ShouldSeeMailingListInRecipientsAsync(string mailingListName)
        {
            var label = Page.Locator("span.p-button-label")
                .Filter(new LocatorFilterOptions { HasText = mailingListName })
                .First;
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await label.IsVisibleAsync(),
                $"Expected mailing list '{mailingListName}' to appear in Recipients. URL: {Page.Url}");
        }

        /// <summary>
        /// Opens the Hub Users search panel, searches by name, waits for the result row,
        /// then clicks the circle-plus button.
        /// </summary>
        public async Task SelectHubUserAsync(string userName)
        {
            var addBtn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Add" })
                .Nth(1);
            await addBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(addBtn, timeoutMs: 5000);
            await addBtn.ClickAsync();

            var searchInput = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").Last;
            await searchInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await searchInput.FillAsync(userName);
            await searchInput.PressAsync("Enter");

            // Wait for the result row before clicking plus
            var resultRow = Page.Locator($"//td[contains(normalize-space(),'{userName}')]").First;
            await resultRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            var circlePlusBtn = Page.Locator("button.p-element.btn-outline-success.ml-1").First;
            await circlePlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(circlePlusBtn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(circlePlusBtn);
        }

        public async Task ShouldSeeUserInHubUsersRecipientsAsync(string userName)
        {
            var label = Page.Locator("span.p-button-label")
                .Filter(new LocatorFilterOptions { HasText = userName })
                .First;
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await label.IsVisibleAsync(),
                $"Expected hub user '{userName}' to appear in Hub Users Recipients. URL: {Page.Url}");
        }

        // ── Customers tab ─────────────────────────────────────────────────────────

        public async Task GoToCustomersTabAsync()
        {
            var link = Page.Locator("a.nav-link[href*='view=customers']");
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(link);
        }

        /// <summary>
        /// Types the customer name into the second search input with delay, presses Enter,
        /// waits for the result row to appear, then clicks the plus button.
        /// </summary>
        public async Task SelectCustomerAsync(string customerName)
        {
            var input = Page.Locator("//th//input[@formcontrolname='name']").Nth(1);
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 5000);
            await input.ClearAsync();
            await input.PressSequentiallyAsync(customerName, new LocatorPressSequentiallyOptions { Delay = 80 });
            await input.PressAsync("Enter");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var customerRow = Page.Locator($"//td[contains(normalize-space(),'{customerName}')]").First;
            await customerRow.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });

            var userPlusBtn = Page.Locator("//button[contains(@class,'btn-outline-success') and .//fa-icon]").First;
            await userPlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(userPlusBtn, timeoutMs: 5000);
            await userPlusBtn.ClickAsync();
        }

        // ── Hub Notifications verification ───────────────────────────────────────

        public async Task ShouldSeeLastNotificationsAsync()
        {
            var link = Page.Locator("a[href='/notifications']")
                .Filter(new LocatorFilterOptions { HasText = "Recent Notifications" })
                .First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await link.IsVisibleAsync(),
                $"Expected 'Recent Notifications' link to be visible. URL: {Page.Url}");
        }

        public async Task ShouldSeeNotificationForShipmentAsync(string notificationText)
        {
            var notification = Page.Locator($"//div[contains(normalize-space(),'{notificationText}')]").First;
            await notification.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await notification.IsVisibleAsync(),
                $"Expected notification containing '{notificationText}' to be visible. URL: {Page.Url}");
        }

        public async Task ClickViewShipmentButtonAsync()
        {
            var btn = Page.Locator("button.btn.btn-primary.btn-sm")
                .Filter(new LocatorFilterOptions { HasText = "View" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNavigationAsync(btn);
        }

        /// <summary>
        /// Waits for NetworkIdle after navigation, then verifies the URL contains the shipmentId
        /// and the shipment name is visible on the page.
        /// </summary>
        public async Task ShouldSeeShipmentDetailsAsync(string shipmentId, string shipmentName)
        {
            // The "View" button may open a new tab. If so, switch context to it.
            var targetPage = Page;
            if (!Page.Url.Contains(shipmentId, StringComparison.OrdinalIgnoreCase))
            {
                // Give the browser up to 5s to open a new tab before falling back.
                var newPage = await TryGetNewPageAsync(timeoutMs: 5000);
                if (newPage != null)
                    targetPage = newPage;
            }

            // Wait for the URL to include the shipment ID (up to 15s).
            try
            {
                await targetPage.WaitForURLAsync(
                    url => url.Contains(shipmentId, StringComparison.OrdinalIgnoreCase),
                    new PageWaitForURLOptions { Timeout = 15000 });
            }
            catch
            {
                // Ignore timeout — the assert below will report the actual URL.
            }

            Assert.IsTrue(targetPage.Url.Contains(shipmentId, StringComparison.OrdinalIgnoreCase),
                $"Expected URL to contain shipment ID '{shipmentId}'. Actual URL: {targetPage.Url}");

            if (!string.IsNullOrWhiteSpace(shipmentName))
            {
                var nameEl = targetPage.Locator($"//*[contains(normalize-space(),'{shipmentName}')]").First;
                await nameEl.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
                Assert.IsTrue(await nameEl.IsVisibleAsync(),
                    $"Expected shipment name '{shipmentName}' to be visible on details page. URL: {targetPage.Url}");
            }
        }

        private async Task<IPage?> TryGetNewPageAsync(int timeoutMs)
        {
            var context = Page.Context;
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                var pages = context.Pages;
                var newTab = pages.FirstOrDefault(p => p != Page && !string.IsNullOrEmpty(p.Url) && p.Url != "about:blank");
                if (newTab != null)
                    return newTab;
                await Task.Delay(300);
            }
            return null;
        }

        /// <summary>
        /// Saves the mailing rule and waits for NetworkIdle to confirm the page settled.
        /// </summary>
        public async Task ClickSaveButtonAsync()
        {
            var btn = Page.Locator("button[type='submit'].btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Save" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(btn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}
