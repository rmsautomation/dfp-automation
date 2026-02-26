using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentsUnhideStepDefinitions
    {
        private HttpResponseMessage? _unhideResponse;
        private string _unhideResponseBody = "";
        private readonly DFP.Playwright.Support.TestContext _tc;

        public ShipmentsUnhideStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

        [When(@"I unhide shipment with id ""([^""]+)"" via API")]
        public async Task WhenIUnhideShipmentWithIdViaApi(string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _unhideResponse = await client.UnhideShipmentAsync(token, shipmentId);
            _unhideResponseBody = _unhideResponse == null ? "" : await _unhideResponse.Content.ReadAsStringAsync();
        }

        [Then("the unhide shipment request should succeed")]
        public void ThenTheUnhideShipmentRequestShouldSucceed()
        {
            Assert.IsNotNull(_unhideResponse, "Unhide shipment response is null.");
            AssertOkResponse(_unhideResponse!, _unhideResponseBody, "Unhide shipment");
        }

        private string GetPortalToken()
        {
            if (_tc.Data.TryGetValue("portalToken", out var value) && value is string token && !string.IsNullOrWhiteSpace(token))
                return token;

            throw new InvalidOperationException("Portal token not found. Run step 'I have a portal API token' first.");
        }

        private static void AssertOkResponse(HttpResponseMessage response, string body, string action)
        {
            Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode,
                $"{action} failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");

            var status = TryReadStatus(body);
            Assert.IsTrue(string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase),
                $"{action} failed: expected status 'ok' in response body. Body: {body}");
        }

        private static string TryReadStatus(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("status", out var statusProp) &&
                    statusProp.ValueKind == JsonValueKind.String)
                {
                    return statusProp.GetString() ?? "";
                }
            }
            catch (JsonException)
            {
            }

            return "";
        }
    }
}
