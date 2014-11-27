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
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Models;
    using PayPal.Authentication;
    using PayPal.NVP;

    /// <summary>
    /// The class for interacting with the <c>PayPal</c> <c>NVP</c> <c>API</c>.
    /// <c>https://developer.paypal.com/docs/classic/api/NVPAPIOverview/</c>
    /// </summary>
    public class ExpressCheckoutService : PayPalService
    {
        private const string AccessTokenPath = @"C:\Users\Dan\Documents\GitHub\scratchpad\src\scratchpad\scratchpad\" +
                                                @"Services\AccessToken.txt";

        /// <summary>
        /// The <see cref="HttpClient"/>.
        /// </summary>
        private readonly HttpClient _client;

        private readonly SignatureCredential _credential;

        private readonly SignatureHttpHeaderAuthStrategy _authStrategy;

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
        /// Initializes a new instance of the <see cref="ExpressCheckoutService"/> class.
        /// </summary>
        public ExpressCheckoutService()
        {
            _client = new HttpClient();

            //// Team Marvel facilitator Credentials
            //const string user = "team-marvel-facilitator_api1.mopowered.co.uk";
            //const string password = "XXZ8ABL9WRWGMCKB";
            //const string signature = "AiPC9BjkCyDFQXbSkoZcgqH3hpacA50XSqbSfQ4CllGeJQDFDnEzHRrT";

            // Team Marvel Credentials for third party authentication
            const string user = "team-marvel-third-party_api1.mopowered.co.uk";
            const string password = "ZLYCW77K5ZU3P9Q7";
            const string signature = "AFcWxV21C7fd0v3bYYYRCpSSRl31ARl8V.b3gXLlfbYuOseHqi0nkGSV";

            _payPalCancelUri = "http://localhost:59709/Forzieri/Payment/PayPalCancelled";
            _payPalSuccessUri = "http://localhost:6945/Home/PayPalSuccess";
            _endpoint = GetEndpoint();
            _apiVersion = "119";
            _credential = new SignatureCredential(user, password, signature);
            _authStrategy = new SignatureHttpHeaderAuthStrategy(GetEndpoint().ToString());
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
                _credential.UserName,
                _credential.Password,
                _credential.Signature,
                _apiVersion,
                token,
                payerId);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallExpressCheckoutApiAsync(requestUri.Uri);

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
                _credential.UserName,
                _credential.Password,
                _credential.Signature,
                _apiVersion,
                token);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallExpressCheckoutApiAsync(requestUri.Uri);

            return result;
        }

        /// <summary>
        /// Handles the 'SetExpressCheckout' method of the <c>API</c>.
        /// <c>https://developer.paypal.com/docs/classic/api/merchant/SetExpressCheckout_API_Operation_NVP/</c>
        /// </summary>
        /// <returns>An <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        public async Task<InteractionModel<ExpressCheckout>> SetExpressCheckoutAsync()
        {
            var authorization = GetThirdPartyAuthentication();

            Dictionary<string, string> headerStrategy =
                _authStrategy.GenerateHeaderStrategy(new SignatureCredential("username", "password", "sign",
                    authorization));

            var queryParams = string.Format(
                "USER={0}&PWD={1}&SIGNATURE={2}&METHOD=SetExpressCheckout&VERSION={3}&" +
                    "PAYMENTREQUEST_0_PAYMENTACTION=SALE&PAYMENTREQUEST_0_AMT=19&" +
                    "PAYMENTREQUEST_0_CURRENCYCODE=USD&cancelUrl={4}&returnUrl={5}&SUBJECT={6}",
                _credential.UserName,
                _credential.Password,
                _credential.Signature,
                _apiVersion,
                _payPalCancelUri,
                _payPalSuccessUri,
                "team-marvel-facilitator@mopowered.co.uk");

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallExpressCheckoutApiAsync(requestUri.Uri, headerStrategy);

            return result;
        }

        private async Task<InteractionModel<ExpressCheckout>> CallExpressCheckoutApiAsync(
            Uri apiUri,
            Dictionary<string, string> headers = null)
        {
            var response = await CallApiWithHeadersAsync(apiUri, HttpMethod.Get, headers);

            if (!response.IsSuccessful)
            {
                return InteractionModel<ExpressCheckout>.Failure(response.Errors);
            }

            if (response.Data.Contains("ACK=SuccessWithWarning"))
            {
                return GetModelWithWarning(response.Data);
            }

            if (response.Data.Contains("ACK=Success"))
            {
                return GetSuccessfulModel(response.Data);
            }

            string exceptionMessage = string.Format(
                "There has been an error making the request to paypal. The API response was '{0}'",
                Uri.UnescapeDataString(response.Data));

            return InteractionModel<ExpressCheckout>.Failure(exceptionMessage);
        }

        public async Task<InteractionModel<string>> CallApiWithHeadersAsync(
            Uri apiUri,
            HttpMethod method,
            Dictionary<string, string> headers = null,
            string payload = null)
        {
            var request = new HttpRequestMessage(method, apiUri);
            if (method == HttpMethod.Post)
            {
                request.Content = new StringContent(payload ?? string.Empty, Encoding.UTF8,
                    "application/x-www-form-urlencoded");
            }


            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent))
            {
                return InteractionModel<string>.Failure("The api call has failed.");
            }

            return InteractionModel<string>.Successful(responseContent);
        }

        /// <summary>
        /// Gets a successful <see cref="InteractionModel{ExpressCheckout}"/>.
        /// </summary>
        /// <param name="responseContent">The <see cref="string"/> containing the response of the API call.</param>
        /// <returns>A successful <see cref="InteractionModel{ExpressCheckout}"/>.</returns>
        private InteractionModel<ExpressCheckout> GetSuccessfulModel(string responseContent)
        {
            var responseDictionary = NvpToDictionary(responseContent);

            var checkout = ExpressCheckout.InitializeFromDict(responseDictionary);

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


        private TokenAuthorization GetThirdPartyAuthentication()
        {
            List<string> lines = System.IO.File.ReadAllLines(AccessTokenPath).ToList();

            if (lines.Count != 1)
                throw new InvalidOperationException("The data in the access token file is missing or incorrectly formatted.");

            var line = lines.Single();

            var splitLine = line.Split('&');

            var token = splitLine[0].Split('=')[1];
            var secret = splitLine[1].Split('=')[1];

            var result = new TokenAuthorization(token, secret);

            return result;
        }
        // Define other methods and classes here
    }
}