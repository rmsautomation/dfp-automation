using System;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using TestContext = DFP.Playwright.Support.TestContext;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class PortalAuthStepDefinitions
    {
        private readonly TestContext _tc;

        public PortalAuthStepDefinitions(TestContext tc)
        {
            _tc = tc;
        }

        [Given("I have a portal API token")]
        public async Task GivenIHaveAPortalApiToken()
        {
            var email = Environment.GetEnvironmentVariable(Constants.API_EMAIL) ?? "";
            var password = Environment.GetEnvironmentVariable(Constants.API_PASSWORD) ?? "";
            var organizationId = Environment.GetEnvironmentVariable(Constants.PORTAL_ORGANIZATION_ID) ?? "";
            var siteId = Environment.GetEnvironmentVariable(Constants.PORTAL_SITE_ID) ?? "";

            var client = PortalApiClient.FromEnvironment();
            var token = await client.GetPortalTokenAsync(email, password, organizationId, siteId);

            _tc.Data["portalToken"] = token;
            Console.WriteLine($"Portal token obtained (length={token?.Length ?? 0}).");
        }
    }
}
