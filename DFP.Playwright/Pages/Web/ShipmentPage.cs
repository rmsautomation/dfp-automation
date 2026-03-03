using Microsoft.Playwright;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ShipmentPage(IPage page) : BasePage(page)
    {
        private string _shipmentName = string.Empty;
        private string _tagName = string.Empty;
        private readonly List<string> _allTagNames = [];

        private static string GetPortalBaseUrl()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.PORTAL_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("PORTAL_BASE_URL (or BASE_URL) is required.");
            return baseUrl;
        }

        // codegen:selectors-start
        // Selectors captured by codegen for 'createshipmentfromquotation'
        public static readonly string[] Selectors = [];
        // codegen:selectors-end

        // ── Quotation / Shipment creation Selectors ──────────────────────────────

        private static readonly string[] QuotationDetailsSelectors =
        [
            "internal:role=link[name=\"Offers\"i]",
            "internal:role=link[name=\"Details\"i]"
        ];

        private static readonly string[] OffersTabSelectors =
        [
            "internal:role=link[name=\"Offers\"i]"
        ];

        private static readonly string[] OffersListSelectors =
        [
            "internal:role=heading[name=\"Instant Ocean Quotation\"i]",
            "qwyk-quotation-offer-card"
        ];

        private static readonly string[] BookNowButtonSelectors =
        [
            "internal:role=button[name=\"Book now\"i]",
            "qwyk-quotation-offer-card >> internal:role=button"
        ];

        private static readonly string[] ConfirmDialogSelectors =
        [
            "internal:role=textbox[name=\"Give your booking a name or\"i]",
            "internal:role=button[name=\"Confirm\"i]"
        ];

        private static readonly string[] ConfirmButtonSelectors =
        [
            "internal:role=button[name=\"Confirm\"i]"
        ];

        private static readonly string[] ShipmentDetailsSelectors =
        [
            "internal:role=button[name=\"Send booking\"i]",
            "internal:role=button[name=\"Edit\"i]",
            "//button[normalize-space(text())='Send booking']",
            "fa-icon:has(svg[data-icon='rss'])",
            "//fa-icon[.//svg[@data-icon='rss']]",
            "svg[data-icon='rss']",
            "svg.fa-rss",
            "//svg[@data-icon='rss']"
        ];

        private static readonly string[] ShipmentNameInputSelectors =
        [
            "internal:role=textbox[name=\"Give your booking a name or\"i]"
        ];

        private static readonly string[] SaveButtonSelectors =
        [
            "internal:role=button[name=\"Save\"i]",
            "//button[normalize-space(text())='Save']"
        ];

        private static readonly string[] SendBookingButtonSelectors =
        [
            "internal:role=button[name=\"Send booking\"i]"
        ];

        private static readonly string[] BookingConfirmationSelectors =
        [
            "internal:text=\"Your booking has been sent.\"i"
        ];

        private static readonly string[] GoToShipmentButtonSelectors =
        [
            "internal:role=button[name=\"Go to shipment\"i]"
        ];

        // ── Tag Selectors ─────────────────────────────────────────────────────────
        // Actual element:
        // <button class="p-element rounded-circle btn btn-primary btn-sm plus-icon">
        //   <svg id="mdi-tag-plus" .../>
        // </button>

        // CSS-only — safe to use in Page.Locator() and card.Locator()
        private static readonly string[] TagIconSelectors =
        [
            // Most specific first: both classes present (matches the actual rendered element)
            "button.plus-icon.rounded-circle",
            "//button[contains(@class,'plus-icon') and contains(@class,'rounded-circle')]",
            // Fallbacks
            "button:has(svg#mdi-tag-plus)",
            "//button[.//*[@id='mdi-tag-plus']]",
            "//svg[@id='mdi-tag-plus']/.."
        ];

        // Actual element:
        // <p-autocomplete ...>
        //   <input class="p-autocomplete-input" placeholder="Type to search" role="combobox" .../>
        // </p-autocomplete>
        private static readonly string[] TagInputFieldSelectors =
        [
            "input[placeholder='Type to search']",
            "p-autocomplete input",
            "input.p-autocomplete-input",
            "//p-autocomplete//input"
        ];

        // PrimeNG autocomplete dropdown items
        private static readonly string[] TagDropdownOptionSelectors =
        [
            "//li[@role='option'][1]",
            "ul.p-autocomplete-items li",
            "li.p-autocomplete-item",
            "//ul[contains(@class,'p-autocomplete-items')]//li",
            ".p-autocomplete-panel li"
        ];

        // Save button shown after selecting a tag
        // May render as <button type="submit">Save</button> or <button type="submit"><span>Save</span></button>
        private static readonly string[] TagSaveButtonSelectors =
        [
            "button:has-text('Save')",
            "[type='submit']:has-text('Save')",
            "internal:role=button[name='Save'i]",
            "//button[normalize-space()='Save' or .//text()[normalize-space()='Save']]",
            "//button[@type='submit' and normalize-space()='Save']"
        ];

        // ── ShipmentSearch Selectors ──────────────────────────────────────────────

        // Actual element: <span class="nav-link-text">Shipments</span>
        // Used both for initial navigation (dashboard → shipments) and post-search reload.
        private static readonly string[] ShipmentsNavLinkSelectors =
        [
            "#navSidebar >> internal:role=link[name=\"Shipments\"i]",
            "internal:role=link[name=\"Shipments\"i]",
            "//span[contains(@class,'nav-link-text') and normalize-space()='Shipments']",
            "a:has-text(\"Shipments\")"
        ];

        // Used to force a full page reload between retries: navigate away then back to Shipments
        private static readonly string[] WarehouseNavLinkSelectors =
        [
            "//a[.//span[normalize-space()='Warehouse']]"
        ];

        // Table view toggle button: <div class="p-element btn btn-outline-primary"><fa-icon data-icon="table">
        private static readonly string[] TableViewButtonSelectors =
        [
            "//div[contains(@class,'btn-outline-primary') and contains(@class,'p-element')]",
            "div.btn-outline-primary:has(svg[data-icon='table'])",
            "//div[contains(@class,'btn-outline-primary') and .//svg[@data-icon='table']]"
        ];

        // List view toggle button (active state has btn-primary class)
        private static readonly string[] ListViewButtonSelectors =
        [
            "//div[contains(@class,'btn-primary') and .//svg[@data-icon='list']]",
            "div.btn-primary:has(svg[data-icon='list'])",
            "//div[contains(@class,'btn-primary')]"
        ];

        // Empty results message shown when no shipments match the search criteria
        private static readonly string[] NoResultsMessageSelectors =
        [
            "//p[normalize-space()='You may also want to adjust your search criteria and try again.']",
            "p:has-text('You may also want to adjust your search criteria and try again.')"
        ];

        private static readonly string[] BookedQuotationLinkSelectors =
        [
            "qwyk-quotation-card:has-text('Booked') a",
            "qwyk-quotation-list-item:has-text('Booked') a",
            "//article[contains(., 'Booked')]//a",
            "//li[contains(., 'Booked')]//a",
            "//*[contains(@class,'card')][contains(., 'Booked')]//a",
            "//*[contains(@class,'item')][contains(., 'Booked')]//a",
            "//*[contains(@class,'row')][contains(., 'Booked')]//a",
            "//a[contains(@href,'/quotations/') and not(contains(@class,'nav-link'))]"
        ];

        private static readonly string[] DialogCloseButtonSelectors =
        [
            "button:has-text('Close')",
            "button[aria-label='Close']",
            "button:has(.pi-times)",
            "button:has(svg[data-icon='xmark'])"
        ];

        private static readonly string[] EditShipmentButtonSelectors =
        [
            "internal:role=button[name=\"Edit\"i]",
            "button:has(svg[data-icon='pen-to-square'])",
            "button:has(fa-icon svg[data-icon='pen-to-square'])",
            "//button[.//*[name()='svg' and @data-icon='pen-to-square']]",
            "//button[contains(@aria-label,'edit') or contains(@title,'Edit') or normalize-space(text())='Edit']"
        ];

        private static readonly string[] ShowMoreFiltersSelectors =
        [
            "internal:role=button[name='Show more'i]",
            "//button[contains(normalize-space(text()),'Show more') or contains(normalize-space(text()),'More filters')]",
            "a:has-text('Show more')",
            "//a[contains(normalize-space(text()),'Show more')]",
            "[data-testid='show-more-filters']"
        ];

        private static readonly string[] ShipmentReferenceInputSelectors =
        [
            "input[placeholder*='reference' i]",
            "//input[contains(@placeholder,'reference') or contains(@placeholder,'Reference')]",
            "//label[contains(text(),'Shipment Reference')]/..//input",
            "//label[contains(text(),'Reference')]/..//input",
            "[data-testid='shipment-reference-input']"
        ];

        private static readonly string[] SearchSubmitButtonSelectors =
        [
            "internal:role=button[name='Search'i]",
            "//button[normalize-space(text())='Search']",
            "button[type='submit']:has-text('Search')",
            "//button[@type='submit'][contains(normalize-space(text()),'Search')]"
        ];

        private static readonly string[] ResetFiltersButtonSelectors =
        [
            "internal:role=button[name='Reset'i]",
            "//button[normalize-space(text())='Reset']",
            "button:has-text('Reset')",
            "//button[contains(normalize-space(text()),'Reset')]"
        ];

        private static readonly string[] FirstShipmentLinkSelectors =
        [
            "(//qwyk-shipment-list-item)[1]//a",
            "(//article[contains(@class,'shipment') or contains(@class,'card')])[1]//a",
            "(//a[contains(@href,'/shipments/')])[1]"
        ];

        private static readonly string[] DisabledTagIconSelectors =
        [
            "//button[contains(@class,'plus-icon') and @disabled]",
            "button.plus-icon[disabled]"
        ];

        private static readonly string[] TooltipSelectors =
        [
            ".tooltip-inner",
            ".p-tooltip-text",
            "[role='tooltip']",
            ".tippy-content",
            ".tooltip"
        ];
        //////////Purchase Order selectors/////////////
        private static readonly string[] POSectionInSHSelectors =
        [
            "//div[contains(@class,'card-header')]//h5[normalize-space()='Purchase Orders']",
            "//h5[normalize-space()='Purchase Orders']"
        ];

         private static readonly string[] POInProgress =
        [
            "//span[contains(@class,'badge') and normalize-space()='In Progress']"
        ];

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Waits for the p-autocomplete tag input to exist in the DOM (BasePage polling)
        /// and then to be visible (Playwright WaitForAsync). Returns the ready locator.
        /// </summary>
        private async Task<ILocator> WaitForTagInputAsync()
        {
            // FindLocatorAsync polls every 250ms via BasePage until the element is in the DOM
            var tagInput = await FindLocatorAsync(TagInputFieldSelectors, timeoutMs: 15000);
            // Then wait for it to be visible and enabled before interacting
            await WaitForEnabledAsync(tagInput, timeoutMs: 10000);
            return tagInput;
        }

        /// <summary>
        /// Full tag-adding flow: type tagName → select from PrimeNG dropdown → click Save if present.
        /// For new tags: Save button appears and must be clicked.
        /// For existing tags selected from the dropdown: tag auto-applies, Save button does not appear.
        /// </summary>
        private async Task AddTagInternalAsync(string tagName)
        {
            var tagInput = await WaitForTagInputAsync();
            await TypeAsync(tagInput, tagName);

            // Allow the PrimeNG autocomplete panel to render suggestions
            await Page.WaitForTimeoutAsync(800);

            var option = await TryFindLocatorAsync(TagDropdownOptionSelectors, timeoutMs: 5000);
            if (option != null)
                await ClickAsync(option);
            else
                await tagInput.PressAsync("Enter");

            // Save button appears only when creating a new tag; existing tags auto-apply after selection
            var saveButton = await TryFindLocatorAsync(TagSaveButtonSelectors, timeoutMs: 3000);
            if (saveButton != null)
                await ClickAndWaitForNetworkAsync(saveButton);
            else
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public string GetShipmentName() => _shipmentName;

        public void SetShipmentName(string name) => _shipmentName = name;

        // ── Quotation / Shipment creation methods ─────────────────────────────────

        public async Task IAmOnTheQuotationsListPage()
        {
            var baseUrl = GetPortalBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/quotations");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IOpenTheFirstQuotationInStatusBooked()
        {
            await DismissBlockingDialogIfPresentAsync();
            var quotationLink = await FindLocatorAsync(BookedQuotationLinkSelectors);
            await ClickAndWaitForNetworkAsync(quotationLink);
        }

        private async Task DismissBlockingDialogIfPresentAsync()
        {
            var dialogMask = Page.Locator(".p-dialog-mask, .p-component-overlay, p-dynamicdialog");
            if (await dialogMask.CountAsync() == 0)
                return;

            // Try ESC first
            await Page.Keyboard.PressAsync("Escape");
            try
            {
                await dialogMask.First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = 5000
                });
                return;
            }
            catch (TimeoutException)
            {
                // fall through
            }

            // Try clicking a close button if present
            var closeBtn = await TryFindLocatorAsync(DialogCloseButtonSelectors, timeoutMs: 2000);
            if (closeBtn != null)
                await ClickAsync(closeBtn);

            try
            {
                await dialogMask.First.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Hidden,
                    Timeout = 5000
                });
            }
            catch (TimeoutException)
            {
                // ignore and continue
            }
        }

        public async Task IShouldBeOnTheQuotationDetailsPage()
        {
            await Page.WaitForURLAsync(url => url.Contains("/quotations/"),
                new PageWaitForURLOptions { Timeout = 15000 });

            Assert.Contains("/quotations/", Page.Url,
                $"Expected to be on a Quotation Details page but current URL was: {Page.Url}");

            var offersTab = await TryFindLocatorAsync(QuotationDetailsSelectors, timeoutMs: 10000);
            Assert.IsNotNull(offersTab,
                $"Quotation Details page did not load: Offers/Details tab not found. URL: {Page.Url}");
        }

        public async Task IClickTheOffersButton()
        {
            var offersTab = await FindLocatorAsync(OffersTabSelectors);
            await ClickAndWaitForNetworkAsync(offersTab);
        }

        public async Task TheListOfTheOffersShouldAppear()
        {
            var offersCard = await TryFindLocatorAsync(OffersListSelectors, timeoutMs: 15000);
            Assert.IsNotNull(offersCard,
                "Offers list did not appear after clicking the Offers tab.");
        }

        public async Task ClicksOnBookNowButton()
        {
            var bookNowButton = await FindLocatorAsync(BookNowButtonSelectors);
            await ClickAsync(bookNowButton);
        }

        public async Task AConfirmationDialogShouldAppear()
        {
            var dialog = await TryFindLocatorAsync(ConfirmDialogSelectors, timeoutMs: 10000);
            Assert.IsNotNull(dialog,
                "Confirmation dialog did not appear after clicking Book now.");
        }

        public async Task IConfirmTheShipmentCreation()
        {
            var confirmButton = await FindLocatorAsync(ConfirmButtonSelectors);
            await ClickAndWaitForNetworkAsync(confirmButton);
        }

        public async Task IShouldBeOnTheShipmentDetailsPage()
        {
            await Page.WaitForURLAsync(
                url => url.Contains("/booking/new/") || url.Contains("/shipments/"),
                new PageWaitForURLOptions { Timeout = 20000 });

            var currentUrl = Page.Url;
            Assert.IsTrue(
                currentUrl.Contains("/booking/new/") || currentUrl.Contains("/shipments/"),
                $"Expected to be on a Shipment Details page but current URL was: {currentUrl}");

            var details = await TryFindLocatorAsync(ShipmentDetailsSelectors, timeoutMs: 15000);
            Assert.IsNotNull(details,
                $"Shipment Details page did not load after confirming the booking. URL: {currentUrl}");
        }

        public async Task IClickOnEditButtonToEditTheShipmentName()
        {
            var editButton = await FindLocatorAsync(EditShipmentButtonSelectors);
            await ClickAsync(editButton);
        }

        public async Task IShouldEditTheShipmentName()
        {
            _shipmentName = $"AutoShipment-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var nameInput = await TryFindLocatorAsync(ShipmentNameInputSelectors, timeoutMs: 10000);
            Assert.IsNotNull(nameInput,
                "Shipment name input field was not found after clicking Edit.");

            await TypeAsync(nameInput, _shipmentName);

            var filledValue = await nameInput.InputValueAsync();
            Assert.AreEqual(_shipmentName, filledValue,
                $"Shipment name input was not filled correctly. Expected: '{_shipmentName}', Got: '{filledValue}'");
        }

        public async Task IClickOnSaveButton()
        {
            var saveButton = await FindLocatorAsync(SaveButtonSelectors);
            await ClickAndWaitForNetworkAsync(saveButton);
        }

        public async Task IShouldSeeTheNewShipmentName()
        {
            var nameVisible = await TryFindLocatorAsync(
            [
                $"internal:text=\"{_shipmentName}\"i",
                $"//*[contains(text(),'{_shipmentName}')]"
            ], timeoutMs: 10000);
            Assert.IsNotNull(nameVisible,
                $"New shipment name '{_shipmentName}' was not visible on the page after saving.");
        }

        public async Task IClickOnSendBookingButton()
        {
            var sendBookingButton = await FindLocatorAsync(SendBookingButtonSelectors);
            await ClickAndWaitForNetworkAsync(sendBookingButton);
        }

        public async Task IShouldClickOnGoToShipmentButtonToSeeTheShipment()
        {
            var confirmation = await TryFindLocatorAsync(BookingConfirmationSelectors, timeoutMs: 15000);
            Assert.IsNotNull(confirmation,
                "Booking confirmation message 'Your booking has been sent.' was not displayed.");

            var goToShipmentButton = await TryFindLocatorAsync(GoToShipmentButtonSelectors, timeoutMs: 5000);
            Assert.IsNotNull(goToShipmentButton,
                "'Go to shipment' button was not found after the booking confirmation.");

            await ClickAsync(goToShipmentButton);
        }

        public async Task TheShipmentShouldDisplayTheShipmentName()
        {
            var nameDisplayed = await TryFindLocatorAsync(
            [
                $"internal:text=\"{_shipmentName}\"i",
                $"//*[contains(text(),'{_shipmentName}')]"
            ], timeoutMs: 15000);
            Assert.IsNotNull(nameDisplayed,
                $"Shipment name '{_shipmentName}' was not displayed on the Shipment Details page. URL: {Page.Url}");
        }

        // ── ShipmentSearch methods ────────────────────────────────────────────────

        public async Task UserNavigatedToShipmentsList()
        {
            // Replicates "Open shipments from dashboard": go to dashboard, then click Shipments nav link
            var baseUrl = GetPortalBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/dashboard?view=ops");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var shipmentsNavLink = await FindLocatorAsync(ShipmentsNavLinkSelectors);
            await ClickAndWaitForNetworkAsync(shipmentsNavLink);
        }

        public async Task IClickOnShowMoreFilters()
        {
            // Ensure the Shipments list page has fully loaded before trying to interact with filters.
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var showMore = await FindLocatorAsync(ShowMoreFiltersSelectors);
            // ClickAsync: "Show more filters" is a UI toggle — no API call, no navigation.
            // ClickAndWaitForNetworkAsync would resolve NetworkIdle before the filter panel renders.
            await ClickAsync(showMore);

            // Assert the filter panel actually expanded: the Shipment Reference input must appear.
            var referenceInput = await TryFindLocatorAsync(ShipmentReferenceInputSelectors, timeoutMs: 10000);
            Assert.IsNotNull(referenceInput,
                $"'Show More Filters' was clicked but the Shipment Reference input did not appear — the filter panel may not have expanded. URL: {Page.Url}");
        }

        public async Task IEnterShipmentNameInShipmentReferenceField()
        {
            var referenceInput = await FindLocatorAsync(ShipmentReferenceInputSelectors);
            await WaitForEnabledAsync(referenceInput);
            if (string.IsNullOrWhiteSpace(_shipmentName))
                _shipmentName = $"NoSuchShipment-{DateTime.UtcNow:yyyyMMddHHmmss}";
            await TypeAsync(referenceInput, _shipmentName);

            // Assert the value was actually typed — detects cases where the field was read-only or wrong.
            var typed = await referenceInput.InputValueAsync();
            Assert.IsTrue(typed.Contains(_shipmentName, StringComparison.OrdinalIgnoreCase),
                $"Shipment name was not typed correctly into the Shipment Reference field. Expected: '{_shipmentName}', Actual: '{typed}'. URL: {Page.Url}");
        }

        public async Task IClickOnSearchButton()
        {
            var searchButton = await FindLocatorAsync(SearchSubmitButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);

            // After filtering on the Shipments list, click the nav link to reload the filtered list.
            // Skip when called from /my-portal/reports/shipments — that URL also contains "shipments"
            // but belongs to the Reports section and must not navigate away.
            if (Page.Url.Contains("/my-portal/shipments") && !Page.Url.Contains("/reports"))
            {
                var shipmentsNavLink = await TryFindLocatorAsync(ShipmentsNavLinkSelectors, timeoutMs: 5000);
                if (shipmentsNavLink != null)
                    await ClickAndWaitForNetworkAsync(shipmentsNavLink);
            }
        }

        public async Task TheShipmentShouldAppearInSearchResults()
        {
            const int maxRetries = 5;
            const int retryDelayMs = 4000;

            string[] resultSelectors =
            [
                $"internal:text=\"{_shipmentName}\"i",
                $"//*[contains(text(),'{_shipmentName}')]"
            ];

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var result = await TryFindLocatorAsync(resultSelectors, timeoutMs: 5000);
                if (result != null)
                    return;

                if (attempt < maxRetries)
                {
                    await Page.WaitForTimeoutAsync(retryDelayMs);

                    // Navigate Warehouse → Shipments to force a full list reload
                    var warehouseLink = await TryFindLocatorAsync(WarehouseNavLinkSelectors, timeoutMs: 5000);
                    if (warehouseLink != null)
                        await ClickAndWaitForNetworkAsync(warehouseLink);

                    var shipmentsNavLink = await TryFindLocatorAsync(ShipmentsNavLinkSelectors, timeoutMs: 5000);
                    if (shipmentsNavLink != null)
                        await ClickAndWaitForNetworkAsync(shipmentsNavLink);

                    var searchButton = await TryFindLocatorAsync(SearchSubmitButtonSelectors, timeoutMs: 3000);
                    if (searchButton != null)
                        await ClickAndWaitForNetworkAsync(searchButton);
                }
            }

            Assert.Fail($"Shipment '{_shipmentName}' was not found in the search results after {maxRetries} attempts ({maxRetries * retryDelayMs / 1000}s total).");
        }

        // ── Tag methods ───────────────────────────────────────────────────────────

        public async Task ATagIconShouldBeDisplayed()
        {
            var tagIcon = await TryFindLocatorAsync(TagIconSelectors, timeoutMs: 10000);
            Assert.IsNotNull(tagIcon,
                "Tag icon was not displayed below the shipment name or on the left side of existing tags.");
        }

        public async Task TheTagIconTooltipShouldSay(string expectedTooltip)
        {
            var tagIcon = await FindLocatorAsync(TagIconSelectors);
            await HoverAsync(tagIcon);

            // Wait for the tooltip DOM element to render
            await Page.WaitForTimeoutAsync(600);

            var tooltipEl = await TryFindLocatorAsync(TooltipSelectors, timeoutMs: 5000);
            if (tooltipEl != null)
            {
                var tooltipText = await tooltipEl.InnerTextAsync();
                Assert.IsTrue(tooltipText.Contains(expectedTooltip, StringComparison.OrdinalIgnoreCase),
                    $"Tooltip text mismatch. Expected to contain: '{expectedTooltip}', Got: '{tooltipText}'");
                return;
            }

            // Fallback: check title / ngbTooltip / aria-label attributes on the button
            var title = await GetAttributeAsync(tagIcon, "title")
                        ?? await GetAttributeAsync(tagIcon, "ngbtooltip")
                        ?? await GetAttributeAsync(tagIcon, "aria-label")
                        ?? string.Empty;

            Assert.IsTrue(title.Contains(expectedTooltip, StringComparison.OrdinalIgnoreCase),
                $"Tag icon tooltip not found or mismatch. Expected: '{expectedTooltip}', Got attribute: '{title}'");
        }

        public async Task UserClicksTheTagIcon()
        {
            // The tag button HTML: <button class="p-element rounded-circle btn btn-primary btn-sm plus-icon">
            //                        <svg id="mdi-tag-plus" .../>
            //                      </button>
            // svg[@id='mdi-tag-plus'] is the unique identifier. Class-only selectors are unreliable
            // because @class='row' matches Bootstrap grid rows including the filter form area,
            // causing the selector to scope to the filter panel when the shipment name is also
            // present in the search input field at the top of the page.
            // Scope to qwyk-shipment-list-item (the Angular component) or article — never to 'row'.
            string[] scopedSelectors =
            [
                // 1. Angular list-item component scoped + SVG id (most specific — avoids filter area)
                $"//qwyk-shipment-list-item[contains(normalize-space(),'{_shipmentName}')]//button[./svg[@id='mdi-tag-plus']]",
                // 2. Article element scoped + SVG id
                $"//article[contains(normalize-space(),'{_shipmentName}')]//button[./svg[@id='mdi-tag-plus']]",
                // 3. Unscoped but identified by unique SVG id (btn-primary = enabled state)
                "//button[contains(@class,'plus-icon') and ./svg[@id='mdi-tag-plus']]",
                // 4. CSS fallback using :has() with SVG id
                "button:has(svg#mdi-tag-plus)",
            ];

            var tagIcon = await FindLocatorAsync(scopedSelectors);

            // When 5 tags already exist the button is rendered as disabled.
            // Clicking a disabled button times out in Playwright (actionability check).
            // The tooltip verification is handled by TheSystemShouldShowTheMaxTagsError.
            var disabledAttr = await tagIcon.GetAttributeAsync("disabled");
            if (disabledAttr == null)
            {
                await WaitForEnabledAsync(tagIcon);
                await ClickAsync(tagIcon);
            }
        }

        public async Task ATagInputFieldShouldAppear()
        {
            // Reuses WaitForTagInputAsync: polls DOM (BasePage) + waits for visibility (Playwright)
            await WaitForTagInputAsync();
        }

        /// <summary>
        /// Creates a new auto-generated tag and assigns it to the current shipment.
        /// Stores the tag name in _tagName for reuse in subsequent steps.
        /// </summary>
        public async Task UserCreatesAndAssignsNewTag(string tagName)
        {
            _tagName = tagName;
            _allTagNames.Add(tagName);
            await AddTagInternalAsync(_tagName);
        }

        /// <summary>
        /// Assigns the previously created tag (_tagName) to the current shipment.
        /// Reuses AddTagInternalAsync — same flow, existing tag name.
        /// </summary>
        public async Task UserAssignsExistingTagToShipment()
        {
            await AddTagInternalAsync(_tagName);
        }

        public async Task TheTagShouldBeVisibleOnTheShipment()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                var tagVisible = await TryFindLocatorAsync(
                [
                    $"internal:text=\"{_tagName}\"i",
                    $"//*[contains(text(),'{_tagName}')]",
                    $"[class*='tag']:has-text('{_tagName}')",
                    $"span[class*='badge']:has-text('{_tagName}')"
                ], timeoutMs: 8000);

                if (tagVisible != null)
                    return;

                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Page.WaitForTimeoutAsync(1500);
            }

            Assert.Fail($"Tag '{_tagName}' was not visible on the selected shipment.");
        }

        public async Task UserOpensTaggedShipmentDetailsView()
        {
            var shipmentLink = await FindLocatorAsync(
            [
                $"//*[contains(text(),'{_shipmentName}')]/ancestor::*[contains(@class,'card') or contains(@class,'item')]//a",
                $"//*[contains(text(),'{_shipmentName}')]/ancestor::article//a",
                "//a[contains(@href,'/shipments/')]"
            ]);
            await ClickAndWaitForNavigationAsync(shipmentLink);
        }

        public async Task TheTagShouldBeVisibleInShipmentDetails()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var tagInDetails = await TryFindLocatorAsync(
            [
                $"internal:text=\"{_tagName}\"i",
                $"//*[contains(text(),'{_tagName}')]",
                $"[class*='tag']:has-text('{_tagName}')"
            ], timeoutMs: 15000);
            Assert.IsNotNull(tagInDetails,
                $"Tag '{_tagName}' was not visible in the Shipment Details view. URL: {Page.Url}");
        }

        public async Task TheTagShouldBeVisibleInShipmentListView()
        {
            var tagInList = await TryFindLocatorAsync(
            [
                $"//span[contains(@class,'status-badge') and contains(normalize-space(),'{_tagName}')]",
                $"span.status-badge:has-text('{_tagName}')"
            ], timeoutMs: 15000);
            Assert.IsNotNull(tagInList,
                $"Tag '{_tagName}' was not visible in Shipment List view.");
        }

        public async Task TheTagShouldBeVisibleInShipmentTableView()
        {
            var tableViewBtn = await FindLocatorAsync(TableViewButtonSelectors);
            await ClickAndWaitForNetworkAsync(tableViewBtn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var tagInTable = await TryFindLocatorAsync(
            [
                $"//span[contains(@class,'status-badge') and contains(normalize-space(),'{_tagName}')]",
                $"span.status-badge:has-text('{_tagName}')"
            ], timeoutMs: 15000);
            Assert.IsNotNull(tagInTable,
                $"Tag '{_tagName}' was not visible in Shipment Table view.");
        }

        public async Task IResetSearchFilters()
        {
            var resetButton = await FindLocatorAsync(ResetFiltersButtonSelectors);
            await ClickAndWaitForNetworkAsync(resetButton);
        }

        public async Task IClickOnListViewButton()
        {
            var listViewBtn = await FindLocatorAsync(ListViewButtonSelectors);
            await ClickAndWaitForNetworkAsync(listViewBtn);

            Assert.Contains("/my-portal/shipments", Page.Url,
                $"After clicking List View, the page navigated away from the shipments list. URL: {Page.Url}");
        }

        public async Task IClickOnTableViewButton()
        {
            var tableViewBtn = await FindLocatorAsync(TableViewButtonSelectors);
            await ClickAndWaitForNetworkAsync(tableViewBtn);

            Assert.Contains("/my-portal/shipments", Page.Url,
                $"After clicking Table View, the page navigated away from the shipments list. URL: {Page.Url}");
        }

        public async Task TheShipmentShouldNotAppearInSearchResults()
        {
            var noResults = await TryFindLocatorAsync(NoResultsMessageSelectors, timeoutMs: 15000);
            Assert.IsNotNull(noResults,
                $"Expected no results message but it was not displayed after searching for '{_shipmentName}'.");
        }

        public async Task TheTagShouldBeVisibleOn2ShipmentsInListView()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var tagBadges = Page.Locator($"span.status-badge:has-text('{_tagName}')");
            var count = await tagBadges.CountAsync();
            Assert.IsGreaterThanOrEqualTo(count, 2,
                $"Expected tag '{_tagName}' on at least 2 shipments in List view, but found {count} shipment(s) with this tag.");
        }

        public async Task TheTagShouldBeVisibleOn2ShipmentsInTableView()
        {
            var tableViewBtn = await FindLocatorAsync(TableViewButtonSelectors);
            await ClickAndWaitForNetworkAsync(tableViewBtn);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var tagBadges = Page.Locator($"span.status-badge:has-text('{_tagName}')");
            var count = await tagBadges.CountAsync();
            Assert.IsGreaterThanOrEqualTo(count, 2,
                $"Expected tag '{_tagName}' on at least 2 shipments in Table view, but found {count} row(s) with this tag.");
        }

        // ── @9344 methods ─────────────────────────────────────────────────────────

        /// <summary>
        /// Identifies the first shipment in the list and stores its name in _shipmentName
        /// so that subsequent steps (open details, verify tags) can reference it.
        /// Also resets _allTagNames ready for a fresh tag-collection cycle.
        /// </summary>
        public async Task UserSelectsFirstShipmentFromList()
        {
            _allTagNames.Clear();

            var firstShipmentLink = await TryFindLocatorAsync(FirstShipmentLinkSelectors, timeoutMs: 10000);

            Assert.IsNotNull(firstShipmentLink,
                "No shipments were found in the Shipments List.");

            var name = (await firstShipmentLink.InnerTextAsync()).Trim();
            if (!string.IsNullOrEmpty(name))
                _shipmentName = name;
        }

        /// <summary>
        /// When 5 tags exist the tag button is disabled (pointer-events:none).
        /// Playwright's actionability check would timeout on a plain HoverAsync,
        /// so we use Force=true to bypass it and trigger the tooltip.
        /// </summary>
        public async Task TheSystemShouldShowTheMaxTagsError(string expectedError)
        {
            // Target the disabled button specifically to avoid matching an enabled button
            var disabledTagIcon = await FindLocatorAsync(DisabledTagIconSelectors);

            // Force=true skips actionability checks (visible, stable, receives-events)
            // which would fail because Angular sets pointer-events:none on disabled buttons
            await disabledTagIcon.HoverAsync(new LocatorHoverOptions { Force = true });

            // Allow the tooltip to render
            await Page.WaitForTimeoutAsync(600);

            var tooltip = await TryFindLocatorAsync(TooltipSelectors, timeoutMs: 5000);

            if (tooltip != null)
            {
                var tooltipText = await tooltip.InnerTextAsync();
                Assert.IsTrue(
                    tooltipText.Contains('5') ||
                    tooltipText.Contains("limit", StringComparison.OrdinalIgnoreCase) ||
                    tooltipText.Contains("maximum", StringComparison.OrdinalIgnoreCase),
                    $"Tooltip did not mention the 5-tag limit. Expected something like '{expectedError}', Got: '{tooltipText}'");
                return;
            }

            // Fallback: check title / ngbTooltip / aria-label attributes on the button
            var attrText = await GetAttributeAsync(disabledTagIcon, "title")
                           ?? await GetAttributeAsync(disabledTagIcon, "ngbtooltip")
                           ?? await GetAttributeAsync(disabledTagIcon, "aria-label")
                           ?? string.Empty;

            Assert.IsFalse(string.IsNullOrEmpty(attrText),
                $"Expected a tooltip on the disabled tag icon (5-tag limit reached), but none was found. Expected: '{expectedError}'");
            Assert.IsTrue(
                attrText.Contains('5') ||
                attrText.Contains("limit", StringComparison.OrdinalIgnoreCase) ||
                attrText.Contains("maximum", StringComparison.OrdinalIgnoreCase),
                $"Tag icon attribute tooltip did not mention the limit. Expected: '{expectedError}', Got: '{attrText}'");
        }

        /// <summary>
        /// Verifies every tag in _allTagNames is shown as a badge in the Shipment List view.
        /// </summary>
        public async Task AllCreatedTagsShouldBeVisibleInShipmentListView()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            foreach (var tag in _allTagNames)
            {
                var tagBadge = await TryFindLocatorAsync(
                [
                    $"//span[contains(@class,'status-badge') and contains(normalize-space(),'{tag}')]",
                    $"span.status-badge:has-text('{tag}')"
                ], timeoutMs: 10000);
                Assert.IsNotNull(tagBadge,
                    $"Tag '{tag}' was not visible in Shipment List view.");
            }
        }

        /// <summary>
        /// Verifies every tag in _allTagNames is shown on the current Shipment Details page.
        /// </summary>
        public async Task AllCreatedTagsShouldBeVisibleInShipmentDetails()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            foreach (var tag in _allTagNames)
            {
                var tagInDetails = await TryFindLocatorAsync(
                [
                    $"internal:text=\"{tag}\"i",
                    $"//*[contains(text(),'{tag}')]",
                    $"[class*='tag']:has-text('{tag}')"
                ], timeoutMs: 10000);
                Assert.IsNotNull(tagInDetails,
                    $"Tag '{tag}' was not visible in Shipment Details view. URL: {Page.Url}");
            }
        }

        /// <summary>
        /// Navigates to the Shipments List, switches to Table view,
        /// then verifies every tag in _allTagNames is visible as a badge.
        /// </summary>
        public async Task AllCreatedTagsShouldBeVisibleInShipmentTableView()
        {
           // var baseUrl = GetPortalBaseUrl();
          //  await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/shipments");
         //   await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Select Table View to set the preference
            var tableViewBtn = await FindLocatorAsync(TableViewButtonSelectors);
            await ClickAndWaitForNetworkAsync(tableViewBtn);

            // Navigate away then back so the page reloads directly in Table View
           // await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/cargo-detail");

           // await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/shipments");
           // await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Wait for "Loading default view..." spinner to disappear
            await Page.Locator("text=Loading default view").WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Hidden,
                Timeout = 280000
            });

            foreach (var tag in _allTagNames)
            {
                var tagBadge = await TryFindLocatorAsync(
                [
                    $"//span[contains(@class,'status-badge') and normalize-space()='{tag}']",
                    $"span.status-badge:has-text('{tag}')"
                ], timeoutMs: 10000);
                Assert.IsNotNull(tagBadge,
                    $"Tag '{tag}' was not visible in Shipment Table view.");
            }
        }

        /////////////ADDITIONALS STEPS FOR 7873 LINK PO WITH SHIPMENT DETAILS PAGE
        /// 
        public async Task IClickOnBookingDetailsTab()
        {
            var locator = await FindLocatorAsync(new[] { "internal:role=link[name=\"Booking Details\"i]" });
            await ClickAndWaitForNetworkAsync(locator);
        }

        public async Task IShouldSeeThePurchaseOrderSectionInTheShipmentPortal()
        {
            var poSection = await TryFindLocatorAsync(POSectionInSHSelectors, timeoutMs: 10000);
            Assert.IsNotNull(poSection,
                $"Purchase Orders section was not visible in Shipment Portal view. URL: {Page.Url}");
        }

        public async Task IClickOnPurchaseOrderLink(string purchaseOrderId)
        {
            string[] selectors = string.IsNullOrWhiteSpace(purchaseOrderId)
                ? [
                    "//a[contains(@href,'/orders/')]",
                    "//a[contains(@href,'/purchase-orders/')]"
                  ]
                : [
                    $"a[href='/my-portal/orders/{purchaseOrderId}']",
                    $"//a[contains(@href,'{purchaseOrderId}')]"
                  ];

            var locator = await FindLocatorAsync(selectors);
            await ClickAndWaitForNavigationAsync(locator);
        }

        public async Task IShouldBeOnThePurchaseOrderDetails(string purchaseOrderId)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // 1) Verify the "Buyer" label is visible on the PO Details page
            var buyerLabel = await TryFindLocatorAsync(
            [
                "label.font-weight-bold:has-text('Buyer')",
                "//label[contains(@class,'font-weight-bold') and normalize-space()='Buyer']"
            ], timeoutMs: 10000);
            Assert.IsNotNull(buyerLabel,
                $"'Buyer' label was not visible on the Purchase Order Details page. URL: {Page.Url}");

            // 2) Verify the PO link with the dynamic order id is visible
            if (!string.IsNullOrWhiteSpace(purchaseOrderId))
            {
                var orderLink = await TryFindLocatorAsync(
                [
                    $"a[href='/my-portal/orders/{purchaseOrderId}']",
                    $"//a[contains(@href,'{purchaseOrderId}')]"
                ], timeoutMs: 10000);
                Assert.IsNotNull(orderLink,
                    $"Purchase Order link for id '{purchaseOrderId}' was not visible on the PO Details page. URL: {Page.Url}");
            }
        }

        public async Task IShouldSeeTheStatusOfThePOInProgress()
        {
            var statusPO = await TryFindLocatorAsync(POInProgress, timeoutMs: 10000);
            Assert.IsNotNull(statusPO,
                $"Status 'In Progress' was not visible on the Purchase Order Details page. URL: {Page.Url}");
        }

        public async Task IShouldSeeBookedShipmentsSectionInThePurchaseOrder()
        {
            await FindLocatorAsync(new[] { "internal:role=heading[name=\"Booked Shipments\"i]" });
        }

        public async Task IClickOnTheShipmentNameLink(string shipmentId)
        {
            // HTML: <a href="/my-portal/shipments/{shipmentId}"> {_shipmentName} - {_shipmentName} </a>
            string[] selectors = !string.IsNullOrWhiteSpace(shipmentId)
                ? [
                    $"a[href='/my-portal/shipments/{shipmentId}']",
                    $"//a[@href='/my-portal/shipments/{shipmentId}']",
                    $"//a[contains(@href,'{shipmentId}')]"
                  ]
                : [
                    $"//a[contains(@href,'/shipments/') and contains(normalize-space(),'{_shipmentName}')]",
                    "//a[contains(@href,'/my-portal/shipments/')]"
                  ];

            var locator = await FindLocatorAsync(selectors);
            await ClickAndWaitForNavigationAsync(locator);
        }

        public async Task IClickOnTheShipmentAsync()
        {
            var shipmentLink = await FindLocatorAsync(
            [
                $"//div[contains(@class,'h4')][.//span[contains(normalize-space(),'{_shipmentName}')]]",
                $"//qwyk-shipment-list-item[contains(normalize-space(),'{_shipmentName}')]//div[contains(@class,'h4')]",
                $"//span[contains(normalize-space(),'{_shipmentName}')]"
            ]);
            var currentUrl = Page.Url;
            await ClickAsync(shipmentLink);
            await Page.WaitForURLAsync(
                url => url != currentUrl && url.Contains("/shipments/"),
                new PageWaitForURLOptions { Timeout = 15000 });
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IClickOnCargoSectionWithPO(string purchaseOrderId)
        {
    Assert.IsFalse(string.IsNullOrWhiteSpace(purchaseOrderId),
        "purchaseOrderId cannot be null or empty.");

    var orderLink = await TryFindLocatorAsync(
    [
        $"a[href='/my-portal/orders/{purchaseOrderId}']:has-text('P/O')"
    ], timeoutMs: 10000);

    Assert.IsNotNull(orderLink,
        $"Purchase Order link for id '{purchaseOrderId}' was not visible on the PO Details page. URL: {Page.Url}");

    await orderLink.ClickAsync();
        }

        /// <summary>
        /// Verifies that the last column (Shipment / Status) of the order lines table
        /// contains an anchor whose href includes the shipmentId and whose text contains the shipmentName.
        /// HTML: <a href="/my-portal/shipments/{shipmentId}"> {shipmentName} - {shipmentName} - Booked </a>
        /// </summary>
        public async Task VerifyShipmentLinkInOrderLineAsync(string shipmentId, string shipmentName)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var shipmentLink = await TryFindLocatorAsync(
            [
                $"tbody tr td:last-child a[href*='/my-portal/shipments/{shipmentId}']",
                $"//tbody//tr//td[last()]//a[contains(@href,'/my-portal/shipments/{shipmentId}')]"
            ], timeoutMs: 10000);

            Assert.IsNotNull(shipmentLink,
                $"Shipment link for id '{shipmentId}' was not found in the last column of the order lines table. URL: {Page.Url}");

            var href = await GetAttributeAsync(shipmentLink, "href") ?? "";
            Assert.IsTrue(href.Contains($"/my-portal/shipments/{shipmentId}", StringComparison.OrdinalIgnoreCase),
                $"Shipment link href does not contain expected shipment id. Expected: '/my-portal/shipments/{shipmentId}', Got: '{href}'");

            var linkText = (await shipmentLink.InnerTextAsync()).Trim();
            Assert.IsTrue(linkText.Contains(shipmentName, StringComparison.OrdinalIgnoreCase),
                $"Shipment link text does not contain expected shipment name '{shipmentName}'. Got: '{linkText}'");
        }


    }
}
