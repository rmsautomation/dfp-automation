using Microsoft.Playwright;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class UsersHubPage : BasePage
    {
        private string _searchEmail = "";

        private static readonly string[] PortalUsersHeadingSelectors =
        [
            "internal:role=heading[name=\"Portal Users\"i]",
            "h3:has-text('Portal Users')",
            "//h3[normalize-space(text())='Portal Users']"
        ];

        private static readonly string[] SearchEmailInputSelectors =
        [
            "internal:role=textbox[name=\"Search by Email\"i]",
            "input[formcontrolname='email']",
            "input[placeholder='Search by Email']"
        ];

        private static readonly string[] SearchButtonSelectors =
        [
            "internal:role=button[name=\"Search\"i]",
            "button[type='submit'].btn-primary",
            "button.btn-primary.btn-sm"
        ];

        private static readonly string[] PortalUserHeadingSelectors =
        [
            "internal:role=heading[name=\"Portal User\"i]",
            "h3:has-text('Portal User')",
            "//h3[normalize-space(text())='Portal User']"
        ];

        private static readonly string[] PermissionsDropdownTriggerSelectors =
        [
            ".p-icon.p-multiselect-trigger-icon",
            ".p-multiselect-trigger",
            ".p-multiselect"
        ];

        private static readonly string[] PermissionsSearchboxSelectors =
        [
            "internal:role=searchbox",
            ".p-multiselect-filter",
            "input.p-multiselect-filter"
        ];

        private static readonly string[] PermissionCheckedItemSelectors =
        [
            "li[data-p-highlight='true']",
            "li.p-multiselect-item[data-p-highlight='true']",
            "//li[@data-p-highlight='true']",
            ".p-checkbox-box.p-highlight"
        ];

        private static readonly string[] SaveUserButtonSelectors =
        [
            "internal:role=button[name=\"Save User\"i]",
            "button.btn-primary:has-text('Save User')",
            "//button[normalize-space(text())='Save User']"
        ];

        public UsersHubPage(IPage page) : base(page) { }

        public async Task IGoToPortalUsers()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");

            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/administration/portal-users");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task ThePortalUsersPageShouldBeDisplayed()
        {
            var heading = await TryFindLocatorAsync(PortalUsersHeadingSelectors, timeoutMs: 10000);
            Assert.IsNotNull(heading, $"'Portal Users' heading not found. URL: {Page.Url}");
        }

        public async Task ISearchTheUserByEmail(string email)
        {
            _searchEmail = email;
            var input = await FindLocatorAsync(SearchEmailInputSelectors);
            await input.FillAsync(email);
        }

        public async Task IClickOnSearchButton()
        {
            var button = await FindLocatorAsync(SearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(button);
        }

        public async Task IShouldSeeTheUserInTheResults()
        {
            var emailPrefix = _searchEmail.Length > 0 ? _searchEmail.Split('@')[0] + "@" : _searchEmail;
            var cell = await TryFindLocatorAsync(
            [
                $"internal:role=cell[name=\"{emailPrefix}\"i] >> nth=0",
                $"//td[contains(normalize-space(),'{_searchEmail}')]",
                "table tbody tr:first-child td"
            ], timeoutMs: 10000);
            Assert.IsNotNull(cell, $"User '{_searchEmail}' not found in search results. URL: {Page.Url}");
        }

        public async Task IClickOnTheCustomerNameInTheUserPage()
        {
            var link = await FindLocatorAsync(
            [
                "internal:role=link[name=\"Automation Permissions\"i]",
                "//table//tbody//tr[1]//a[contains(@href,'/portal-users/')]",
                "table tbody tr:first-child a"
            ]);
            var currentUrl = Page.Url;
            await ClickAsync(link);
            await Page.WaitForURLAsync(
                url => url != currentUrl && url.Contains("/portal-users/"),
                new PageWaitForURLOptions { Timeout = 15000 });
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IShouldSeeTheUserDetailsPage()
        {
            var heading = await TryFindLocatorAsync(PortalUserHeadingSelectors, timeoutMs: 10000);
            Assert.IsNotNull(heading, $"'Portal User' detail heading not found. URL: {Page.Url}");
        }

        public async Task IClickOnThePermissionsDropdown()
        {
            var trigger = await FindLocatorAsync(PermissionsDropdownTriggerSelectors);
            await ClickAsync(trigger);
        }

        public async Task IEnterThePermissionInTheSearchSection(string permissionName)
        {
            var searchbox = await FindLocatorAsync(PermissionsSearchboxSelectors);
            await searchbox.FillAsync(permissionName);
        }

        public async Task IUncheckedThePermission()
        {
            // If the item is already unchecked (.p-highlight absent), skip — nothing to do
            var item = await TryFindLocatorAsync(PermissionCheckedItemSelectors, timeoutMs: 5000);
            if (item != null)
                await ClickAsync(item);
        }

        public async Task IClickOnSaveUserButton()
        {
            // Close the multiselect dropdown before saving
            await Page.Keyboard.PressAsync("Escape");
            var button = await FindLocatorAsync(SaveUserButtonSelectors);
            await ClickAndWaitForNetworkAsync(button);
        }
    }
}
