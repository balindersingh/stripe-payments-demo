using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeApp.Models
{
    public class Constants
    {
        public static string SUBSCRIPTION_COLLECTION_METHOD= "charge_automatically";
        public static string SUBSCRIPTION_OPTION_LATESTINVOICE_PAYMENTINTENT = "latest_invoice.payment_intent";
        public static string DEFAULT_CURRENCY_CODE = "usd";
        public static string PAYMENT_METHOD_TYPE = "card";
        public static string PAYMENT_INTENT_CONFIRMATION_METHOD = "manual";
        public static class PaymentIntent
        {
            public static string STATUS_REQUIRES_ACTION = "requires_action";
            public static string STATUS_REQUIRES_PAYMENT_METHOD = "requires_payment_method";
            public static string STATUS_SUCCEEDED = "succeeded";
            public static string NEXTACTIONTYPE_REDIRECT_TO_URL = "redirect_to_url";
            public static string NEXTACTIONTYPE_USE_STRIPE_SDK = "use_stripe_sdk";
        }

    }
}
