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
        private IBraintreeGateway BraintreeGateway { get; set; }

        public IBraintreeGateway CreateGateway() {
            Environment = GetEnvironment();
            MerchantId = GetConfigurationSetting("BraintreeMerchantId");
            PublicKey = GetConfigurationSetting("BraintreePublicKey");
            PrivateKey = GetConfigurationSetting("BraintreePrivateKey");

            return new BraintreeGateway
            {
                Environment = Environment,
                MerchantId = MerchantId,
                PublicKey = PublicKey,
                PrivateKey = PrivateKey
            };
        }

        public string GetConfigurationSetting(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        public Braintree.Environment GetEnvironment()
        {
            string environment = GetConfigurationSetting("BraintreeEnvironment");
            return environment == "production" ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX;
        }

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
}