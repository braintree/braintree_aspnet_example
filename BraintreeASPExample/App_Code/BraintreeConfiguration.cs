using Braintree;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace BraintreeASPExample
{
    public class BraintreeConfiguration
    {
        public Braintree.Environment Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public BraintreeGateway gateway { get; private set; }

        public BraintreeConfiguration() {
            createGateway();
        }

        private void createGateway() {
            Environment = getEnvironment();
            MerchantId = getConfigurationSetting("BraintreeMerchantId");
            PublicKey = getConfigurationSetting("BraintreePublicKey");
            PrivateKey = getConfigurationSetting("BraintreePrivateKey");

            gateway = new BraintreeGateway
            {
                Environment = Environment,
                MerchantId = MerchantId,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey
            };
        }

        private String getConfigurationSetting(String setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        private Braintree.Environment getEnvironment()
        {
            String environment = getConfigurationSetting("BraintreeEnvironment");
            return environment == "production" ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX;
        }
    }
}