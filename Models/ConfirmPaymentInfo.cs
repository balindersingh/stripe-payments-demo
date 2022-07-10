using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class ConfirmPaymentInfo
    {
        [JsonProperty("payment_intent_client_secret")]
        public string PaymentClientSecret { get; set; }
        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("paymentIntentId")]
        public string PaymentIntentId { get; set; }
    }
}
