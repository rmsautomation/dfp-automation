using Reqnroll;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly ShipmentPage _shipmentPage;

        private HttpResponseMessage? _hideResponse;
        private string _hideResponseBody = "";
        private HttpResponseMessage? _unhideResponse;
        private string _unhideResponseBody = "";
        private HttpResponseMessage? _linkShipmentResponse;
        private HttpResponseMessage? _linkCargoResponse;
        private string _linkShipmentBody = "";
        private string _linkCargoBody = "";

        public ShipmentStepDefinitions(DFP.Playwright.Support.TestContext tc, ShipmentPage shipmentPage)
        {
            _tc = tc;
            _shipmentPage = shipmentPage;
        }
// ── Shipment Creation steps ───────────────────────────────────────────────
        [Given("I am on the Quotations List page")]
        public async Task IAmOnTheQuotationsListPage()
        {
            await _shipmentPage.IAmOnTheQuotationsListPage();
        }

        [When("I open the first quotation in Status Booked")]
        public async Task IOpenTheFirstQuotationInStatusBooked()
        {
            await _shipmentPage.IOpenTheFirstQuotationInStatusBooked();
        }

        [Then("I should be on the Quotation Details page")]
        public async Task IShouldBeOnTheQuotationDetailsPage()
        {
            await _shipmentPage.IShouldBeOnTheQuotationDetailsPage();
        }

        [When("I click the \"Offers\" button")]
        public async Task IClickTheOffersButton()
        {
            await _shipmentPage.IClickTheOffersButton();
        }

        [Then("the list of the offers should appear")]
        public async Task TheListOfTheOffersShouldAppear()
        {
            await _shipmentPage.TheListOfTheOffersShouldAppear();
        }

        [When("I click on Book Now button")]
        public async Task IClickOnBookNowButton()
        {
            await _shipmentPage.ClicksOnBookNowButton();
        }

        [Then("a confirmation dialog should appear")]
        public async Task AConfirmationDialogShouldAppear()
        {
            await _shipmentPage.AConfirmationDialogShouldAppear();
        }

        [When("I confirm the shipment creation")]
        public async Task IConfirmTheShipmentCreation()
        {
            await _shipmentPage.IConfirmTheShipmentCreation();
        }

        [Then("I should be on the Shipment Details page")]
        public async Task IShouldBeOnTheShipmentDetailsPage()
        {
            await _shipmentPage.IShouldBeOnTheShipmentDetailsPage();
        }

        [When("I click on Edit button to Edit the Shipment Name")]
        public async Task IClickOnEditButtonToEditTheShipmentName()
        {
            await _shipmentPage.IClickOnEditButtonToEditTheShipmentName();
        }

        [Then("I should edit the Shipment Name")]
        public async Task IShouldEditTheShipmentName()
        {
            await _shipmentPage.IShouldEditTheShipmentName();
        }

        [When("I click on save button")]
        public async Task IClickOnSaveButton()
        {
            await _shipmentPage.IClickOnSaveButton();
        }

        [Then("I should see the new Shipment Name")]
        public async Task IShouldSeeTheNewShipmentName()
        {
            await _shipmentPage.IShouldSeeTheNewShipmentName();
        }

        [When("I click on Send Booking button")]
        public async Task IClickOnSendBookingButton()
        {
            await _shipmentPage.IClickOnSendBookingButton();
        }

        [Then("I should click on Go To Shipment button to see the shipment")]
        public async Task IShouldClickOnGoToShipmentButtonToSeeTheShipment()
        {
            await _shipmentPage.IShouldClickOnGoToShipmentButtonToSeeTheShipment();
        }

        [Then("the shipment should display the shipment name")]
        public async Task TheShipmentShouldDisplayTheShipmentName()
        {
            await _shipmentPage.TheShipmentShouldDisplayTheShipmentName();
        }

        // ── ShipmentSearch steps ─────────────────────────────────────────────────

        [Given("user navigated to Shipments List")]
        [Given("I navigated to Shipments List")]
        public async Task UserNavigatedToShipmentsList()
        {
            await _shipmentPage.UserNavigatedToShipmentsList();
        }

        [When("I click on Show More filters")]
        public async Task IClickOnShowMoreFilters()
        {
            await _shipmentPage.IClickOnShowMoreFilters();
        }

        [When("I enter the shipment name in Shipment Reference field")]
        public async Task IEnterTheShipmentNameInShipmentReferenceField()
        {
            await _shipmentPage.IEnterShipmentNameInShipmentReferenceField();
        }

        [When("I click on Search button")]
        public async Task IClickOnSearchButton()
        {
            await _shipmentPage.IClickOnSearchButton();
        }

        [Then("the shipment should appear in the search results")]
        public async Task TheShipmentShouldAppearInTheSearchResults()
        {
            await _shipmentPage.TheShipmentShouldAppearInSearchResults();
        }

        // ── Tag steps ────────────────────────────────────────────────────────────

        [Then("a tag icon should be displayed below the shipment name or on the left side of existing tags")]
        public async Task ATagIconShouldBeDisplayedBelowTheShipmentName()
        {
            await _shipmentPage.ATagIconShouldBeDisplayed();
        }

        [Then("the tag icon tooltip should say {string}")]
        public async Task TheTagIconTooltipShouldSay(string expectedTooltip)
        {
            await _shipmentPage.TheTagIconTooltipShouldSay(expectedTooltip);
        }

        [When("I click the tag icon on the shipment")]
        public async Task UserClicksTheTagIconOnTheShipment()
        {
            await _shipmentPage.UserClicksTheTagIcon();
        }

        [Then("a field should appear to select an existing tag or create a new tag")]
        public async Task AFieldShouldAppearToSelectOrCreateTag()
        {
            await _shipmentPage.ATagInputFieldShouldAppear();
        }

        [When("I create and assign a new tag to the shipment")]
        [When("I create and assigns a new tag to the shipment")]
        public async Task UserCreatesAndAssignsANewTagToTheShipment()
        {
            var tagName = $"{DateTime.UtcNow:MMddHHmmss}auto";
            await _shipmentPage.UserCreatesAndAssignsNewTag(tagName);
        }

        [When("I assign the existing tag to the shipment")]
        public async Task UserAssignsTheExistingTagToTheShipment()
        {
            await _shipmentPage.UserAssignsExistingTagToShipment();
        }

        [Then("the tag should be visible on the selected shipment")]
        public async Task TheTagShouldBeVisibleOnTheSelectedShipment()
        {
            await _shipmentPage.TheTagShouldBeVisibleOnTheShipment();
        }

        [When("I open the tagged shipment details view")]
        public async Task UserOpensTheTaggedShipmentDetailsView()
        {
            await _shipmentPage.UserOpensTaggedShipmentDetailsView();
        }

        [Then("the tag should be visible in Shipment details")]
        public async Task TheTagShouldBeVisibleInShipmentDetails()
        {
            await _shipmentPage.TheTagShouldBeVisibleInShipmentDetails();
        }

        [Then("the tag should be visible in the Tags column in Shipment Table view")]
        public async Task TheTagShouldBeVisibleInTheTagsColumnInShipmentTableView()
        {
            await _shipmentPage.TheTagShouldBeVisibleInShipmentTableView();
        }

        [Then("the tag should be visible in Shipment list view")]
        public async Task TheTagShouldBeVisibleInShipmentListView()
        {
            await _shipmentPage.TheTagShouldBeVisibleInShipmentListView();
        }

        [When("I reset the search filters")]
        public async Task IResetTheSearchFilters()
        {
            await _shipmentPage.IResetSearchFilters();
        }

        [Then("the tag should be visible on 2 shipments in Shipment list view")]
        public async Task TheTagShouldBeVisibleOn2ShipmentsInShipmentListView()
        {
            await _shipmentPage.TheTagShouldBeVisibleOn2ShipmentsInListView();
        }

        [Then("the tag should be visible on 2 shipments in Shipment Table view")]
        public async Task TheTagShouldBeVisibleOn2ShipmentsInShipmentTableView()
        {
            await _shipmentPage.TheTagShouldBeVisibleOn2ShipmentsInTableView();
        }

        // ── @9344_MoreThan5tagsSH steps ───────────────────────────────────────────

        [When("I select the first shipment from the list")]
        public async Task UserSelectsTheFirstShipmentFromTheList()
        {
            await _shipmentPage.UserSelectsFirstShipmentFromList();
        }

        [Then("the system should show the error {string}")]
        public async Task TheSystemShouldShowTheError(string expectedError)
        {
            await _shipmentPage.TheSystemShouldShowTheMaxTagsError(expectedError);
        }

        [Then("all created tags should be visible in Shipment list view")]
        public async Task AllCreatedTagsShouldBeVisibleInShipmentListView()
        {
            await _shipmentPage.AllCreatedTagsShouldBeVisibleInShipmentListView();
        }

        [Then("all created tags should be visible in Shipment details")]
        public async Task AllCreatedTagsShouldBeVisibleInShipmentDetails()
        {
            await _shipmentPage.AllCreatedTagsShouldBeVisibleInShipmentDetails();
        }

        [Then("all created tags should be visible in Shipment Table view")]
        public async Task AllCreatedTagsShouldBeVisibleInShipmentTableView()
        {
            await _shipmentPage.AllCreatedTagsShouldBeVisibleInShipmentTableView();
        }

        [When(@"I hide shipment with id ""([^""]+)"" via API")]
        public async Task WhenIHideShipmentWithIdViaApi(string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(shipmentId))
                shipmentId = GetShipmentIdFromContext();

            var token = GetHubToken();
            var client = PortalApiClient.FromEnvironment();
            _hideResponse = await client.HideShipmentAsync(token, shipmentId);
            _hideResponseBody = _hideResponse == null ? "" : await _hideResponse.Content.ReadAsStringAsync();
        }

        [When("I hide shipment via API")]
        public async Task WhenIHideShipmentViaApi()
        {
            var shipmentId = GetShipmentIdFromContext();
            var token = GetHubToken();
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

        [When(@"I unhide shipment with id ""([^""]+)"" via API")]
        public async Task WhenIUnhideShipmentWithIdViaApi(string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(shipmentId))
                shipmentId = GetShipmentIdFromContext();

            var token = GetHubToken();
            var client = PortalApiClient.FromEnvironment();
            _unhideResponse = await client.UnhideShipmentAsync(token, shipmentId);
            _unhideResponseBody = _unhideResponse == null ? "" : await _unhideResponse.Content.ReadAsStringAsync();
        }

        [When("I unhide shipment via API")]
        public async Task WhenIUnhideShipmentViaApi()
        {
            var shipmentId = GetShipmentIdFromContext();
            var token = GetHubToken();
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

        [When(@"I link shipment with id ""([^""]+)"" to purchase order ""([^""]+)"" via API")]
        public async Task WhenILinkShipmentWithIdToPurchaseOrderViaApi(string shipmentId, string purchaseOrderId)
        {
            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _linkShipmentResponse = await client.LinkShipmentToPurchaseOrderAsync(token, shipmentId, purchaseOrderId);
            _linkShipmentBody = _linkShipmentResponse == null ? "" : await _linkShipmentResponse.Content.ReadAsStringAsync();
        }

        [When("I link shipment to purchase order via API")]
        public async Task WhenILinkShipmentToPurchaseOrderViaApi()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found. Create a shipment first.");
            var purchaseOrderId = GetRequiredContextValue("purchaseOrderId", "Purchase order ID not found. Create a purchase order first.");
            await WhenILinkShipmentWithIdToPurchaseOrderViaApi(shipmentId, purchaseOrderId);
        }

        [When(@"I link cargo item ""([^""]+)"" to order line ""([^""]+)"" for shipment ""([^""]+)"" via API")]
        public async Task WhenILinkCargoItemToOrderLineForShipmentViaApi(string cargoItemId, string orderLineId, string shipmentId)
        {
            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            _linkCargoResponse = await client.LinkCargoItemToOrderLineAsync(token, shipmentId, cargoItemId, orderLineId);
            _linkCargoBody = _linkCargoResponse == null ? "" : await _linkCargoResponse.Content.ReadAsStringAsync();
        }

        [When("I link cargo item to order line for shipment via API")]
        public async Task WhenILinkCargoItemToOrderLineForShipmentViaApi()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found. Create a shipment first.");
            var cargoItemId = GetRequiredContextValue("cargoItemId", "Cargo item ID not found. Get cargo items first.");
            var orderLineId = GetRequiredContextValue("orderLineId", "Order line ID not found. Create a purchase order line first.");
            await WhenILinkCargoItemToOrderLineForShipmentViaApi(cargoItemId, orderLineId, shipmentId);
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

        private string GetHubToken()
        {
            if (_tc.Data.TryGetValue("hubToken", out var value) && value is string token && !string.IsNullOrWhiteSpace(token))
                return token;

            throw new InvalidOperationException("Hub token not found. Run step 'I have a hub API token' first.");
        }

        private string GetShipmentIdFromContext()
        {
            if (_tc.Data.TryGetValue("shipmentId", out var value) && value is string id && !string.IsNullOrWhiteSpace(id))
                return id;

            throw new InvalidOperationException("Shipment ID not found. Capture it from the UI first.");
        }

        private string GetRequiredContextValue(string key, string message)
        {
            if (_tc.Data.TryGetValue(key, out var value) && value is string id && !string.IsNullOrWhiteSpace(id))
                return id;

            throw new InvalidOperationException(message);
        }

        private void StoreShipmentIdFromUrlIfPresent(bool required = false)
        {
            var url = _tc.Page?.Url ?? "";
            var id = TryExtractShipmentIdFromUrl(url);
            if (!string.IsNullOrWhiteSpace(id))
            {
                _tc.Data["shipmentId"] = id;
                return;
            }

            if (required)
                throw new InvalidOperationException($"Shipment ID not found in URL: {url}");
        }

        private static string TryExtractShipmentIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";

            var match = Regex.Match(url, @"\/shipments\/([0-9a-fA-F-]{36})");
            if (match.Success)
                return match.Groups[1].Value;

            match = Regex.Match(url, @"\/booking\/new\/([0-9a-fA-F-]{36})");
            if (match.Success)
                return match.Groups[1].Value;

            return "";
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

        [Then("I store the shipment id from the URL")]
        public void ThenIStoreTheShipmentIdFromTheUrl()
        {
            StoreShipmentIdFromUrlIfPresent(required: true);
        }
    }
}
