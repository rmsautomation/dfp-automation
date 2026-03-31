using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;
using System;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ReportsStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly ReportsPage _reportsPage;
        private readonly ShipmentPage _shipmentPage;
        private readonly WarehouseReceiptPage _warehouseReceiptPage;
        private IDownload? _lastDownload;

        public ReportsStepDefinitions(DFP.Playwright.Support.TestContext tc, ReportsPage reportsPage, ShipmentPage shipmentPage, WarehouseReceiptPage warehouseReceiptPage)
        {
            _tc = tc;
            _reportsPage = reportsPage;
            _shipmentPage = shipmentPage;
            _warehouseReceiptPage = warehouseReceiptPage;
        }

        [Given("I am on the Reports page")]
        public async Task IAmOnTheReportsPage()
        {
            await _reportsPage.IAmOnTheReportsPage();
        }

        [When("I click on \"Shipments\" option")]
        public async Task IClickOnShipmentsOption()
        {
            await _reportsPage.IClickOnShipmentsOption();
        }

        [When("I click on \"Warehouse Receipts\" option")]
        public async Task IClickOnWarehouseReceiptsOption()
        {
            await _reportsPage.IClickOnWarehouseReceiptsOption();
        }

        [When("I click on \"Invoices\" option")]
        public async Task IClickOnInvoicesOption()
            => await _reportsPage.IClickOnInvoicesOption();

        [Given("I go to Reports Warehouse")]
        [When("I go to Reports Warehouse")]
        public async Task IGoToReportsWarehouse()
        {
            await _reportsPage.NavigateToWarehouseReceiptsReportAsync();
        }

        [Then("I should see {string} Report text")]
        public async Task IShouldSeeReportText(string expectedText)
        {
            await _reportsPage.IShouldSeeText(expectedText);
        }

        [When(@"^I select Predefined Range with text (.+)$")]
        public async Task ISelectPredefinedRangeWithText(string rangeText)
        {
            await _reportsPage.ISelectPredefinedRangeWithText(rangeText);
        }

        [Then("I should select Custom option")]
        public async Task IShouldSelectCustomOption()
        {
            await _reportsPage.IShouldSelectCustomOption();
        }

        [When("I select the Calendar")]
        public async Task ISelectTheCalendar()
        {
            await _reportsPage.ISelectTheCalendar();
        }

        [When("I should click on Today option")]
        public async Task IShouldClickOnTodayOption()
        {
            await _reportsPage.IClickOnTodayOption();
        }

        [Then("I should see the Save report button")]
        public async Task IShouldSeeTheSaveReportButton()
        {
            await _reportsPage.IShouldSeeTheSaveReportButton();
        }

        [Given("I already click on Search button in the Reports Section")]
        public async Task IAlreadyClickOnSearchButtonInReportsSection()
        {
            await _reportsPage.IAlreadyClickOnSearchButtonInReportsSection();
        }

        [When("I see the Save report button")]
        public async Task ISeeTheSaveReportButton()
        {
            await _reportsPage.IShouldSeeTheSaveReportButton();
        }

        [Then(@"^I see the text (.+)$")]
        public async Task ISeeTheText(string expectedText)
        {
            await _reportsPage.IShouldSeeText(expectedText);
        }

        [Then(@"^I could not see the text (.+)$")]
        public async Task ICouldNotSeeTheText(string unexpectedText)
        {
            await _reportsPage.IShouldNotSeeText(unexpectedText);
        }

        [Then("I should see the shipment Name")]
        public async Task IShouldSeeTheShipmentName()
        {
            var name = _shipmentPage.GetShipmentName();
            await _reportsPage.IShouldSeeText(name);
        }

        [Then("the shipment name should not appear in the report results")]
        public async Task TheShipmentNameShouldNotAppearInReportResults()
        {
            var name = _shipmentPage.GetShipmentName();
            await _reportsPage.TheShipmentNameShouldNotAppearInResults(name);
        }

        [Then("the warehouse receipt name should not appear in the report results")]
        public async Task TheWarehouseReceiptNameShouldNotAppearInReportResults()
        {
            var name = _warehouseReceiptPage.GetWarehouseReceiptName();
            await _reportsPage.TheNameShouldNotAppearInResults(name, "Warehouse Receipt");
        }

        [Given("I select saved reports with name {string}")]
        [When("I select saved reports with name {string}")]
        [Then("I select saved reports with name {string}")]
        public async Task ISelectSavedReportsWithName(string name)
            => await _reportsPage.SelectSavedReportAsync(name);

        [Given("I should see the invoices in the reports Results")]
        [When("I should see the invoices in the reports Results")]
        [Then("I should see the invoices in the reports Results")]
        public async Task IShouldSeeTheInvoicesInTheReportsResults()
            => await _reportsPage.ShouldSeeInvoicesInReportResultsAsync();

        // Scoped to Reports — handles both plain clicks (Search) and downloads (Download to Excel)
        // without the dialog-open wait logic used by the global IClickOnButton in ShipmentStepDefinitions.
        [Given("I click on {string} button"), Scope(Feature = "Reports")]
        [When("I click on {string} button"), Scope(Feature = "Reports")]
        [Then("I click on {string} button"), Scope(Feature = "Reports")]
        public async Task IClickOnButtonReports(string buttonText)
        {
            if (buttonText.Contains("Download", StringComparison.OrdinalIgnoreCase))
                _lastDownload = await _reportsPage.ClickDownloadToExcelAsync();
            else
                await _reportsPage.ClickButtonAsync(buttonText);
        }

        [Given("I should verify the downloaded excel contains {string} rows")]
        [When("I should verify the downloaded excel contains {string} rows")]
        [Then("I should verify the downloaded excel contains {string} rows")]
        public async Task IShouldVerifyDownloadedExcelContainsRows(string expectedRows)
        {
            Assert.IsNotNull(_lastDownload,
                "No Excel download captured. Run 'I click on Download to Excel button' before this step.");
            await ReportsPage.VerifyExcelRowCountAsync(_lastDownload!, int.Parse(expectedRows));
        }
    }
}
