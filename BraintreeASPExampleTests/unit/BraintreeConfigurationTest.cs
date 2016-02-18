using Braintree;
using BraintreeASPExample;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BraintreeASPExampleTests
{
    [TestClass]
    public class BraintreeConfigurationTest
    {
        [TestMethod]
        public void TestConfiguringGateway()
        {
            BraintreeConfiguration config = new BraintreeConfiguration();
            var gateway = config.GetGateway();

            Assert.AreEqual(gateway.Environment, Braintree.Environment.SANDBOX);
            Assert.AreEqual(gateway.MerchantId, "MerchantId");
            Assert.AreEqual(gateway.PublicKey, "PublicKey");
            Assert.AreEqual(gateway.PrivateKey, "PrivateKey");
        }
    }
}
