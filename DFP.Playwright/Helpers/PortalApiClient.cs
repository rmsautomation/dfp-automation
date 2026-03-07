using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DFP.Playwright.Helpers
{
    public sealed class PortalApiClient
    {
        private readonly HttpClient _http;

        public PortalApiClient(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public static PortalApiClient FromEnvironment()
        {
            var baseUrl = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.API_BASE_URL) ?? "");
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("API_BASE_URL is required.");
            var basePath = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.API_BASE_PATH) ?? "");
            var hubBasePath = ResolveEnvValue(Environment.GetEnvironmentVariable(Constants.API_HUB_BASE_PATH) ?? "");

            var http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
            };

            var client = new PortalApiClient(http)
            {
                _basePath = NormalizeBasePath(basePath),
                _hubBasePath = NormalizeBasePath(hubBasePath)
            };

            return client;
        }

        public async Task<string> GetPortalTokenAsync(string email, string password, string organizationId, string siteId)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("API_EMAIL is required.");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("API_PASSWORD is required.");
            if (string.IsNullOrWhiteSpace(organizationId))
                throw new InvalidOperationException("PORTAL_ORGANIZATION_ID is required.");
            if (string.IsNullOrWhiteSpace(siteId))
                throw new InvalidOperationException("PORTAL_SITE_ID is required.");

            var payload = new
            {
                email,
                password,
                organization_id = organizationId,
                site_id = siteId
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var loginPath = BuildPath("portals/auth/login");
            Console.WriteLine($"Portal login URL: {_http.BaseAddress}{loginPath}");
            using var response = await _http.PostAsync(loginPath, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Portal login failed ({(int)response.StatusCode}): {body}");

            var token = ExtractToken(body);
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal login succeeded but token was not found in response.");

            return token;
        }

        public async Task<HttpResponseMessage> HideShipmentAsync(string token, string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var hidePath = BuildHubPath($"shipments/v2/{shipmentId}/hide");
            using var request = new HttpRequestMessage(HttpMethod.Post, hidePath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Hide shipment URL: {_http.BaseAddress}{hidePath}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UnhideShipmentAsync(string token, string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var unhidePath = BuildHubPath($"shipments/v2/{shipmentId}/unhide");
            using var request = new HttpRequestMessage(HttpMethod.Post, unhidePath);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Unhide shipment URL: {_http.BaseAddress}{unhidePath}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> LinkShipmentToPurchaseOrderAsync(string token, string shipmentId, string purchaseOrderId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");
            if (string.IsNullOrWhiteSpace(purchaseOrderId))
                throw new InvalidOperationException("Purchase Order ID is required.");

            var path = BuildPath($"portals/shipments/v2/{shipmentId}/order/{purchaseOrderId}/link");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Link shipment/PO URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> LinkCargoItemToOrderLineAsync(string token, string shipmentId, string cargoItemId, string orderLineId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");
            if (string.IsNullOrWhiteSpace(cargoItemId))
                throw new InvalidOperationException("Cargo item ID is required.");
            if (string.IsNullOrWhiteSpace(orderLineId))
                throw new InvalidOperationException("Order line ID is required.");

            var path = BuildPath($"portals/shipments/v2/{shipmentId}/data/packages/{cargoItemId}/link-order-line/{orderLineId}");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Link cargo/order line URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> CreateShipmentViaWebhookAsync(string chainIoToken, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(chainIoToken))
                throw new InvalidOperationException("CHAINIO_TOKEN is required.");
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new InvalidOperationException("Webhook payload is required.");

            var path = BuildRootPath("webhooks/portals/shipments/shipment-update");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", chainIoToken);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            Console.WriteLine($"Shipment webhook URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetWebhookLogAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Webhook log username is required.");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Webhook log password is required.");

            var path = BuildRootPath($"webhooks/logs/{password}");
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", creds);
            Console.WriteLine($"Webhook logs URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetCargoItemsAsync(string token, string shipmentId, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var path = BuildPath($"portals/shipments/v2/{shipmentId}/data/packages?page={page}&page_size={pageSize}");
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Get cargo items URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SubscribeShipmentAsync(string token, string shipmentId, string internalTrackingNumber)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Hub token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");
            if (string.IsNullOrWhiteSpace(internalTrackingNumber))
                throw new InvalidOperationException("Internal tracking number is required.");

            var path = BuildHubPath($"shipments/v2/{shipmentId}/subscribe");
            var payload = JsonSerializer.Serialize(new
            {
                internal_tracking_number = internalTrackingNumber
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            Console.WriteLine($"Subscribe shipment URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SubscribeContainerAsync(string token, string shipmentId, string containerId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Hub token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");
            if (string.IsNullOrWhiteSpace(containerId))
                throw new InvalidOperationException("Container ID is required.");

            var path = BuildHubPath($"shipments/v2/{shipmentId}/data/containers/{containerId}/subscribe");
            var payload = JsonSerializer.Serialize(new
            {
                unique_container_id = containerId
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            Console.WriteLine($"Subscribe container URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetContainersAsync(string token, string shipmentId, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var path = BuildPath($"portals/shipments/v2/{shipmentId}/data/containers?page={page}&page_size={pageSize}");
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Get containers URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PushTrackingEventAsync(string project44Token, string organizationConnectionId, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(project44Token))
                throw new InvalidOperationException("Project44 token is required.");
            if (string.IsNullOrWhiteSpace(organizationConnectionId))
                throw new InvalidOperationException("Organization connection ID is required.");
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new InvalidOperationException("Tracking event payload is required.");

            var path = BuildRootPath($"webhooks/portals/project44/tracking-event/{organizationConnectionId}/push-event");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", project44Token);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            Console.WriteLine($"Push tracking event URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GetHubContainersAsync(string token, string shipmentId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Hub token is required.");
            if (string.IsNullOrWhiteSpace(shipmentId))
                throw new InvalidOperationException("Shipment ID is required.");

            var path = BuildHubPath($"shipments/v2/{shipmentId}/data/containers");
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Get hub containers URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> CreatePurchaseOrderAsync(string token, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new InvalidOperationException("Purchase order payload is required.");

            var path = BuildPath("portals/orders");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            Console.WriteLine($"Create purchase order URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        public async Task<HttpResponseMessage> CreatePurchaseOrderLineAsync(string token, string purchaseOrderId, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Portal token is required.");
            if (string.IsNullOrWhiteSpace(purchaseOrderId))
                throw new InvalidOperationException("Purchase Order ID is required.");
            if (string.IsNullOrWhiteSpace(payloadJson))
                throw new InvalidOperationException("Purchase order line payload is required.");

            var path = BuildPath($"portals/orders/{purchaseOrderId}/lines");
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
            Console.WriteLine($"Create purchase order line URL: {_http.BaseAddress}{path}");

            return await _http.SendAsync(request);
        }

        private string _basePath = "";
        private string _hubBasePath = "";

        private string BuildPath(string relative)
        {
            if (string.IsNullOrWhiteSpace(_basePath))
                return relative.TrimStart('/');
            return _basePath + "/" + relative.TrimStart('/');
        }

        private string BuildHubPath(string relative)
        {
            if (string.IsNullOrWhiteSpace(_hubBasePath))
                return BuildPath(relative);
            return _hubBasePath + "/" + relative.TrimStart('/');
        }

        private string BuildRootPath(string relative)
        {
            return relative.TrimStart('/');
        }

        private static string NormalizeBasePath(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                return "";
            return basePath.Trim().Trim('/').Trim();
        }

        private static string ExtractToken(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (TryGetToken(root, out var token))
                return token;

            if (root.TryGetProperty("data", out var data) && TryGetToken(data, out token))
                return token;

            if (root.TryGetProperty("result", out var result) && TryGetToken(result, out token))
                return token;

            return "";
        }

        private static bool TryGetToken(JsonElement element, out string token)
        {
            token = "";
            if (element.ValueKind != JsonValueKind.Object)
                return false;

            if (element.TryGetProperty("token", out var tokenProp) && tokenProp.ValueKind == JsonValueKind.String)
            {
                token = tokenProp.GetString() ?? "";
                return !string.IsNullOrWhiteSpace(token);
            }

            if (element.TryGetProperty("access_token", out var accessProp) && accessProp.ValueKind == JsonValueKind.String)
            {
                token = accessProp.GetString() ?? "";
                return !string.IsNullOrWhiteSpace(token);
            }

            if (element.TryGetProperty("jwt", out var jwtProp) && jwtProp.ValueKind == JsonValueKind.String)
            {
                token = jwtProp.GetString() ?? "";
                return !string.IsNullOrWhiteSpace(token);
            }

            return false;
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
