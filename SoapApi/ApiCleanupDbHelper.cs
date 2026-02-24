using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SoapApi.Helpers
{
    public class ApiCleanupDbHelper
    {
        private readonly CSSoapServiceSoapClient _soap;
        private readonly int _accessKey;

        private readonly int _recordQuantity;
        private readonly int _backwardsOrder;
        private readonly string _startDate; // yyyy-MM-dd
        private readonly string _endDate;   // yyyy-MM-dd
        private readonly HashSet<string> _excludedGuids;

        // type to XML container node name
        private static readonly Dictionary<string, string> TypeToXmlNode = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CK"] = "Checks",
            ["DP"] = "Deposits",
            ["PM"] = "Payments",
            ["IN"] = "Invoices",
            ["Bl"] = "Bills",
            ["SH"] = "Shipments",
            ["CR"] = "CargoReleases",
            ["SO"] = "SalesOrders",
            ["WH"] = "WarehouseReceipts",
            ["PK"] = "PickupOrders",
            ["BK"] = "Bookings",
            ["PO"] = "PurchaseOrders",
            ["QT"] = "Quotations",
            ["JB"] = "Jobs",
            ["IV"] = "ItemDefinitions",
        };

        public ApiCleanupDbHelper(
            CSSoapServiceSoapClient soap,
            int accessKey,
            DateTime startDate,
            DateTime endDate,
            int recordQuantity = 150,
            int backwardsOrder = 0,
            IEnumerable<string> excludedGuids = null)
        {
            _soap = soap ?? throw new ArgumentNullException(nameof(soap));
            _accessKey = accessKey;

            _recordQuantity = recordQuantity;
            _backwardsOrder = backwardsOrder;
            _startDate = startDate.ToString("yyyy-MM-dd");
            _endDate = endDate.ToString("yyyy-MM-dd");

            _excludedGuids = new HashSet<string>(excludedGuids ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Cleans transactions by type (e.g. "SH", "CR", "SO"...). Returns how many deletes were attempted.
        /// </summary>
        public async Task<int> CleanupTransactionsAsync(string type, int flags = 0)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type is required.", nameof(type));

            if (!TypeToXmlNode.TryGetValue(type, out var xmlNodeName))
                throw new ArgumentException($"Unknown transaction type '{type}'.", nameof(type));

            // 1) Get first page cookie
            var firstReq = new GetFirstTransbyDateRequest
            {
                access_key = _accessKey,
                type = type,
                start_date = _startDate,
                end_date = _endDate,
                flags = flags,
                record_quantity = _recordQuantity,
                backwards_order = _backwardsOrder
            };

            GetFirstTransbyDateResponse firstRes = await _soap.GetFirstTransbyDateAsync(firstReq);

            // NOTE: property names depend on generated code; these are the common ones:
            // - firstRes.cookie
            // - firstRes.moreResults
            // - firstRes.@return  (api_session_error)
            if (firstRes.@return != api_session_error.no_error)
                throw new Exception($"Error executing GetFirstTransbyDateAsync. Type={type}, Error={firstRes.@return}");

            string cookie = firstRes.cookie;
            int moreResults = firstRes.more_results;

            var ids = new List<string>();

            // 2) Page through results
            while (!string.IsNullOrEmpty(cookie) && moreResults != 0)
            {
                var nextReq = new GetNextTransbyDateRequest
                {
                    cookie = cookie
                };

                GetNextTransbyDateResponse nextRes = await _soap.GetNextTransbyDateAsync(nextReq);

                if (nextRes.@return != api_session_error.no_error)
                    break;

                // Again: typical property names
                // - nextRes.transListXML
                // - nextRes.cookie (updated)
                // - nextRes.moreResults
                string transListXml = nextRes.trans_list_xml;
                cookie = nextRes.cookie;
                moreResults = nextRes.more_results;

                if (string.IsNullOrWhiteSpace(transListXml))
                    continue;

                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(transListXml);

                    var container = doc.GetElementsByTagName(xmlNodeName).Item(0);
                    if (container == null)
                        continue;

                    var nodes = container.ChildNodes;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var guid = nodes[i]?.Attributes?["GUID"]?.Value;
                        if (!string.IsNullOrWhiteSpace(guid))
                            ids.Add(guid);
                    }
                }
                catch
                {
                    Console.WriteLine("Error parsing GetNextTransbyDate XML");
                    Console.WriteLine("Type: " + type);
                }
            }

            // 3) Delete
            int deleted = 0;

            foreach (var id in ids)
             {
                if (_excludedGuids.Contains(id))
                    continue;

                try
                {
                    // matches: Task<api_session_error> DeleteTransactionAsync(int access_key, string type, string number)
                    var err = await _soap.DeleteTransactionAsync(_accessKey, type, id);
                    deleted++;
                }
                catch
                {
                    Console.WriteLine($"{xmlNodeName} cleanup error. GUID={id}");
                }
            }

            return deleted;
        }

        /*UNUSED BUT COULD BE HELPFUL IN THE FUTURE
         * public async Task CleanupDefaultSetAsync(int flags = 0)
        {
            var result = await CleanupTransactionsAsync("SH", flags);
            result = await CleanupTransactionsAsync("CR", flags);
            result = await CleanupTransactionsAsync("SO", flags);
            result = await CleanupTransactionsAsync("WH", flags);
            result = await CleanupTransactionsAsync("PK", flags);
            result = await CleanupTransactionsAsync("PO", flags);
            result = await CleanupTransactionsAsync("BK", flags);
            result = await CleanupTransactionsAsync("QT", flags);
            result = await CleanupTransactionsAsync("JB", flags);
        }*/
    }
}
