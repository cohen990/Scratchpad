namespace scratchpad.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Models;

    public class PermissionsService : PayPalService
    {
        private const string AccessTokenPath = @"C:\Users\Dan\Documents\GitHub\scratchpad\src\scratchpad\scratchpad\" +
                                                @"Services\AccessToken.txt";

        private PermissionsServiceModel GetPermissionServiceModel(string responseContent)
        {
            var dict = NvpToDictionary(responseContent);

            var result = PermissionsServiceModel.InitializeFromDict(dict);

            return result;
        }

        private readonly HttpClient _client;

        public PermissionsService()
        {
            _client = new HttpClient();
        }

        public async Task<InteractionModel<PermissionsServiceModel>> RequestPermissions()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var endPoint = new UriBuilder("https://svcs.sandbox.paypal.com/Permissions/RequestPermissions");

            const string callback = "http://localhost:6945/Permissions/Success";

            const string payload = "{\"scope\":\"EXPRESS_CHECKOUT\", \"callback\":\"" + callback +
                                   "\",  \"requestEnvelope\": {\"errorLanguage\":\"en_US\"}}";


            var request = new HttpRequestMessage(HttpMethod.Post, endPoint.Uri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            // Sandbox API credentials for the API Caller account
            request.Content.Headers.Add("X-PAYPAL-SECURITY-USERID", "team-marvel-third-party_api1.mopowered.co.uk");
            request.Content.Headers.Add("X-PAYPAL-SECURITY-PASSWORD", "ZLYCW77K5ZU3P9Q7");
            request.Content.Headers.Add("X-PAYPAL-SECURITY-SIGNATURE", "AFcWxV21C7fd0v3bYYYRCpSSRl31ARl8V.b3gXLlfbYuOseHqi0nkGSV");

            // Sandbox Application ID
            const string globalSandboxTestId = "APP-80W284485P519543T";
            request.Content.Headers.Add("X-PAYPAL-APPLICATION-ID", globalSandboxTestId);

            // Input and output formats
            request.Content.Headers.Add("X-PAYPAL-REQUEST-DATA-FORMAT", "JSON");
            request.Content.Headers.Add("X-PAYPAL-RESPONSE-DATA-FORMAT", "NV");

            var response = await _client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var resultDict = NvpToDictionary(responseContent);

            if (responseContent.Contains("ack=Success"))
            {
                var result = PermissionsServiceModel.InitializeFromDict(resultDict);

                return InteractionModel<PermissionsServiceModel>.Successful(result);
            }

            Dictionary<string, string> responseDictionary = NvpToDictionary(responseContent);

            string warningMessage;
            responseDictionary.TryGetValue("error(0).message", out warningMessage);

            warningMessage = string.IsNullOrEmpty(warningMessage)
                ? "The API has returned a warning"
                : warningMessage;

            return InteractionModel<PermissionsServiceModel>.Failure(warningMessage);
        }

        public async Task<InteractionModel<PermissionsServiceModel>> GetAccessToken(string requestToken, string verificationCode)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var endPoint = new Uri("https://svcs.sandbox.paypal.com/Permissions/GetAccessToken");

            string payload = "{\"requestEnvelope\":{\"errorLanguage\":\"en_US\"},\"token\":\"" + requestToken +
                             "\",\"verifier\":\"" + verificationCode + "\"}";


            var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            // Sandbox API credentials for the API Caller account
            request.Content.Headers.Add("X-PAYPAL-SECURITY-USERID", "team-marvel-third-party_api1.mopowered.co.uk");
            request.Content.Headers.Add("X-PAYPAL-SECURITY-PASSWORD", "ZLYCW77K5ZU3P9Q7");
            request.Content.Headers.Add("X-PAYPAL-SECURITY-SIGNATURE", "AFcWxV21C7fd0v3bYYYRCpSSRl31ARl8V.b3gXLlfbYuOseHqi0nkGSV");

            // Sandbox Application ID
            const string globalSandboxTestId = "APP-80W284485P519543T";
            request.Content.Headers.Add("X-PAYPAL-APPLICATION-ID", globalSandboxTestId);

            // Input and output formats
            request.Content.Headers.Add("X-PAYPAL-REQUEST-DATA-FORMAT", "JSON");
            request.Content.Headers.Add("X-PAYPAL-RESPONSE-DATA-FORMAT", "NV");

            var response = await _client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!responseContent.Contains("error(0)"))
            {
                var result = GetPermissionServiceModel(responseContent);

                WriteTokenToFile(result.Token, result.Secret);

                return InteractionModel<PermissionsServiceModel>.Successful(result);
            }

            var responseDict = NvpToDictionary(responseContent);

            const string errorMessageKey = "error(0).message";
            string errorMessage = responseDict.ContainsKey(errorMessageKey)
                ? responseDict[errorMessageKey]
                : "There has been a problem getting the authentication key from paypal.";

            return InteractionModel<PermissionsServiceModel>.Failure(errorMessage);
        }

        private void WriteTokenToFile(string token, string secret)
        {
            var fileContent = string.Format("Token={0}&Secret={1}", token, secret);

            System.IO.File.WriteAllText(AccessTokenPath, fileContent);
        }
    }
}