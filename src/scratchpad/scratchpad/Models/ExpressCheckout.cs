namespace scratchpad.Models
{
    using System;
    using System.Collections.Generic;

    public class ExpressCheckout : PayPalDataModel
    {
        public string Token { get; set; }

        public string BillingAgreementAcceptedStatus { get; set; }

        public string CheckoutStatus { get; set; }

        public string TimeStamp { get; set; }

        public string CorrelationId { get; set; }

        public string Ack { get; set; }

        public string Version { get; set; }

        public string Build { get; set; }

        public string CurrencyCode { get; set; }

        public string Amt { get; set; }

        public string ShippingAmt { get; set; }

        public string HandlingAmt { get; set; }

        public string TaxAmt { get; set; }

        public string InsuranceAmt { get; set; }

        public string ShipDiscAmt { get; set; }

        public PaymentRequest PaymentRequest { get; set; }

        public UserDetails UserDetails { get; set; }

        public PaymentInfo PaymentInfo { get; set; }

        public override void InitializeFromDict(Dictionary<string, string> dict)
        {
            if (dict == null)
                throw new ArgumentNullException("dict");

            Token = TryGetValue(dict, "TOKEN");
            BillingAgreementAcceptedStatus = TryGetValue(dict, "BILLINGAGREEMENTACCEPTEDSTATUS");
            CheckoutStatus = TryGetValue(dict, "CHECKOUTSTATUS");
            TimeStamp = TryGetValue(dict, "TIMESTAMP");
            CorrelationId = TryGetValue(dict, "CORRELATIONID");
            Ack = TryGetValue(dict, "ACK");
            Version = TryGetValue(dict, "VERSION");
            Build = TryGetValue(dict, "BUILD");
            CurrencyCode = TryGetValue(dict, "CURRENCYCODE");
            Amt = TryGetValue(dict, "AMT");
            ShippingAmt = TryGetValue(dict, "SHIPPINGAMT");
            HandlingAmt = TryGetValue(dict, "HANDLINGAMT");
            TaxAmt = TryGetValue(dict, "TAXAMT");
            InsuranceAmt = TryGetValue(dict, "INSURANCEAMT");
            ShipDiscAmt = TryGetValue(dict, "SHIPDISCAMT");

            PaymentRequest = new PaymentRequest();
            PaymentRequest.InitializeFromDict(dict);

            PaymentInfo = new PaymentInfo();
            PaymentInfo.InitializeFromDict(dict);

            UserDetails = new UserDetails();
            UserDetails.InitializeFromDict(dict);
        }
    }
}