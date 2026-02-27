using System;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ApiAuthStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;

        public ApiAuthStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

        [Given("I have a hub API token")]
        public void GivenIHaveAHubApiToken()
        {
            var token = ResolveEnvValue(Environment.GetEnvironmentVariable(Helpers.Constants.HUB_TOKEN) ?? "");
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("HUB_TOKEN is required.");

            _tc.Data["hubToken"] = token;
            Console.WriteLine($"Hub token obtained (length={token.Length}).");
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
