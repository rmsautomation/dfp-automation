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

        public static async Task ImportAllFromResourcesAsync(CSSoapServiceSoapClient soap, int accessKey, bool forceDelete = true)
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

                await UpsertTransactionFromFileAsync(soap, accessKey, type, guid, file, forceDelete);
            }
        }

        public static async Task ImportWarehouseReceiptFromResourcesAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string warehouseGuid,
            string warehouseNumber,
            bool forceDelete = true)
        {
            await ImportTransactionFromResourcesAsync(soap, accessKey, "WH", warehouseGuid, warehouseNumber, forceDelete);
        }

        public static async Task ImportShipmentFromResourcesAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string shipmentGuid,
            string shipmentNumber,
            bool forceDelete = true)
        {
            await ImportTransactionFromResourcesAsync(soap, accessKey, "SH", shipmentGuid, shipmentNumber, forceDelete);
        }

        public static async Task ImportTransactionFromResourcesAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string transactionType,
            string transactionGuid,
            string transactionNumber,
            bool forceDelete = true)
        {
            var root = TransactionsRootPath();
            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException($"Transaction XML folder not found: {root}");

            var type = (transactionType ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("transactionType is required.", nameof(transactionType));
            if (string.IsNullOrWhiteSpace(transactionGuid))
                throw new ArgumentException("transactionGuid is required.", nameof(transactionGuid));
            if (string.IsNullOrWhiteSpace(transactionNumber))
                throw new ArgumentException("transactionNumber is required.", nameof(transactionNumber));

            var folder = FolderToType
                .FirstOrDefault(kvp => string.Equals(kvp.Value, type, StringComparison.OrdinalIgnoreCase))
                .Key;
            if (string.IsNullOrWhiteSpace(folder))
                throw new InvalidOperationException($"No folder mapping found for transaction type '{type}'.");

            var transactionFolder = Path.Combine(root, folder);
            var files = Directory.EnumerateFiles(transactionFolder, "*.xml", SearchOption.AllDirectories)
                                 .OrderBy(f => f)
                                 .ToList();
            if (files.Count == 0)
                throw new FileNotFoundException($"No XML files found under: {transactionFolder}");

            string? file = null;
            var targetNumber = NormalizeNumber(transactionNumber);
            foreach (var f in files)
            {
                try
                {
                    var doc = XDocument.Load(f);
                    var number = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "Number")?.Value?.Trim()
                              ?? doc.Descendants().FirstOrDefault(x => x.Name.LocalName == "PartNumber")?.Value?.Trim();
                    if (!string.IsNullOrWhiteSpace(number) &&
                        string.Equals(NormalizeNumber(number), targetNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        file = f;
                        break;
                    }
                }
                catch
                {
                    // ignore malformed file
                }
            }

            if (file == null)
            {
                throw new FileNotFoundException($"No XML matched Number '{transactionNumber}' under: {transactionFolder}");
            }

            await UpsertTransactionFromFileAsync(
                soap,
                accessKey,
                type,
                transactionGuid,
                file,
                forceDelete,
                transactionGuid,
                transactionNumber);
        }

        public static async Task UpsertTransactionFromFileAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string guid,
            string transactionXmlPath,
            bool forceDelete = true,
            string? overrideGuid = null,
            string? overrideNumber = null)
        {
            // Keep parameter for compatibility at call sites; imports now always upsert by SetTransaction only.
            _ = forceDelete;

            await SetTransactionOnlyAsync(soap, accessKey, type, transactionXmlPath, overrideGuid, overrideNumber);
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
            string transactionXmlPath,
            string? overrideGuid = null,
            string? overrideNumber = null)
        {
            var doc = XDocument.Load(transactionXmlPath);
            if (string.Equals(type, "WH", StringComparison.OrdinalIgnoreCase))
            {
                NormalizeWarehouseReceipt(doc, overrideGuid, overrideNumber);
            }
            else if (string.Equals(type, "SH", StringComparison.OrdinalIgnoreCase))
            {
                NormalizeShipment(doc, overrideGuid, overrideNumber);
            }
            else if (string.Equals(type, "IN", StringComparison.OrdinalIgnoreCase))
            {
                NormalizeInvoice(doc, overrideGuid, overrideNumber);
            }
            else
            {
                // Generic normalization for IV, CR, SO, PK, PO, BK, QT and any future types.
                // Sets the GUID on the root element so AfterScenario cleanup can delete the record.
                NormalizeGuid(doc, overrideGuid);
            }
            var xml = doc.ToString(SaveOptions.DisableFormatting);

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

        private static void NormalizeGuid(XDocument doc, string? overrideGuid)
        {
            if (string.IsNullOrWhiteSpace(overrideGuid)) return;

            var root = doc.Root;
            if (root == null) return;

            // Set GUID on the root element (covers IV, CR, SO, PK, PO, BK, QT, etc.)
            root.SetAttributeValue("GUID", overrideGuid);
        }

        private static string NormalizeNumber(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var chars = s.Where(char.IsLetterOrDigit).ToArray();
            return new string(chars).ToUpperInvariant();
        }

        private static void NormalizeWarehouseReceipt(XDocument doc, string? overrideGuid, string? overrideNumber)
        {
            var root = doc.Root;
            if (root == null)
                return;

            var now = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            if (!string.IsNullOrWhiteSpace(overrideGuid))
            {
                root.SetAttributeValue("GUID", overrideGuid);

                foreach (var el in root.Descendants())
                {
                    if (el.Name.LocalName == "WarehouseReceiptGUID")
                        el.Value = overrideGuid;
                    if (el.Name.LocalName == "OwnerGUID")
                        el.Value = overrideGuid;
                }
            }

            if (!string.IsNullOrWhiteSpace(overrideNumber))
            {
                foreach (var el in root.Descendants())
                {
                    if (el.Name.LocalName == "Number")
                        el.Value = overrideNumber;
                    if (el.Name.LocalName == "WarehouseReceiptNumber")
                        el.Value = overrideNumber;
                }
            }

            // Update CreatedOn only
            foreach (var el in root.Descendants())
            {
                if (el.Name.LocalName == "CreatedOn")
                    el.Value = now;
            }
        }

        private static void NormalizeShipment(XDocument doc, string? overrideGuid, string? overrideNumber)
        {
            var root = doc.Root;
            if (root == null)
                return;

            var now = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            if (!string.IsNullOrWhiteSpace(overrideGuid))
            {
                root.SetAttributeValue("GUID", overrideGuid);

                foreach (var el in root.Descendants())
                {
                    if (el.Name.LocalName == "ShipmentGUID")
                        el.Value = overrideGuid;
                    if (el.Name.LocalName == "OutShipmentGUID")
                        el.Value = overrideGuid;
                    if (el.Name.LocalName == "OwnerGUID")
                        el.Value = overrideGuid;
                }
            }

            if (!string.IsNullOrWhiteSpace(overrideNumber))
            {
                foreach (var el in root.Descendants())
                {
                    if (el.Name.LocalName == "Number")
                        el.Value = overrideNumber;
                    if (el.Name.LocalName == "ShipmentNumber")
                        el.Value = overrideNumber;
                }
            }

            foreach (var el in root.Descendants())
            {
                if (el.Name.LocalName == "CreatedOn")
                    el.Value = now;
            }
        }

        private static void NormalizeInvoice(XDocument doc, string? overrideGuid, string? overrideNumber)
        {
            // XML structure: <Invoices><Invoice GUID="..." Type="IN">...</Invoice></Invoices>
            // The GUID and Number are on the <Invoice> child element.
            var ns = doc.Root?.Name.Namespace ?? XNamespace.None;
            var invoiceEl = doc.Root?.Element(ns + "Invoice")
                         ?? doc.Root?.Descendants().FirstOrDefault(x => x.Name.LocalName == "Invoice");
            if (invoiceEl == null) return;

            var now = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

            if (!string.IsNullOrWhiteSpace(overrideGuid))
            {
                invoiceEl.SetAttributeValue("GUID", overrideGuid);

                foreach (var el in invoiceEl.Descendants())
                {
                    if (el.Name.LocalName == "InvoiceGUID")
                        el.Value = overrideGuid;
                    if (el.Name.LocalName == "OwnerGUID")
                        el.Value = overrideGuid;
                }
            }

            if (!string.IsNullOrWhiteSpace(overrideNumber))
            {
                foreach (var el in invoiceEl.Descendants())
                {
                    if (el.Name.LocalName == "Number")
                        el.Value = overrideNumber;
                }
            }

            foreach (var el in invoiceEl.Descendants())
            {
                if (el.Name.LocalName == "CreatedOn")
                    el.Value = now;
            }
        }
    }
}
