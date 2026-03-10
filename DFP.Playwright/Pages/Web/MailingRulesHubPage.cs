using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        /// <summary>
        /// Navigates to /administration/notifications using the current hub origin.
        /// </summary>
        public async Task NavigateToNotificationsAsync()
        {
            var origin = new Uri(Page.Url).GetLeftPart(UriPartial.Authority);
            await Page.GotoAsync(origin + "/administration/notifications");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Mailing Lists ─────────────────────────────────────────────────────────

        /// <summary>
        /// Asserts the "Create mailing list" button is visible.
        /// Verified from HTML: button.btn-primary containing "Create mailing list"
        /// </summary>
        public async Task ShouldSeeCreateMailingListButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await btn.IsVisibleAsync(),
                $"Expected 'Create mailing list' button to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the "Search by Name" input in the Mailing Lists section.
        /// Verified from HTML: input[formcontrolname='name'][placeholder='Search by Name']
        /// </summary>
        public async Task SearchMailingListByNameAsync(string name)
        {
            var input = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// Clicks the Search submit button (shared by mailing list and mailing rule sections).
        /// Verified from HTML: button[type='submit'].btn-primary.btn-sm with text "Search"
        /// </summary>
        public async Task ClickSearchButtonAsync()
        {
            var btn = Page.Locator("button[type='submit'].btn-primary.btn-sm")
                .Filter(new LocatorFilterOptions { HasText = "Search" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// If the trash (delete) button is visible in search results, clicks it and confirms with "Yes".
        /// Used for mailing list cleanup before creating a new one.
        /// Verified from HTML: button.p-element.btn-outline-danger.ml-1 → button.btn-danger "Yes"
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
            }
            catch (Exception)
            {
                // Trash button not found or not actionable within 10s — nothing to delete, continue
            }
        }

        /// <summary>
        /// Clicks the link of the created mailing list in the search results table.
        /// Verified from HTML: //td/a[contains(text(),'{name}')]
        /// </summary>
        public async Task SelectCreatedMailingListAsync(string name)
        {
            var link = Page.Locator($"//td/a[contains(text(),'{name}')]").First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await link.ClickAsync();
        }

        /// <summary>
        /// Asserts the "Available members" heading is visible in the mailing list detail.
        /// Verified from HTML: h5.font-weight-normal.m-0 "Available members"
        /// </summary>
        public async Task ShouldSeeAvailableMembersListAsync()
        {
            var heading = Page.Locator("h5").Filter(new LocatorFilterOptions { HasText = "Available members" }).First;
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'Available members' heading to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the second email input and presses Enter.
        /// Verified from HTML: second //input[@formcontrolname='email']
        /// </summary>
        public async Task EnterEmailToAddMemberAsync(string email)
        {
            var input = Page.Locator("//input[@formcontrolname='email']").Nth(1);
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 5000);
            await input.ClearAsync();
            await input.PressSequentiallyAsync(email, new LocatorPressSequentiallyOptions { Delay = 80 });
            await input.PressAsync("Enter");
            await Page.WaitForTimeoutAsync(1500);
        }

        /// <summary>
        /// Clicks the add-member (circle-plus / btn-outline-success with fa-icon) button.
        /// Verified from HTML: //button[contains(@class,'btn-outline-success') and .//fa-icon]
        /// </summary>
        public async Task AddMemberAsync()
        {
            var btn = Page.Locator("//button[contains(@class,'btn-outline-success') and .//fa-icon]").First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await btn.ClickAsync();
        }

        /// <summary>
        /// Clicks the "Save list" button (floppy-disk icon, btn-primary ml-2).
        /// Verified from HTML: button.btn-primary.ml-2 containing "Save list"
        /// </summary>
        public async Task SaveListAsync()
        {
            var btn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Save list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Clicks the main "Create mailing list" button to open the creation form.
        /// Verified from HTML: button.btn-primary (no ml-2) containing "Create mailing list"
        /// </summary>
        public async Task ClickCreateMailingListButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary:not(.ml-2)")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(btn);
        }

        /// <summary>
        /// Fills the Name input in the mailing list creation form.
        /// Verified from HTML: input#name[placeholder='Name']
        /// </summary>
        public async Task EnterMailingNameAsync(string name)
        {
            var input = Page.Locator("input#name[placeholder='Name']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// Clicks the "Create mailing list" save button (floppy-disk icon, has ml-2 class).
        /// Verified from HTML: button.btn-primary.ml-2 containing "Create mailing list"
        /// </summary>
        public async Task ClickCreateMailingListSaveButtonAsync()
        {
            await Page.WaitForTimeoutAsync(5000);
            var btn = Page.Locator("button.btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing list" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }

        // ── Mailing Rules ─────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the "Mailing Rules" nav tab.
        /// Verified from HTML: a.nav-link[href*='view=mailing-rules']
        /// </summary>
        public async Task GoToMailingRulesAsync()
        {
            var link = Page.Locator("a.nav-link[href*='view=mailing-rules']");
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(link);
        }

        /// <summary>
        /// Asserts the Mailing Rules section is visible via its notice heading.
        /// Verified from HTML: h6.w-100.text-left "Please note..."
        /// </summary>
        public async Task ShouldSeeMailingRulesAsync()
        {
            var heading = Page.Locator("h6.w-100.text-left");
            await heading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected Mailing Rules notice heading to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the "Search by Name" input in the Mailing Rules section.
        /// Verified from HTML: input[formcontrolname='name'][placeholder='Search by Name']
        /// </summary>
        public async Task SearchMailingRuleByNameAsync(string name)
        {
            var input = Page.Locator("input[formcontrolname='name'][placeholder='Search by Name']").First;
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// If the trash button is visible in search results, clicks it and confirms "Yes".
        /// Logs to console if rule was not found (new rule will be created).
        /// Verified from HTML: button.p-element.btn-outline-danger.ml-1 → button.btn-danger "Yes"
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
            }
            catch (Exception)
            {
                // Trash button not found or not actionable within 10s — rule does not exist, a new one will be created
                Console.WriteLine("[MailingRulesHubPage] Rule not found — a new rule will be created.");
            }
        }

        /// <summary>
        /// Clicks the "Create rule" button to open the rule creation form.
        /// Verified from HTML: button.btn-primary containing "Create rule"
        /// </summary>
        public async Task ClickCreateRuleButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create rule" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 8000);
            await ClickAndWaitForNavigationAsync(btn);
        }

        /// <summary>
        /// Asserts the rule creation view is visible via "Notification Type" label.
        /// Verified from HTML: label[for='template_id'] "Notification Type"
        /// </summary>
        public async Task ShouldSeeCreateRuleViewAsync()
        {
            var label = Page.Locator("label[for='template_id']");
            await label.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await label.IsVisibleAsync(),
                $"Expected 'Notification Type' label to be visible on create rule page. URL: {Page.Url}");
        }

        /// <summary>
        /// Fills the Name input in the mailing rule creation form.
        /// Verified from HTML: input#name[placeholder='Name']
        /// </summary>
        public async Task EnterMailingRuleNameAsync(string name)
        {
            var input = Page.Locator("input#name[placeholder='Name']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(name);
        }

        /// <summary>
        /// Selects the notification type from the PrimeNG p-dropdown combobox.
        /// Verified from HTML: span[role='combobox'][aria-label='Select'] → li.p-dropdown-item with type text
        /// </summary>
        public async Task SelectNotificationTypeAsync(string type)
        {
            // Click the p-dropdown container to open the panel
            var dropdown = Page.Locator("p-dropdown").First;
            await dropdown.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await dropdown.ClickAsync();

            // Wait for panel and let animation settle
            var panel = Page.Locator(".p-dropdown-panel").First;
            await panel.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await Page.WaitForTimeoutAsync(500);

            // Use page-level GetByText — PrimeNG overlays are appended to body outside the panel locator scope
            var option = Page.GetByText(type, new PageGetByTextOptions { Exact = true }).First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await option.ClickAsync();
        }

        /// <summary>
        /// Clicks the "Create mailing rule" submit button.
        /// Verified from HTML: button[type='submit'].btn-primary.ml-2 containing "Create mailing rule"
        /// </summary>
        public async Task ClickCreateMailingRuleButtonAsync()
        {
            var btn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Create mailing rule" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(btn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        // ── Recipients ────────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the first "Add" button (mailing list section), searches by name, then clicks circle-plus.
        /// Verified from HTML: button.btn-primary.ml-2 "Add" → input Search by Name → button.btn-outline-success circle-plus
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

            var circlePlusBtn = Page.Locator("button.p-element.btn-outline-success.ml-1").First;
            await circlePlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(circlePlusBtn, timeoutMs: 5000);
            await circlePlusBtn.ClickAsync();
        }

        /// <summary>
        /// Asserts the mailing list appears in the Recipients tab as a p-button label.
        /// Verified from HTML: button.p-button span.p-button-label with mailing list name
        /// </summary>
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
        /// Clicks the second "Add" button (Hub Users section), searches by name, then clicks circle-plus.
        /// Verified from HTML: second button.btn-primary.ml-2 "Add" → input Search by Name → button.btn-outline-success circle-plus
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

            var circlePlusBtn = Page.Locator("button.p-element.btn-outline-success.ml-1").First;
            await circlePlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(circlePlusBtn, timeoutMs: 5000);
            await circlePlusBtn.ClickAsync();
        }

        /// <summary>
        /// Asserts the hub user appears in the Hub Users Recipients section.
        /// Verified from HTML: span.p-button-label with user name
        /// </summary>
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

        /// <summary>
        /// Clicks the "Customers" nav tab inside the mailing rule detail.
        /// Verified from HTML: a.nav-link[href*='view=customers']
        /// </summary>
        public async Task GoToCustomersTabAsync()
        {
            var link = Page.Locator("a.nav-link[href*='view=customers']");
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNavigationAsync(link);
        }

        /// <summary>
        /// Searches for the customer by name, presses Enter, then clicks the user-plus icon button.
        /// Verified from HTML: input.p-inputtext[formcontrolname='name'] → fa-icon svg[data-icon='user-plus']
        /// </summary>
        public async Task SelectCustomerAsync(string customerName)
        {
            var input = Page.Locator("//th//input[@formcontrolname='name']").Nth(1);
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(input, timeoutMs: 5000);
            await input.ClearAsync();
            await input.PressSequentiallyAsync(customerName, new LocatorPressSequentiallyOptions { Delay = 80 });
            await input.PressAsync("Enter");

            var userPlusBtn = Page.Locator("//button[contains(@class,'btn-outline-success') and .//fa-icon]").First;
            await userPlusBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await WaitForEnabledAsync(userPlusBtn, timeoutMs: 5000);
            await userPlusBtn.ClickAsync();
        }

        // ── Hub Notifications verification ───────────────────────────────────────

        /// <summary>
        /// Asserts the "Recent Notifications" link is visible on the dashboard.
        /// Verified from HTML: a[href='/notifications'] "Recent Notifications"
        /// </summary>
        public async Task ShouldSeeLastNotificationsAsync()
        {
            var link = Page.Locator("a[href='/notifications']")
                .Filter(new LocatorFilterOptions { HasText = "Recent Notifications" })
                .First;
            await link.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await link.IsVisibleAsync(),
                $"Expected 'Recent Notifications' link to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Asserts a notification containing the given text exists in the notifications list.
        /// Verified from HTML: div containing notification text inside the notifications section.
        /// </summary>
        public async Task ShouldSeeNotificationForShipmentAsync(string notificationText)
        {
            var notification = Page.Locator($"//div[contains(normalize-space(),'{notificationText}')]").First;
            await notification.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await notification.IsVisibleAsync(),
                $"Expected notification containing '{notificationText}' to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Clicks the "View Shipment" button in the notification card.
        /// Verified from HTML: button.btn.btn-primary.btn-sm with text "View"
        /// </summary>
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
        /// Asserts the current URL contains the shipment GUID and the shipment name text is visible.
        /// </summary>
        public async Task ShouldSeeShipmentDetailsAsync(string shipmentId, string shipmentName)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForTimeoutAsync(2000);

            Assert.IsTrue(Page.Url.Contains(shipmentId, StringComparison.OrdinalIgnoreCase),
                $"Expected URL to contain shipment ID '{shipmentId}'. Actual URL: {Page.Url}");

            if (!string.IsNullOrWhiteSpace(shipmentName))
            {
                var nameEl = Page.Locator($"//*[contains(normalize-space(),'{shipmentName}')]").First;
                await nameEl.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
                Assert.IsTrue(await nameEl.IsVisibleAsync(),
                    $"Expected shipment name '{shipmentName}' to be visible on details page. URL: {Page.Url}");
            }
        }

        /// <summary>
        /// Clicks the "Save" submit button in the mailing rule detail.
        /// Verified from HTML: button[type='submit'].btn-primary.ml-2 containing "Save"
        /// </summary>
        public async Task ClickSaveButtonAsync()
        {
            var btn = Page.Locator("button[type='submit'].btn-primary.ml-2")
                .Filter(new LocatorFilterOptions { HasText = "Save" })
                .First;
            await btn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await ClickAndWaitForNetworkAsync(btn);
        }
    }
}
