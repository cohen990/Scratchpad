namespace scratchpad.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PayPalService
    {
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
    }
}