using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentHubStepDefinitions(ShipmentHubPage shipmentHubPage, ShipmentPage shipmentPage, DFP.Playwright.Support.TestContext tc)
    {
        private readonly DFP.Playwright.Support.TestContext _tc = tc;

        [Given("I navigated to shipment List in the Hub")]
        public async Task INavigatedToShipmentListInTheHub()
            => await shipmentHubPage.INavigatedToShipmentListInTheHub();

        [When("I click on Customer Reference input field in the Hub")]
        public async Task IClickOnCustomerReferenceInputFieldInTheHub()
            => await shipmentHubPage.IClickOnCustomerReferenceInputFieldInTheHub();

        [When("I enter the shipment name in Customer Reference field in the Hub")]
        public async Task IEnterTheShipmentNameInCustomerReferenceFieldInTheHub()
        {
            var shipmentRef = shipmentPage.GetShipmentName();
            if (string.IsNullOrWhiteSpace(shipmentRef))
            {
                // API-created shipments store their searchable reference in context.
                // Use it when no UI-created shipment name was captured.
                shipmentRef = _tc.Data.TryGetValue("shipment_reference", out var refVal)
                    ? refVal as string ?? ""
                    : "";
            }

            await shipmentHubPage.IEnterTheShipmentNameInCustomReferenceFieldInTheHub(shipmentRef);
            await Task.Delay(5000); // Temporary delay to allow for API complete unhide Shipment.
        }
        [When("I click on Search button in the hub")]
        public async Task IClickOnSearchButtonInTheHub()
            => await shipmentHubPage.IClickOnSearchButtonInTheHub();

        [Then("the shipment should appear in the search results in the hub")]
        public async Task TheShipmentShouldAppearInTheSearchResultsInTheHub()
            => await shipmentHubPage.TheShipmentShouldAppearInTheSearchResultsInTheHub(shipmentPage.GetShipmentName());

        [Then("the shipment should NOT appear in the search results in the hub")]
        public async Task TheShipmentShouldNotAppearInTheSearchResultsInTheHub()
            => await shipmentHubPage.TheShipmentShouldNotAppearInTheSearchResultsInTheHub(shipmentPage.GetShipmentName());

        [When("I select the created Shipment")]
        [Then("I select the created Shipment")]
        [Given("I select the created Shipment")]
        public async Task ISelectTheCreatedShipment()
            => await shipmentHubPage.SelectCreatedShipmentAsync();

        [When("I go to Milestones tab")]
        public async Task IGoToMilestonesTab()
            => await shipmentHubPage.GoToMilestonesTabAsync();

        [Then("I click on Edit button related to {string}")]
        public async Task IClickOnEditButtonRelatedTo(string milestoneName)
            => await shipmentHubPage.ClickEditButtonForMilestoneAsync(milestoneName);

        [When("I click on the calendar button")]
        public async Task IClickOnTheCalendarButton()
            => await shipmentHubPage.ClickCalendarButtonInMilestoneAsync();

        // If the date parameter is "date" (literal keyword from the feature), today's date is used.
        // Otherwise, the provided date string is parsed (ISO or MM/dd/yyyy format).
        [Then("I should select the {string} in the calendar")]
        public async Task IShouldSelectTheDateInTheCalendar(string dateParam)
            => await shipmentHubPage.SelectDateInCalendarAsync(dateParam);

        // Selects the date 7 days ahead relative to dateParam (or today + 7 if dateParam is "date"/empty).
        [Then("I should select the next week {string} in the calendar")]
        public async Task IShouldSelectTheNextDateInTheCalendar(string dateParam)
            => await shipmentHubPage.SelectNextDateInCalendarAsync(dateParam);

        [When("I click on save changes button")]
        public async Task IClickOnSaveChangesButton()
            => await shipmentHubPage.ClickSaveChangesButtonAsync();

        // If the date parameter is "date" (literal keyword from the feature), uses the date stored during selection.
        [Then("I should see the {string} in the Milestone tab")]
        public async Task IShouldSeeTheDateInTheMilestoneTab(string dateParam)
            => await shipmentHubPage.ShouldSeeDateInMilestoneTabAsync(dateParam);

        // Verifies the date 7 days ahead (stored date + 7) is visible in the milestone tab.
        [Then("I should see the next week {string} in the Milestone tab")]
        public async Task IShouldSeeTheNextDateInTheMilestoneTab(string dateParam)
            => await shipmentHubPage.ShouldSeeNextDateInMilestoneTabAsync(dateParam);

            // ── Milestone confirmation steps ──────────────────────────────────────────

        [Then("I click on Confirm button from {string} section")]
        public async Task ThenIClickOnConfirmButtonFromSection(string sectionName)
            => await shipmentHubPage.ClickConfirmButtonInSectionAsync(sectionName);

        [Then("I should see the Confirmation Page")]
        [When("I should see the Confirmation Page")]
        public async Task ThenIShouldSeeTheConfirmationPage()
            => await shipmentHubPage.ShouldSeeConfirmationPageAsync();

        [Then("I select the Actual {string} in the calendar")]
        [When("I select the Actual {string} in the calendar")]
        [Then("I select the Actual date {string} in the calendar")]
        [When("I select the Actual date {string} in the calendar")]
        public async Task ThenISelectTheActualDateInTheCalendar(string date)
            => await shipmentHubPage.SelectActualDateInCalendarAsync(date);

        [Then("I select the Expected {string} in the calendar")]
        [When("I select the Expected {string} in the calendar")]
        [Then("I select the Expected date {string} in the calendar")]
        [When("I select the Expected date {string} in the calendar")]
        public async Task ThenISelectTheExpectedDateInTheCalendar(string date)
            => await shipmentHubPage.SelectExpectedDateInCalendarAsync(date);

        [Then("I should see the green icon")]
        public async Task ThenIShouldSeeTheGreenIcon()
            => await shipmentHubPage.ShouldSeeGreenIconAsync();
    }
}
