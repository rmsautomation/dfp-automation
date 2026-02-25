using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly ShipmentPage _shipmentPage;

        public ShipmentStepDefinitions(DFP.Playwright.Support.TestContext tc, ShipmentPage shipmentPage)
        {
            _tc = tc;
            _shipmentPage = shipmentPage;
        }

        [Given("I am on the Quotations List page")]
        public async Task IAmOnTheQuotationsListPage()
        {
            await _shipmentPage.IAmOnTheQuotationsListPage();
        }

        [When("I open the first  quotation in Status Booked")]
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

        [When("clicks on Book Now button")]
        public async Task ClicksOnBookNowButton()
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

        [Then("I should click on Go To Shipment button to see the shipemnt")]
        public async Task IShouldClickOnGoToShipmentButtonToSeeTheShipemnt()
        {
            await _shipmentPage.IShouldClickOnGoToShipmentButtonToSeeTheShipemnt();
        }

        [Then("the shipment should display the shipment name")]
        public async Task TheShipmentShouldDisplayTheShipmentName()
        {
            await _shipmentPage.TheShipmentShouldDisplayTheShipmentName();
        }
    }
}
