using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DFP.Playwright.Helpers
{
    public static class TransactionImportHelper
    {
        // Map folder names to Magaya transaction type codes
        private static readonly Dictionary<string, string> FolderToType = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Warehouse Receipts"] = "WH",
            ["Shipments"] = "SH",
            ["Cargo Release"] = "CR",
            ["Sales Orders"] = "SO",
            ["Pickup Orders"] = "PK",
            ["Purchase Orders"] = "PO",
            ["Booking"] = "BK",
            ["Quotations"] = "QT",
            ["Invoice"] = "IN",
            ["Inventory"] = "IV",
        };

        public static async Task ImportAllFromResourcesAsync(CSSoapServiceSoapClient soap, int accessKey)
        {
            var root = TransactionsRootPath();

            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException($"Transaction XML folder not found: {root}");

            var files = Directory.EnumerateFiles(root, "*.xml", SearchOption.AllDirectories)
                                 .OrderBy(f => f)
                                 .ToList();

            foreach (var file in files)
            {
                var type = InferTypeFromPath(file);
                var guid = ExtractGuidFromTransactionXml(file);

                if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(guid))
                    continue;

                await UpsertTransactionFromFileAsync(soap, accessKey, type, guid, file);
            }
        }

        public static async Task UpsertTransactionFromFileAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string guid,
            string transactionXmlPath)
        {
            var existingXml = await TryGetTransactionXmlAsync(soap, accessKey, type, guid);

            if (existingXml != null)
            {
                // Skip if already exists
                return;
            }

            await SetTransactionOnlyAsync(soap, accessKey, type, transactionXmlPath);
        }

        public static string TransactionsRootPath()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            return Path.Combine(baseDir, "Resources", "TransactionsXml");
        }

        private static string InferTypeFromPath(string filePath)
        {
            // Folder immediately under TransactionsXml (e.g. "Shipments", "Sales Orders", etc.)
            var folder = new DirectoryInfo(Path.GetDirectoryName(filePath) ?? "").Name;

            if (!FolderToType.TryGetValue(folder, out var type))
                return "";

            return type;
        }

        private static string ExtractGuidFromTransactionXml(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var node = doc.Descendants().FirstOrDefault(x => x.Attribute("GUID") != null);
            return node?.Attribute("GUID")?.Value ?? "";
        }

        private static async Task SetTransactionOnlyAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string transactionXmlPath)
        {
            var xml = XDocument.Load(transactionXmlPath).ToString(SaveOptions.DisableFormatting);

            var req = new SetTransactionRequest
            {
                access_key = accessKey,
                type = type,
                flags = 0,
                trans_xml = xml
            };

            var res = await soap.SetTransactionAsync(req);

            if (res.@return != api_session_error.no_error)
                throw new InvalidOperationException($"SetTransaction failed. Type={type}, File={transactionXmlPath}, Error={res.@return}, Msg={res.error_desc}");
        }

        private static async Task<string?> TryGetTransactionXmlAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string transactionNumberOrGuid)
        {
            var req = new GetTransactionRequest
            {
                access_key = accessKey,
                type = type,
                number = transactionNumberOrGuid
            };

            var res = await soap.GetTransactionAsync(req);
            return res.@return == api_session_error.no_error ? res.trans_xml : null;
        }
    }
}
