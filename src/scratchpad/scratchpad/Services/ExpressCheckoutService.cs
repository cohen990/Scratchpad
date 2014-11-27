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
        /// <summary>
        /// The path on disk of the Access Token.
        /// </summary>
        private const string AccessTokenPath = @"C:\Users\Dan\Documents\GitHub\scratchpad\src\scratchpad\scratchpad\" +
                                                @"Services\AccessToken.txt";

        /// <summary>
        /// The <see cref="TokenAuthorization"/>.
        /// </summary>
        private readonly TokenAuthorization _authorization;

        /// <summary>
        /// The <see cref="SignatureCredential"/>.
        /// </summary>
        private readonly SignatureCredential _credential;

        /// <summary>
        /// The <see cref="SignatureHttpHeaderAuthStrategy"/>
        /// </summary>
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

        /// <summary>
        /// The api version being used.
        /// </summary>
        private readonly string _apiVersion;

        /// <summary>
        /// The email of the client we are acting as a third party for.
        /// </summary>
        private readonly string _subjectEmail;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressCheckoutService"/> class.
        /// </summary>
        public ExpressCheckoutService(HttpClient client) : base(client)
        {
            _authorization = GetTokenAuthorization();

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
            _endpoint = GetExpressCheckoutEndpointEndpoint();
            _apiVersion = "119";
            _credential = new SignatureCredential(user, password, signature);
            _authStrategy = new SignatureHttpHeaderAuthStrategy(GetExpressCheckoutEndpointEndpoint().ToString());
            _subjectEmail = "team-marvel-facilitator@mopowered.co.uk";
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

            string queryParams = string.Format(
                "USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&METHOD=DoExpressCheckoutPayment&TOKEN={4}&PAYERID={5}" +
                    "&PAYMENTREQUEST_0_AMT=19.00&SUBJECT={6}",
                _credential.UserName,
                _credential.Password,
                _credential.Signature,
                _apiVersion,
                token,
                payerId,
                _subjectEmail);

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
                "USER={0}&PWD={1}&SIGNATURE={2}&VERSION={3}&METHOD=GetExpressCheckoutDetails&TOKEN={4}&SUBJECT={5}",
                _credential.UserName,
                _credential.Password,
                _credential.Signature,
                _apiVersion,
                token,
                _subjectEmail);

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
                _subjectEmail);

            var requestUri = new UriBuilder(_endpoint) { Query = queryParams };

            var result = await CallExpressCheckoutApiAsync(requestUri.Uri);

            return result;
        }

        /// <summary>
        /// This method processes the <c>API</c> call.
        /// </summary>
        /// <param name="apiUri">The Uri (with all query parameters included)</param>
        /// <param name="successIndicator">The string which will indicate a successful <c>API</c> interaction.</param>
        /// <returns>
        /// An <see cref="InteractionModel{ExpressCheckout}"/> containing the data relevant to the <c>API</c> call.
        /// </returns>
        private async Task<InteractionModel<ExpressCheckout>> CallExpressCheckoutApiAsync(
            Uri apiUri,
            string successIndicator = "ACK=Success")
        {
            var headerStrategy =
                _authStrategy.GenerateHeaderStrategy(new SignatureCredential(_credential.UserName, _credential.Password,
                    _credential.Signature, _authorization));

            var response = await CallApiWithHeadersAsync(apiUri, HttpMethod.Get, headerStrategy);

            if (response.IsSuccessful && response.Data.Contains(successIndicator))
            {
                var result = GetSuccessfulApiModel<ExpressCheckout>(response.Data);
                return InteractionModel<ExpressCheckout>.Successful(result);
            }

            string exceptionMessage = "There has been an error making the request to paypal.";

            exceptionMessage += response.Data != null
                ? string.Format(" The API response was '{0}'", Uri.UnescapeDataString(response.Data))
                : " The API call recieved no reponse.";

            return InteractionModel<ExpressCheckout>.Failure(exceptionMessage);
        }

        /// <summary>
        /// Gets the end point of an <c>API</c> call depending on the settings.
        /// </summary>
        /// <returns>
        /// A <see cref="Uri"/>.
        /// Either <c>https://api-3t.sandbox.paypal.com/nvp</c> if this is a test transaction
        /// or <c>https://api-3t.paypal.com/nvp</c> if it is a real transaction.
        /// </returns>
        private Uri GetExpressCheckoutEndpointEndpoint()
        {
            var isTest = true;

            if (isTest)
            {
                return new Uri("https://api-3t.sandbox.paypal.com/nvp");
            }

            return new Uri("https://api-3t.paypal.com/nvp");
        }

        /// <summary>
        /// Gets the third party authentication token and secret.
        /// </summary>
        /// <returns>A <see cref="TokenAuthorization"/>.</returns>
        private TokenAuthorization GetTokenAuthorization()
        {
            List<string> lines = System.IO.File.ReadAllLines(AccessTokenPath).ToList();

            if (lines.Count != 1)
                throw new InvalidOperationException(
                    "The data in the access token file is missing or incorrectly formatted.");

            var line = lines.Single();

            var splitLine = line.Split('&');

            var token = splitLine[0].Split('=')[1];
            var secret = splitLine[1].Split('=')[1];

            var result = new TokenAuthorization(token, secret);

            return result;
        }
    }
}