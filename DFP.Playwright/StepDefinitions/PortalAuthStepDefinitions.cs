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
            var email = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.API_EMAIL) ?? "");
            var password = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.API_PASSWORD) ?? "");
            var organizationId = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_ORGANIZATION_ID) ?? "");
            var siteId = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.PORTAL_SITE_ID) ?? "");

            var client = PortalApiClient.FromEnvironment();
            var token = await client.GetPortalTokenAsync(email, password, organizationId, siteId);

            _tc.Data["portalToken"] = token;
            Console.WriteLine($"Portal token obtained (length={token?.Length ?? 0}).");
        }

        private static string ResolveEnvValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmed = value.Trim();
            if (trimmed.StartsWith("Env.", StringComparison.OrdinalIgnoreCase))
            {
                var key = trimmed.Substring(4);
                return Environment.GetEnvironmentVariable(key) ?? "";
            }

            var indirect = Environment.GetEnvironmentVariable(trimmed);
            if (!string.IsNullOrWhiteSpace(indirect))
                return indirect;

            return value;
        }
    }
}
