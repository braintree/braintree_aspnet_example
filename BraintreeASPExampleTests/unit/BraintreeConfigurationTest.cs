using Braintree;
using BraintreeASPExample;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BraintreeASPExampleTests
{
    [TestClass]
    public class BraintreeConfigurationTest
    {
        private TestHelper testHelper;

        [TestInitialize]
        public void CaptureEnvironmentVariables()
        {
            testHelper = new TestHelper();
            testHelper.StoreEnvironmentVariables();
        }

        [TestMethod]
        public void TestConfiguringGateway()
        {
            testHelper.RemoveEnvironmentVariables();

            IBraintreeConfiguration config = new BraintreeConfiguration();
            var gateway = config.GetGateway();

            Assert.AreEqual(gateway.Environment, Braintree.Environment.SANDBOX);
            Assert.AreEqual(gateway.MerchantId, "MerchantId");
            Assert.AreEqual(gateway.PublicKey, "PublicKey");
            Assert.AreEqual(gateway.PrivateKey, "PrivateKey");
        }

        [TestMethod]
        public void TestEnvironmentVariableGateway()
        {
            IBraintreeConfiguration config = new BraintreeConfiguration();

            System.Environment.SetEnvironmentVariable(TestHelper.BRAINTREE_ENVIRONMENT, "sandbox", EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(TestHelper.BRAINTREE_MERCHANT_ID, "TestEnvMerchantId", EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(TestHelper.BRAINTREE_PUBLIC_KEY, "TestEnvPublicKey", EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(TestHelper.BRAINTREE_PRIVATE_KEY, "TestEnvPrivateKey", EnvironmentVariableTarget.Process);

            var gateway = config.GetGateway();

            Assert.AreEqual(gateway.Environment, Braintree.Environment.SANDBOX);
            Assert.AreEqual(gateway.MerchantId, "TestEnvMerchantId");
            Assert.AreEqual(gateway.PublicKey, "TestEnvPublicKey");
            Assert.AreEqual(gateway.PrivateKey, "TestEnvPrivateKey");
        }

        [TestCleanup]
        public void CleanUpEnvironmentVariables()
        {
            testHelper.RestoreEnvironmentVariables();
        }
    }
}
