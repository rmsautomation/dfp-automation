using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoapApi
{
    public static class TransactionAttachmentHelper
    {
        public static async Task<api_session_error> SetTransactionAndAttachmentAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,              // "WH"
            string transactionGuid,    // "730c..."
            string transactionXmlPath, // @"Resources\WHR_4.xml"
            string attachmentXmlPath   // @"Resources\Attachment.xml"
        )
        {
            if (soap == null) throw new ArgumentNullException(nameof(soap));
            if (accessKey == 0) throw new ArgumentException("accessKey is required.", nameof(accessKey));
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("type is required.", nameof(type));
            if (string.IsNullOrWhiteSpace(transactionGuid)) throw new ArgumentException("transactionGuid is required.", nameof(transactionGuid));

            var transXml = XDocument.Load(transactionXmlPath).ToString(SaveOptions.DisableFormatting);

            // ---- SetTransactionRequest ----
            var setTransReq = new SetTransactionRequest
            {
                access_key = accessKey,
                type = type,
                flags = 0,
                trans_xml = transXml
            };

            SetTransactionResponse setTransRes = await soap.SetTransactionAsync(setTransReq);

            // Most generated proxies expose:
            // - setTransRes.@return  (api_session_error)
            // - setTransRes.error    (string)
            var setTransErr = setTransRes.@return;
            var setTransErrorText = setTransRes.error_desc;

            if (setTransErr != api_session_error.no_error)
                throw new Exception($"SetTransaction failed. Type={type}, Error={setTransErr}, Message={setTransErrorText}");

            // ---- SetAttachmentAsync ----
            var attachXml = XDocument.Load(attachmentXmlPath).ToString(SaveOptions.DisableFormatting);

            var setAttReq = new SetAttachmentRequest
            {
                access_key = accessKey,
                flags = 0,
                type = type,
                number = transactionGuid,
                attach_xml = attachXml
            };

            SetAttachmentResponse setAttRes = await soap.SetAttachmentAsync(setAttReq);

            var setAttErr = setAttRes.@return;
            var setAttErrorText = setAttRes.error_desc;

            if (setAttErr != api_session_error.no_error)
                throw new Exception($"SetAttachment failed. Type={type}, GUID={transactionGuid}, Error={setAttErr}, Message={setAttErrorText}");

            return api_session_error.no_error;
        }

        /// <summary>
        /// Convenience builder for your "Resources" folder next to the test assembly.
        /// </summary>
        public static string ResourcePath(string fileName)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(baseDir ?? "", "Resources", fileName);
        }
    }
}
