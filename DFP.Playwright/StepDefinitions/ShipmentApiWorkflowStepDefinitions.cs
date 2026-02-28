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
                var shipmentId = await PollShipmentIdFromWebhookLogAsync(transactionId);
                if (!string.IsNullOrWhiteSpace(shipmentId))
                    _tc.Data["shipmentId"] = shipmentId;
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
            SaveApiResponse("purchase-order", body);
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
            const int maxAttempts = 10;
            const int delayMs = 3000;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var response = await client.CreatePurchaseOrderLineAsync(token, purchaseOrderId, payload);
                var body = await response.Content.ReadAsStringAsync();
                SaveApiResponse("purchase-order-line", body);

                if ((int)response.StatusCode == 404 && attempt < maxAttempts)
                {
                    await Task.Delay(delayMs);
                    continue;
                }

                EnsureSuccess(response, body, "Create purchase order line");

                var orderLineId = TryReadOrderLineId(body);
                if (!string.IsNullOrWhiteSpace(orderLineId))
                    _tc.Data["orderLineId"] = orderLineId;
                return;
            }
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

        private async Task<string> PollShipmentIdFromWebhookLogAsync(string transactionId)
        {
            var webhookUser = Environment.GetEnvironmentVariable(Constants.CHAINIO_WEBHOOK_USERNAME) ?? "";
            var client = PortalApiClient.FromEnvironment();

            const int maxAttempts = 15;
            const int delayMs = 2000;
            string lastBody = "";
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var response = await client.GetWebhookLogAsync(webhookUser, transactionId);
                var body = await response.Content.ReadAsStringAsync();
                lastBody = body;

                if ((int)response.StatusCode == 404 && attempt < maxAttempts)
                {
                    await Task.Delay(delayMs);
                    continue;
                }

                EnsureSuccess(response, body, "Get webhook logs");
                SaveWebhookLogsResponse(transactionId, body);

                var shipmentId = TryReadFirstId(body, "transactionable_id", "transactionableId", "shipment_id", "shipmentId");
                if (!string.IsNullOrWhiteSpace(shipmentId))
                    return shipmentId;

                if (attempt < maxAttempts)
                    await Task.Delay(delayMs);
            }

            throw new InvalidOperationException($"Shipment ID not found in webhook logs after polling. Last response: {Truncate(lastBody, 500)}");
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

            if (string.Equals(key, "purchase_order_id", StringComparison.OrdinalIgnoreCase))
            {
                if (_tc.Data.TryGetValue("purchaseOrderId", out var poValue) && poValue is string poStr && !string.IsNullOrWhiteSpace(poStr))
                    return poStr;
            }

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
            if (!_tc.Data.ContainsKey("shipment_reference"))
                _tc.Data["shipment_reference"] = $"REF-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("containerization_type"))
                _tc.Data["containerization_type"] = "FCL";
            if (!_tc.Data.ContainsKey("carrier_scac"))
                _tc.Data["carrier_scac"] = "HLCU";
            if (!_tc.Data.ContainsKey("vessel_name"))
                _tc.Data["vessel_name"] = "SEAMASTER";
            if (!_tc.Data.ContainsKey("voyage_number"))
                _tc.Data["voyage_number"] = "17S";
            if (!_tc.Data.ContainsKey("marks_and_numbers"))
                _tc.Data["marks_and_numbers"] = "M/N";

            var now = DateTime.UtcNow;
            string Iso(DateTime dt) => dt.ToString("o");
            if (!_tc.Data.ContainsKey("sent_date"))
                _tc.Data["sent_date"] = Iso(now);
            if (!_tc.Data.ContainsKey("booking_confirmed_actual"))
                _tc.Data["booking_confirmed_actual"] = Iso(now);
            if (!_tc.Data.ContainsKey("cargo_ready_estimated"))
                _tc.Data["cargo_ready_estimated"] = Iso(now);
            if (!_tc.Data.ContainsKey("departure_estimated"))
                _tc.Data["departure_estimated"] = Iso(now);
            if (!_tc.Data.ContainsKey("arrival_port_estimated"))
                _tc.Data["arrival_port_estimated"] = Iso(now.AddDays(30));

            // Legs defaults (simple progression)
            if (!_tc.Data.ContainsKey("leg1.departure_estimated"))
                _tc.Data["leg1.departure_estimated"] = Iso(now);
            if (!_tc.Data.ContainsKey("leg1.departure_actual"))
                _tc.Data["leg1.departure_actual"] = Iso(now.AddDays(1));
            if (!_tc.Data.ContainsKey("leg1.arrival_estimated"))
                _tc.Data["leg1.arrival_estimated"] = Iso(now.AddDays(5));
            if (!_tc.Data.ContainsKey("leg1.arrival_actual"))
                _tc.Data["leg1.arrival_actual"] = Iso(now.AddDays(7));

            if (!_tc.Data.ContainsKey("leg2.departure_estimated"))
                _tc.Data["leg2.departure_estimated"] = Iso(now.AddDays(7));
            if (!_tc.Data.ContainsKey("leg2.departure_actual"))
                _tc.Data["leg2.departure_actual"] = Iso(now.AddDays(8));
            if (!_tc.Data.ContainsKey("leg2.arrival_estimated"))
                _tc.Data["leg2.arrival_estimated"] = Iso(now.AddDays(17));
            if (!_tc.Data.ContainsKey("leg2.arrival_actual"))
                _tc.Data["leg2.arrival_actual"] = Iso(now.AddDays(18));

            if (!_tc.Data.ContainsKey("leg3.departure_estimated"))
                _tc.Data["leg3.departure_estimated"] = Iso(now.AddDays(18));
            if (!_tc.Data.ContainsKey("leg3.departure_actual"))
                _tc.Data["leg3.departure_actual"] = Iso(now.AddDays(18));
            if (!_tc.Data.ContainsKey("leg3.arrival_estimated"))
                _tc.Data["leg3.arrival_estimated"] = Iso(now.AddDays(23));
            if (!_tc.Data.ContainsKey("leg3.arrival_actual"))
                _tc.Data["leg3.arrival_actual"] = Iso(now.AddDays(23));

            if (!_tc.Data.ContainsKey("leg4.departure_estimated"))
                _tc.Data["leg4.departure_estimated"] = Iso(now.AddDays(23));
            if (!_tc.Data.ContainsKey("leg4.departure_actual"))
                _tc.Data["leg4.departure_actual"] = Iso(now.AddDays(25));
            if (!_tc.Data.ContainsKey("leg4.arrival_estimated"))
                _tc.Data["leg4.arrival_estimated"] = Iso(now.AddDays(26));
            if (!_tc.Data.ContainsKey("leg4.arrival_actual"))
                _tc.Data["leg4.arrival_actual"] = Iso(now.AddDays(28));

            if (!_tc.Data.ContainsKey("source_container_id_1"))
                _tc.Data["source_container_id_1"] = $"C1-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("container_number_1"))
                _tc.Data["container_number_1"] = $"C1-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("source_container_id_2"))
                _tc.Data["source_container_id_2"] = $"C2-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("container_number_2"))
                _tc.Data["container_number_2"] = $"C2-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("source_container_id_3"))
                _tc.Data["source_container_id_3"] = $"C3-{_tc.Data["timestamp"]}";
            if (!_tc.Data.ContainsKey("container_number_3"))
                _tc.Data["container_number_3"] = $"C3-{_tc.Data["timestamp"]}";

            if (!_tc.Data.ContainsKey("po_number"))
                _tc.Data["po_number"] = $"{DateTime.UtcNow:ddMMyy}{new Random().Next(10, 99)}";
            if (!_tc.Data.ContainsKey("purchase_order.mode"))
                _tc.Data["purchase_order.mode"] = "OCEAN";
            if (!_tc.Data.ContainsKey("load_type"))
                _tc.Data["load_type"] = "fcl";
            if (!_tc.Data.ContainsKey("shipt_to.address_type"))
                _tc.Data["shipt_to.address_type"] = "ship_to";

            if (!_tc.Data.ContainsKey("line_no"))
                _tc.Data["line_no"] = "1";
            if (!_tc.Data.ContainsKey("quantity_requested"))
                _tc.Data["quantity_requested"] = "150";
            if (!_tc.Data.ContainsKey("units_per_pack"))
                _tc.Data["units_per_pack"] = "100";
            if (!_tc.Data.ContainsKey("unit_price"))
                _tc.Data["unit_price"] = "10";
            if (!_tc.Data.ContainsKey("product_name"))
                _tc.Data["product_name"] = "Product name";
            if (!_tc.Data.ContainsKey("product_description"))
                _tc.Data["product_description"] = "Product Description";
            if (!_tc.Data.ContainsKey("product_category_code"))
                _tc.Data["product_category_code"] = "Category";
            if (!_tc.Data.ContainsKey("product_category_description"))
                _tc.Data["product_category_description"] = "Description";
            if (!_tc.Data.ContainsKey("color"))
                _tc.Data["color"] = "Color";
            if (!_tc.Data.ContainsKey("color_description"))
                _tc.Data["color_description"] = "Description";
            if (!_tc.Data.ContainsKey("size_code"))
                _tc.Data["size_code"] = "Size";
            if (!_tc.Data.ContainsKey("size_description"))
                _tc.Data["size_description"] = "Description";
            if (!_tc.Data.ContainsKey("customer_product_id"))
                _tc.Data["customer_product_id"] = "Product ID";
            if (!_tc.Data.ContainsKey("customer_sku"))
                _tc.Data["customer_sku"] = "SKU CODE";
            if (!_tc.Data.ContainsKey("sku_description"))
                _tc.Data["sku_description"] = "SKU Description";
            if (!_tc.Data.ContainsKey("upc"))
                _tc.Data["upc"] = "UPC / EAN";
            if (!_tc.Data.ContainsKey("total_packages"))
                _tc.Data["total_packages"] = "1";
            if (!_tc.Data.ContainsKey("packaging.id"))
                _tc.Data["packaging.id"] = "1";
            if (!_tc.Data.ContainsKey("packaging.code"))
                _tc.Data["packaging.code"] = "20";
            if (!_tc.Data.ContainsKey("packaging.description"))
                _tc.Data["packaging.description"] = "20' Container";
            if (!_tc.Data.ContainsKey("packaging.description_plural"))
                _tc.Data["packaging.description_plural"] = "20' Containers";
            if (!_tc.Data.ContainsKey("packaging.transport_mode"))
                _tc.Data["packaging.transport_mode"] = "OCEAN";
            if (!_tc.Data.ContainsKey("gross_weight_kgs"))
                _tc.Data["gross_weight_kgs"] = "1000";
            if (!_tc.Data.ContainsKey("volume_cbms"))
                _tc.Data["volume_cbms"] = "10";

            var today = DateTime.UtcNow;
            void SetDateParts(string prefix, DateTime dt)
            {
                if (!_tc.Data.ContainsKey($"{prefix}.year")) _tc.Data[$"{prefix}.year"] = dt.Year.ToString();
                if (!_tc.Data.ContainsKey($"{prefix}.month")) _tc.Data[$"{prefix}.month"] = dt.Month.ToString();
                if (!_tc.Data.ContainsKey($"{prefix}.day")) _tc.Data[$"{prefix}.day"] = dt.Day.ToString();
            }
            SetDateParts("order_date", today);
            SetDateParts("latest_delivery_date", today.AddDays(90));
            SetDateParts("earliest_delivery_date", today.AddDays(60));
            SetDateParts("inspection_passed_date", today);
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

        private static string TryReadOrderLineId(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("created_id", out var createdId))
                {
                    var id = createdId.ToString();
                    if (!string.IsNullOrWhiteSpace(id))
                        return id;
                }

                if (root.TryGetProperty("data", out var data))
                {
                    if (data.TryGetProperty("created_id", out var dataCreated))
                    {
                        var id = dataCreated.ToString();
                        if (!string.IsNullOrWhiteSpace(id))
                            return id;
                    }

                    if (data.TryGetProperty("lines", out var lines) && lines.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var line in lines.EnumerateArray())
                        {
                            if (line.TryGetProperty("id", out var lineId))
                            {
                                var id = lineId.ToString();
                                if (!string.IsNullOrWhiteSpace(id))
                                    return id;
                            }
                        }
                    }
                }
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
                    if (element.TryGetProperty(key, out var numProp) &&
                        (numProp.ValueKind == JsonValueKind.Number || numProp.ValueKind == JsonValueKind.String))
                    {
                        id = numProp.ToString();
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

        private static void SaveApiResponse(string prefix, string body)
        {
            try
            {
                var cwd = Directory.GetCurrentDirectory();
                var root = FindRepoRoot(cwd) ?? FindRepoRoot(AppContext.BaseDirectory);
                if (string.IsNullOrWhiteSpace(root))
                    return;

                var dir = Path.Combine(root, "DFP.Playwright", "Artifacts", "Logs");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
                File.WriteAllText(file, body);
            }
            catch
            {
                // ignore logging failures
            }
        }

        private static void SaveWebhookLogsResponse(string transactionId, string body)
        {
            try
            {
                var cwd = Directory.GetCurrentDirectory();
                var root = FindRepoRoot(cwd) ?? FindRepoRoot(AppContext.BaseDirectory);
                if (string.IsNullOrWhiteSpace(root))
                    return;

                var dir = Path.Combine(root, "DFP.Playwright", "Artifacts", "Logs");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, $"webhook-logs-{transactionId}-{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
                File.WriteAllText(file, body);
            }
            catch
            {
                // ignore logging failures
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
                return value;
            return value.Substring(0, maxLength) + "...";
        }
    }
}
