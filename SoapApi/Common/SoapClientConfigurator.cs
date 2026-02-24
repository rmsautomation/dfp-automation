using SoapApi.CssSoap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoapApi.Common
{
    public static class SoapClientConfigurator
    {
        public static void Configure(CSSoapServiceSoapClient client, int timeoutMinutes = 3, int maxMb = 20)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var ts = TimeSpan.FromMinutes(timeoutMinutes);
            client.Endpoint.Binding.SendTimeout = ts;
            client.Endpoint.Binding.ReceiveTimeout = ts;
            client.Endpoint.Binding.OpenTimeout = ts;
            client.Endpoint.Binding.CloseTimeout = ts;

            if (client.Endpoint.Binding is BasicHttpBinding basic)
            {
                long bytes = maxMb * 1024L * 1024L;

                basic.TransferMode = TransferMode.Buffered;

                //  MUST match for Buffered
                basic.MaxReceivedMessageSize = bytes;
                basic.MaxBufferSize = (int)Math.Min(bytes, int.MaxValue);

                basic.ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxDepth = 64,
                    MaxStringContentLength = (int)Math.Min(bytes, int.MaxValue),
                    MaxArrayLength = (int)Math.Min(bytes, int.MaxValue),
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 16384
                };
            }
        }
    }
}
