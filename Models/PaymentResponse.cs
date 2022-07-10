using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class PaymentResponse
    {

        [JsonProperty("paymentIntentId")]
        public string PaymentIntentId { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("requireAction")]
        public Boolean RequireAction { get; set; }
        [JsonProperty("success")] 
        public Boolean Success { get; set; }
        [JsonProperty("error")] 
        public string Error { get; set; }

    }
}
