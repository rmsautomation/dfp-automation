using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DFP.Playwright.Helpers;
using DFP.Playwright.Pages.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentTrackingStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly ShipmentPage _shipmentPage;
        private readonly ShipmentHubPage _shipmentHubPage;

        private HttpResponseMessage? _subscribeShipmentResponse;
        private string _subscribeShipmentBody = "";
        private HttpResponseMessage? _subscribeContainerResponse;
        private string _subscribeContainerBody = "";
        private HttpResponseMessage? _trackingEventResponse;
        private string _trackingEventBody = "";
        private string _trackingEventTransactionId = "";
        private string _trackingEventOrganizationConnectionId = "";
        private HttpResponseMessage? _unsubscribeContainerResponse;
        private string _unsubscribeContainerBody = "";
        private HttpResponseMessage? _unsubscribeShipmentResponse;
        private string _unsubscribeShipmentBody = "";
        private string _notificationEmail = "";
        private string _notificationUsername = "";
        private readonly List<string> _latestNotificationEmailBodies = [];

        public ShipmentTrackingStepDefinitions(
            DFP.Playwright.Support.TestContext tc,
            ShipmentPage shipmentPage,
            ShipmentHubPage shipmentHubPage)
        {
            _tc = tc;
            _shipmentPage = shipmentPage;
            _shipmentHubPage = shipmentHubPage;
        }

        [Then("a shipment id should be available for tracking")]
        public void ThenAShipmentIdShouldBeAvailableForTracking()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found. Create a shipment first.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(shipmentId), "Shipment ID is empty.");
        }

        [When("I subscribe current shipment to live tracking via API")]
        public async Task WhenISubscribeCurrentShipmentToLiveTrackingViaApi()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found.");
            var internalTrackingNumber = GetOrCreateInternalTrackingNumber();
            var hubToken = GetHubToken();
            var client = PortalApiClient.FromEnvironment();

            _subscribeShipmentResponse = await client.SubscribeShipmentAsync(hubToken, shipmentId, internalTrackingNumber);
            _subscribeShipmentBody = _subscribeShipmentResponse == null
                ? ""
                : await _subscribeShipmentResponse.Content.ReadAsStringAsync();
        }

        [Then("the shipment subscribe request should succeed")]
        public void ThenTheShipmentSubscribeRequestShouldSucceed()
        {
            Assert.IsNotNull(_subscribeShipmentResponse, "Subscribe shipment response is null.");
            Assert.IsTrue(
                _subscribeShipmentResponse!.IsSuccessStatusCode,
                $"Subscribe shipment failed: {(int)_subscribeShipmentResponse.StatusCode} {_subscribeShipmentResponse.ReasonPhrase}. Body: {_subscribeShipmentBody}");
        }

        [Then("I Check the tracking is enabled for the shipment in the hub")]
        public async Task ThenCheckTheTrackingIsEnabledForTheShipment()
        {
            var shipmentReference = GetRequiredContextValue("shipment_reference", "Shipment reference not found.");

            await _shipmentHubPage.INavigatedToShipmentListInTheHub();
            await _shipmentHubPage.IClickOnCustomerReferenceInputFieldInTheHub();
            await _shipmentHubPage.IEnterTheShipmentNameInCustomReferenceFieldInTheHub(shipmentReference);
            await _shipmentHubPage.IClickOnSearchButtonInTheHub();
            await _shipmentHubPage.TheShipmentShouldAppearInTheSearchResultsInTheHub(shipmentReference);
            await _shipmentHubPage.OpenShipmentByReferenceAsync(shipmentReference);
            await _shipmentHubPage.CheckTheTrackingIsEnabledForTheShipmentAsync();
        }

        [When("I subscribe first container of current shipment to live tracking via API")]
        public async Task WhenISubscribeFirstContainerOfCurrentShipmentToLiveTrackingViaApi()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found.");
            var (containerId, _) = await EnsureFirstContainerIdentifiersAsync(shipmentId);
            _tc.Data["container_id_1"] = containerId;
            _tc.Data["shipment.containerID"] = containerId;
            _tc.Data["shipmentId"] = shipmentId;
            LogReusableTrackingContext("Before SubscribeContainer");
            var hubToken = GetHubToken();
            var client = PortalApiClient.FromEnvironment();

            _subscribeContainerResponse = await client.SubscribeContainerAsync(hubToken, shipmentId, containerId);
            _subscribeContainerBody = _subscribeContainerResponse == null
                ? ""
                : await _subscribeContainerResponse.Content.ReadAsStringAsync();
        }

        [Then("the container subscribe request should succeed")]
        public void ThenTheContainerSubscribeRequestShouldSucceed()
        {
            Assert.IsNotNull(_subscribeContainerResponse, "Subscribe container response is null.");
            Assert.IsTrue(
                _subscribeContainerResponse!.IsSuccessStatusCode,
                $"Subscribe container failed: {(int)_subscribeContainerResponse.StatusCode} {_subscribeContainerResponse.ReasonPhrase}. Body: {_subscribeContainerBody}");
        }

        [When("I open the shipment from search results")]
        public async Task WhenIOpenTheShipmentFromSearchResults()
        {
            var shipmentReference = GetRequiredContextValue("shipment_reference", "Shipment reference not found.");
            await _shipmentPage.OpenShipmentFromSearchResultsAsync(shipmentReference);
        }

        [Then("subscribed containers should be available in Shipment Summary dropdown")]
        [Then("subscribed container should be available in Shipment Summary dropdown")]
        public async Task ThenSubscribedContainersShouldBeAvailableInShipmentSummaryDropdown()
        {
            var containerForUi = GetExpectedSummaryContainerForUi();
            await _shipmentPage.SubscribedContainersShouldBeVisibleInSummaryDropdownAsync(containerForUi);
            await _shipmentPage.SummaryShouldShowLiveTrackAndMapAsync();
        }

        [When("I send tracking coordinates for the subscribed container via API")]
        public async Task WhenISendATrackingEventForTheSubscribedContainerViaApi()
        {
            var shipmentReference = GetRequiredContextValue("shipment_reference", "Shipment reference not found.");
            var internalTrackingNumber = GetOrCreateInternalTrackingNumber();
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found.");
            var webhookTransactionId = GetRequiredContextValue("transactionId", "Webhook transactionId not found.");
            var containerTrackingId = _tc.Data.TryGetValue("container_id_1", out var c1Val)
                && c1Val is string c1Str
                && !string.IsNullOrWhiteSpace(c1Str)
                ? c1Str
                : GetRequiredContextValue("shipment.containerID", "Container tracking ID not found.");
            var project44Token = ResolveEnvValue(
                Environment.GetEnvironmentVariable(Constants.PROJECT44_TOKEN)
                ?? Environment.GetEnvironmentVariable("project44.Token")
                ?? "");
            var organizationConnectionId = ResolveEnvValue(
                Environment.GetEnvironmentVariable(Constants.ORGANIZATION_CONNECTION_ID)
                ?? Environment.GetEnvironmentVariable("organizationConnectionId")
                ?? "");

            if (string.IsNullOrWhiteSpace(project44Token))
                throw new InvalidOperationException("PROJECT44 token is required.");
            if (string.IsNullOrWhiteSpace(organizationConnectionId))
                throw new InvalidOperationException("Organization connection ID is required.");
            _trackingEventOrganizationConnectionId = organizationConnectionId;

            var now = DateTime.UtcNow;
            var eventName = $"Event-{now:yyyyMMddHHmmss}";
            _tc.Data["tracking_event_name"] = eventName;
            var latitude = ResolveDoubleEnv(Constants.TRACKING_LATITUDE, 41.88);
            var longitude = ResolveDoubleEnv(Constants.TRACKING_LONGITUDE, -87.63);
            _tc.Data["tracking_event_latitude"] = latitude.ToString(CultureInfo.InvariantCulture);
            _tc.Data["tracking_event_longitude"] = longitude.ToString(CultureInfo.InvariantCulture);
            LogReusableTrackingContext("Before PushTrackingEvent");
            var shipmentTrackingId = shipmentReference;
            var payload = JsonSerializer.Serialize(new
            {
                @event = new
                {
                    shipment_id = shipmentTrackingId,
                    details = new
                    {
                        latitude,
                        longitude,
                        pos_name = eventName,
                        transport_mode = 10,
                        pos_datetime = now.ToString("yyyy-MM-dd HH:mm:ss"),
                        message = $"New coordinates added - Container {containerTrackingId}",
                        container_tracking_id = containerTrackingId
                    },
                    code = 20,
                    walltime = now.ToString("yyyy-MM-ddTHH:mm+0000"),
                    created = now.ToString("yyyy-MM-ddTHH:mm+0000")
                },
                event_class = "ContainerShipmentEvent",
                generated = now.ToString("yyyy-MM-ddTHH:mm+0000")
            });

            Console.WriteLine(
                $"PushTrackingEvent => shipment_reference:{shipmentTrackingId}, container_id_1:{containerTrackingId}, " +
                $"lat:{latitude.ToString(CultureInfo.InvariantCulture)}, lon:{longitude.ToString(CultureInfo.InvariantCulture)}");

            var client = PortalApiClient.FromEnvironment();
            _trackingEventResponse = await client.PushTrackingEventAsync(project44Token, organizationConnectionId, payload);
            _trackingEventBody = _trackingEventResponse == null
                ? ""
                : await _trackingEventResponse.Content.ReadAsStringAsync();

            if (_trackingEventResponse != null)
            {
                Console.WriteLine(
                    $"PushTrackingEvent response => status:{(int)_trackingEventResponse.StatusCode} {_trackingEventResponse.ReasonPhrase}");
            }

            _trackingEventTransactionId = TryReadFirstId(_trackingEventBody, "transaction_id", "transactionId");
            if (!string.IsNullOrWhiteSpace(_trackingEventTransactionId))
            {
                _tc.Data["tracking_event_transaction_id"] = _trackingEventTransactionId;
                Console.WriteLine($"PushTrackingEvent transaction_id => {_trackingEventTransactionId}");
            }
            else
            {
                Console.WriteLine("PushTrackingEvent transaction_id => <not returned>");
            }
        }

        [Then("the tracking event request should succeed")]
        public void ThenTheTrackingEventRequestShouldSucceed()
        {
            Assert.IsNotNull(_trackingEventResponse, "Tracking event response is null.");
            Assert.AreEqual(
                201,
                (int)_trackingEventResponse!.StatusCode,
                $"Push tracking event expected HTTP 201. Got: {(int)_trackingEventResponse.StatusCode} {_trackingEventResponse.ReasonPhrase}. Body: {_trackingEventBody}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(_trackingEventTransactionId),
                $"Push tracking event did not return transaction_id. Body: {_trackingEventBody}");
        }

        [When("I select the subscribed container in Shipment Summary")]
        public async Task WhenISelectTheSubscribedContainerInShipmentSummary()
        {
            var containerToSelect = GetRequiredContextValue("shipment.containerUniqueID", "shipment.containerUniqueID not found.");

            await _shipmentPage.SelectContainerInShipmentSummaryAsync(containerToSelect);
        }

        [Given("I refresh the page")]
        [When("I refresh the page")]
        [Then("I refresh the page")]
        public async Task WhenIRefreshThePage()
        {
            await _shipmentPage.RefreshCurrentPageAsync();
        }

        [Then("I Check that Container LiveTrack and map coordinates are displayed")]
        public async Task ThenTheMapShouldDisplayTrackingPointsForTheSelectedContainer()
        {
            var containerForUi = GetRequiredContextValue("shipment.containerUniqueID", "shipment.containerUniqueID not found.");

            var latRaw = GetRequiredContextValue("tracking_event_latitude", "Tracking latitude not found.");
            var lonRaw = GetRequiredContextValue("tracking_event_longitude", "Tracking longitude not found.");
            var latitude = double.Parse(latRaw, CultureInfo.InvariantCulture);
            var longitude = double.Parse(lonRaw, CultureInfo.InvariantCulture);
            var expectedPorts = _tc.Data.TryGetValue("expected_port_codes", out var portsValue)
                && portsValue is string portsText
                && !string.IsNullOrWhiteSpace(portsText)
                ? portsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();
            Console.WriteLine($"Summary validation containerUniqueID => {containerForUi}");

            await _shipmentPage.MapShouldDisplayTrackingPointsAsync(containerForUi, expectedPorts, latitude, longitude);
        }

        [When("I click on Shipment Tracking tab")]
        public async Task WhenIClickOnShipmentTrackingTab()
        {
            await _shipmentPage.IClickOnShipmentTrackingTab();
        }

        [Then("the Tracking Events section should display the latest container event")]
        public async Task ThenTheTrackingEventsSectionShouldDisplayTheLatestContainerEvent()
        {
            var eventName = GetRequiredContextValue("tracking_event_name", "Tracking event name not found.");
            var containerForUi = GetRequiredContextValue("shipment.containerUniqueID", "shipment.containerUniqueID not found.");
            var latRaw = GetRequiredContextValue("tracking_event_latitude", "Tracking latitude not found.");
            var lonRaw = GetRequiredContextValue("tracking_event_longitude", "Tracking longitude not found.");
            var latitude = double.Parse(latRaw, CultureInfo.InvariantCulture);
            var longitude = double.Parse(lonRaw, CultureInfo.InvariantCulture);
            await _shipmentPage.TrackingEventsShouldDisplayLatestContainerEventAsync(
                eventName,
                latitude,
                longitude,
                containerForUi);
        }

        [When("I Unsubscribe from a container with tracking already added")]
        [When(@"I Unsubscribe the container ""([^""]*)"" with tracking already added ""([^""]*)""")]
        public async Task WhenIUnsubscribeFromAContainerWithTrackingAlreadyAdded(string? shipmentIdArg = null, string? containerIdArg = null)
        {
            var shipmentId = ResolveShipmentIdForUnsubscribe(shipmentIdArg);
            var containerId = ResolveContainerIdForUnsubscribe(containerIdArg);
            _tc.Data["shipmentId"] = shipmentId;
            _tc.Data["container_id_1"] = containerId;
            _tc.Data["shipment.containerID"] = containerId;
            var hubToken = GetHubToken();
            var client = PortalApiClient.FromEnvironment();

            _unsubscribeContainerResponse = await client.UnsubscribeContainerAsync(hubToken, shipmentId, containerId);
            _unsubscribeContainerBody = _unsubscribeContainerResponse == null
                ? ""
                : await _unsubscribeContainerResponse.Content.ReadAsStringAsync();
        }

        private string ResolveShipmentIdForUnsubscribe(string? shipmentIdArg)
        {
            var candidate = NormalizeStepArgument(shipmentIdArg);
            if (string.IsNullOrWhiteSpace(candidate))
                return GetRequiredContextValue("shipmentId", "Shipment ID not found.");

            if (candidate.Equals("shipmentID", StringComparison.OrdinalIgnoreCase)
                || candidate.Equals("shipmentId", StringComparison.OrdinalIgnoreCase))
            {
                return GetRequiredContextValue("shipmentId", "Shipment ID not found.");
            }

            return candidate;
        }

        private string ResolveContainerIdForUnsubscribe(string? containerIdArg)
        {
            var candidate = NormalizeStepArgument(containerIdArg);
            if (string.IsNullOrWhiteSpace(candidate)
                || candidate.Equals("containerID", StringComparison.OrdinalIgnoreCase)
                || candidate.Equals("containerId", StringComparison.OrdinalIgnoreCase))
            {
                return _tc.Data.TryGetValue("container_id_1", out var c1Val)
                && c1Val is string c1Str
                && !string.IsNullOrWhiteSpace(c1Str)
                ? c1Str
                : GetRequiredContextValue("shipment.containerID", "Container ID not found.");
            }

            return candidate;
        }

        private static string NormalizeStepArgument(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            var trimmed = raw.Trim();
            if (trimmed.StartsWith("{", StringComparison.Ordinal) && trimmed.EndsWith("}", StringComparison.Ordinal) && trimmed.Length > 2)
                return trimmed[1..^1].Trim();
            return trimmed;
        }

        [Then("the container unsubscribe request should succeed")]
        public void ThenTheContainerUnsubscribeRequestShouldSucceed()
        {
            Assert.IsNotNull(_unsubscribeContainerResponse, "Unsubscribe container response is null.");
            Assert.IsTrue(
                _unsubscribeContainerResponse!.IsSuccessStatusCode,
                $"Unsubscribe container failed: {(int)_unsubscribeContainerResponse.StatusCode} {_unsubscribeContainerResponse.ReasonPhrase}. Body: {_unsubscribeContainerBody}");
        }

        [Then("unsubscribed container should not be available in Shipment Summary dropdown")]
        public async Task ThenUnsubscribedContainerShouldNotBeAvailableInShipmentSummaryDropdown()
        {
            var containerForUi = GetExpectedSummaryContainerForUi();

            await _shipmentPage.UnsubscribedContainerShouldNotBeVisibleInSummaryDropdownAsync(containerForUi);
        }

        [When("I unsubscribe current shipment from live tracking via API")]
        public async Task WhenIUnsubscribeCurrentShipmentFromLiveTrackingViaApi()
        {
            var shipmentId = GetRequiredContextValue("shipmentId", "Shipment ID not found.");
            var hubToken = GetHubToken();
            var client = PortalApiClient.FromEnvironment();

            _unsubscribeShipmentResponse = await client.UnsubscribeShipmentAsync(hubToken, shipmentId);
            _unsubscribeShipmentBody = _unsubscribeShipmentResponse == null
                ? ""
                : await _unsubscribeShipmentResponse.Content.ReadAsStringAsync();
        }

        [Then("the shipment unsubscribe request should succeed")]
        public void ThenTheShipmentUnsubscribeRequestShouldSucceed()
        {
            Assert.IsNotNull(_unsubscribeShipmentResponse, "Unsubscribe shipment response is null.");
            Assert.IsTrue(
                _unsubscribeShipmentResponse!.IsSuccessStatusCode,
                $"Unsubscribe shipment failed: {(int)_unsubscribeShipmentResponse.StatusCode} {_unsubscribeShipmentResponse.ReasonPhrase}. Body: {_unsubscribeShipmentBody}");
        }

        [Then("I Check the tracking is disabled for the shipment in the hub")]
        public async Task ThenCheckTheTrackingIsDisabledForTheShipmentInTheHub()
        {
            var shipmentReference = GetRequiredContextValue("shipment_reference", "Shipment reference not found.");

            await _shipmentHubPage.INavigatedToShipmentListInTheHub();
            await _shipmentHubPage.IClickOnCustomerReferenceInputFieldInTheHub();
            await _shipmentHubPage.IEnterTheShipmentNameInCustomReferenceFieldInTheHub(shipmentReference);
            await _shipmentHubPage.IClickOnSearchButtonInTheHub();
            await _shipmentHubPage.TheShipmentShouldAppearInTheSearchResultsInTheHub(shipmentReference);
            await _shipmentHubPage.OpenShipmentByReferenceAsync(shipmentReference);
            await _shipmentHubPage.CheckTheTrackingIsDisabledForTheShipmentAsync();
        }

        [When(@"I Check the email for ""([^""]*)"" with username ""([^""]*)""")]
        public async Task WhenICheckTheEmailForWithUsername(string emailAddress, string username)
        {
            var fromStep = (emailAddress ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(fromStep))
                throw new InvalidOperationException("Email is required in the step.");

            _notificationEmail = fromStep;
            var resolvedUsername = string.IsNullOrWhiteSpace(username)
                ? _notificationEmail
                : username.Trim();
            _notificationUsername = resolvedUsername;

            _tc.Data["notification_email"] = _notificationEmail;
            await RefreshNotificationEmailsAsync();
        }

        [Then(@"I should receive the notification ""([^""]*)"" status ""([^""]*)"" for shipment ""([^""]*)""")]
        public async Task ThenIShouldReceiveTheNotificationStatusForShipment(string notificationTextArg = "", string statusArg = "", string shipmentIdArg = "")
        {
            var expectedNotificationText = NormalizeStepArgument(notificationTextArg);
            var shipmentGuid = string.IsNullOrWhiteSpace(shipmentIdArg)
                ? GetRequiredContextValue("shipmentId", "Shipment GUID (shipmentId) not found in context.")
                : NormalizeStepArgument(shipmentIdArg);

            var expectedStatus = NormalizeStepArgument(statusArg);
            if (string.IsNullOrWhiteSpace(expectedStatus))
                throw new InvalidOperationException("Status is required in the step.");

            string? latestShipmentNotification = null;
            for (var attempt = 0; attempt < 6; attempt++)
            {
                await RefreshNotificationEmailsAsync();
                Console.WriteLine($"Notification assert poll {attempt + 1}/6 => emailsLoaded:{_latestNotificationEmailBodies.Count}, shipment:{shipmentGuid}, expectedStatus:{expectedStatus}");
                latestShipmentNotification = _latestNotificationEmailBodies.FirstOrDefault(body =>
                    (string.IsNullOrWhiteSpace(expectedNotificationText)
                        || body.Contains(expectedNotificationText, StringComparison.OrdinalIgnoreCase))
                    && body.Contains(shipmentGuid, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(latestShipmentNotification)
                    && latestShipmentNotification.Contains(expectedStatus, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Notification assert matched => shipment:{shipmentGuid}, status:{expectedStatus}");
                    return;
                }

                if (attempt < 5)
                    await Task.Delay(5000);
            }

            Assert.IsTrue(_latestNotificationEmailBodies.Count > 0,
                $"No emails from today found in the last checked messages for '{_notificationEmail}'.");

            Assert.IsNotNull(latestShipmentNotification,
                $"No email from today in the checked set contained notification text '{expectedNotificationText}' and shipment '{shipmentGuid}' after waiting 30 seconds.");

            Assert.IsTrue(latestShipmentNotification.Contains(expectedStatus, StringComparison.OrdinalIgnoreCase),
                $"The most recent email from today for shipment '{shipmentGuid}' did not contain status '{expectedStatus}' after waiting 30 seconds.");
        }

        [Then(@"I should receive an email with text ""([^""]*)"" in the body for shipment ""([^""]*)""")]
        public async Task ThenIShouldReceiveAnEmailWithTextInTheBodyForShipment(string textArg = "", string shipmentNameArg = "")
        {
            var expectedTexts = (NormalizeStepArgument(textArg) ?? string.Empty)
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            var shipmentName = string.IsNullOrWhiteSpace(shipmentNameArg)
                ? (_tc.Data.TryGetValue("shipmentName", out var ctxName) ? ctxName?.ToString() ?? "" : "")
                : NormalizeStepArgument(shipmentNameArg) ?? "";

            // Poll every 5 seconds for up to 60 seconds (12 attempts).
            const int maxAttempts = 12;
            string? latestShipmentEmail = null;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                await RefreshNotificationEmailsAsync();
                Console.WriteLine($"Email body assert poll {attempt + 1}/{maxAttempts} => emailsLoaded:{_latestNotificationEmailBodies.Count}, shipmentName:{shipmentName}");
                latestShipmentEmail = _latestNotificationEmailBodies.FirstOrDefault(body =>
                    expectedTexts.All(text => body.Contains(text, StringComparison.OrdinalIgnoreCase))
                    && (string.IsNullOrWhiteSpace(shipmentName) || body.Contains(shipmentName, StringComparison.OrdinalIgnoreCase)));

                if (!string.IsNullOrWhiteSpace(latestShipmentEmail))
                {
                    Console.WriteLine($"Email body assert matched => shipmentName:{shipmentName}, texts:{string.Join(" | ", expectedTexts)}");
                    return;
                }

                if (attempt < maxAttempts - 1)
                    await Task.Delay(5000);
            }

            Assert.IsTrue(_latestNotificationEmailBodies.Count > 0,
                $"No emails from today found in the last checked messages for '{_notificationEmail}'.");

            Assert.IsNotNull(latestShipmentEmail,
                expectedTexts.Length == 0
                    ? $"No email from today in the last 3 checked messages contained shipment '{shipmentName}' after waiting 60 seconds."
                    : $"No email from today in the last 3 checked messages contained texts '{string.Join(" | ", expectedTexts)}' and shipment '{shipmentName}' after waiting 60 seconds.");
        }

        /// <summary>
        /// Refreshes the last 3 emails and asserts that none contains both the shipmentName and all expectedTexts.
        /// Used for negative email verification (e.g. no notification was sent).
        /// </summary>
        public async Task ShouldNotReceiveEmailWithTextForShipmentAsync(string textArg, string shipmentNameArg)
        {
            var expectedTexts = (textArg ?? string.Empty)
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToArray();
            var shipmentName = shipmentNameArg ?? "";

            await RefreshNotificationEmailsAsync();
            var matchingEmail = _latestNotificationEmailBodies.FirstOrDefault(body =>
                expectedTexts.All(text => body.Contains(text, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrWhiteSpace(shipmentName) || body.Contains(shipmentName, StringComparison.OrdinalIgnoreCase)));

            Assert.IsNull(matchingEmail,
                $"Expected NO email containing texts '{string.Join(" | ", expectedTexts)}'" +
                (string.IsNullOrWhiteSpace(shipmentName) ? "" : $" and '{shipmentName}'") +
                $" but one was found. URL: checked inbox for '{_notificationEmail}'.");
        }

        private async Task RefreshNotificationEmailsAsync()
        {
            _latestNotificationEmailBodies.Clear();

            var domain = _notificationEmail.Split('@').LastOrDefault()?.Trim().ToLowerInvariant() ?? "";
            if (domain.Contains("yopmail"))
            {
                var inboxPage = new EmailInboxPage(_tc.Page!);
                await inboxPage.OpenYopmailInboxAsync(_notificationUsername);
                _latestNotificationEmailBodies.AddRange(await inboxPage.GetLatestYopmailEmailBodiesFromTodayAsync(maxMessages: 3));
                return;
            }

            var password = ResolveEnvValue(Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("EMAIL_PASSWORD is required for non-yopmail inbox providers.");

            _latestNotificationEmailBodies.AddRange(
                await EmailInboxPage.GetLatestImapEmailBodiesFromTodayAsync(
                    _notificationEmail,
                    _notificationUsername,
                    password,
                    maxMessages: 3));
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

        private string GetOrCreateInternalTrackingNumber()
        {
            if (_tc.Data.TryGetValue("shipment_reference", out var referenceValue)
                && referenceValue is string shipmentReference
                && !string.IsNullOrWhiteSpace(shipmentReference))
            {
                _tc.Data["shipment.internal_tracking_number"] = shipmentReference;
                return shipmentReference;
            }

            if (_tc.Data.TryGetValue("shipment.internal_tracking_number", out var value)
                && value is string trackingId
                && !string.IsNullOrWhiteSpace(trackingId))
            {
                return trackingId;
            }

            var fallback = $"S{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            _tc.Data["shipment.internal_tracking_number"] = fallback;
            return fallback;
        }

        private string GetExpectedSummaryContainerForUi()
        {
            return _tc.Data.TryGetValue("shipment.containerUniqueID", out var unique)
                && unique is string uniqueStr
                && !string.IsNullOrWhiteSpace(uniqueStr)
                ? uniqueStr
                : throw new InvalidOperationException("shipment.containerUniqueID not found.");
        }

        private async Task<(string containerId, string uniqueContainerId)> EnsureFirstContainerIdentifiersAsync(string shipmentId)
        {
            if (_tc.Data.TryGetValue("shipment.containerID", out var existing)
                && existing is string containerId
                && IsLikelyContainerRouteId(containerId)
                && _tc.Data.TryGetValue("shipment.containerUniqueID", out var uniqueExistingValue)
                && uniqueExistingValue is string uniqueExisting
                && !string.IsNullOrWhiteSpace(uniqueExisting))
            {
                return (containerId, uniqueExisting);
            }

            var portalToken = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();

            using (var containersResponse = await client.GetContainersAsync(portalToken, shipmentId))
            {
                var containersBody = await containersResponse.Content.ReadAsStringAsync();
                if (containersResponse.IsSuccessStatusCode
                    && TryReadFirstContainerIdentifiers(containersBody, out var containerIdFromPortal, out var uniqueIdFromPortal))
                {
                    if (string.IsNullOrWhiteSpace(uniqueIdFromPortal))
                        throw new InvalidOperationException("GetContainers did not return source_container_id / unique container identifier.");

                    _tc.Data["shipment.containerID"] = containerIdFromPortal;
                    _tc.Data["shipment.containerUniqueID"] = uniqueIdFromPortal;
                    return (containerIdFromPortal, uniqueIdFromPortal);
                }
            }

            throw new InvalidOperationException("Container identifiers not found in GetContainers response for current shipment.");
        }

        private async Task<(string containerId, string uniqueContainerId)?> TryGetContainerIdentifiersFromWebhookLogsAsync()
        {
            var webhookUser = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.CHAINIO_WEBHOOK_USERNAME) ?? "");
            if (string.IsNullOrWhiteSpace(webhookUser))
                return null;

            if (!_tc.Data.TryGetValue("transactionId", out var txValue) || txValue is not string transactionId || string.IsNullOrWhiteSpace(transactionId))
                return null;

            var client = PortalApiClient.FromEnvironment();
            const int maxAttempts = 8;
            const int delayMs = 2000;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var response = await client.GetWebhookLogAsync(webhookUser, transactionId);
                var body = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode == 404 && attempt < maxAttempts)
                {
                    await Task.Delay(delayMs);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                    return null;

                if (TryReadFirstContainerIdentifiers(body, out var containerId, out var uniqueContainerId))
                {
                    var unique = string.IsNullOrWhiteSpace(uniqueContainerId) ? containerId : uniqueContainerId;
                    return (containerId, unique);
                }

                if (attempt < maxAttempts)
                    await Task.Delay(delayMs);
            }

            return null;
        }

        private string GetRequiredContextValue(string key, string message)
        {
            if (_tc.Data.TryGetValue(key, out var value) && value is string id && !string.IsNullOrWhiteSpace(id))
                return id;

            throw new InvalidOperationException(message);
        }

        private static string TryReadFirstId(string json, params string[] keys)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (TryReadIdFromElement(doc.RootElement, keys, out var id))
                    return id;
            }
            catch (JsonException)
            {
            }

            return "";
        }

        private static bool TryReadIdFromElement(JsonElement element, string[] keys, out string id)
        {
            id = "";
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in keys)
                {
                    if (element.TryGetProperty(key, out var prop)
                        && (prop.ValueKind == JsonValueKind.String || prop.ValueKind == JsonValueKind.Number))
                    {
                        id = prop.ToString();
                        if (!string.IsNullOrWhiteSpace(id))
                            return true;
                    }
                }

                if (element.TryGetProperty("data", out var data) && TryReadIdFromElement(data, keys, out id))
                    return true;
                if (element.TryGetProperty("result", out var result) && TryReadIdFromElement(result, keys, out id))
                    return true;
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (TryReadIdFromElement(item, keys, out id))
                        return true;
                }
            }

            return false;
        }

        private static bool TryReadFirstContainerIdentifiers(string json, out string containerId, out string uniqueContainerId)
        {
            containerId = "";
            uniqueContainerId = "";
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(json);
                return TryReadFirstContainerIdentifiersFromElement(doc.RootElement, ref containerId, ref uniqueContainerId);
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private static bool TryReadFirstContainerIdentifiersFromElement(JsonElement element, ref string containerId, ref string uniqueContainerId)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var hasContainerFingerprint =
                    element.TryGetProperty("container_number", out _)
                    || element.TryGetProperty("unique_container_id", out _)
                    || element.TryGetProperty("uniqueContainerId", out _)
                    || element.TryGetProperty("source_container_id", out _)
                    || element.TryGetProperty("size_code", out _)
                    || element.TryGetProperty("type_code", out _)
                    || element.TryGetProperty("containerization_type", out _);

                var localId = TryReadPropertyValue(element, "id")
                              ?? TryReadPropertyValue(element, "container_id")
                              ?? TryReadPropertyValue(element, "containerId");

                var localUnique = TryReadPropertyValue(element, "source_container_id")
                                  ?? TryReadPropertyValue(element, "unique_container_id")
                                  ?? TryReadPropertyValue(element, "uniqueContainerId")
                                  ?? TryReadPropertyValue(element, "container_number");

                if (hasContainerFingerprint
                    && !string.IsNullOrWhiteSpace(localId)
                    && IsLikelyContainerRouteId(localId))
                {
                    containerId = localId;
                    uniqueContainerId = string.IsNullOrWhiteSpace(localUnique) ? localId : localUnique;
                    return true;
                }

                foreach (var prop in element.EnumerateObject())
                {
                    if (TryReadFirstContainerIdentifiersFromElement(prop.Value, ref containerId, ref uniqueContainerId))
                        return true;
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (TryReadFirstContainerIdentifiersFromElement(item, ref containerId, ref uniqueContainerId))
                        return true;
                }
            }

            return false;
        }

        private static string? TryReadPropertyValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var prop))
                return null;

            if (prop.ValueKind == JsonValueKind.String || prop.ValueKind == JsonValueKind.Number)
                return prop.ToString();

            return null;
        }

        private static bool IsLikelyContainerRouteId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // push-event expects real numeric container id (not unique code, not GUID).
            return int.TryParse(value, out _)
                   || long.TryParse(value, out _);
        }

        private static string ResolveShipmentTrackingId(
            string shipmentIdSource,
            string shipmentId,
            string internalTrackingNumber,
            string shipmentReference)
        {
            var source = (shipmentIdSource ?? "").Trim().ToLowerInvariant();

            switch (source)
            {
                case "reference":
                    return shipmentReference;
                case "shipment_id":
                case "id":
                    return shipmentId;
                case "internal":
                    return internalTrackingNumber;
                case "auto":
                    return internalTrackingNumber;
                default:
                    return internalTrackingNumber;
            }
        }

        private static string ResolveEnvValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmed = value.Trim();
            if (trimmed.StartsWith("Env.", StringComparison.OrdinalIgnoreCase))
            {
                var key = trimmed.Substring(4);
                return Environment.GetEnvironmentVariable(key) ?? "";
            }

            var indirect = Environment.GetEnvironmentVariable(trimmed);
            if (!string.IsNullOrWhiteSpace(indirect))
                return indirect;

            return value;
        }

        private void LogReusableTrackingContext(string title)
        {
            string[] keys =
            [
                "shipment_reference",
                "shipment.internal_tracking_number",
                "transactionId",
                "shipmentId",
                "container_id_1",
                "shipment.containerID",
                "shipment.containerUniqueID",
                "tracking_event_name",
                "tracking_event_latitude",
                "tracking_event_longitude"
            ];

            Console.WriteLine($"=== Tracking Context ({title}) ===");
            foreach (var key in keys.Take(6))
            {
                if (_tc.Data.TryGetValue(key, out var value) && value is string text && !string.IsNullOrWhiteSpace(text))
                    Console.WriteLine($"{key}={text}");
            }
            Console.WriteLine("=== End Tracking Context ===");
        }

        private string? TryReadPushEventBackendStatus(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                return null;

            var client = PortalApiClient.FromEnvironment();
            const int maxAttempts = 3;
            const int delayMs = 1200;

            foreach (var webhookUser in GetWebhookLogUserCandidates())
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    using var response = client.GetWebhookLogAsync(webhookUser, transactionId).GetAwaiter().GetResult();
                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if ((int)response.StatusCode == 401)
                        break;

                    if ((int)response.StatusCode == 404)
                    {
                        if (attempt < maxAttempts)
                        {
                            Task.Delay(delayMs).GetAwaiter().GetResult();
                            continue;
                        }
                        return null;
                    }

                    var status = TryReadFirstId(body, "status", "state");
                    if (string.Equals(status, "pending", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(status, "processing", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(status, "queued", StringComparison.OrdinalIgnoreCase))
                    {
                        if (attempt < maxAttempts)
                        {
                            Task.Delay(delayMs).GetAwaiter().GetResult();
                            continue;
                        }
                    }

                    return status;
                }
            }

            return null;
        }

        private string[] GetWebhookLogUserCandidates()
        {
            var configuredTrackingWebhookUser = ResolveEnvValue(Environment.GetEnvironmentVariable("TRACKING_WEBHOOK_USERNAME") ?? "");
            var chainIoWebhookUser = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.CHAINIO_WEBHOOK_USERNAME) ?? "");
            var orgConnectionFromEnv = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.ORGANIZATION_CONNECTION_ID) ?? "");
            var orgConnection = string.IsNullOrWhiteSpace(_trackingEventOrganizationConnectionId)
                ? orgConnectionFromEnv
                : _trackingEventOrganizationConnectionId;

            var webhookUserCandidates = new System.Collections.Generic.List<string>();
            void AddUser(string? user)
            {
                if (string.IsNullOrWhiteSpace(user))
                    return;
                if (!webhookUserCandidates.Contains(user, StringComparer.OrdinalIgnoreCase))
                    webhookUserCandidates.Add(user);
            }

            AddUser(configuredTrackingWebhookUser);
            AddUser(orgConnection);
            AddUser(chainIoWebhookUser);
            return webhookUserCandidates.ToArray();
        }

        private static double ResolveDoubleEnv(string key, double fallback)
        {
            var raw = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(raw))
                return fallback;

            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                return parsed;

            throw new InvalidOperationException(
                $"Environment variable '{key}' must be a valid number (InvariantCulture). Got '{raw}'.");
        }

        private void ValidatePushEventBackendProcessing()
        {
            if (string.IsNullOrWhiteSpace(_trackingEventTransactionId))
            {
                Console.WriteLine("[WARN] Push-event backend validation skipped: transaction_id not found in response.");
                return;
            }

            var webhookUserCandidates = GetWebhookLogUserCandidates();

            if (webhookUserCandidates.Length == 0)
            {
                Console.WriteLine("[WARN] Push-event backend validation skipped: no webhook log username candidate (TRACKING_WEBHOOK_USERNAME/ORGANIZATION_CONNECTION_ID/CHAINIO_WEBHOOK_USERNAME).");
                return;
            }

            var client = PortalApiClient.FromEnvironment();
            const int maxAttempts = 8;
            const int delayMs = 2000;

            foreach (var webhookUser in webhookUserCandidates)
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    using var response = client.GetWebhookLogAsync(webhookUser, _trackingEventTransactionId).GetAwaiter().GetResult();
                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine($"PushEvent log poll {attempt}/{maxAttempts} with user '{webhookUser}' => status:{(int)response.StatusCode}");

                    if ((int)response.StatusCode == 401)
                    {
                        // Wrong webhook user for this transaction type. Try next candidate.
                        Console.WriteLine($"PushEvent log auth failed with user '{webhookUser}'. Trying next credential candidate.");
                        break;
                    }

                    if ((int)response.StatusCode == 404)
                    {
                        if (attempt < maxAttempts)
                        {
                            Task.Delay(delayMs).GetAwaiter().GetResult();
                            continue;
                        }

                        Console.WriteLine("[WARN] Push-event backend log not found (404) after retries.");
                        return;
                    }

                    Console.WriteLine($"PushEvent log body => {body}");

                    var status = TryReadFirstId(body, "status", "state");
                    if (string.Equals(status, "pending", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(status, "processing", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(status, "queued", StringComparison.OrdinalIgnoreCase))
                    {
                        if (attempt < maxAttempts)
                        {
                            Task.Delay(delayMs).GetAwaiter().GetResult();
                            continue;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(status)
                        && !string.Equals(status, "completed", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(status, "success", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Fail(
                            $"Push-event transaction '{_trackingEventTransactionId}' finished with status '{status}'. " +
                            $"Body: {body}");
                    }

                    var errorSummary = TryExtractErrorSummary(body);
                    if (!string.IsNullOrWhiteSpace(errorSummary))
                    {
                        Assert.Fail($"Push-event transaction '{_trackingEventTransactionId}' shows backend processing error: {errorSummary}");
                    }

                    return;
                }
            }

            Console.WriteLine("[WARN] Push-event backend validation could not read logs due authorization (401) with all available username candidates.");
        }

        private static string TryExtractErrorSummary(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                var sb = new StringBuilder();
                CollectErrorTokens(doc.RootElement, sb);
                return sb.ToString().Trim();
            }
            catch (JsonException)
            {
                return "";
            }
        }

        private static void CollectErrorTokens(JsonElement element, StringBuilder sb)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    var key = prop.Name.ToLowerInvariant();
                    if (key.Contains("error") || key.Contains("fail") || key.Contains("reject"))
                    {
                        var value = prop.Value.ToString();
                        var lowerValue = value.ToLowerInvariant();
                        var meaningful = !(string.IsNullOrWhiteSpace(value)
                                           || lowerValue == "false"
                                           || lowerValue == "0"
                                           || lowerValue == "null");
                        if (meaningful)
                        {
                            if (sb.Length > 0) sb.Append(" | ");
                            sb.Append($"{prop.Name}:{value}");
                        }
                    }

                    if ((key == "status" || key == "state") && prop.Value.ValueKind == JsonValueKind.String)
                    {
                        var value = prop.Value.GetString() ?? "";
                        var lower = value.ToLowerInvariant();
                        if (lower.Contains("error") || lower.Contains("failed") || lower.Contains("rejected"))
                        {
                            if (sb.Length > 0) sb.Append(" | ");
                            sb.Append($"{prop.Name}:{value}");
                        }
                    }

                    CollectErrorTokens(prop.Value, sb);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                    CollectErrorTokens(item, sb);
            }
        }
    }
}
