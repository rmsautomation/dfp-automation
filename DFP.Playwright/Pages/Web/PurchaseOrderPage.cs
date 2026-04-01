using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Support;
using TestContext = DFP.Playwright.Support.TestContext;

namespace DFP.Playwright.Pages.Web
{
    public sealed class PurchaseOrderPage : BasePage
    {
        // Stores the generated PO number for use across steps (e.g. buyer/supplier names derived from it).
        private string _poNumber = string.Empty;
        private readonly TestContext _tc;

        public PurchaseOrderPage(IPage page, TestContext tc) : base(page)
        {
            _tc = tc;
        }

        private string PortalOrigin()
            => !string.IsNullOrEmpty(_tc.ActivePortalBaseUrl)
                ? _tc.ActivePortalBaseUrl
                : new Uri(Page.Url).GetLeftPart(UriPartial.Authority);

        // ── Navigation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigates to the Purchase Order list page at /my-portal/orders/list.
        /// </summary>
        public async Task NavigateToPurchaseOrderListAsync()
        {
            await Page.GotoAsync(PortalOrigin() + "/my-portal/orders/list");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // ── Selectors ─────────────────────────────────────────────────────────────

        // "Create new purchase order" button
        // Verified from HTML: button.btn-primary.btn-sm containing "Create" + span "new purchase order"
        private static readonly string[] CreatePOButtonSelectors =
        [
            "button.btn-primary.btn-sm:has-text('Create')",
            "//button[contains(@class,'btn-primary') and contains(normalize-space(),'Create')]"
        ];

        // P/O NUMBER heading — confirms we are on the creation page
        // Verified from HTML: div.head-label.text-muted "P/O NUMBER"
        private static readonly string[] PONumberHeadingSelectors =
        [
            "div.head-label.text-muted",
            "//div[contains(@class,'head-label') and contains(normalize-space(),'P/O NUMBER')]"
        ];

        // ── Action methods ────────────────────────────────────────────────────────

        /// <summary>
        /// Clicks the "Create new purchase order" button in the list page.
        /// Verified from HTML: button.btn-primary.btn-sm with text "Create new purchase order"
        /// </summary>
        public async Task ClickCreatePurchaseOrderButtonAsync()
        {
            var btn = await FindLocatorAsync(CreatePOButtonSelectors, timeoutMs: 10000);
            await WaitForEnabledAsync(btn, timeoutMs: 10000);
            await ClickAndWaitForNetworkAsync(btn);
        }

        /// <summary>
        /// Verifies the creation form is visible by waiting for the "P/O NUMBER" heading.
        /// Verified from HTML: div.head-label.text-muted "P/O NUMBER"
        /// </summary>
        public async Task ShouldBeOnPurchaseOrderCreationPageAsync()
        {
            var heading = await FindLocatorAsync(PONumberHeadingSelectors, timeoutMs: 15000);
            Assert.IsTrue(await heading.IsVisibleAsync(),
                $"Expected 'P/O NUMBER' heading to be visible on the creation page. URL: {Page.Url}");
        }

        /// <summary>
        /// Generates a unique PO number (e.g. "POAuto030914523") and enters it in the PO number input.
        /// Stores the value in _poNumber for use in buyer/supplier steps.
        /// Verified from HTML: input[formcontrolname='po_number'][placeholder='Purchase order number']
        /// </summary>
        public async Task EnterPurchaseOrderNumberAsync()
        {
            _poNumber = $"POAuto{DateTime.Now:ddHHmmss}";
            var input = Page.Locator("input[formcontrolname='po_number']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ClearAsync();
            await input.FillAsync(_poNumber);
        }

        /// <summary>
        /// Enters the buyer name ("{_poNumber}Buyer") in the first Name input of the buyer section.
        /// Verified from HTML: input[formcontrolname='name'][placeholder='Name'] (first occurrence)
        /// </summary>
        public async Task EnterBuyerDetailsAsync()
        {
            var buyerInput = Page.Locator("input[formcontrolname='name'][placeholder='Name']").First;
            await buyerInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await buyerInput.ClearAsync();
            await buyerInput.FillAsync(_poNumber + "Buyer");
        }

        /// <summary>
        /// Selects "USD" in the currency ng-select combobox.
        /// Verified from HTML: input[role='combobox'][aria-autocomplete='list'] → div.ng-option with "USD"
        /// </summary>
        public async Task SelectCurrencyAsync()
        {
            var currencyContainer = Page.Locator("#currency .ng-select-container");
            await currencyContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await currencyContainer.ClickAsync();
            var usdOption = Page.Locator("div.ng-option")
                .Filter(new LocatorFilterOptions { HasText = "USD" })
                .First;
            await usdOption.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await usdOption.ClickAsync();
        }

        /// <summary>
        /// Enters the supplier name ("{_poNumber}Supplier") in the second Name input of the supplier section.
        /// Verified from HTML: input[formcontrolname='name'][placeholder='Name'] (second occurrence)
        /// </summary>
        public async Task EnterSupplierDetailsAsync()
        {
            var supplierInput = Page.Locator("input[formcontrolname='name'][placeholder='Name']").Nth(1);
            await supplierInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await supplierInput.ClearAsync();
            await supplierInput.FillAsync(_poNumber + "Supplier");
        }

        /// <summary>Returns the generated PO number for use in other step definitions.</summary>
        public string GetPoNumber() => _poNumber;

        // ── Additional PO creation steps ──────────────────────────────────────────

        /// <summary>
        /// Selects the transport mode (Ocean / Air / Rail / Truck) from the ng-select combobox.
        /// Verified from HTML: input[role='combobox'] → div.ng-option with mode text and fa-icon (e.g. fa-ship for Ocean)
        /// NOTE: Uses .First — adjust to .Nth(1) if the transport mode combobox is not the first in the form.
        /// </summary>
        public async Task SelectTransportModeAsync(string mode)
        {
            // Scope to the Transport Mode label to avoid clicking the wrong ng-select
            var transportSection = Page.Locator("//label[normalize-space()='Transport Mode']");
            await transportSection.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await transportSection.ScrollIntoViewIfNeededAsync();
            await Page.WaitForTimeoutAsync(500);
            var combobox = transportSection.Locator("xpath=following-sibling::*//input[@role='combobox']");
            if (await combobox.CountAsync() == 0)
                combobox = Page.Locator("ng-select[formcontrolname='mode'] input[role='combobox']");
            await combobox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await combobox.ClickAsync();
            var option = Page.Locator("div.ng-option")
                .Filter(new LocatorFilterOptions { HasText = mode })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
            await option.ClickAsync();
        }

        /// <summary>
        /// Types in the Cargo Origin p-autocomplete input and clicks the first suggestion.
        /// Verified from HTML: input.p-autocomplete-input[placeholder='Cargo origin'] → li.p-autocomplete-item (first)
        /// </summary>
        public async Task EnterCargoOriginAsync(string origin)
        {
            var input = Page.Locator("input.p-autocomplete-input[placeholder='Cargo origin']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ScrollIntoViewIfNeededAsync();
            await input.ClearAsync();
            await input.PressSequentiallyAsync(origin, new LocatorPressSequentiallyOptions { Delay = 80 });
            var firstSuggestion = Page.Locator("li.p-autocomplete-item").First;
            await firstSuggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await firstSuggestion.ClickAsync();
        }

        /// <summary>
        /// Types in the Cargo Destination p-autocomplete input and clicks the first suggestion.
        /// Verified from HTML: input.p-autocomplete-input[placeholder='Cargo destination'] → li.p-autocomplete-item (first)
        /// </summary>
        public async Task EnterCargoDestinationAsync(string destination)
        {
            var input = Page.Locator("input.p-autocomplete-input[placeholder='Cargo destination']");
            await input.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await input.ScrollIntoViewIfNeededAsync();
            await input.ClearAsync();
            await input.PressSequentiallyAsync(destination, new LocatorPressSequentiallyOptions { Delay = 80 });
            var firstSuggestion = Page.Locator("li.p-autocomplete-item").First;
            await firstSuggestion.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            await firstSuggestion.ClickAsync();
        }

        /// <summary>
        /// Clicks the "Save" submit button in the PO creation form.
        /// Verified from HTML: button[type='submit'].btn-primary "Save"
        /// </summary>
        public async Task ClickSaveButtonAsync()
        {
            // Dismiss cookie consent banner if it is blocking the click
            var cookieBanner = Page.Locator("div.cc-window[role='dialog']").First;
            if (await cookieBanner.IsVisibleAsync())
                await Page.EvaluateAsync("document.querySelectorAll('.cc-window').forEach(el => el.remove())");

            var saveBtn = Page.Locator("button[type='submit'].btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Save" })
                .First;
            await saveBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(saveBtn, timeoutMs: 10000);
            await saveBtn.ClickAsync();
            // Wait for navigation to the PO detail page URL which contains a GUID
            await Page.WaitForURLAsync(
                new Regex(@"/orders/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"),
                new PageWaitForURLOptions { Timeout = 30000 });
        }

        /// <summary>
        /// Verifies the PO detail page is displayed: "Edit order" button is visible
        /// and the h3 heading contains the generated PO number.
        /// Verified from HTML: button.btn-primary with "Edit order" text + h3.m-0 with _poNumber.
        /// </summary>
        public async Task ShouldSeePurchaseOrderDetailsAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var editBtn = Page.Locator("button.btn-primary")
                .Filter(new LocatorFilterOptions { HasText = "Edit order" })
                .First;
            await editBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 20000 });
            Assert.IsTrue(await editBtn.IsVisibleAsync(),
                $"Expected 'Edit order' button to be visible on the PO detail page. URL: {Page.Url}");

            var poHeading = Page.Locator("h3.m-0")
                .Filter(new LocatorFilterOptions { HasText = _poNumber })
                .First;
            await poHeading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            Assert.IsTrue(await poHeading.IsVisibleAsync(),
                $"Expected PO heading with number '{_poNumber}' to be visible. URL: {Page.Url}");
        }

        /// <summary>
        /// Enters the stored PO number in the search input on the list page.
        /// Verified from HTML: input[formcontrolname='po_number'].form-control-sm (list search — different from the creation input which uses form-control-lg)
        /// </summary>
        public async Task EnterPurchaseOrderNumberInSearchAsync()
        {
            var searchInput = Page.Locator("input[formcontrolname='po_number'].form-control-sm");
            await searchInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await searchInput.ClearAsync();
            await searchInput.FillAsync(_poNumber);
        }

        /// <summary>
        /// Clicks the "Search" submit button on the PO list page.
        /// Verified from HTML: button[type='submit'].btn-primary.btn-sm "Search"
        /// </summary>
        public async Task ClickSearchButtonAsync()
        {
            var searchBtn = Page.Locator("button[type='submit'].btn-primary.btn-sm")
                .Filter(new LocatorFilterOptions { HasText = "Search" })
                .First;
            await searchBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
            await WaitForEnabledAsync(searchBtn, timeoutMs: 5000);
            await ClickAndWaitForNetworkAsync(searchBtn);
        }

        /// <summary>
        /// Verifies the generated PO number appears as a link in the results list.
        /// Verified from HTML: a.text-dark with text matching _poNumber.
        /// </summary>
        public async Task ShouldSeePurchaseOrderInListAsync()
        {
            var poLink = Page.Locator("a.text-dark")
                .Filter(new LocatorFilterOptions { HasText = _poNumber })
                .First;
            await poLink.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 15000 });
            Assert.IsTrue(await poLink.IsVisibleAsync(),
                $"Expected PO '{_poNumber}' to appear as a link in the results list. URL: {Page.Url}");
        }
    }
}
