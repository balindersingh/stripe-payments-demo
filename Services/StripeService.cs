using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Newtonsoft.Json;
using StripeApp.Models;

namespace StripeApp.Services
{

    public class StripeService
    {
        public const string CONFIRMATION_ENPOINT_RELATIVE_PATH = "/Home/ConfirmPayment";
        private IConfiguration configuration;
        private StripeSettingsOptions stripeSettingsOptions;
        public StripeService(IConfiguration configuration)
        {
            this.configuration = configuration;
            stripeSettingsOptions = new StripeSettingsOptions();
            this.configuration.GetSection(StripeSettingsOptions.StripeSettings).Bind(stripeSettingsOptions);
        }
        public PaymentResponse createPayment(CardInfo cardInfo)
        {
            try
            {
                if (cardInfo.CardNumber != null)
                {
                    if (!string.IsNullOrEmpty(cardInfo.RecurringFrequency))
                    {
                        // Recurring
                        return MakeRecurringPayment(cardInfo);
                    }
                    else {
                        // Single payment
                        return MakeSinglePayment(cardInfo);
                    }

                }
                else
                {
                   // return new { customError = "Something is wrong. Pleas fill out all the information", detail = cardInfo };
                    return new PaymentResponse { Error = "Something is wrong. Please fill out all the information" };
                }

            }
            catch (StripeException e)
            {
                       // return new { error = e.StripeError.Message, cardDetail = cardInfo };
                return new PaymentResponse { Error = "Stripe Exception: "+ e.StripeError.Message };
            }
            return new PaymentResponse { Error = "Unexpected error happend" };
        }

        private PaymentResponse MakeSinglePayment(CardInfo cardInfo)
        {
            StripeConfiguration.ApiKey = stripeSettingsOptions.SecretKey;
            var options = new PaymentMethodCreateOptions
            {
                Type = Constants.PAYMENT_METHOD_TYPE,
                Card = new PaymentMethodCardOptions
                {
                    Number = cardInfo.CardNumber,
                    ExpMonth = cardInfo.CardExpiryMonth,
                    ExpYear = cardInfo.CardExpiryYear,
                    Cvc = cardInfo.CardCVC,
                },
            };
            var service = new PaymentMethodService();
            var paymentMethod = service.Create(options);
            PaymentIntent paymentIntentObj = createPaymentIntent(paymentMethod.Id, cardInfo.Amount);
            return generatePaymentResponse(paymentIntentObj);
        }

        private PaymentResponse MakeRecurringPayment(CardInfo cardInfo)
        {
            StripeConfiguration.ApiKey = stripeSettingsOptions.SecretKey;
            var options = new PaymentMethodCreateOptions
            {
                Type = Constants.PAYMENT_METHOD_TYPE,
                Card = new PaymentMethodCardOptions
                {
                    Number = cardInfo.CardNumber,
                    ExpMonth = cardInfo.CardExpiryMonth,
                    ExpYear = cardInfo.CardExpiryYear,
                    Cvc = cardInfo.CardCVC,
                },
            };
            var service = new PaymentMethodService();
            var paymentMethod = service.Create(options);
            CustomerCreateOptions customerCreateOptions = new CustomerCreateOptions
            {
                Description = String.IsNullOrEmpty( cardInfo.FullName)? "Customer "+DateTime.Now.ToString("yyyyMMddhhmmss") : cardInfo.FullName,
                PaymentMethod = paymentMethod.Id,
            };
            Customer customer = CreateCustomer(customerCreateOptions);

            Plan plan = CreatePlan(cardInfo.Amount, Constants.DEFAULT_CURRENCY_CODE, cardInfo.RecurringFrequency);
            Subscription subscription = CreateSubscription(paymentMethod.Id,customer, plan);
            PaymentIntent paymentIntent = subscription.LatestInvoice.PaymentIntent;

            PaymentResponse paymentResponse = generatePaymentResponse(paymentIntent);
            if (paymentResponse.Success)
            {
                return paymentResponse;
            }
            else
            {
                string stripeRedirectUrl = stripeSettingsOptions.RedirectUrl + CONFIRMATION_ENPOINT_RELATIVE_PATH;

                PaymentIntentService paymentIntentService = new PaymentIntentService();
                PaymentIntentConfirmOptions paymentIntentConfirmOptions = new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethod.Id,
                    ReturnUrl = stripeRedirectUrl
                };
                paymentIntent = paymentIntentService.Confirm(paymentIntent.Id, paymentIntentConfirmOptions);

                return generatePaymentResponse(paymentIntent);
            }
        }
        public PaymentResponse confirmPayment(ConfirmPaymentInfo confirmPaymentInfo)
        {
            string stripeRedirectUrl = String.IsNullOrEmpty(confirmPaymentInfo.RedirectUrl) ? stripeSettingsOptions.RedirectUrl + CONFIRMATION_ENPOINT_RELATIVE_PATH : confirmPaymentInfo.RedirectUrl;
            return confirmPayment(confirmPaymentInfo.PaymentIntentId, stripeRedirectUrl);
        }

        private PaymentResponse confirmPayment(string paymentIntentId, string stripeRedirectUrl)
        {
            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = null;

            try
            {
                if (paymentIntentId != null)
                {
                    paymentIntent = paymentIntentService.Get(paymentIntentId);
                    if (paymentIntent.Status == Constants.PaymentIntent.STATUS_REQUIRES_PAYMENT_METHOD)
                    {
                        return generatePaymentResponse(paymentIntent);
                    }
                    else
                    {
                        var confirmOptions = new PaymentIntentConfirmOptions { ReturnUrl = stripeRedirectUrl };
                        paymentIntent = paymentIntentService.Confirm(
                            paymentIntentId,
                            confirmOptions
                        );
                    }
                }
            }
            catch (StripeException e)
            {
                return new PaymentResponse { Error = e.StripeError.Message };
            }
            return generatePaymentResponse(paymentIntent);
        }
        private PaymentResponse generatePaymentResponse(PaymentIntent intent)
        {
            // Note that if your API version is before 2019-02-11, 'requires_action'
            // appears as 'requires_source_action'.requires_payment_method
            if (intent.Status == Constants.PaymentIntent.STATUS_REQUIRES_ACTION && intent.NextAction.Type == Constants.PaymentIntent.NEXTACTIONTYPE_USE_STRIPE_SDK)
            {
                // Tell the client to handle the action
                return new PaymentResponse
                {
                    RequireAction = true,
                    PaymentIntentId = intent.Id

                };
            }
            if (intent.Status == Constants.PaymentIntent.STATUS_REQUIRES_ACTION && intent.NextAction.Type == Constants.PaymentIntent.NEXTACTIONTYPE_REDIRECT_TO_URL)
            {
                return new PaymentResponse
                {
                    RequireAction = true,
                    RedirectUrl = intent.NextAction.RedirectToUrl.Url,
                    PaymentIntentId = intent.Id
                };
            }
            else if (intent.Status == Constants.PaymentIntent.STATUS_REQUIRES_PAYMENT_METHOD)
            {
                // The payment didn’t need any additional actions and failed to authenticate using 3DSecure 2
                return new PaymentResponse { Error = "Payment failed" };
            }
            else if (intent.Status == Constants.PaymentIntent.STATUS_SUCCEEDED)
            {
                // The payment didn’t need any additional actions and completed successfully using 3DSecure 2
                return new PaymentResponse { Success = true };
            }
            else
            {
                // Invalid status
                return new PaymentResponse { Error = "Invalid PaymentIntent status" };
            }
        }
        private PaymentIntent createPaymentIntent(string paymentMethodId, int paymentAmount)
        {
            string stripeRedirectUrl = stripeSettingsOptions.RedirectUrl + CONFIRMATION_ENPOINT_RELATIVE_PATH;
            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = null;
            if (paymentMethodId != null)
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    PaymentMethod = paymentMethodId,
                    Amount = paymentAmount,
                    Currency = Constants.DEFAULT_CURRENCY_CODE,
                    ConfirmationMethod = Constants.PAYMENT_INTENT_CONFIRMATION_METHOD,
                    Confirm = true,
                    ReturnUrl = stripeRedirectUrl
                };
                paymentIntent = paymentIntentService.Create(createOptions);

            }
            return paymentIntent;
        }
        private Plan CreatePlan(int amount, string currencyCode, string intervalFrequency)
        {
            PlanCreateOptions planCreateOptions = new PlanCreateOptions
            {
                Product = new PlanProductOptions
                {
                    Name = "Lite Pae "+ DateTime.Now.ToString("yyyyMMddHHmmss")
                },
                Amount = amount,
                Currency = currencyCode,
                Interval = intervalFrequency
            };
            PlanService service = new PlanService();
            Plan plan = service.Create(planCreateOptions);
            return plan;
        }

        private Customer CreateCustomer(CustomerCreateOptions customerCreateOptions)
        {
            CustomerService service = new CustomerService();
            Customer stripeCustomer = service.Create(customerCreateOptions);
            return stripeCustomer;

        }
        private Subscription CreateSubscription(string paymentMethodId, Customer customer, Plan plan)
        {
            List<SubscriptionItemOptions> listOfSubscriptionItemOptions = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                    Plan = plan.Id,
                }
            };
            SubscriptionCreateOptions subscriptionCreateOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = listOfSubscriptionItemOptions,
                DefaultPaymentMethod = paymentMethodId,
                CollectionMethod = Constants.SUBSCRIPTION_COLLECTION_METHOD
            };
            subscriptionCreateOptions.AddExpand(Constants.SUBSCRIPTION_OPTION_LATESTINVOICE_PAYMENTINTENT);
            SubscriptionService subscriptionService = new SubscriptionService();
            Subscription subscription = subscriptionService.Create(subscriptionCreateOptions);
            return subscription;
        }
    }
}
