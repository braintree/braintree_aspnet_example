using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Braintree;

namespace BraintreeASPExample.Controllers
{
    public class HostedController : Controller
    {
        public IBraintreeConfiguration config = new BraintreeConfiguration();

        public static readonly TransactionStatus[] transactionSuccessStatuses = {
            TransactionStatus.AUTHORIZED,
            TransactionStatus.AUTHORIZING,
            TransactionStatus.SETTLED,
            TransactionStatus.SETTLING,
            TransactionStatus.SETTLEMENT_CONFIRMED,
            TransactionStatus.SETTLEMENT_PENDING,
            TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };

        public ActionResult New()
        {
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }

        public ActionResult Process(string nonce, string email, decimal amount)
        {
            var gateway = config.GetGateway();

            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var result = gateway.Transaction.Sale(request);

            Transaction transaction = result.Target;
            return RedirectToAction("Show", new { id = transaction.Id });
        }
    }
}
