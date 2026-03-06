using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentHubStepDefinitions(ShipmentHubPage shipmentHubPage, ShipmentPage shipmentPage)
    {
        [Given("I navigated to shipment List in the Hub")]
        public async Task INavigatedToShipmentListInTheHub()
            => await shipmentHubPage.INavigatedToShipmentListInTheHub();

        [When("I click on Customer Reference input field in the Hub")]
        public async Task IClickOnCustomerReferenceInputFieldInTheHub()
            => await shipmentHubPage.IClickOnCustomerReferenceInputFieldInTheHub();

        [When("I enter the shipment name in Customer Reference field in the Hub")]
        public async Task IEnterTheShipmentNameInCustomerReferenceFieldInTheHub()
        {
            await shipmentHubPage.IEnterTheShipmentNameInCustomReferenceFieldInTheHub(shipmentPage.GetShipmentName());
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
    }
}
