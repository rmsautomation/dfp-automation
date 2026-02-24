using Reqnroll;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class DashboardStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;
        private readonly DashboardPage _dashboard;

        public DashboardStepDefinitions(DFP.Playwright.Support.TestContext tc, DashboardPage dashboard)
        {
            _tc = tc;
            _dashboard = dashboard;
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



