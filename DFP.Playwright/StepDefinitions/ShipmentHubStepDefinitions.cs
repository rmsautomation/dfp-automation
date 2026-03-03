using System.Threading.Tasks;
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
    }
}
