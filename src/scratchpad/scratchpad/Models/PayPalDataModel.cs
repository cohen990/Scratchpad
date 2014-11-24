namespace scratchpad.Models
{
    using System.Collections.Generic;

    public abstract class PayPalDataModel
    {
        protected static string TryGetValue(Dictionary<string, string> dict, string key)
        {
            var result = dict.ContainsKey(key) ? dict[key] : null;

            return result;
        }

        public abstract void InitializeFromDict(Dictionary<string, string> dict);
    }
}