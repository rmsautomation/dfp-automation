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

        [Given("the Warehouse Receipt {string} with Custom Fields is imported")]
        public async Task TheWarehouseReceiptWithCustomFieldsIsImported(string receiptNumber)
        {
            var username = Environment.GetEnvironmentVariable("CORRECT_USERNAME")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "";
            var password = Environment.GetEnvironmentVariable("CORRECT_PASSWORD")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "";
            var whId = receiptNumber?.Trim() ?? "";

            var suffix = new string(whId
                .Select(ch => char.IsLetterOrDigit(ch) ? char.ToUpperInvariant(ch) : '_')
                .ToArray());
            var specificGuidKey = $"WAREHOUSE_RECEIPT_GUID_{suffix}";
            var whGuid = Environment.GetEnvironmentVariable(specificGuidKey)
                         ?? Environment.GetEnvironmentVariable("WAREHOUSE_RECEIPT_GUID")
                         ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SOAP credentials are missing. Set CORRECT_USERNAME/CORRECT_PASSWORD (or DFP_USERNAME/DFP_PASSWORD).");
            if (string.IsNullOrWhiteSpace(whGuid) || string.IsNullOrWhiteSpace(whId))
                throw new InvalidOperationException($"Warehouse GUID is required. Set {specificGuidKey} or WAREHOUSE_RECEIPT_GUID.");

            var session = new ApiSession(username, password);
            var err = await session.StartSessionAsync();
            if (err != api_session_error.no_error)
                throw new InvalidOperationException($"StartSession failed: {err}");

            SoapClientConfigurator.Configure(session.CSSoap);

            await TransactionImportHelper.ImportWarehouseReceiptFromResourcesAsync(
                session.CSSoap,
                session.Key,
                whGuid,
                whId,
                forceDelete: true);

            await session.EndSessionAsync();
        }

        [Given("the Shipment {string} with Custom Fields is imported")]
        public async Task TheShipmentWithCustomFieldsIsImported(string shipmentNumber)
        {
            var username = Environment.GetEnvironmentVariable("CORRECT_USERNAME")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "";
            var password = Environment.GetEnvironmentVariable("CORRECT_PASSWORD")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "";
            var shId = shipmentNumber?.Trim() ?? "";

            var suffix = new string(shId
                .Select(ch => char.IsLetterOrDigit(ch) ? char.ToUpperInvariant(ch) : '_')
                .ToArray());
            var specificGuidKey = $"SHIPMENT_GUID_{suffix}";
            var shGuid = Environment.GetEnvironmentVariable(specificGuidKey)
                         ?? Environment.GetEnvironmentVariable("SHIPMENT_GUID")
                         ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SOAP credentials are missing. Set CORRECT_USERNAME/CORRECT_PASSWORD (or DFP_USERNAME/DFP_PASSWORD).");
            if (string.IsNullOrWhiteSpace(shGuid) || string.IsNullOrWhiteSpace(shId))
                throw new InvalidOperationException($"Shipment GUID is required. Set {specificGuidKey} or SHIPMENT_GUID.");

            var session = new ApiSession(username, password);
            var err = await session.StartSessionAsync();
            if (err != api_session_error.no_error)
                throw new InvalidOperationException($"StartSession failed: {err}");

            SoapClientConfigurator.Configure(session.CSSoap);

            await TransactionImportHelper.ImportShipmentFromResourcesAsync(
                session.CSSoap,
                session.Key,
                shGuid,
                shId,
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

    }
}



