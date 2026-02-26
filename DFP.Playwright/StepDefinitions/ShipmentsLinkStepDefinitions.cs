using System.Net.Http;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentsLinkStepDefinitions
    {
        private HttpResponseMessage? _linkShipmentResponse;
        private HttpResponseMessage? _linkCargoResponse;
        private string _linkShipmentBody = "";
        private string _linkCargoBody = "";
        private readonly DFP.Playwright.Support.TestContext _tc;

        public ShipmentsLinkStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

        [When(@"I link shipment with id ""([^""]+)"" to purchase order ""([^""]+)"" via API")]
        public async Task WhenILinkShipmentWithIdToPurchaseOrderViaApi(string shipmentId, string purchaseOrderId)
        {
            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _linkShipmentResponse = await client.LinkShipmentToPurchaseOrderAsync(token, shipmentId, purchaseOrderId);
            _linkShipmentBody = _linkShipmentResponse == null ? "" : await _linkShipmentResponse.Content.ReadAsStringAsync();
        }

        [When(@"I link cargo item ""([^""]+)"" to order line ""([^""]+)"" for shipment ""([^""]+)"" via API")]
        public async Task WhenILinkCargoItemToOrderLineForShipmentViaApi(string cargoItemId, string orderLineId, string shipmentId)
        {
            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _linkCargoResponse = await client.LinkCargoItemToOrderLineAsync(token, shipmentId, cargoItemId, orderLineId);
            _linkCargoBody = _linkCargoResponse == null ? "" : await _linkCargoResponse.Content.ReadAsStringAsync();
        }

        [Then("the link requests should succeed")]
        public void ThenTheLinkRequestsShouldSucceed()
        {
            Assert.IsNotNull(_linkShipmentResponse, "Link shipment/PO response is null.");
            Assert.IsNotNull(_linkCargoResponse, "Link cargo/order line response is null.");

            Assert.IsTrue(
                _linkShipmentResponse!.IsSuccessStatusCode,
                $"Link shipment/PO failed: {(int)_linkShipmentResponse.StatusCode} {_linkShipmentResponse.ReasonPhrase}. Body: {_linkShipmentBody}");

            Assert.IsTrue(
                _linkCargoResponse!.IsSuccessStatusCode,
                $"Link cargo/order line failed: {(int)_linkCargoResponse.StatusCode} {_linkCargoResponse.ReasonPhrase}. Body: {_linkCargoBody}");
        }

        private string GetPortalToken()
        {
            if (_tc.Data.TryGetValue("portalToken", out var value) && value is string token && !string.IsNullOrWhiteSpace(token))
                return token;

            throw new InvalidOperationException("Portal token not found. Run step 'I have a portal API token' first.");
        }
    }
}
