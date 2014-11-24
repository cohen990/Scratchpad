using System.Collections.Generic;

namespace scratchpad.Models
{

    public class UserDetails : PayPalDataModel
    {
        public string Email { get; set; }
        public string PayerId { get; set; }
        public string PayerStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public string ShipToName { get; set; }
        public string ShipToStreet { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToZip { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToCountryName { get; set; }
        public string AddressStatus { get; set; }

        public override void InitializeFromDict(Dictionary<string, string> dict)
        {
            Email = TryGetValue(dict, "EMAIL");
            PayerId = TryGetValue(dict, "PAYERID");
            PayerStatus = TryGetValue(dict, "PAYERSTATUS");
            FirstName = TryGetValue(dict, "FIRSTNAME");
            LastName = TryGetValue(dict, "LASTNAME");
            CountryCode = TryGetValue(dict, "COUNTRYCODE");
            ShipToName = TryGetValue(dict, "SHIPTONAME");
            ShipToStreet = TryGetValue(dict, "SHIPTOSTREET");
            ShipToCity = TryGetValue(dict, "SHIPTOCITY");
            ShipToZip = TryGetValue(dict, "SHIPTOZIP");
            ShipToCountryCode = TryGetValue(dict, "SHIPTOCOUNTRYCODE");
            ShipToCountryName = TryGetValue(dict, "SHIPTOCOUNTRYNAME");
            AddressStatus = TryGetValue(dict, "ADDRESSSTATUS");
        }
    }
}