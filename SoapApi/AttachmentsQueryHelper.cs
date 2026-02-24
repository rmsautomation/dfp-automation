using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoapApi
{
    public static class AttachmentsQueryHelper
    {
        public static async Task<string> GetAllAttachmentsOnlyAsync(
            CSSoapServiceSoapClient soap,
            int accessKey,
            string type,
            string transactionGuid,
            int flags = 0)
        {
            if (soap == null) throw new ArgumentNullException(nameof(soap));
            if (accessKey == 0) throw new ArgumentException("accessKey is required.", nameof(accessKey));
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("type is required.", nameof(type));
            if (string.IsNullOrWhiteSpace(transactionGuid)) throw new ArgumentException("transactionGuid is required.", nameof(transactionGuid));

            var req = new GetAllAttachmentsRequest
            {
                access_key = accessKey,
                flags = flags,
                type = type,
                number = transactionGuid
            };

            var res = await soap.GetAllAttachmentsAsync(req);

            if (res.@return != api_session_error.no_error)
                throw new Exception(
                    $"GetAllAttachments failed. Type={type}, GUID={transactionGuid}, Error={res.@return}, Msg={res.@return}");

            return res.attach_list_xml;
        }
    }
}
