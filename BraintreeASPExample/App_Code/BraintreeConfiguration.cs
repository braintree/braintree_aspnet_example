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
            string environment = System.Environment.GetEnvironmentVariable("BraintreeEnvironment");

            Environment = GetEnvironment(environment);
            MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
            PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
            PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                environment = GetConfigurationSetting("BraintreeEnvironment");

                Environment = GetEnvironment(environment);
                MerchantId = GetConfigurationSetting("BraintreeMerchantId");
                PublicKey = GetConfigurationSetting("BraintreePublicKey");
                PrivateKey = GetConfigurationSetting("BraintreePrivateKey");
            }

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

        public Braintree.Environment GetEnvironment(string environment)
        {
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