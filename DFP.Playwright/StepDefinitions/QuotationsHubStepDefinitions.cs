using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class QuotationsHubStepDefinitions
    {
        private readonly QuotationsHubPage _quotationsHubPage;
        private readonly QuotationPage _quotationPage;
        private readonly ShipmentTrackingStepDefinitions _emailSteps;

        public QuotationsHubStepDefinitions(QuotationsHubPage quotationsHubPage, QuotationPage quotationPage, ShipmentTrackingStepDefinitions emailSteps)
        {
            _quotationsHubPage = quotationsHubPage;
            _quotationPage = quotationPage;
            _emailSteps = emailSteps;
        }

        // ── TC129: Hub Create Quotation (Full Load-Ocean) ─────────────────────────

        [Given("I click on Create Quotation button in the Hub")]
        [When("I click on Create Quotation button in the Hub")]
        [Then("I click on Create Quotation button in the Hub")]
        public async Task IClickOnCreateQuotationButtonInTheHub()
        {
            await _quotationsHubPage.ClickCreateQuotationButtonInHubAsync();
        }

        [Then("I should see the create quotation page in the Hub")]
        public async Task IShouldSeeTheCreateQuotationPageInHub()
        {
            await _quotationsHubPage.ShouldSeeCreateQuotationPageInHubAsync();
        }

        [Given("I select the Customer {string} in the Hub")]
        [When("I select the Customer {string} in the Hub")]
        [Then("I select the Customer {string} in the Hub")]
        public async Task ISelectTheCustomerInHub(string customer)
        {
            await _quotationsHubPage.SelectCustomerInHubAsync(customer);
        }

        [Given("I select the Load Type {string} in the Hub")]
        [When("I select the Load Type {string} in the Hub")]
        [Then("I select the Load Type {string} in the Hub")]
        public async Task ISelectTheLoadTypeInHub(string loadType)
        {
            await _quotationsHubPage.SelectLoadTypeInHubAsync(loadType);
        }

        [Given("I select the modality {string} in the Hub")]
        [When("I select the modality {string} in the Hub")]
        [Then("I select the modality {string} in the Hub")]
        public async Task ISelectTheModalityInHub(string modality)
        {
            await _quotationsHubPage.SelectModalityInHubAsync(modality);
        }

        [Given("I enter the Origin {string} in the Hub")]
        [When("I enter the Origin {string} in the Hub")]
        [Then("I enter the Origin {string} in the Hub")]
        public async Task IEnterTheOriginInHub(string origin)
        {
            await _quotationsHubPage.EnterOriginInHubAsync(origin);
        }

        [Given("I enter the Destination {string} in the Hub")]
        [When("I enter the Destination {string} in the Hub")]
        [Then("I enter the Destination {string} in the Hub")]
        public async Task IEnterTheDestinationInHub(string destination)
        {
            await _quotationsHubPage.EnterDestinationInHubAsync(destination);
        }

        [Given("I click on continue button in the Hub")]
        [When("I click on continue button in the Hub")]
        [Then("I click on continue button in the Hub")]
        public async Task IClickOnContinueButtonInTheHub()
        {
            await _quotationsHubPage.ClickContinueButtonInHubAsync();
        }

        [Given("I select the Package {string} in the Hub")]
        [When("I select the Package {string} in the Hub")]
        [Then("I select the Package {string} in the Hub")]
        public async Task ISelectThePackageInHub(string package)
        {
            await _quotationsHubPage.SelectPackageInHubAsync(package);
        }

        [Given("I enter the following cargo details in the Hub:")]
        [When("I enter the following cargo details in the Hub:")]
        [Then("I enter the following cargo details in the Hub:")]
        public async Task IEnterTheFollowingCargoDetailsInHub(Table table)
        {
            var row = table.Rows[0];
            await _quotationsHubPage.EnterCargoDetailsInHubAsync(
                weight: row["Weight"],
                length: row["Length"],
                width:  row["Width"],
                height: row["Height"]
            );
        }

        [Then("I should select the commodity in the Hub")]
        public async Task IShouldSelectTheCommodityInHub()
        {
            await _quotationsHubPage.ShouldSeeCommoditySectionInHubAsync();
        }

        [Then("I should see the quotation page to enter the details in the Hub")]
        public async Task IShouldSeeTheQuotationDetailsPageInHub()
        {
            await _quotationsHubPage.ShouldSeeQuotationDetailsPageInHubAsync();
        }

        [Given("I select the commodity {string} in the Hub")]
        [When("I select the commodity {string} in the Hub")]
        [Then("I select the commodity {string} in the Hub")]
        public async Task ISelectTheCommodityInHub(string commodity)
        {
            await _quotationsHubPage.SelectCommodityInHubAsync(commodity);
        }

        [Given("I select the currency {string} in the Hub")]
        [When("I select the currency {string} in the Hub")]
        [Then("I select the currency {string} in the Hub")]
        public async Task ISelectTheCurrencyInHub(string currency)
        {
            await _quotationsHubPage.SelectCurrencyInHubAsync(currency);
        }

        [Given("I select the container Size {string} in the Hub")]
        [When("I select the container Size {string} in the Hub")]
        [Then("I select the container Size {string} in the Hub")]
        public async Task ISelectTheContainerSizeInHub(string size)
        {
            await _quotationsHubPage.SelectContainerSizeInHubAsync(size);
        }

        [Given("I select additionals {string} in the Hub")]
        [When("I select additionals {string} in the Hub")]
        [Then("I select additionals {string} in the Hub")]
        public async Task ISelectAdditionalsInHub(string additional)
        {
            await _quotationsHubPage.SelectAdditionalInHubAsync(additional);
        }

        [Given("I store the quote id in the Hub")]
        [When("I store the quote id in the Hub")]
        [Then("I store the quote id in the Hub")]
        public async Task IStoreTheQuoteIdInTheHub()
        {
            await _quotationsHubPage.StoreQuoteIdInHubAsync();
        }

        [Then("I should see the quote id in the notifications")]
        public async Task IShouldSeeTheQuoteIdInTheNotifications()
        {
            var quoteId = _quotationsHubPage.GetQuoteId();
            await _quotationPage.ShouldSeeQuoteIdInNotificationsAsync(quoteId);
        }

        [Then("I should not see the quote id in the notifications")]
        public async Task IShouldNotSeeTheQuoteIdInTheNotifications()
        {
            var quoteId = _quotationsHubPage.GetQuoteId();
            await _quotationPage.ShouldNotSeeQuoteIdInNotificationsAsync(quoteId);
        }

        [Then("I should not receive a quotation email with the stored quote id and text {string}")]
        public async Task IShouldNotReceiveAQuotationEmailWithTheStoredQuoteIdAndText(string text)
        {
            var quoteId = _quotationsHubPage.GetQuoteId();
            var expectedText = string.IsNullOrWhiteSpace(text) ? "" : text;
            await _emailSteps.ShouldNotReceiveEmailWithTextForShipmentAsync(expectedText, quoteId);
        }

        [Then("I should receive a quotation email with the stored quote id and text {string}")]
        public async Task IShouldReceiveAQuotationEmailWithTheStoredQuoteIdAndText(string text)
        {
            var quoteId = _quotationsHubPage.GetQuoteId();
            // Pass quoteId as shipmentName so the existing method uses it as a body filter (no context lookup needed).
            // If additional text is provided, include it as expected text alongside the quoteId.
            var expectedText = string.IsNullOrWhiteSpace(text) ? "" : text;
            await _emailSteps.ThenIShouldReceiveAnEmailWithTextInTheBodyForShipment(expectedText, quoteId);
        }

        [Then("I should see the quotation in {string} status in the Hub")]
        public async Task IShouldSeeTheQuotationInStatusInHub(string status)
        {
            await _quotationsHubPage.ShouldSeeQuotationInStatusInHubAsync(status);
        }

        [Given("I click on Publish Quotation in the Hub")]
        [When("I click on Publish Quotation in the Hub")]
        [Then("I click on Publish Quotation in the Hub")]
        [Given("I click on Publish Quotation in the hub")]
        [When("I click on Publish Quotation in the hub")]
        [Then("I click on Publish Quotation in the hub")]
        public async Task IClickOnPublishQuotationInTheHub()
        {
            await _quotationsHubPage.ClickPublishQuotationInHubAsync();
        }

        [Then("I should see offers in the Hub")]
        public async Task IShouldSeeOffersInHub()
        {
            await _quotationsHubPage.ShouldSeeOffersInHubAsync();
        }

        [Given("I click on Yes button in the Hub")]
        [When("I click on Yes button in the Hub")]
        [Then("I click on Yes button in the Hub")]
        [Given("I click on Yes button in the hub")]
        [When("I click on Yes button in the hub")]
        [Then("I click on Yes button in the hub")]
        public async Task IClickOnYesButtonInTheHub()
        {
            await _quotationsHubPage.ClickYesButtonInHubAsync();
        }

        [Given("I click on Create Quotation in the Hub")]
        [When("I click on Create Quotation in the Hub")]
        [Then("I click on Create Quotation in the Hub")]
        public async Task IClickOnCreateQuotationInTheHub()
        {
            await _quotationsHubPage.ClickCreateQuotationFinalInHubAsync();
        }

        // ── TC145: Hub quotation search ───────────────────────────────────────────

        [Given("I navigated to quotation List in the Hub")]
        public async Task INavigatedToQuotationListInTheHub()
        {
            await _quotationsHubPage.NavigateToQuotationListInHubAsync();
        }

        [When("I click on system id input field in the Hub")]
        public async Task IClickOnSystemIdInputFieldInTheHub()
        {
            await _quotationsHubPage.ClickSystemIdInputInHubAsync();
        }

        [When("I enter the quote id in field in the Hub")]
        [Then("I enter the quote id in field in the Hub")]
        public async Task IEnterTheQuoteIdInFieldInTheHub()
        {
            // Sync quote ID from Portal page if the Hub page has no ID stored (cross-context scenario)
            if (string.IsNullOrEmpty(_quotationsHubPage.GetQuoteId()))
                _quotationsHubPage.SetQuoteId(_quotationPage.GetQuoteId());
            await _quotationsHubPage.EnterQuoteIdInHubAsync();
        }

        [Then("the quote should appear in the search results in the hub")]
        public async Task TheQuoteShouldAppearInTheSearchResultsInTheHub()
        {
            // Sync quote ID from Portal page if the Hub page has no ID stored (cross-context scenario)
            if (string.IsNullOrEmpty(_quotationsHubPage.GetQuoteId()))
                _quotationsHubPage.SetQuoteId(_quotationPage.GetQuoteId());
            await _quotationsHubPage.QuoteShouldAppearInHubResultsAsync();
        }

        [Then("the status should be {string}")]
        public async Task TheStatusShouldBe(string status)
        {
            await _quotationsHubPage.HubStatusShouldBeAsync(status);
        }


        // ── TC162: Requests > Change Status to Closed ─────────────────────────────

        [Given("I filter quotations by status {string} in the Hub")]
        [When("I filter quotations by status {string} in the Hub")]
        [Then("I filter quotations by status {string} in the Hub")]
        public async Task IFilterQuotationsByStatusInHub(string status)
        {
            await _quotationsHubPage.FilterQuotationsByStatusInHubAsync(status);
        }

        [Given("I click on Search button in the Hub")]
        [When("I click on Search button in the Hub")]
        [Then("I click on Search button in the Hub")]
        public async Task IClickOnSearchButtonInHub()
        {
            await _quotationsHubPage.ClickSearchButtonInHubAsync();
        }

        [Given("I select the first quotation in the Hub")]
        [When("I select the first quotation in the Hub")]
        [Then("I select the first quotation in the Hub")]
        public async Task ISelectTheFirstQuotationInHub()
        {
            await _quotationsHubPage.SelectFirstQuotationInHubAsync();
        }

        [Given("I navigate to Requests tab in the Hub")]
        [When("I navigate to Requests tab in the Hub")]
        [Then("I navigate to Requests tab in the Hub")]
        public async Task INavigateToRequestsTabInHub()
        {
            await _quotationsHubPage.NavigateToRequestsTabInHubAsync();
        }

        [Given("I click on Close request button in the Hub")]
        [When("I click on Close request button in the Hub")]
        [Then("I click on Close request button in the Hub")]
        public async Task IClickOnCloseRequestButtonInHub()
        {
            await _quotationsHubPage.ClickCloseRequestButtonInHubAsync();
        }

        [Given("I enter the close reason {string} in the Hub")]
        [When("I enter the close reason {string} in the Hub")]
        [Then("I enter the close reason {string} in the Hub")]
        public async Task IEnterTheCloseReasonInHub(string reason)
        {
            await _quotationsHubPage.EnterCloseReasonInHubAsync(reason);
        }

        [Then("I should see the first request in {string} status in the Hub")]
        public async Task IShouldSeeTheFirstRequestInStatusInHub(string status)
        {
            await _quotationsHubPage.ShouldSeeFirstRequestInStatusInHubAsync(status);
        }
    }
}
