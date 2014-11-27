using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scratchpad.Models
{
    public class PermissionsServiceModel : PayPalDataModel<PermissionsServiceModel>
    {

        public string Ack { get; set; }

        public string Token { get; set; }

        public string Secret { get; set; }

        public string Build { get; set; }

        public string CorrelationId { get; set; }

        public string TimeStamp { get; set; }

        protected override void InitializeFromDictionary(Dictionary<string, string> dict)
        {
            TimeStamp = TryGetValue(dict, "responseEnvelope.timestamp");
            Ack = TryGetValue(dict, "responseEnvelope.ack");
            CorrelationId = TryGetValue(dict, "responseEnvelope.correlationId");
            Build = TryGetValue(dict, "responseEnvelope.build");
            Token = TryGetValue(dict, "token");
            Secret = TryGetValue(dict, "tokenSecret");
        }
    }
}