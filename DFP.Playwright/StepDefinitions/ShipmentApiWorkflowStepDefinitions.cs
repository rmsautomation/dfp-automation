using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DFP.Playwright.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace DFP.Playwright.StepDefinitions
{
    [Binding]
    public sealed class ShipmentApiWorkflowStepDefinitions
    {
        private readonly DFP.Playwright.Support.TestContext _tc;

        public ShipmentApiWorkflowStepDefinitions(DFP.Playwright.Support.TestContext tc)
        {
            _tc = tc;
        }

        [When("I create shipment via webhook")]
        public async Task WhenICreateShipmentViaWebhook()
        {
            var payload = LoadPayloadFromEnvOrFile(Constants.SHIPMENT_WEBHOOK_PAYLOAD_PATH, "SHIPMENT_WEBHOOK_PAYLOAD");
            payload = ReplaceTemplateVariables(payload);
            var token = Environment.GetEnvironmentVariable(Constants.CHAINIO_TOKEN) ?? "";

            var client = PortalApiClient.FromEnvironment();
            using var response = await client.CreateShipmentViaWebhookAsync(token, payload);
            var body = await response.Content.ReadAsStringAsync();
            EnsureAccepted(response, body, "Create shipment via webhook");
            SaveWebhookResponse(body);

            var transactionId = TryReadFirstId(body, "transactionId", "transaction_id");
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                _tc.Data["transactionId"] = transactionId;
                _tc.Data["shipmentId"] = transactionId;
            }
        }

        [When(@"I get cargo items for shipment ""([^""]+)"" via API")]
        public async Task WhenIGetCargoItemsForShipmentViaApi(string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(shipmentId) && _tc.Data.TryGetValue("shipmentId", out var value))
                shipmentId = value as string ?? "";

            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var token = GetPortalToken();
            var client = PortalApiClient.FromEnvironment();
            const int maxAttempts = 60; // ~5 minutes
            const int delayMs = 5000;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var response = await client.GetCargoItemsAsync(token, shipmentId);
                var body = await response.Content.ReadAsStringAsync();

                if ((int)response.StatusCode == 404 && attempt < maxAttempts)
                {
                    Console.WriteLine($"Get cargo items attempt {attempt}/{maxAttempts} -> 404. Retrying in {delayMs}ms...");
                    await Task.Delay(delayMs);
                    continue;
                }

                EnsureSuccess(response, body, "Get cargo items");

                var cargoItemId = TryReadFirstCargoItemId(body);
                if (!string.IsNullOrWhiteSpace(cargoItemId))
                    _tc.Data["cargoItemId"] = cargoItemId;
                return;
            }
        }

        [When("I get cargo items for current shipment via API")]
        public async Task WhenIGetCargoItemsForCurrentShipmentViaApi()
        {
            var shipmentId = GetRequiredValue("shipmentId", "Shipment ID not found. Create a shipment first.");
            await WhenIGetCargoItemsForShipmentViaApi(shipmentId);
        }

        [When("I create purchase order via API")]
        public async Task WhenICreatePurchaseOrderViaApi()
        {
            var payload = LoadPayloadFromEnvOrFile(Constants.PURCHASE_ORDER_PAYLOAD_PATH, "PURCHASE_ORDER_PAYLOAD");
            payload = ReplaceTemplateVariables(payload);
            var token = GetPortalToken();

            var client = PortalApiClient.FromEnvironment();
            using var response = await client.CreatePurchaseOrderAsync(token, payload);
            var body = await response.Content.ReadAsStringAsync();
            EnsureSuccess(response, body, "Create purchase order");

            var purchaseOrderId = TryReadFirstId(body, "purchase_order_id", "purchaseOrderId", "id");
            if (!string.IsNullOrWhiteSpace(purchaseOrderId))
                _tc.Data["purchaseOrderId"] = purchaseOrderId;
        }

        [When("I create purchase order line via API")]
        public async Task WhenICreatePurchaseOrderLineViaApi()
        {
            var payload = LoadPayloadFromEnvOrFile(Constants.PURCHASE_ORDER_LINE_PAYLOAD_PATH, "PURCHASE_ORDER_LINE_PAYLOAD");
            payload = ReplaceTemplateVariables(payload);
            var token = GetPortalToken();
            var purchaseOrderId = GetRequiredValue("purchaseOrderId", "Purchase order ID not found. Create a purchase order first.");

            var client = PortalApiClient.FromEnvironment();
            using var response = await client.CreatePurchaseOrderLineAsync(token, purchaseOrderId, payload);
            var body = await response.Content.ReadAsStringAsync();
            EnsureSuccess(response, body, "Create purchase order line");

            var orderLineId = TryReadFirstId(body, "order_line_id", "orderLineId", "id");
            if (!string.IsNullOrWhiteSpace(orderLineId))
                _tc.Data["orderLineId"] = orderLineId;
        }

        [Then("a cargo item id should be available")]
        public void ThenCargoItemIdShouldBeAvailable()
        {
            var id = GetRequiredValue("cargoItemId", "Cargo item ID not found.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(id), "Cargo item ID is empty.");
        }

        [Then("a purchase order id should be available")]
        public void ThenPurchaseOrderIdShouldBeAvailable()
        {
            var id = GetRequiredValue("purchaseOrderId", "Purchase order ID not found.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(id), "Purchase order ID is empty.");
        }

        [Then("an order line id should be available")]
        public void ThenOrderLineIdShouldBeAvailable()
        {
            var id = GetRequiredValue("orderLineId", "Order line ID not found.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(id), "Order line ID is empty.");
        }

        private string GetPortalToken()
        {
            if (_tc.Data.TryGetValue("portalToken", out var value) && value is string token && !string.IsNullOrWhiteSpace(token))
                return token;

            throw new InvalidOperationException("Portal token not found. Run step 'I have a portal API token' first.");
        }

        private static void EnsureSuccess(System.Net.Http.HttpResponseMessage response, string body, string action)
        {
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"{action} failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        private static void EnsureAccepted(System.Net.Http.HttpResponseMessage response, string body, string action)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                throw new InvalidOperationException($"{action} failed: expected 202 Accepted, got {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        private static string LoadPayloadFromEnvOrFile(string pathEnvVar, string inlineEnvVar)
        {
            var path = Environment.GetEnvironmentVariable(pathEnvVar) ?? "";
            if (!string.IsNullOrWhiteSpace(path))
            {
                var resolved = ResolvePath(path);
                if (!File.Exists(resolved))
                    throw new FileNotFoundException($"Payload file not found: {resolved}");
                return File.ReadAllText(resolved);
            }

            var inline = Environment.GetEnvironmentVariable(inlineEnvVar) ?? "";
            if (!string.IsNullOrWhiteSpace(inline))
                return inline;

            throw new InvalidOperationException($"Provide payload via {pathEnvVar} (file path) or {inlineEnvVar} (inline JSON).");
        }

        private static string ResolvePath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            var cwd = Directory.GetCurrentDirectory();
            var candidate = Path.Combine(cwd, path);
            if (File.Exists(candidate))
                return candidate;

            var candidateInProject = Path.Combine(cwd, "DFP.Playwright", path);
            if (File.Exists(candidateInProject))
                return candidateInProject;

            var root = FindRepoRoot(cwd) ?? FindRepoRoot(AppContext.BaseDirectory);
            if (!string.IsNullOrWhiteSpace(root))
            {
                var fromRoot = Path.Combine(root, "DFP.Playwright", path);
                if (File.Exists(fromRoot))
                    return fromRoot;
            }

            return candidate;
        }

        private static string? FindRepoRoot(string startDirectory)
        {
            if (string.IsNullOrWhiteSpace(startDirectory))
                return null;

            var dir = new DirectoryInfo(startDirectory);
            while (dir != null)
            {
                var projectDir = Path.Combine(dir.FullName, "DFP.Playwright");
                if (Directory.Exists(projectDir))
                    return dir.FullName;
                dir = dir.Parent;
            }

            return null;
        }

        private string ReplaceTemplateVariables(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
                return payload;

            EnsureDefaultTemplateValues();
            var missing = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var result = System.Text.RegularExpressions.Regex.Replace(payload, "\\{\\{([^}]+)\\}\\}", match =>
            {
                var key = match.Groups[1].Value.Trim();
                var value = GetTemplateValue(key);
                if (value == null)
                {
                    missing.Add(key);
                    return match.Value;
                }
                return value;
            });

            if (missing.Count > 0)
                throw new InvalidOperationException($"Missing template values for: {string.Join(", ", missing)}. Provide them via environment variables.");

            return result;
        }


        private string? GetTemplateValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            if (_tc.Data.TryGetValue(key, out var ctxValue) && ctxValue is string ctxStr && !string.IsNullOrWhiteSpace(ctxStr))
                return ctxStr;

            var direct = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(direct))
                return ResolveEnvValue(direct);

            var normalized = key.Replace(".", "_").Replace("-", "_").ToUpperInvariant();
            var fromNormalized = Environment.GetEnvironmentVariable(normalized);
            if (!string.IsNullOrWhiteSpace(fromNormalized))
                return ResolveEnvValue(fromNormalized);

            return null;
        }

        private void EnsureDefaultTemplateValues()
        {
            if (!_tc.Data.ContainsKey("timestamp"))
                _tc.Data["timestamp"] = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            if (!_tc.Data.ContainsKey("iso_now"))
                _tc.Data["iso_now"] = DateTime.UtcNow.ToString("o");
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

        private string GetRequiredValue(string key, string errorMessage)
        {
            if (_tc.Data.TryGetValue(key, out var value) && value is string id && !string.IsNullOrWhiteSpace(id))
                return id;

            throw new InvalidOperationException(errorMessage);
        }

        private static string TryReadFirstId(string json, params string[] keys)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (TryReadIdFromElement(doc.RootElement, keys, out var id))
                    return id;
            }
            catch (JsonException)
            {
            }

            return "";
        }

        private static bool TryReadIdFromElement(JsonElement element, string[] keys, out string id)
        {
            id = "";
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in keys)
                {
                    if (element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String)
                    {
                        id = prop.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(id))
                            return true;
                    }
                }

                if (element.TryGetProperty("data", out var data) && TryReadIdFromElement(data, keys, out id))
                    return true;
                if (element.TryGetProperty("result", out var result) && TryReadIdFromElement(result, keys, out id))
                    return true;
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (TryReadIdFromElement(item, keys, out id))
                        return true;
                }
            }

            return false;
        }

        private static string TryReadFirstCargoItemId(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (TryReadIdFromElement(doc.RootElement, new[] { "cargo_item_id", "cargoItemId", "id" }, out var id))
                    return id;
            }
            catch (JsonException)
            {
            }

            return "";
        }

        private static void SaveWebhookResponse(string body)
        {
            try
            {
                var cwd = Directory.GetCurrentDirectory();
                var root = FindRepoRoot(cwd) ?? FindRepoRoot(AppContext.BaseDirectory);
                if (string.IsNullOrWhiteSpace(root))
                    return;

                var dir = Path.Combine(root, "DFP.Playwright", "Artifacts", "Logs");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"shipment-webhook-response-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
                File.WriteAllText(file, body);
            }
            catch
            {
                // ignore logging failures
            }
        }
    }
}
