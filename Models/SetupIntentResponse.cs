using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class SetupIntentResponse
    {

        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }

    }
}
