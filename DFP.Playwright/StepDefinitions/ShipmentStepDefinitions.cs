using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
namespace DFP.Playwright.StepDefinitions
{
    public sealed class ShipmentStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly ShipmentPage _shipmentPage;
        private readonly ShipmentHubPage _shipmentHubPage;
        private readonly QuotationPage _quotationPage;

        private HttpResponseMessage? _hideResponse;
        private string _hideResponseBody = "";
        private HttpResponseMessage? _unhideResponse;
        private string _unhideResponseBody = "";
        private HttpResponseMessage? _linkShipmentResponse;
        private HttpResponseMessage? _linkCargoResponse;
        private string _linkShipmentBody = "";
        private string _linkCargoBody = "";

        public ShipmentStepDefinitions(DFP.Playwright.Support.TestContext tc, ShipmentPage shipmentPage, ShipmentHubPage shipmentHubPage, QuotationPage quotationPage)
        {
            _tc = tc;
            _shipmentPage = shipmentPage;
            _shipmentHubPage = shipmentHubPage;
            _quotationPage = quotationPage;
        }
// ── Shipment Creation steps ───────────────────────────────────────────────
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
            // Update shipmentId from the URL so subsequent steps always use the correct GUID
            StoreShipmentIdFromUrlIfPresent();
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

        [Then("I store shipment Name")]
        [When("I store shipment Name")]
        public void IStoreShipmentName()
        {
            _tc.Data["shipmentName"] = _shipmentPage.GetShipmentName();
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

        [Given("I set the shipment name to {string}")]
        public void ISetTheShipmentNameTo(string name)
        {
            _shipmentPage.SetShipmentName(name);
        }

        [Given("user navigated to Shipments List")]
        [Given("I navigated to Shipments List")]
        [When("I navigated to Shipments List")]
        [Then("I navigated to Shipments List")]
        public async Task UserNavigatedToShipmentsList()
        {
            await _shipmentPage.UserNavigatedToShipmentsList();
        }

        [Given("I click on Show More filters")]
        [When("I click on Show More filters")]
        public async Task IClickOnShowMoreFilters()
        {
            await _shipmentPage.IClickOnShowMoreFilters();
        }

        [Given("I enter the shipment name in Shipment Reference field")]
        [When("I enter the shipment name in Shipment Reference field")]
        public async Task IEnterTheShipmentNameInShipmentReferenceField()
        {
            // Webhook scenario: _shipmentName is empty; use shipment_reference (REF-...) from API context
            if (string.IsNullOrWhiteSpace(_shipmentPage.GetShipmentName())
                && _tc.Data.TryGetValue("shipment_reference", out var refVal)
                && refVal is string shipRef
                && !string.IsNullOrWhiteSpace(shipRef))
            {
                _shipmentPage.SetShipmentName(shipRef);
            }
            await _shipmentPage.IEnterShipmentNameInShipmentReferenceField();
        }

        [Given("I click on Search button")]
        [When("I click on Search button")]
        [Then("I click on Search button")]
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

        [Then("I should not see House tab")]
        public async Task IShouldNotSeeHouseTab()
        {
            await _shipmentPage.IShouldNotSeeHouseTab();
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

        [Given("I enter the shipment Reference in Quick filter")]
        [When("I enter the shipment Reference in Quick filter")]
        [Then("I enter the shipment Reference in Quick filter")]
        public async Task IEnterTheShipmentReferenceInQuickFilter()
        {
            await _shipmentPage.IEnterShipmentReferenceInQuickFilter();
        }

        [Given("I enter {string} in Quick filter")]
        [When("I enter {string} in Quick filter")]
        [Then("I enter {string} in Quick filter")]
        public async Task IEnterTextInQuickFilter(string text)
        {
            await _shipmentPage.EnterTextInQuickFilterAsync(text);
        }

        [Then("I should not see the quick filter field")]
        public async Task IShouldNotSeeTheQuickFilterField()
        {
            await _shipmentPage.IShouldNotSeeTheQuickFilterField();
        }

        [When("I click on Show Less")]
        public async Task IClickOnShowLess()
        {
            await _shipmentPage.IClickOnShowLess();
        }

        [Then("I should see the quick filter field")]
        public async Task IShouldSeeTheQuickFilterField()
        {
            await _shipmentPage.IShouldSeeTheQuickFilterField();
        }

        [When("I reset the search filters")]
        public async Task IResetTheSearchFilters()
        {
            await _shipmentPage.IResetSearchFilters();
        }

        [Given("I click on List View button")]
        [When("I click on List View button")]
        public async Task IClickOnListViewButton()
        {
            await _shipmentPage.IClickOnListViewButton();
        }

        [Given("I click on Table View")]
        [When("I click on Table View")]
        [Then("I click on Table View")]
        [Given("I click on Table View button")]
        [When("I click on Table View button")]
        [Then("I click on Table View button")]
        public async Task IClickOnTableViewButton()
        {
            await _shipmentPage.IClickOnTableViewButton();
        }

        [Given("I select the {string} column view")]
        [When("I select the {string} column view")]
        [Then("I select the {string} column view")]
        public async Task ISelectTheColumnView(string viewName)
        {
            await _shipmentPage.SelectColumnViewAsync(viewName);
        }

        [Then("the shipment should not appear in the search results")]
        public async Task TheShipmentShouldNotAppearInTheSearchResults()
        {
            await _shipmentPage.TheShipmentShouldNotAppearInSearchResults();
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


        /////////////////////////////////////////////////////////ADDITIONALS STEPS FOR VERIFY LINK PO WITH SH/////////////////////
        /// 
        [When("I click on Booking Details Tab")]
        public async Task IClickOnBookingDetailsTab()
        {
            await _shipmentPage.IClickOnBookingDetailsTab();
        }

        
        [Then("I should see the Purchase Order section in the Shipment Portal")]
        public async Task IShouldSeeThePurchaseOrderSectionInTheShipmentPortal()
        {
            await _shipmentPage.IShouldSeeThePurchaseOrderSectionInTheShipmentPortal();
        }

        [When("I click on Purchase Order link")]
        public async Task IClickOnPurchaseOrderLink()
        {
            _tc.Data.TryGetValue("purchaseOrderId", out var value);
            var purchaseOrderId = value as string ?? "";
            await _shipmentPage.IClickOnPurchaseOrderLink(purchaseOrderId);
        }
        [Then("I should be on the Purchase Order Details")]
        public async Task IShouldBeOnThePurchaseOrderDetails()
        {
            _tc.Data.TryGetValue("purchaseOrderId", out var value);
            var purchaseOrderId = value as string ?? "";
            await _shipmentPage.IShouldBeOnThePurchaseOrderDetails(purchaseOrderId);
        }
        [Then("I should see the Status of the PO In Progress")]
        public async Task IShouldSeeTheStatusOfThePOInProgress()
        {
            await _shipmentPage.IShouldSeeTheStatusOfThePOInProgress();
        }

        [Then("I should see Booked Shipments section in the Purchase order")]
        public async Task IShouldSeeBookedShipmentsSectionInThePurchaseOrder()
        {
            await _shipmentPage.IShouldSeeBookedShipmentsSectionInThePurchaseOrder();
        }

        [When("I click on the Shipment Name link")]
        public async Task IClickOnTheShipmentNameLink()
        {
            _tc.Data.TryGetValue("shipmentId", out var value);
            var shipmentId = value as string ?? "";
            await _shipmentPage.IClickOnTheShipmentNameLink(shipmentId);
        }

        [When("I click on the shipment")]
        public async Task IClickOnTheShipment()
        {
            await _shipmentPage.IClickOnTheShipmentAsync();
        }

        [When("I click on Cargo section with PO")]
        public async Task IClickOnCargoSectionWithPO()
        {
            _tc.Data.TryGetValue("purchaseOrderId", out var value);
            var purchaseOrderId = value as string ?? "";
            await _shipmentPage.IClickOnCargoSectionWithPO(purchaseOrderId);
        }

        [Then("Order Line has a Shipment Name link related")]
        public async Task OrderLineHasAShipmentNameLinkRelated()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found. Create a shipment first.");

            // shipmentName is the Shipment Reference (REF-...) generated during webhook creation
            var shipmentName = _tc.Data.TryGetValue("shipment_reference", out var refVal) && refVal is string sRef && !string.IsNullOrWhiteSpace(sRef)
                ? sRef
                : _shipmentPage.GetShipmentName();

            await _shipmentPage.VerifyShipmentLinkInOrderLineAsync(shipmentId, shipmentName);
        }

        // ── TC10255: Milestone date history steps (Portal side) ───────────────────

        [Then("I should see a new label next week to the milestone date in {string}")]
        public async Task IShouldSeeANewLabelNextToTheMilestoneDateIn(string milestoneName)
            => await _shipmentPage.ShouldSeeHistoryBadgeNextToMilestoneAsync(milestoneName);

        [When("I click on the new label next week to the milestone date in {string}")]
        public async Task IClickOnTheNewLabelNextToTheMilestoneDateIn(string milestoneName)
            => await _shipmentPage.ClickHistoryBadgeForMilestoneAsync(milestoneName);

        [Then("I should see a popup  with the current date")]
        public async Task IShouldSeeAPopupWithTheCurrentDate()
            => await _shipmentPage.ShouldSeePopupWithCurrentDateAsync();

        // Uses the date stored in ShipmentHubPage during the Hub calendar steps.
        // _shipmentHubPage is the same scoped instance used in ShipmentHubStepDefinitions.
        [Then("I should see the historical changes")]
        public async Task IShouldSeeTheHistoricalChanges()
            => await _shipmentPage.ShouldSeeHistoricalChangesAsync(_shipmentHubPage.GetSelectedDate());

        // ── TC4520: Attachments ───────────────────────────────────────────────────

        [Then("I click on Attahments tab")]
        [When("I click on Attahments tab")]
        public async Task IClickOnAttachmentsTab()
            => await _shipmentPage.ClickAttachmentsTabAsync();

        [When("I click on Attach document button")]
        public async Task IClickOnAttachDocumentButton()
            => await _shipmentPage.ClickAttachDocumentButtonAsync();

        [Then("I should see the screen to upload the attachment")]
        public async Task IShouldSeeTheScreenToUploadTheAttachment()
            => await _shipmentPage.ShouldSeeUploadScreenAsync();

        [When("I select the file to upload {string}")]
        public async Task ISelectTheFileToUpload(string fileName)
            => await _shipmentPage.SelectFileToUploadAsync(fileName);

        [Then("I click on Upload button")]
        [When("I click on Upload button")]
        public async Task IClickOnUploadButton()
            => await _shipmentPage.ClickUploadButtonAsync();

        [Then("I should see the uploaded file {string}")]
        public async Task IShouldSeeTheUploadedFile(string fileName)
            => await _shipmentPage.ShouldSeeUploadedFileAsync(fileName);

        // ── Subscribe / Unsubscribe steps ─────────────────────────────────────────

        [When("I click on the Subscribe button")]
        public async Task WhenIClickOnTheSubscribeButton()
            => await _shipmentPage.ClickSubscribeButtonAsync();

        [Then("I should see a new panel to select the Notification")]
        public async Task ThenIShouldSeeANewPanelToSelectTheNotification()
            => await _shipmentPage.ShouldSeeSubscriptionPanelAsync();

        [When(@"I enable the option ""?(.+?)""?$")]
        [Then(@"I enable the option ""?(.+?)""?$")]
        public async Task WhenIEnableTheOption(string option)
            => await _shipmentPage.EnableNotificationOptionAsync(option);

        [Then("I should see the subscribe text changed to Unsubscribe in the List")]
        public async Task ThenIShouldSeeUnsubscribeInList()
        {
            var shipmentName = _tc.Data.TryGetValue("shipmentName", out var nm) && nm is string s ? s : "";
            await _shipmentPage.ShouldSeeUnsubscribeInListAsync(shipmentName);
        }

        [Then("I should see the subscribe text changed to Unsubscribe in the Details View")]
        public async Task ThenIShouldSeeUnsubscribeInDetailsView()
            => await _shipmentPage.ShouldSeeUnsubscribeInDetailsAsync();

        // ── Portal notifications steps ────────────────────────────────────────────

        [Then("I click on notifications button")]
        [When("I click on notifications button")]
        public async Task ThenIClickOnNotificationsButton()
            => await _shipmentPage.ClickNotificationsButtonAsync();

        [Then("I should see the updated status {string} in the notifications")]
        public async Task ThenIShouldSeeTheUpdatedStatusInNotifications(string status)
            => await _shipmentPage.ShouldSeeStatusInNotificationsAsync(status);

        [Then("I should see the shipment Name in the notifications")]
        public async Task ThenIShouldSeeTheShipmentNameInTheNotifications()
        {
            var shipmentName = _tc.Data.TryGetValue("shipmentName", out var nm) && nm is string s ? s : "";
            await _shipmentPage.ShouldSeeShipmentNameInNotificationsAsync(shipmentName);
        }

        [Given("I check the following custom field values in the table view for shipment")]
        [When("I check the following custom field values in the table view for shipment")]
        [Then("I check the following custom field values in the table view for shipment")]
        public async Task ICheckCustomFieldValuesInShipmentTableView(Table dataTable)
        {
            var pairs = dataTable.Rows.Select(r => (columnName: r[0], expectedValue: r[1]));
            await _shipmentPage.CheckCustomFieldValuesInShipmentTableViewAsync(pairs);
        }

        // ── Portal Shipment Form steps ────────────────────────────────────────────

        [When("I enter the vessel {string} in the Portal")]
        public async Task IEnterTheVesselInThePortal(string vessel)
            => await _shipmentPage.EnterVesselAsync(vessel);

        [When("I go to {string} Tab in the Shipment Portal")]
        public async Task IGoToTabInTheShipmentPortal(string tabName)
            => await _shipmentPage.ClickShipmentTabByNameAsync(tabName);

        // Single regex step covers: Shipper, Consignee, Notify, Forwarder, name, address, Instructions remarks
        // Feature file usage:  When I enter the Shipper "RefValue" in the Shipment Portal
        //                      When I enter the Instructions remarks "" in the Shipment Portal
        [When(@"I enter the ([\w][\w\s]*) ""([^""]*)"" in the Shipment Portal")]
        [Then(@"I enter the ([\w][\w\s]*) ""([^""]*)"" in the Shipment Portal")]
        [Given(@"I enter the ([\w][\w\s]*) ""([^""]*)"" in the Shipment Portal")]
        public async Task IEnterFieldInTheShipmentPortal(string fieldKey, string value)
            => await _shipmentPage.EnterShipmentFormFieldAsync(fieldKey, value);

        // ── Portal Shipment Detail — verification steps ───────────────────────────

        [Then("the shipment name should contains {string}")]
        public async Task TheShipmentNameShouldContains(string text)
            => await _shipmentPage.VerifyShipmentNameContainsAsync(text);

        [Then("I verify the origin {string}")]
        public async Task IVerifyTheOrigin(string city)
            => await _shipmentPage.VerifyOriginAsync(city);

        [Then("I verify the status {string} in the shipment detail page")]
        public async Task IVerifyTheStatusInTheShipmentDetailPage(string status)
            => await _shipmentPage.VerifyStatusAsync(status);

        // Single regex step covers: shipper, consignee, forwarder (and any future label)
        // Feature file: Then I verify the shipper contains "Updated" in the shipment detail page
        [Then(@"I verify the (\w+) contains ""([^""]*)"" in the shipment detail page")]
        public async Task IVerifyFieldContainsInShipmentDetailPage(string fieldLabel, string expectedText)
            => await _shipmentPage.VerifyShipmentDetailFieldAsync(fieldLabel, expectedText);

        [Then("I verify the quote id in the shipment detail page")]
        public async Task IVerifyTheQuoteIdInTheShipmentDetailPage()
            => await _shipmentPage.VerifyQuoteIdInDetailAsync(_quotationPage.GetQuoteId());

        [When("I go to Tracking tab")]
        [Then("I go to Tracking tab")]
        public async Task IGoToTrackingTab()
            => await _shipmentPage.ClickTrackingTabAsync();

        [Then("I should see the event {string}")]
        public async Task IShouldSeeTheEvent(string eventText)
            => await _shipmentPage.VerifyTrackingEventAsync(eventText);

        // ── Booking Details tab steps ─────────────────────────────────────────────

        [When("I go to Booking Details tab")]
        [Then("I go to Booking Details tab")]
        public async Task IGoToBookingDetailsTab()
            => await _shipmentPage.ClickBookingDetailsTabAsync();

        [Then("I should see the commodity {string}")]
        public async Task IShouldSeeTheCommodity(string commodity)
            => await _shipmentPage.VerifyCommodityAsync(commodity);

        [Then("I should see the Remarks Instructions contains {string}")]
        public async Task IShouldSeeTheRemarksInstructionsContains(string text)
            => await _shipmentPage.VerifyRemarksInstructionsAsync(text);

        // Single regex step covers: "shipper", "billing client", "link entities shipper", etc.
        // Feature file: Then I should see the shipper contains "updated"
        //               Then I should see billing client contains "automation"
        //               Then I should see link entities shipper contains "updated"
        [Then(@"I should see (?:the )?([\w][\w\s]*) contains ""([^""]*)""")]
        public async Task IShouldSeeEntityContains(string roleLabel, string expectedText)
            => await _shipmentPage.VerifyEntityCardContainsAsync(roleLabel.Trim(), expectedText);

        // ── Charges & Invoices tab steps ──────────────────────────────────────────

        [When("I go to Charge and Invoices tab")]
        [Then("I go to Charge and Invoices tab")]
        public async Task IGoToChargeAndInvoicesTab()
            => await _shipmentPage.ClickChargesInvoicesTabAsync();

        [Then("I should see the charge {string}")]
        public async Task IShouldSeeTheCharge(string chargeName)
            => await _shipmentPage.VerifyChargeAsync(chargeName);
    }
}
