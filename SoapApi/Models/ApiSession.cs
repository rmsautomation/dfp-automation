using SoapApi.CssSoap;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SoapApi.Models
{
    public class ApiSession
    {
        public int Key { get; private set; }
        public string Username { get; }
        public string Password { get; }
        public CSSoapServiceSoapClient CSSoap { get; }
        public api_session_error ApiSessionError { get; set; }

        public ApiSession(string username, string password)
        {
            Username = username;
            Password = password;

            CSSoap = new CSSoapServiceSoapClient();

            var url = Environment.GetEnvironmentVariable("CSS_SOAP_URL");
            if (!string.IsNullOrWhiteSpace(url))
                CSSoap.Endpoint.Address = new EndpointAddress(url);

            Key = 0;
        }

        /// <summary>
        /// Starts the SOAP session (must be called explicitly)
        /// </summary>
        public async Task<api_session_error> StartSessionAsync()
        {
            var request = new StartSessionRequest
            {
                user = Username,
                pass = Password
            };

            var response = await CSSoap.StartSessionAsync(request);

            Key = response.access_key;
            ApiSessionError = response.@return;

            return ApiSessionError;
        }

        public Task<api_session_error> EndSessionAsync() => CSSoap.EndSessionAsync(Key);
    }
}
