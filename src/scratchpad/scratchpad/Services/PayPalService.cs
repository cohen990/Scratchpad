// -----------------------------------------------------------------------
// <copyright company="MoPowered">
// Copyright (c) MoPowered. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace scratchpad.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// The class for interacting with the <c>PayPal</c> <c>NVP</c> <c>API</c>.
    /// <c>https://developer.paypal.com/docs/classic/api/NVPAPIOverview/</c>
    /// </summary>
    public class PayPalService
    {

        /// <summary>
        /// The <see cref="HttpClient"/>.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// The <c>API</c> authentication user.
        /// <c>https://developer.paypal.com/docs/classic/api/apiCredentials/#creating-classic-api-credentials</c>
        /// </summary>
        private readonly string _user;

        /// <summary>
        /// The <c>API</c> authentication password.
        /// <c>https://developer.paypal.com/docs/classic/api/apiCredentials/#creating-classic-api-credentials</c>
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The <c>API</c> authentication signature.
        /// <c>https://developer.paypal.com/docs/classic/api/apiCredentials/#creating-classic-api-credentials</c>
        /// </summary>
        private readonly string _signature;

        /// <summary>
        /// The <c>API</c> end point.
        /// </summary>
        private readonly Uri _endpoint;

        /// <summary>
        /// The <see cref="Uri"/> to be redirected to after a successful <c>PayPal</c> payment.
        /// </summary>
        private readonly string _payPalSuccessUri;

        /// <summary>
        /// The <see cref="Uri"/> to be redirected to after a cancelled <c>PayPal</c> payment.
        /// </summary>
        private readonly string _payPalCancelUri;

        private readonly string _apiVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalService"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="ForzieriConfiguration"/>.</param>
        /// <param name="client">The <see cref="HttpClient"/>.</param>
        public PayPalService()
        {
            _client = new HttpClient();
            // Ben West Credentials
            //_user = "ben.we_1347891576_biz_api1.mobankgroup.com";
            //_password = "1347891624";
            //_signature = "ABQaomIXAXCdYJSw0qCD.83z-JdXAphfeq8PxbiDycSoYC.Gd4NLWcr.";

            // Team Marvel Credentials
            _user = "team-marvel-facilitator_api1.mopowered.co.uk";
            _password = "XXZ8ABL9WRWGMCKB";
            _signature = "AiPC9BjkCyDFQXbSkoZcgqH3hpacA50XSqbSfQ4CllGeJQDFDnEzHRrT";

            _payPalCancelUri = "http://localhost:59709/Forzieri/Payment/PayPalCancelled";
            _payPalSuccessUri = "http://localhost:6945/Home/PayPalSuccess";
            _endpoint = GetEndpoint();
            _apiVersion = "119";
        }

        /// <summary>
        /// Handles the 'DoExpressCheckoutPayment' method of the <c>API</c>
        /// <c>https://developer.paypal.com/docs/classic/api/merchant/DoExpressCheckoutPayment_API_Operation_NVP/</c>
        /// </summary>
        /// <param name="token">The token identifying the <c>PayPal</c> purchase.</param>
        /// <param name="payerId">The payerId - identifying the user who has logged into <c>PayPal</c>.</param>
        /// <returns>An <see cref="InteractionModel"/>.</returns>
        public async Task<InteractionModel<ExpressCheckout>> DoExpressCheckoutPaymentAsync(string token, string payerId)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");
            if (string.IsNullOrEmpty(payerId))
                throw new ArgumentNullException("payerId");

            var queryParams = string.Format(
                "USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&METHOD=DoExpressCheckoutPayment&TOKEN={4}&PAYERID={5}" +
                    "&PAYMENTREQUEST_0_AMT=19.00",
                _user,
                _password,
                _signature,
                _apiVersion,
                token,
                payerId);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallApiAsync(requestUri.Uri);

            return result;
        }

        /// <summary>
        /// Handles the 'GetExpressCheckoutDetails' method of the <c>API</c>.
        /// <c>https://developer.paypal.com/docs/classic/api/merchant/GetExpressCheckoutDetails_API_Operation_NVP/</c>
        /// </summary>
        /// <param name="token">The token identifying the <c>PayPal</c> purchase.</param>
        /// <returns>An <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        public async Task<InteractionModel<ExpressCheckout>> GetExpressCheckoutDetailsAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");

            var queryParams = string.Format(
                "USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&METHOD=GetExpressCheckoutDetails&TOKEN={4}",
                _user,
                _password,
                _signature,
                _apiVersion,
                token);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallApiAsync(requestUri.Uri);

            return result;
        }

        /// <summary>
        /// Handles the 'SetExpressCheckout' method of the <c>API</c>.
        /// <c>https://developer.paypal.com/docs/classic/api/merchant/SetExpressCheckout_API_Operation_NVP/</c>
        /// </summary>
        /// <returns>An <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        public async Task<InteractionModel<ExpressCheckout>> SetExpressCheckoutAsync()
        {
            var queryParams = string.Format(
                "USER={0}&PWD={1}&SIGNATURE={2}&METHOD=SetExpressCheckout&VERSION={3}&" +
                    "PAYMENTREQUEST_0_PAYMENTACTION=SALE&PAYMENTREQUEST_0_AMT=19&" +
                    "PAYMENTREQUEST_0_CURRENCYCODE=USD&cancelUrl={4}&returnUrl={5}",
                _user,
                _password,
                _signature,
                _apiVersion,
                _payPalCancelUri,
                _payPalSuccessUri);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallApiAsync(requestUri.Uri);

            return result;
        }

        private async Task<InteractionModel<ExpressCheckout>> CallApiAsync(Uri apiUri)
        {
            string responseContent = await _client.GetStringAsync(apiUri);

            if (string.IsNullOrEmpty(responseContent))
            {
                return InteractionModel<ExpressCheckout>.Failure("The PayPal API call failed. The response was empty.");
            }

            if (responseContent.Contains("ACK=SuccessWithWarning"))
            {
                return GetModelWithWarning(responseContent);
            }

            if (responseContent.Contains("ACK=Success"))
            {
                return GetSuccessfulModel(responseContent);
            }

            string exceptionMessage = string.Format(
                "There has been an error making the request to paypal. The API response was '{0}'",
                Uri.UnescapeDataString(responseContent));

            return InteractionModel<ExpressCheckout>.Failure(exceptionMessage);
        }

        /// <summary>
        /// Unescapes and converts the <c>NVP</c> response of the <c>API</c> into a dictionary.
        /// </summary>
        /// <param name="nvpInput">The <c>NVP</c> response of an <c>API</c> call.</param>
        /// <returns>
        /// A <see cref="Dictionary{String, String}"/> containing the Names as Keys and the Values as Values.
        /// </returns>
        private Dictionary<string, string> NvpToDictionary(string nvpInput)
        {
            if (string.IsNullOrEmpty(nvpInput))
                throw new ArgumentNullException("nvpInput");

            var nameValuePairs = nvpInput.Split('&');

            Dictionary<string, string> result = nameValuePairs.Select(pair => pair.Split('='))
                .ToDictionary(
                    splitPair => Uri.UnescapeDataString(splitPair[0]),
                    splitPair => Uri.UnescapeDataString(splitPair[1])
                );

            return result;
        }

        /// <summary>
        /// Gets a successful <see cref="InteractionModel{ExpressCheckout}"/>.
        /// </summary>
        /// <param name="responseContent">The <see cref="string"/> containing the response of the API call.</param>
        /// <returns>A successful <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        private InteractionModel<ExpressCheckout> GetSuccessfulModel(string responseContent)
        {
            var responseDictionary = NvpToDictionary(responseContent);

            var checkout = new ExpressCheckout();
            checkout.InitializeFromDict(responseDictionary);

            return InteractionModel<ExpressCheckout>.Successful(checkout);
        }

        /// <summary>
        /// Gets a failed <see cref="InteractionModel{ExpressCheckout}"/> with the error message being the warning
        /// message returned by the API (or, if this is missing, a default error message).
        /// </summary>
        /// <param name="responseContent">The <see cref="string"/> containing the response of the API call.</param>
        /// <returns>A failed <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        private InteractionModel<ExpressCheckout> GetModelWithWarning(string responseContent)
        {
            Dictionary<string, string> responseDictionary = NvpToDictionary(responseContent);

            string warningMessage;
            responseDictionary.TryGetValue("L_LONG_MESSAGE", out warningMessage);

            warningMessage = string.IsNullOrEmpty(warningMessage)
                ? "The API has returned a warning"
                : warningMessage;

            return InteractionModel<ExpressCheckout>.Failure(warningMessage);
        }

        /// <summary>
        /// Gets the end point of an <c>API</c> call depending on the settings.
        /// </summary>
        /// <returns>
        /// A <see cref="Uri"/>.
        /// Either <c>https://api-3t.sandbox.paypal.com/nvp</c> if this is a test transaction
        /// or <c>https://api-3t.paypal.com/nvp</c> if it is a real transaction.
        /// </returns>
        private Uri GetEndpoint()
        {
            var isTest = true;

            if (isTest)
            {
                return new Uri("https://api-3t.sandbox.paypal.com/nvp");
            }

            return new Uri("https://api-3t.paypal.com/nvp");
        }

            // Define other methods and classes here
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
                    var result = new PermissionsServiceModel();
                    result.InitializeFromDict(resultDict);

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

        public async Task GetAccessToken(string requestToken, string verificationCode)
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

            var resultDict = NvpToDictionary(responseContent);
        }
    }
}