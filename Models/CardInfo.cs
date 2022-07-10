using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class CardInfo
    {
        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("cardExpiryMonth")]
        public int CardExpiryMonth { get; set; }

        [JsonProperty("cardExpiryYear")]
        public int CardExpiryYear { get; set; }

        [JsonProperty("cardCVC")]
        public string CardCVC { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("recurringFrequency")]
        public string RecurringFrequency { get; set; }

    }
}
