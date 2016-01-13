using Braintree;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace BraintreeASPExample
{
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        public Braintree.Environment Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway gateway { get; set; }

        public IBraintreeGateway createGateway() {
            Environment = getEnvironment();
            MerchantId = getConfigurationSetting("BraintreeMerchantId");
            PublicKey = getConfigurationSetting("BraintreePublicKey");
            PrivateKey = getConfigurationSetting("BraintreePrivateKey");

            return new BraintreeGateway
            {
                Environment = Environment,
                MerchantId = MerchantId,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey
            };
        }

        public String getConfigurationSetting(String setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        public Braintree.Environment getEnvironment()
        {
            String environment = getConfigurationSetting("BraintreeEnvironment");
            return environment == "production" ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX;
        }

        public IBraintreeGateway getGateway()
        {
            if (gateway == null)
            {
                gateway = createGateway();
            }

            return gateway;
        }
    }
}