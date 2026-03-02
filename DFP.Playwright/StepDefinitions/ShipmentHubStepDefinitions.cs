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

        [When("I click on Shipment Reference input field in the Hub")]
        public async Task IClickOnShipmentReferenceInputFieldInTheHub()
            => await shipmentHubPage.IClickOnShipmentReferenceInputFieldInTheHub();

        [When("I enter the shipment name in Shipment Reference field in the Hub")]
        public async Task IEnterTheShipmentNameInShipmentReferenceFieldInTheHub()
            => await shipmentHubPage.IEnterTheShipmentNameInShipmentReferenceFieldInTheHub(shipmentPage.GetShipmentName());

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
