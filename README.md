# Braintree ASP.NET Example
An example Braintree integration for ASP.NET

## Setup Instructions
These instructions assume you are using Visual Studio Professional 2013. This has not been tested in Xamarin Studio, or other verisons of Visual Studio. This project is also set up 
to use NuGet package restore, so all dependencies should be automatically installed.

1. Open `braintree_aspnet_example.sln` in Visual Studio

2. Open the `Web.config` in `BraintreeASPExample`, and fill in your Braintree API Credentials in the `BraintreeEnvironment`, `BraintreeMerchantId`, `BraintreePublicKey`, and `BraintreePrivateKey` keys.
   Credentials can be found by navigating to Account > My user > View API Keys in the Braintree control panel. Full instructions can be [found on our support site](https://articles.braintreepayments.com/control-panel/important-gateway-credentials#api-credentials).

3. Start rails
   `rails server`

## Running Tests

### Running Unit Tests

Unit tests do not make api calls to Braintree and do not require Braintree credentials. You can run this project's unit tests by
calling `rake` on the command line.

### Running Integration Tests

Integration tests make api calls to Braintree and require that you set up your Braintree credentials. You can run this project's integration tests by adding your sandbox api credentials to `.env` and calling `rake spec:integration` on the command line.

### Running All Tests

You can run both unit and integrations tests by calling `rake spec` on the command line.

## Pro Tips

 * If you do not want to use NuGet package restore, make sure to install the following packages from NuGet:
   * Braintree 2.53.0 or higher
   * Moq 3.0 or higher (needed for `BraintreeASPExampleTests` only)

## Disclaimer

This code is provided as is and is only intended to be used for illustration purposes. This code is not production-ready and is not meant to be used in a production environment. This repository is to be used as a tool to help merchants learn how to integrate with Braintree. Any use of this repository or any of its code in a production environment is highly discouraged.

