using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class QuotationStepDefinitions
    {
        private readonly QuotationPage _quotationPage;

        public QuotationStepDefinitions(QuotationPage quotationPage)
        {
            _quotationPage = quotationPage;
        }

        // ── Navigation steps ──────────────────────────────────────────────────────

        [Given("I am on the Quotations List page")]
        public async Task IAmOnTheQuotationsListPage()
        {
            await _quotationPage.NavigateToQuotationsListAsync();
        }

        // ── Action steps ──────────────────────────────────────────────────────────

        [Given("I click on Create Quotation button")]
        [When("I click on Create Quotation button")]
        [Then("I click on Create Quotation button")]
        public async Task IClickOnCreateQuotationButton()
        {
            await _quotationPage.ClickCreateQuotationButtonAsync();
        }

        [Then("I should see the create Quotation Page")]
        public async Task IShouldSeeTheCreateQuotationPage()
        {
            await _quotationPage.ShouldSeeCreateQuotationPageAsync();
        }

        // Generic transport mode selector: "Ocean" | "Air" | "Truck"
        [Given("I click on {string} transport mode")]
        [When("I click on {string} transport mode")]
        [Then("I click on {string} transport mode")]
        public async Task IClickOnTransportMode(string mode)
        {
            await _quotationPage.ClickTransportModeAsync(mode);
        }

        // Generic load type selector: "Full (FCL)" | "Partial (LCL)"
        [Given("I click on {string} load type")]
        [When("I click on {string} load type")]
        [Then("I click on {string} load type")]
        public async Task IClickOnLoadType(string loadType)
        {
            await _quotationPage.ClickLoadTypeAsync(loadType);
        }

        [Given("I enter {string} as the Origin Port")]
        [When("I enter {string} as the Origin Port")]
        [Then("I enter {string} as the Origin Port")]
        public async Task IEnterTheOriginPort(string port)
        {
            await _quotationPage.EnterOriginPortAsync(port);
        }

        [Given("I enter {string} as the Destination Port")]
        [When("I enter {string} as the Destination Port")]
        [Then("I enter {string} as the Destination Port")]
        public async Task IEnterTheDestinationPort(string port)
        {
            await _quotationPage.EnterDestinationPortAsync(port);
        }

        [When("I click on Continue your quote")]
        public async Task IClickOnContinueYourQuote()
        {
            await _quotationPage.ClickContinueYourQuoteAsync();
        }

        [Then("I should see the Origin and Destination ports")]
        public async Task IShouldSeeTheOriginAndDestinationPorts()
        {
            await _quotationPage.ShouldSeeOriginAndDestinationPortsAsync();
        }

        [When("I click on the calendar")]
        public async Task IClickOnTheCalendar()
        {
            await _quotationPage.ClickCalendarAsync();
        }

        [Then("I select the date")]
        public async Task ISelectTheDate()
        {
            await _quotationPage.SelectTodaysDateAsync();
        }

        [When("I click on currency")]
        public async Task IClickOnCurrency()
        {
            await _quotationPage.ClickCurrencyDropdownAsync();
        }

        [Then("I select {string} as the currency")]
        public async Task ISelectTheCurrency(string currency)
        {
            await _quotationPage.SelectCurrencyAsync(currency);
        }

        // Parametrizable: "40' Container", "20' Container", etc.
        [Given("I select the container size {string}")]
        [When("I select the container size {string}")]
        [Then("I select the container size {string}")]
        public async Task ISelectTheContainerSize(string size)
        {
            await _quotationPage.SelectContainerSizeAsync(size);
        }

        // Parametrizable: "All Types", "Reefer", etc.
        [Given("I select the container type {string}")]
        [When("I select the container type {string}")]
        [Then("I select the container type {string}")]
        public async Task ISelectTheContainerType(string type)
        {
            await _quotationPage.SelectContainerTypeAsync(type);
        }

        [When("I click on Commodity dropdown")]
        [Then("I click on Commodity dropdown")]
        [Given("I click on Commodity dropdown")]
        public async Task IClickOnCommodity()
        {
            await _quotationPage.ClickCommodityDropdownAsync();
        }

        // Parametrizable: "Freight All Kinds (FAK)", etc.
        [Given("I select the Commodity {string}")]
        [When("I select the Commodity {string}")]
        [Then("I select the Commodity {string}")]
        public async Task ISelectTheCommodity(string commodity)
        {
            await _quotationPage.SelectCommodityAsync(commodity);
        }

        [When("I click on Create quotation from details")]
        public async Task IClickOnCreateQuotationFromDetails()
        {
            await _quotationPage.ClickCreateQuotationFromDetailsAsync();
        }

        [Then("I should see the offers")]
        public async Task IShouldSeeTheOffers()
        {
            await _quotationPage.ShouldSeeOffersAsync();
        }

        // Parametrizable: "Maersk Line", "MSC", etc.
        [When("I filter By {string}")]
        public async Task IFilterBy(string carrier)
        {
            await _quotationPage.FilterByCarrierAsync(carrier);
        }

        [When("I select {string} schedules")]
        public async Task ISelectSchedules(string carrier)
        {
            await _quotationPage.SelectCarrierSchedulesAsync();
        }

        [Then("I should see the schedules")]
        public async Task IShouldSeeTheSchedules()
        {
            await _quotationPage.ShouldSeeSchedulesAsync();
        }

        [When("I select the schedules")]
        public async Task ISelectTheSchedules()
        {
            await _quotationPage.SelectScheduleAsync();
        }

        [Then("I confirm the operation")]
        public async Task IConfirmTheOperation()
        {
            await _quotationPage.ConfirmOperationAsync();
        }

        [Then("I should see the quotation details to send the booking")]
        public async Task IShouldSeeTheQuotationDetails()
        {
            await _quotationPage.ShouldSeeQuotationDetailsAsync();
        }

        [When("I store the first Vessel")]
        [Then("I store the first Vessel")]
        [Given("I store the first Vessel")]
        public async Task IStoreTheFirstVessel()
        {
            await _quotationPage.StoreFirstVesselAsync();
        }

        [When("I select first the schedule")]
        [Then("I select first the schedule")]
        [Given("I select first the schedule")]
        public async Task ISelectFirstTheSchedule()
        {
            await _quotationPage.SelectFirstScheduleAsync();
        }

        [When("I store the Vessel for the booking")]
        [Then("I store the Vessel for the booking")]
        [Given("I store the Vessel for the booking")]
        public async Task IStoreTheVesselForTheBooking()
        {
            await _quotationPage.StoreVesselForBookingAsync();
        }

        [Then("I compare the Vessel with the Vessel schedule")]
        public async Task ICompareTheVesselWithTheVesselSchedule()
        {
            await _quotationPage.CompareVesselWithScheduleAsync();
        }

        [Given("I store the quote ID")]
        [When("I store the quote ID")]
        [Then("I store the quote ID")]
        public async Task IStoreTheQuoteID()
        {
            await _quotationPage.StoreQuoteIdAsync();
        }

        [When("I enter the quotation ID in the search section")]
        public async Task IEnterTheQuotationIdInTheSearchSection()
        {
            await _quotationPage.EnterQuotationIdInSearchAsync();
        }

        [When("I enter the quote ID in the search")]
        public async Task IEnterTheQuoteIdInTheSearch()
        {
            await _quotationPage.EnterQuotationIdInSearchAsync();
        }

        [Then("I should see the quote ID in the results")]
        public async Task IShouldSeeTheQuoteIdInTheResults()
        {
            await _quotationPage.ShouldSeeQuoteIdInResultsAsync();
        }

        // ── TC145: Transaction role ───────────────────────────────────────────────

        [Given("I click on {string}")]
        [When("I click on {string}")]
        [Then("I click on {string}")]
        public async Task IClickOn(string role)
        {
            await _quotationPage.ClickTransactionRoleAsync(role);
        }

        // ── TC145: Notifications counter ──────────────────────────────────────────

        [Given("I store the initial total Notifications")]
        [When("I store the initial total Notifications")]
        [Then("I store the initial total Notifications")]
        public async Task IStoreTheInitialTotalNotifications()
        {
            await _quotationPage.StoreInitialNotificationsAsync();
        }

        [Then("the final notifications should be initial notifications +1")]
        public async Task TheFinalNotificationsShouldBeInitialPlusOne()
        {
            await _quotationPage.VerifyFinalNotificationsAsync();
        }

        // ── TC145: Portal quote status ────────────────────────────────────────────

        [Then("I should see the quote  status is {string}")]
        [Then("I should see the quote status is {string}")]
        public async Task IShouldSeeTheQuoteStatusIs(string status)
        {
            await _quotationPage.ShouldSeeQuoteStatusAsync(status);
        }

    
        // ── LCL Cargo: Package ────────────────────────────────────────────────────

        // Parametrizable: "Carton" | "Crate" | etc. (radio buttons, LCL)
        [Given("I select the Package {string}")]
        [When("I select the Package {string}")]
        [Then("I select the Package {string}")]
        public async Task ISelectThePackage(string packageType)
        {
            await _quotationPage.SelectPackageAsync(packageType);
        }

        // Parametrizable: "Bag" | "Carton" | etc. (ng-select combobox, Truck LTL)
        [Given("I select the Package {string} for LTL")]
        [When("I select the Package {string} for LTL")]
        [Then("I select the Package {string} for LTL")]
        public async Task ISelectThePackageForLTL(string packageType)
        {
            await _quotationPage.SelectPackageLTLAsync(packageType);
        }

        // ── LCL Cargo: Dimensions & Weight (DataTable) ────────────────────────────

        // Professional approach: all cargo fields in one step with a DataTable.
        // Feature usage:
        //   And I enter the following cargo details:
        //     | Weight | Length | Width | Height |
        //     | 10     | 10     | 10    | 10     |
        [Given("I enter the following cargo details:")]
        [When("I enter the following cargo details:")]
        [Then("I enter the following cargo details:")]
        public async Task IEnterTheFollowingCargoDetails(Table table)
        {
            var row = table.Rows[0];
            await _quotationPage.EnterCargoDetailsAsync(
                weight: row["Weight"],
                length: row["Length"],
                width:  row["Width"],
                height: row["Height"]
            );
        }

        // ── Spot rate request ─────────────────────────────────────────────────────

        [Given("I click on Request a different rate button")]
        [When("I click on Request a different rate button")]
        [Then("I click on Request a different rate button")]
        [Given("I click on Request  a differente rate button")]
        [When("I click on Request  a differente rate button")]
        [Then("I click on Request  a differente rate button")]
        public async Task IClickOnRequestADifferentRateButton()
        {
            await _quotationPage.ClickRequestDifferentRateAsync();
        }

        [Then("I should see the modal to enter the request")]
        public async Task IShouldSeeTheModalToEnterTheRequest()
        {
            await _quotationPage.ShouldSeeSpotRateModalAsync();
        }

        [Given("I click on select a request dropdown")]
        [When("I click on select a request dropdown")]
        [Then("I click on select a request dropdown")]
        public async Task IClickOnSelectARequestDropdown()
        {
            await _quotationPage.ClickSpotRateDropdownAsync();
        }

        // Parametrizable: "I need a better rate" | etc.
        [Given("I select the option {string}")]
        [When("I select the option {string}")]
        [Then("I select the option {string}")]
        public async Task ISelectTheOption(string option)
        {
            await _quotationPage.SelectSpotRateOptionAsync(option);
        }

        // Parametrizable: "AutomationRequest" | etc.
        [Given("I enter the remarks {string}")]
        [When("I enter the remarks {string}")]
        [Then("I enter the remarks {string}")]
        public async Task IEnterTheRemarks(string remarks)
        {
            await _quotationPage.EnterRemarksAsync(remarks);
        }

        [Given("I send the request")]
        [When("I send the request")]
        [Then("I send the request")]
        public async Task ISendTheRequest()
        {
            await _quotationPage.SendSpotRateRequestAsync();
        }

        // ── LCL Cargo: Accessorials ───────────────────────────────────────────────

        // Parametrizable: "Refrigerated" | etc.
        [Given("I select the accesorials {string}")]
        [When("I select the accesorials {string}")]
        [Then("I select the accesorials {string}")]
        public async Task ISelectTheAccessorials(string accessorial)
        {
            await _quotationPage.SelectAccessorialAsync(accessorial);
        }

    }
}
