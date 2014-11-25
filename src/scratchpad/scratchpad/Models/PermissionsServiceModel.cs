using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scratchpad.Models
{
    public class PermissionsServiceModel : PayPalDataModel
    {

        public string Ack { get; set; }

        public string Token { get; set; }

        public string Build { get; set; }

        public string CorrelationId { get; set; }

        public string TimeStamp { get; set; }

        public override void InitializeFromDict(Dictionary<string, string> dict)
        {
            TimeStamp = TryGetValue(dict, "responseEnvelope.timestamp");
            Ack = TryGetValue(dict, "responseEnvelope.ack");
            CorrelationId = TryGetValue(dict, "responseEnvelope.correlationId");
            Build = TryGetValue(dict, "responseEnvelope.build");
            Token = TryGetValue(dict, "token");
        }
    }
}