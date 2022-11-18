using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using StripeApp.Models;
using StripeApp.Services;

namespace StripeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private IConfiguration configuration;
        private readonly StripeSettingsOptions stripeSettingsOptions;
        public StripeController(IConfiguration configuration){
            this.configuration = configuration;
            stripeSettingsOptions = new StripeSettingsOptions();
            this.configuration.GetSection(StripeSettingsOptions.StripeSettings).Bind(stripeSettingsOptions);
        }
        // GET api/stripe
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "createpayment", "confirmpayment" };
        }

        // GET api/stripe/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/stripe/createpayment
        [Route("[action]")]
        [HttpPost]
        public object CreatePayment([FromBody] CardInfo cardRequest)
        { 
            return (new StripeService(this.configuration)).createPayment(cardRequest);
        }
        // POST api/stripe/confirmpayment
        [Route("[action]")]
        [HttpPost]
        public object ConfirmPayment([FromBody] ConfirmPaymentInfo confirmPaymentInfo)
        { 
            return (new StripeService(this.configuration)).confirmPayment(confirmPaymentInfo);
        }

        // POST api/stripe/getSecret
        [Route("[action]")]
        [HttpPost]
        public object getSecret([FromBody] SetupIntentRequest setupIntentRequest)
        {
            SetupIntentResponse setupIntentResponse = (new StripeService(this.configuration)).SetupIntent(setupIntentRequest);
           return new JsonResult(new  { setupIntentResponse= setupIntentResponse,redirectUrl= this.stripeSettingsOptions.RedirectUrl});
        }

        // POST api/stripe/chargeCustomer
        [Route("[action]")]
        [HttpPost]
        public object chargeCustomer([FromBody] SetupIntentRequest setupIntentRequest)
        {
            string response = (new StripeService(this.configuration)).ChargeCustomer(setupIntentRequest);
            return new JsonResult(new { response = response });
        }
    }
}
