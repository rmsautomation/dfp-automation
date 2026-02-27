using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        public async Task GivenIHaveAHubApiToken()
        {
            var clientId = ResolveEnvValue(Environment.GetEnvironmentVariable(Helpers.Constants.HUB_CLIENT_ID) ?? "");
            var clientSecret = ResolveEnvValue(Environment.GetEnvironmentVariable(Helpers.Constants.HUB_CLIENT_SECRET) ?? "");
            var tokenUrl = ResolveEnvValue(Environment.GetEnvironmentVariable(Helpers.Constants.HUB_AUTH_TOKEN_URL)
                           ?? "https://magaya-prod.us.auth0.com/oauth/token");
            var audience = ResolveEnvValue(Environment.GetEnvironmentVariable(Helpers.Constants.HUB_AUTH_AUDIENCE)
                           ?? "https://apps.qwyk.io");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                throw new InvalidOperationException("HUB_CLIENT_ID/HUB_CLIENT_SECRET are required to obtain a hub token.");

            using var http = new HttpClient();
            var payload = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                audience,
                grant_type = "client_credentials"
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            Console.WriteLine($"Hub token URL: {tokenUrl}");
            using var response = await http.PostAsync(tokenUrl, content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Hub token request failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");

            var token = ExtractToken(body);
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Hub token response did not include access_token.");

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

        private static string ExtractToken(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("access_token", out var tokenProp) && tokenProp.ValueKind == JsonValueKind.String)
                    return tokenProp.GetString() ?? "";
                if (root.TryGetProperty("token", out var tokenProp2) && tokenProp2.ValueKind == JsonValueKind.String)
                    return tokenProp2.GetString() ?? "";
            }
            catch (JsonException)
            {
            }

            return "";
        }
    }
}
