namespace scratchpad.Models
{
    using System.Collections.Generic;

    public class PaymentInfo : PayPalDataModel
    {
        public string TransactionId { get; set; }

        public string TransactionType { get; set; }

        public string PaymentType { get; set; }

        public string OrderTime { get; set; }

        public string Amt { get; set; }

        public string TaxAmt { get; set; }

        public string CurrencyCode { get; set; }

        public string PaymentStatus { get; set; }

        public string PendingReason { get; set; }

        public string ReasonCode { get; set; }

        public string ProtectionEligibility { get; set; }

        public string ProtectionEligibilityType { get; set; }

        public string SecureMerchantAccountId { get; set; }

        public string ErrorCode { get; set; }

        public string Ack { get; set; }

        public override void InitializeFromDict(Dictionary<string, string> dict)
        {
            TransactionId = TryGetValue(dict, "PAYMENTINFO_0_TRANSACTIONID");
            TransactionType = TryGetValue(dict, "PAYMENTINFO_0_TRANSACTIONTYPE");
            PaymentType = TryGetValue(dict, "PAYMENTINFO_0_PAYMENTTYPE");
            OrderTime = TryGetValue(dict, "PAYMENTINFO_0_ORDERTIME");
            Amt = TryGetValue(dict, "PAYMENTINFO_0_AMT");
            TaxAmt = TryGetValue(dict, "PAYMENTINFO_0_TAXAMT");
            CurrencyCode = TryGetValue(dict, "PAYMENTINFO_0_CURRENCYCODE");
            PaymentStatus = TryGetValue(dict, "PAYMENTINFO_0_PAYMENTSTATUS");
            PendingReason = TryGetValue(dict, "PAYMENTINFO_0_PENDINGREASON");
            ReasonCode = TryGetValue(dict, "PAYMENTINFO_0_REASONCODE");
            ProtectionEligibility = TryGetValue(dict, "PAYMENTINFO_0_PROTECTIONELIGIBILITY");
            ProtectionEligibilityType = TryGetValue(dict, "PAYMENTINFO_0_PROTECTIONELIGIBILITYTYPE");
            SecureMerchantAccountId = TryGetValue(dict, "PAYMENTINFO_0_SECUREMERCHANTACCOUNTID");
            ErrorCode = TryGetValue(dict, "PAYMENTINFO_0_ERRORCODE");
            Ack = TryGetValue(dict, "PAYMENTINFO_0_ACK");
        }
    }
}