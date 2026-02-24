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
        private readonly LoginPage _login;
        private readonly WarehouseReceiptsPage _warehouseReceipts;

        public DashboardStepDefinitions(
            DFP.Playwright.Support.TestContext tc,
            DashboardPage dashboard,
            LoginPage login,
            WarehouseReceiptsPage warehouseReceipts)
        {
            _tc = tc;
            _dashboard = dashboard;
            _login = login;
            _warehouseReceipts = warehouseReceipts;
        }

        [Given("a Warehouse Receipt with Custom Fields exists")]
        public async Task AWarehouseReceiptWithCustomFieldsExists()
        {
            var username = Environment.GetEnvironmentVariable("CORRECT_USERNAME")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_USERNAME)
                           ?? "";
            var password = Environment.GetEnvironmentVariable("CORRECT_PASSWORD")
                           ?? Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD)
                           ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SOAP credentials are missing. Set CORRECT_USERNAME/CORRECT_PASSWORD (or DFP_USERNAME/DFP_PASSWORD).");

            var session = new ApiSession(username, password);
            var err = await session.StartSessionAsync();
            if (err != api_session_error.no_error)
                throw new InvalidOperationException($"StartSession failed: {err}");

            SoapClientConfigurator.Configure(session.CSSoap);

            await TransactionImportHelper.ImportAllFromResourcesAsync(session.CSSoap, session.Key);

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

        [Given("Login into the Portal")]
        public async Task LoginIntoThePortal()
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "";
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BASE_URL is required.");

            var username = Environment.GetEnvironmentVariable(Constants.DFP_USERNAME) ?? "";
            var password = Environment.GetEnvironmentVariable(Constants.DFP_PASSWORD) ?? "";
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("DFP_USERNAME/DFP_PASSWORD are required.");

            await _login.NavigateAsync();
            if (await _login.IsUsernameInputVisibleAsync())
            {
                await _login.LoginToDFPAsync(username, password);
            }
        }

        [When("Go to Warehouse / Warehouse Receipts")]
        public async Task GoToWarehouseWarehouseReceipts()
        {
            await _warehouseReceipts.GoToWarehouseReceiptsAsync();
        }

        [When("Select Table View")]
        public async Task SelectTableView()
        {
            await _warehouseReceipts.SelectTableViewAsync();
        }

        [Then("Verify the Custom fields")]
        public async Task VerifyTheCustomFields()
        {
            await _warehouseReceipts.VerifyCustomFieldsAsync();
        }
    }
}



