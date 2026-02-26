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

        // ── ShipmentSearch steps ─────────────────────────────────────────────────

        [Given("user navigated to Shipments List")]
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

        [When("user clicks the tag icon on the shipment")]
        public async Task UserClicksTheTagIconOnTheShipment()
        {
            await _shipmentPage.UserClicksTheTagIcon();
        }

        [Then("a field should appear to select an existing tag or create a new tag")]
        public async Task AFieldShouldAppearToSelectOrCreateTag()
        {
            await _shipmentPage.ATagInputFieldShouldAppear();
        }

        [When("user creates and assigns a new tag to the shipment")]
        public async Task UserCreatesAndAssignsANewTagToTheShipment()
        {
            var tagName = $"{DateTime.UtcNow:MMddHHmmss}auto";
            await _shipmentPage.UserCreatesAndAssignsNewTag(tagName);
        }

        [When("user assigns the existing tag to the shipment")]
        public async Task UserAssignsTheExistingTagToTheShipment()
        {
            await _shipmentPage.UserAssignsExistingTagToShipment();
        }

        [Then("the tag should be visible on the selected shipment")]
        public async Task TheTagShouldBeVisibleOnTheSelectedShipment()
        {
            await _shipmentPage.TheTagShouldBeVisibleOnTheShipment();
        }

        [When("user opens the tagged shipment details view")]
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
    }
}
