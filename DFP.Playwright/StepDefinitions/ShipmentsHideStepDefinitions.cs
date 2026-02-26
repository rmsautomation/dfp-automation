using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;
using System.Text.Json;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentsHideStepDefinitions
    {
        private HttpResponseMessage? _hideResponse;
        private string _hideResponseBody = "";
        private readonly DFP.Playwright.Support.TestContext _tc;

        public ShipmentsHideStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

        [When(@"I hide shipment with id ""([^""]+)"" via API")]
        public async Task WhenIHideShipmentWithIdViaApi(string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _hideResponse = await client.HideShipmentAsync(token, shipmentId);
            _hideResponseBody = _hideResponse == null ? "" : await _hideResponse.Content.ReadAsStringAsync();
        }

        [Then("the hide shipment request should succeed")]
        public void ThenTheHideShipmentRequestShouldSucceed()
        {
            Assert.IsNotNull(_hideResponse, "Hide shipment response is null.");
            AssertOkResponse(_hideResponse!, _hideResponseBody, "Hide shipment");
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
