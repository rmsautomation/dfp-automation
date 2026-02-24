using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoapApi
{
    public static class AttachmentHelper
    {
        public static async Task<api_session_error> SetAttachmentOnlyAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string transactionGuid,
            string attachmentXmlPath,
            int flags = 0)
        {
            if (soap == null) throw new ArgumentNullException(nameof(soap));
            if (accessKey == 0) throw new ArgumentException("accessKey is required.", nameof(accessKey));
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("type is required.", nameof(type));
            if (string.IsNullOrWhiteSpace(transactionGuid)) throw new ArgumentException("transactionGuid is required.", nameof(transactionGuid));
            if (string.IsNullOrWhiteSpace(attachmentXmlPath)) throw new ArgumentException("attachmentXmlPath is required.", nameof(attachmentXmlPath));

            var attachmentXml = XDocument.Load(attachmentXmlPath).ToString(SaveOptions.DisableFormatting);

            var req = new SetAttachmentRequest
            {
                access_key = accessKey,
                flags = flags,
                type = type,
                number = transactionGuid,
                attach_xml = attachmentXml
            };
            if (type == "SH")
            {
                ;
            }

            var res = await soap.SetAttachmentAsync(req);
            if (type == "SH")
            {
                ;
            }

            if (res.@return != api_session_error.no_error)
            {
                Console.WriteLine(
                    $"SetAttachment failed. Type={type}, GUID={transactionGuid}, File={attachmentXmlPath}, " +
                    $"Error={res.@return}, Msg={res.error_desc}");
            }
            else
            {
                Console.WriteLine(
                    $"SetAttachment success. Type={type}, GUID={transactionGuid}, File={attachmentXmlPath}, " +
                    $"Error={res.@return}, Msg={res.error_desc}");
            }

            return res.@return;
        }
    }
}
