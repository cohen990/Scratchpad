using System.Collections.Generic;

namespace scratchpad.Models
{

    public class PaymentRequest : PayPalDataModel<PaymentRequest>
    {
        public string CurrencyCode { get; set; }

        public string Amt { get; set; }

        public string ShippingAmt { get; set; }

        public string HandlingAmt { get; set; }

        public string TaxAmt { get; set; }

        public string InsuranceAmt { get; set; }

        public string ShipDiscAmt { get; set; }

        public string InsuranceOptionOffered { get; set; }

        public string ErrorCode { get; set; }

        protected override void InitializeFromDictionary(Dictionary<string, string> dict)
        {
            CurrencyCode = TryGetValue(dict, "PAYMENTREQUEST_0_CURRENCYCODE");
            Amt = TryGetValue(dict, "PAYMENTREQUEST_0_AMT");
            ShippingAmt = TryGetValue(dict, "PAYMENTREQUEST_0_SHIPPINGAMT");
            HandlingAmt = TryGetValue(dict, "PAYMENTREQUEST_0_HANDLINGAMT");
            TaxAmt = TryGetValue(dict, "PAYMENTREQUEST_0_TAXAMT");
            InsuranceAmt = TryGetValue(dict, "PAYMENTREQUEST_0_INSURANCEAMT");
            ShipDiscAmt = TryGetValue(dict, "PAYMENTREQUEST_0_SHIPDISCAMT");
            InsuranceOptionOffered = TryGetValue(dict, "PAYMENTREQUEST_0_INSURANCEOPTIONOFFERED");
            ErrorCode = TryGetValue(dict, "PAYMENTREQUESTINFO_0_ERRORCODE");
        }
    }
}