using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ShipmentPage : BasePage
    {
        private string _shipmentName = string.Empty;

        public ShipmentPage(IPage page) : base(page)
        {
        }

        // codegen:selectors-start
        // Selectors captured by codegen for 'createshipmentfromquotation'
        public static readonly string[] Selectors = new string[]
        {
        };
        // codegen:selectors-end

        private static readonly string[] QuotationDetailsSelectors =
        {
            "internal:role=link[name=\"Offers\"i]",
            "internal:role=link[name=\"Details\"i]"
        };

        private static readonly string[] OffersTabSelectors =
        {
            "internal:role=link[name=\"Offers\"i]"
        };

        private static readonly string[] OffersListSelectors =
        {
            "internal:role=heading[name=\"Instant Ocean Quotation\"i]",
            "qwyk-quotation-offer-card"
        };

        private static readonly string[] BookNowButtonSelectors =
        {
            "internal:role=button[name=\"Book now\"i]",
            "qwyk-quotation-offer-card >> internal:role=button"
        };

        private static readonly string[] ConfirmDialogSelectors =
        {
            "internal:role=textbox[name=\"Give your booking a name or\"i]",
            "internal:role=button[name=\"Confirm\"i]"
        };

        private static readonly string[] ConfirmButtonSelectors =
        {
            "internal:role=button[name=\"Confirm\"i]"
        };

        private static readonly string[] ShipmentDetailsSelectors =
        {
            "internal:role=button[name=\"Send booking\"i]",
            "internal:role=button[name=\"Edit\"i]",
            "//button[normalize-space(text())='Send booking']"
        };

        private static readonly string[] ShipmentNameInputSelectors =
        {
            "internal:role=textbox[name=\"Give your booking a name or\"i]"
        };

        private static readonly string[] SaveButtonSelectors =
        {
            "internal:role=button[name=\"Save\"i]",
            "//button[normalize-space(text())='Save']"
        };

        private static readonly string[] SendBookingButtonSelectors =
        {
            "internal:role=button[name=\"Send booking\"i]"
        };

        private static readonly string[] BookingConfirmationSelectors =
        {
            "internal:text=\"Your booking has been sent.\"i"
        };

        private static readonly string[] GoToShipmentButtonSelectors =
        {
            "internal:role=button[name=\"Go to shipment\"i]"
        };

        public async Task IAmOnTheQuotationsListPage()
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "";
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/my-portal/quotations");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IOpenTheFirstQuotationInStatusBooked()
        {
            var quotationLink = await FindLocatorAsync(new[]
            {
                // Angular component selectors
                "qwyk-quotation-card:has-text('Booked') a",
                "qwyk-quotation-list-item:has-text('Booked') a",
                // Generic card/container selectors
                "//article[contains(., 'Booked')]//a",
                "//li[contains(., 'Booked')]//a",
                "//*[contains(@class,'card')][contains(., 'Booked')]//a",
                "//*[contains(@class,'item')][contains(., 'Booked')]//a",
                "//*[contains(@class,'row')][contains(., 'Booked')]//a",
                // Fallback: first link with quo in href
                "//a[contains(@href,'quo')]"
            });
            await ClickAndWaitForNetworkAsync(quotationLink);
        }

        public async Task IShouldBeOnTheQuotationDetailsPage()
        {
            await Page.WaitForURLAsync(url => url.Contains("/quotations/"),
                new PageWaitForURLOptions { Timeout = 15000 });

            Assert.IsTrue(
                Page.Url.Contains("/quotations/"),
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
            // After confirming a booking from a quotation, the portal navigates to
            // /my-portal/booking/new/<GUID> (Draft state). Existing shipments use /shipments/.
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
            var editButton = await FindLocatorAsync(new[]
            {
                "internal:role=button[name=\"Edit\"i]",
                "//button[contains(@aria-label,'edit') or contains(@title,'Edit') or normalize-space(text())='Edit']"
            });
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
            var nameVisible = await TryFindLocatorAsync(new[]
            {
                $"internal:text=\"{_shipmentName}\"i",
                $"//*[contains(text(),'{_shipmentName}')]"
            }, timeoutMs: 10000);
            Assert.IsNotNull(nameVisible,
                $"New shipment name '{_shipmentName}' was not visible on the page after saving.");
        }

        public async Task IClickOnSendBookingButton()
        {
            var sendBookingButton = await FindLocatorAsync(SendBookingButtonSelectors);
            await ClickAndWaitForNetworkAsync(sendBookingButton);
        }

        public async Task IShouldClickOnGoToShipmentButtonToSeeTheShipemnt()
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
            var nameDisplayed = await TryFindLocatorAsync(new[]
            {
                $"internal:text=\"{_shipmentName}\"i",
                $"//*[contains(text(),'{_shipmentName}')]"
            }, timeoutMs: 15000);
            Assert.IsNotNull(nameDisplayed,
                $"Shipment name '{_shipmentName}' was not displayed on the Shipment Details page. URL: {Page.Url}");
        }
    }
}
