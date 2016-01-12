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

            Assert.AreEqual(config.gateway.Environment, Braintree.Environment.SANDBOX);
            Assert.AreEqual(config.gateway.MerchantId, "MerchantId");
            Assert.AreEqual(config.gateway.PublicKey, "PublicKey");
            Assert.AreEqual(config.gateway.PrivateKey, "PrivateKey");
        }
    }
}
