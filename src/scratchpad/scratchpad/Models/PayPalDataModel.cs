namespace scratchpad.Models
{
    using System.Collections.Generic;

    public abstract class PayPalDataModel<TData> where TData : PayPalDataModel<TData>, new()
    {
        protected static string TryGetValue(Dictionary<string, string> dict, string key)
        {
            var result = dict.ContainsKey(key) ? dict[key] : null;

            return result;
        }

        protected abstract void InitializeFromDictionary(Dictionary<string, string> dict);

        public static TData InitializeFromDict(Dictionary<string, string> dict)
        {
            var result = new TData();
            result.InitializeFromDictionary(dict);

            return result;
        }
    }
}