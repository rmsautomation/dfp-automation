using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;
using DFP.Playwright.Helpers;
using SoapApi.Common;
using SoapApi.Models;
using SoapApi.CssSoap;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class DashboardStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly DashboardPage _dashboard;
        public DashboardStepDefinitions(
            DFP.Playwright.Support.TestContext tc,
            DashboardPage dashboard)
        {
            _tc = tc;
            _dashboard = dashboard;
        }

        [Given(@"^the transaction ""([^""]+)"" ""([^""]+)"" is imported$")]
        public async Task TheTransactionIsImported(string transactionType, string transactionNumber)
        {
            await ImportTransactionByTypeAndNumberAsync(transactionType, transactionNumber);
        }

        [Given(@"^the transaction ""([^""]+)"" ""([^""]+)"" with Custom Fields is imported$")]
        public async Task TheTransactionWithCustomFieldsIsImported(string transactionType, string transactionNumber)
        {
            await ImportTransactionByTypeAndNumberAsync(transactionType, transactionNumber);
        }

        private async Task ImportTransactionByTypeAndNumberAsync(string transactionType, string transactionNumber)
        {
            var username = Environment.GetEnvironmentVariable("CORRECT_USERNAME")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "";
            var password = Environment.GetEnvironmentVariable("CORRECT_PASSWORD")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "";
            var type = (transactionType ?? "").Trim().ToUpperInvariant();
            var number = transactionNumber?.Trim() ?? "";
            var suffix = new string(number
                .Select(ch => char.IsLetterOrDigit(ch) ? char.ToUpperInvariant(ch) : '_')
                .ToArray());
            var specificGuidKey = type switch
            {
                "WH" => $"WAREHOUSE_RECEIPT_GUID_{suffix}",
                "SH" => $"SHIPMENT_GUID_{suffix}",
                _ => $"{type}_GUID_{suffix}"
            };
            var transactionGuid = Environment.GetEnvironmentVariable(specificGuidKey) ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SOAP credentials are missing. Set CORRECT_USERNAME/CORRECT_PASSWORD (or DFP_USERNAME/DFP_PASSWORD).");
            if (string.IsNullOrWhiteSpace(transactionGuid) || string.IsNullOrWhiteSpace(number))
                throw new InvalidOperationException($"Transaction GUID is required. Set {specificGuidKey}.");
            if (string.IsNullOrWhiteSpace(type))
                throw new InvalidOperationException("Transaction type is required.");

            var session = new ApiSession(username, password);
            var err = await session.StartSessionAsync();
            if (err != api_session_error.no_error)
                throw new InvalidOperationException($"StartSession failed: {err}");

            SoapClientConfigurator.Configure(session.CSSoap);

            await TransactionImportHelper.ImportTransactionFromResourcesAsync(
                session.CSSoap,
                session.Key,
                type,
                transactionGuid,
                number,
                forceDelete: true);

            await session.EndSessionAsync();
        }

        [Given("I am on the dashboard page")]
        public async Task IAmOnTheDashboardPage()
        {
            await _dashboard.IAmOnTheDashboardPage();
        }

        [When("I click on the \"Shipments\" option")]
        public async Task IClickOnTheShipmentsOption()
        {
            await _dashboard.IClickOnTheShipmentsOption();
        }

        [Then("I should see the create shipments option")]
        public async Task IShouldSeeTheCreateShipmentsOption()
        {
            await _dashboard.IShouldSeeTheCreateShipmentsOption();
        }

        [When("I click on the \"Reports\" option")]
        public async Task IClickOnTheReportsOption()
        {
            await _dashboard.IClickOnTheReportsOption();
        }

        [Then("I should see {string} text")]
        public async Task IShouldSeeText(string expectedText)
        {
            await _dashboard.IShouldSeeText(expectedText);
        }

        [When("I click on Dashboard icon")]
        public async Task IClickOnDashboardIcon()
        {
            await _dashboard.IClickOnDashboardIcon();
        }

        [Then("the dashboard should be visible")]
        public async Task TheDashboardShouldBeVisible()
        {
            await _dashboard.TheDashboardShouldBeVisible();
        }

        [When("I store the total Shipments in the Dashboard")]
        [Then("I store the total Shipments in the Dashboard")]
        public async Task IStoreTheTotalShipmentsInTheDashboard()
        {
            var total = await _dashboard.GetTotalShipmentsCountAsync();
            _tc.Data["totalShipments"] = total;
        }

        [Then("I store the total Shipments in the Dashboard after operation")]
        public async Task IStoreTotalShipmentsAfterOperation()
        {
            var total = await _dashboard.GetTotalShipmentsCountAsync();
            _tc.Data["totalShipmentsAfterOperation"] = total;
        }

        [Then("I see initial total shipment -1")]
        public void ISeeInitialTotalShipmentMinusOne()
        {
            var initial         = (int)_tc.Data["totalShipments"];
            var afterOperation  = (int)_tc.Data["totalShipmentsAfterOperation"];
            Assert.AreEqual(initial - 1, afterOperation,
                $"Expected Active Shipments to be {initial - 1} after hiding one shipment, but found {afterOperation}.");
        }

        [Then("I see initial total shipment")]
        public async Task ISeeInitialTotalShipment()
        {
            await _dashboard.IAmOnTheDashboardPage();
            var current = await _dashboard.GetTotalShipmentsCountAsync();
            var initial = (int)_tc.Data["totalShipments"];
            Assert.AreEqual(initial, current,
                $"Expected Active Shipments to be back to {initial} after unhiding, but found {current}.");
        }
    }
}



