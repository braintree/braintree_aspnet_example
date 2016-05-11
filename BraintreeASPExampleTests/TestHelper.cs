using System;

namespace BraintreeASPExampleTests
{
    public class TestHelper
    {
        public const string BRAINTREE_ENVIRONMENT = "BraintreeEnvironment";
        public const string BRAINTREE_MERCHANT_ID = "BraintreeMerchantId";
        public const string BRAINTREE_PUBLIC_KEY = "BraintreePublicKey";
        public const string BRAINTREE_PRIVATE_KEY = "BraintreePrivateKey";

        string environment;
        string merchantId;
        string publicKey;
        string privateKey;

        public void StoreEnvironmentVariables()
        {
            environment = System.Environment.GetEnvironmentVariable(BRAINTREE_ENVIRONMENT);
            merchantId = System.Environment.GetEnvironmentVariable(BRAINTREE_MERCHANT_ID);
            publicKey = System.Environment.GetEnvironmentVariable(BRAINTREE_PUBLIC_KEY);
            privateKey = System.Environment.GetEnvironmentVariable(BRAINTREE_PRIVATE_KEY);
        }

        public void RemoveEnvironmentVariables()
        {
            System.Environment.SetEnvironmentVariable(BRAINTREE_ENVIRONMENT, null, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_MERCHANT_ID, null, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_PUBLIC_KEY, null, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_PRIVATE_KEY, null, EnvironmentVariableTarget.Process);
        }

        public void RestoreEnvironmentVariables()
        {
            System.Environment.SetEnvironmentVariable(BRAINTREE_ENVIRONMENT, environment, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_MERCHANT_ID, merchantId, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_PUBLIC_KEY, publicKey, EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(BRAINTREE_PRIVATE_KEY, privateKey, EnvironmentVariableTarget.Process);
        }
    }
}
