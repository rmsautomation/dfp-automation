using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;
using DFP.Playwright.Helpers;

namespace DFP.Playwright.Pages.Web
{
    public sealed class ShipmentHubPage : BasePage
    {
        public ShipmentHubPage(IPage page) : base(page)
        {
        }

        private static string GetHubBaseUrl()
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.HUB_BASE_URL)
                          ?? Environment.GetEnvironmentVariable("BASE_URL")
                          ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("HUB_BASE_URL (or BASE_URL) is required.");
            return baseUrl;
        }

        private static readonly string[] HubCustomerReferenceInputSelectors =
        [
            "internal:role=textbox[name=\"Customer Reference\"i]",
            "input[placeholder*='customer reference' i]",
            "//input[contains(@placeholder,'customer reference') or contains(@placeholder,'Customer Reference')]"
        ];

        private static readonly string[] HubSearchButtonSelectors =
        [
            "internal:role=button[name=\"Search\"i]",
            "//button[normalize-space(text())='Search']",
            "button[type='submit']:has-text('Search')"
        ];

        /// <summary>
        /// Navigates directly to the Hub Shipments list via URL.
        /// GotoAsync + NetworkIdle is the correct pattern for cross-application navigation.
        /// </summary>
        public async Task INavigatedToShipmentListInTheHub()
        {
            var baseUrl = GetHubBaseUrl();
            await Page.GotoAsync(baseUrl.TrimEnd('/') + "/shipments");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Clicks the Shipment Reference input to focus it.
        /// ClickAsync is correct — this is a simple focus, no navigation or API call.
        /// </summary>
        public async Task IClickOnCustomerReferenceInputFieldInTheHub()
        {
            var customerRef = await FindLocatorAsync(HubCustomerReferenceInputSelectors);
            await ClickAsync(customerRef);
        }

        /// <summary>
        /// Types the shipment name (retrieved from the portal step via the step definition) into the Hub reference field.
        /// </summary>
        public async Task IEnterTheShipmentNameInCustomReferenceFieldInTheHub(string shipmentName)
        {
            var shipmentRefInput = await FindLocatorAsync(HubCustomerReferenceInputSelectors);
            await TypeAsync(shipmentRefInput, shipmentName);
        }

        /// <summary>
        /// Clicks the Search button in the Hub.
        /// ClickAndWaitForNetworkAsync is correct — search triggers an API call.
        /// </summary>
        public async Task IClickOnSearchButtonInTheHub()
        {
            var searchButton = await FindLocatorAsync(HubSearchButtonSelectors);
            await ClickAndWaitForNetworkAsync(searchButton);
        }

        /// <summary>
        /// Verifies the shipment appears in the Hub search results.
        /// Retries up to 3 times to handle delayed list rendering.
        /// </summary>
        public async Task TheShipmentShouldAppearInTheSearchResultsInTheHub(string shipmentName)
        {
            const int maxRetries = 3;
            const int retryDelayMs = 3000;

            var resultSelectors = new[]
            {
                $"internal:text=\"{shipmentName}\"i",
                $"//*[contains(text(),'{shipmentName}')]"
            };

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var result = await TryFindLocatorAsync(resultSelectors, timeoutMs: 5000);
                if (result != null)
                    return;

                if (attempt < maxRetries)
                    await Page.WaitForTimeoutAsync(retryDelayMs);
            }

            Assert.Fail($"Shipment '{shipmentName}' was not found in the Hub search results after {maxRetries} attempts.");
        }

        /// <summary>
        /// Verifies the shipment does NOT appear in the Hub search results.
        /// Used to confirm a hidden shipment is invisible in the Hub.
        /// </summary>
        public async Task TheShipmentShouldNotAppearInTheSearchResultsInTheHub(string shipmentName)
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var resultSelectors = new[]
            {
                $"internal:text=\"{shipmentName}\"i",
                $"//*[contains(text(),'{shipmentName}')]"
            };

            var result = await TryFindLocatorAsync(resultSelectors, timeoutMs: 4000);
            Assert.IsNull(result,
                $"Shipment '{shipmentName}' was found in the Hub search results but it should not appear (shipment is hidden). URL: {Page.Url}");
        }
    }
}
