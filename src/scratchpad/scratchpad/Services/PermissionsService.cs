namespace scratchpad.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Models;

    public class PermissionsService : PayPalService
    {
        /// <summary>
        /// The path of the file containing the access token.
        /// </summary>
        private const string AccessTokenPath = @"C:\Users\Dan\Documents\GitHub\scratchpad\src\scratchpad\scratchpad\" +
                                                @"Services\AccessToken.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsService"/> class.
        /// </summary>
        /// <param name="client"></param>
        public PermissionsService(HttpClient client): base(client)
        {
        }

        /// <summary>
        /// Calls RequestPermissions from the PayPal Permissions API.
        /// <c>https://developer.paypal.com/docs/classic/api/permissions/RequestPermissions_API_Operation/</c>
        /// </summary>
        /// <returns>
        /// An <see cref="InteractionModel{PermissionsServiceModel}"/> containing information relevant to the
        /// <c>API</c> call.
        /// </returns>
        public async Task<InteractionModel<PermissionsServiceModel>> RequestPermissionsAsync()
        {
            var endPoint = new UriBuilder("https://svcs.sandbox.paypal.com/Permissions/RequestPermissions");

            const string callback = "http://localhost:6945/Permissions/Success";

            const string payload = "{\"scope\":\"EXPRESS_CHECKOUT\", \"callback\":\"" + callback +
                                   "\",  \"requestEnvelope\": {\"errorLanguage\":\"en_US\"}}";

            const string globalSandboxTestId = "APP-80W284485P519543T";
            var headersDictionary = new Dictionary<string, string>
            {
                // Sandbox API credentials for the API Caller account
                {"X-PAYPAL-SECURITY-USERID", "team-marvel-third-party_api1.mopowered.co.uk"},
                {"X-PAYPAL-SECURITY-PASSWORD", "ZLYCW77K5ZU3P9Q7"},
                {"X-PAYPAL-SECURITY-SIGNATURE", "AFcWxV21C7fd0v3bYYYRCpSSRl31ARl8V.b3gXLlfbYuOseHqi0nkGSV"},
                // Sandbox Application ID
                {"X-PAYPAL-APPLICATION-ID", globalSandboxTestId},
                // Input and output formats
                {"X-PAYPAL-REQUEST-DATA-FORMAT", "JSON"},
                {"X-PAYPAL-RESPONSE-DATA-FORMAT", "NV"},
            };

            var response = await CallApiWithHeadersAsync(endPoint.Uri, HttpMethod.Post, headersDictionary, payload);

            if (!response.IsSuccessful)
            {
                return InteractionModel<PermissionsServiceModel>.Failure(response.Errors);
            }

            var result = GetSuccessfulApiModel<PermissionsServiceModel>(response.Data);
            return InteractionModel<PermissionsServiceModel>.Successful(result);
        }

        /// <summary>
        /// Calls the GetAccessToken from the PayPal Permissions Service.
        /// <c>https://developer.paypal.com/docs/classic/api/permissions/GetAccessToken_API_Operation/</c>
        /// </summary>
        /// <param name="requestToken">
        /// The token appended as a query string to the callback URI after returning from paypal.
        /// </param>
        /// <param name="verificationCode">
        /// The verification code appended as a query string to the callback URI after returning from paypal.
        /// </param>
        /// <returns>
        /// An <see cref="InteractionModel{PermissionServiceModel}"/> containing information relevant to the <c>API</c>
        /// call and the authorization token and secret.
        /// </returns>
        public async Task<InteractionModel<PermissionsServiceModel>> GetAccessTokenAsync(string requestToken, string verificationCode)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var endPoint = new Uri("https://svcs.sandbox.paypal.com/Permissions/GetAccessToken");

            string payload = "{\"requestEnvelope\":{\"errorLanguage\":\"en_US\"},\"token\":\"" + requestToken +
                             "\",\"verifier\":\"" + verificationCode + "\"}";

            const string globalSandboxTestId = "APP-80W284485P519543T";
            var headersDictionary = new Dictionary<string, string>
            {
                {"X-PAYPAL-SECURITY-USERID", "team-marvel-third-party_api1.mopowered.co.uk"},
                {"X-PAYPAL-SECURITY-PASSWORD", "ZLYCW77K5ZU3P9Q7"},
                {"X-PAYPAL-SECURITY-SIGNATURE", "AFcWxV21C7fd0v3bYYYRCpSSRl31ARl8V.b3gXLlfbYuOseHqi0nkGSV"},
                {"X-PAYPAL-APPLICATION-ID", globalSandboxTestId},
                {"X-PAYPAL-REQUEST-DATA-FORMAT", "JSON"},
                {"X-PAYPAL-RESPONSE-DATA-FORMAT", "NV"}
            };

            var response = await CallApiWithHeadersAsync(endPoint, HttpMethod.Post, headersDictionary, payload);

            if (!response.IsSuccessful)
            {
                return InteractionModel<PermissionsServiceModel>.Failure(response.Errors);
            }

            var result = GetSuccessfulApiModel<PermissionsServiceModel>(response.Data);

            StoreAuthorization(result.Token, result.Secret);

            return InteractionModel<PermissionsServiceModel>.Successful(result);
        }

        /// <summary>
        /// Stores the access token and secret.
        /// </summary>
        /// <param name="token">The authorization token.</param>
        /// <param name="secret">The authorization secret.</param>
        private void StoreAuthorization(string token, string secret)
        {
            var fileContent = string.Format("Token={0}&Secret={1}", token, secret);

            System.IO.File.WriteAllText(AccessTokenPath, fileContent);
        }
    }
}