namespace scratchpad.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Models;

    public class PayPalService
    {
        protected readonly HttpClient _client;

        public PayPalService(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Unescapes and converts the <c>NVP</c> response of the <c>API</c> into a dictionary.
        /// </summary>
        /// <param name="nvpInput">The <c>NVP</c> response of an <c>API</c> call.</param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing the Names as Keys and the Values as Values.
        /// </returns>
        protected Dictionary<string, string> NvpToDictionary(string nvpInput)
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

        protected async Task<InteractionModel<string>> CallApiWithHeadersAsync(
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

            request = AddHeaders(request, headers);

            var result = await SendRequest(request);
            return result;
        }

        private async Task<InteractionModel<string>> SendRequest(HttpRequestMessage request)
        {
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
        protected TData GetSuccessfulApiModel<TData>(string responseContent)
            where TData : PayPalDataModel<TData>, new()
        {
            var responseDictionary = NvpToDictionary(responseContent);

            var result = PayPalDataModel<TData>.InitializeFromDict(responseDictionary);

            return result;
        }

        private HttpRequestMessage AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }
    }
}