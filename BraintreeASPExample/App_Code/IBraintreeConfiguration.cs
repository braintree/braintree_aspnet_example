using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraintreeASPExample
{
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway createGateway();
        string getConfigurationSetting(string setting);
        Braintree.Environment getEnvironment();
        IBraintreeGateway getGateway();
    }
}
