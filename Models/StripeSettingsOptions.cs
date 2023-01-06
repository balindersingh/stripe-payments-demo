using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class StripeSettingsOptions
    {
        public const string StripeSettings = "StripeSettings";

        public string PublishableKey { get; set; } = String.Empty;
        public string SecretKey { get; set; } = String.Empty;
        public string US_PublishableKey { get; set; } = String.Empty;
        public string US_SecretKey { get; set; } = String.Empty;

        public string RedirectUrl { get; set; } = String.Empty;

        public string WebhookSecret  { get; set; } = String.Empty;
    }
}
