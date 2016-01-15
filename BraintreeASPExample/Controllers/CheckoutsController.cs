using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BraintreeASPExample.Controllers
{
    public class CheckoutsController : Controller
    {
        public IBraintreeConfiguration config = new BraintreeConfiguration();

        public ActionResult New()
        {
            var gateway = config.getGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }

        public ActionResult Create()
        {
            var gateway = config.getGateway();
            Decimal amount = Convert.ToDecimal(Request["amount"]); // In production you should not take amouts directly from clients
            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            Transaction transaction = result.Target;
            return RedirectToAction("Show", new { id = transaction.Id });
        }

        public ActionResult Show(String id)
        {
            var gateway = config.getGateway();
            Transaction transaction = gateway.Transaction.Find(id);
            ViewBag.Transaction = transaction;
            return View();
        }
    }
}
