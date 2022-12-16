using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class SetupIntentRequest
    {
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }
        [JsonProperty("customerEmail")]
        public string CustomerEmail { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("paymentMethodType")]
        public string PaymentMethodType { get; set; }

        [JsonProperty("mandateId")]
        public string MandateId { get; set; }

    }
}
