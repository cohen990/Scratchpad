    namespace scratchpad.Services
    {
        using System;
        using System.Collections.Generic;
        using System.Net.Http;
        using System.Threading.Tasks;
        using Models;
        using Newtonsoft.Json.Linq;

        public class PayPalService
        {
            private readonly string _user;
            private readonly string _password;
            private readonly string _signature;
            private readonly Uri _endpoint;
            private readonly string _payPalSuccessUri;

            public PayPalService(bool isTest)
            {
                _user = "ben.we_1347891576_biz_api1.mobankgroup.com";
                _password = "1347891624";
                _signature = "ABQaomIXAXCdYJSw0qCD.83z-JdXAphfeq8PxbiDycSoYC.Gd4NLWcr.";
                _payPalSuccessUri = "http://localhost:6945/Home/PayPalSuccess";
                _endpoint = GetEndpoint(isTest);
            }

            public async Task<ExpressCheckout> DoExpressCheckoutPaymentAsync(HttpClient client, string token, string payerId)
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentNullException("token");
                if (string.IsNullOrEmpty(payerId))
                    throw new ArgumentNullException("payerId");

                var queryParams = string.Format("USER={0}&PWD={1}&SIGNATURE={2}&VERSION=78&METHOD=DoExpressCheckoutPayment&TOKEN={3}&PAYERID={4}&PAYMENTREQUEST_0_AMT=19.00",
                    _user,
                    _password,
                    _signature,
                    token,
                    payerId);

                var requestUri = new UriBuilder(_endpoint) {Query = queryParams};

                var response = await client.GetAsync(requestUri.Uri);

                var responseContent = await response.Content.ReadAsStringAsync();


                if (responseContent.Contains("ACK=Success"))
                {
                    var responseDictionary = NvpToDictionary(responseContent);

                    var checkout = new ExpressCheckout();
                    checkout.InitializeFromDict(responseDictionary);

                    return checkout;
                }

                string exceptionMessage = string.Format(
                    "There has been an error making the request to paypal. The API response was '{0}'",
                    Uri.UnescapeDataString(responseContent));

                throw new Exception(exceptionMessage);

            }

            public async Task<ExpressCheckout> GetExpressCheckoutDetailsAsync(HttpClient client, string token)
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentNullException("token");

                var queryParams = string.Format("USER={0}&PWD={1}&SIGNATURE={2}&VERSION=78&METHOD=GetExpressCheckoutDetails&TOKEN={3}",
                    _user,
                    _password,
                    _signature,
                    token);

                var requestUri = new UriBuilder(_endpoint) {Query = queryParams};

                var response = await client.GetAsync(requestUri.Uri);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (responseContent.Contains("ACK=Success"))
                {
                    var responseDictionary = NvpToDictionary(responseContent);

                    var checkout = new ExpressCheckout();
                    checkout.InitializeFromDict(responseDictionary);

                    return checkout;
                }

                throw new Exception("There has been an error making the request to paypal");
            }

            public async Task<ExpressCheckout> SetExpressCheckoutAsync(HttpClient client)
            {
                if (client == null)
                    throw new ArgumentNullException("client");

                var queryParams = string.Format("USER={0}&PWD={1}&SIGNATURE={2}&METHOD=SetExpressCheckout&VERSION=78&PAYMENTREQUEST_0_PAYMENTACTION=SALE&PAYMENTREQUEST_0_AMT=19&PAYMENTREQUEST_0_CURRENCYCODE=USD&cancelUrl=http://www.example.com/cancel.html&returnUrl={3}",
                    _user,
                    _password,
                    _signature,
                    _payPalSuccessUri);

                var requestUri = new UriBuilder(_endpoint) {Query = queryParams};

                var response = await client.GetAsync(requestUri.Uri);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (responseContent.Contains("ACK=Success"))
                {
                    var responseDictionary = NvpToDictionary(responseContent);

                    var checkout = new ExpressCheckout();
                    checkout.InitializeFromDict(responseDictionary);

                    return checkout;
                }

                throw new Exception("There has been an error making the request to paypal");
            }

            private Dictionary<string, string> NvpToDictionary(string nvpInput)
            {
                if (string.IsNullOrEmpty(nvpInput))
                    throw new ArgumentNullException("nvpInput");

                var nameValuePairs = nvpInput.Split('&');

                var responseDictionary = new Dictionary<string, string>();
                foreach (var pair in nameValuePairs)
                {
                    var splitPair = pair.Split('=');
                    responseDictionary.Add(Uri.UnescapeDataString(splitPair[0]), Uri.UnescapeDataString(splitPair[1]));
                }

                return responseDictionary;
            }

            private Uri GetEndpoint(bool isTest)
            {
                if (isTest)
                {
                    return new Uri("https://api-3t.sandbox.paypal.com/nvp");
                }

                return new Uri("https://api-3t.paypal.com/nvp");
            }

            // Define other methods and classes here
            public async Task<Task<string>> RequestPermissions()
            {
                var client = new HttpClient();

                var endPoint = new Uri("https://svcs.sandbox.paypal.com/Permissions/RequestPermissions");

                var request = new HttpRequestMessage(HttpMethod.Post, endPoint);

                var payload = new Dictionary<string, string>
                {
                    {"scope", "EXPRESS_CHECKOUT"},
                    {"callback", "http://www.example.com/success.html"},
                    {"requestEnvelope", "{\"errorLanguage\",\"en_US\"}"},
                };

                request.Content = new FormUrlEncodedContent(payload);

                // Sandbox API credentials for the API Caller account
                request.Content.Headers.Add("X-PAYPAL-SECURITY-USERID", "Sandbox-Caller-User-Id");
                request.Content.Headers.Add("X-PAYPAL-SECURITY-PASSWORD", "Sandbox-Caller-Password");
                request.Content.Headers.Add("X-PAYPAL-SECURITY-SIGNATURE", "Sandbox-Caller-Security-Signature");

                // Sandbox Application ID
                request.Content.Headers.Add("X-PAYPAL-APPLICATION-ID", "APP-80W284485P519543T");

                // Input and output formats
                request.Content.Headers.Add("X-PAYPAL-REQUEST-DATA-FORMAT", "JSON");
                request.Content.Headers.Add("X-PAYPAL-RESPONSE-DATA-FORMAT", "JSON");

                var response = await client.SendAsync(request);

                var result = response.Content.ReadAsStringAsync();

                return result;
            }
        }
    }