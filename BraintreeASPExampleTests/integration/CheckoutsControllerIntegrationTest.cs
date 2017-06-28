using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace BraintreeASPExampleTests.integration
{
    [TestClass]
    public class CheckoutsControllerIntegrationTest
    {
        // Setup of IIS server with Selenium from http://stephenwalther.com/archive/2011/12/22/asp-net-mvc-selenium-iisexpress
        private int iisPort = 8080;
        private TimeSpan timeoutForgiving = TimeSpan.FromSeconds(60.0);
        private TimeSpan timeoutStrict = TimeSpan.FromSeconds(0.05);
        private string _applicationName = "BraintreeASPExample";
        private Process _iisProcess;
        IWebDriver driver = new FirefoxDriver();

        private WebDriverWait wait;

        [TestInitialize]
        public void TestInitialize()
        {
            StartIIS();
            driver.Manage().Timeouts().SetPageLoadTimeout(timeoutForgiving);
            driver.Manage().Timeouts().ImplicitlyWait(timeoutForgiving);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Ensure IISExpress is stopped
            if (_iisProcess.HasExited == false)
            {
                _iisProcess.Kill();
            }

            driver.Quit();
        }

        private void StartIIS()
        {
            var applicationPath = GetApplicationPath(_applicationName);
            var programFiles = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);

            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            _iisProcess.StartInfo.Arguments = string.Format("/path:{0} /port:{1}", applicationPath, iisPort);
            _iisProcess.Start();
        }

        protected string GetApplicationPath(string applicationName)
        {
            var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            return Path.Combine(solutionFolder, applicationName);
        }

        public string GetAbsoluteUrl(string relativeUrl)
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return String.Format("http://localhost:{0}{1}", iisPort, relativeUrl);
        }

        [TestMethod]
        public void TestCheckoutsPageRenders()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            Assert.IsTrue(driver.Title.Equals(_applicationName));
            Assert.IsTrue(driver.FindElement(By.Id("payment-form")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
            Assert.IsTrue(driver.FindElement(By.XPath("//button[@type='submit']")).Displayed);

            Assert.IsTrue(driver.FindElement(By.ClassName("braintree-dropin")).Displayed);
        }

        [TestMethod]
        public void TestProcessesTransactionAndDisplaysDetails()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("10.00");

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-option__card")));
            driver.FindElement(By.ClassName("braintree-option__card")).Click();

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-form__hosted-field")));
            driver.SwitchTo().Frame("braintree-hosted-field-number");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4242424242424242");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-expirationDate");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-cvv");
            driver.FindElement(By.Id("cvv")).Click();
            driver.FindElement(By.Id("cvv")).SendKeys("123");
            driver.SwitchTo().ParentFrame();

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            ReadOnlyCollection<IWebElement> headerTags = driver.FindElements(By.TagName("h5"));

            Assert.IsTrue(headerTags[0].GetAttribute("innerText").Contains("Transaction"));
            Assert.IsTrue(headerTags[1].GetAttribute("innerText").Contains("Payment"));
        }

        [TestMethod]
        public void TestTransactionProcessorDeclined()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("2000.00");

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-option__card")));
            driver.FindElement(By.ClassName("braintree-option__card")).Click();

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-form__hosted-field")));
            driver.SwitchTo().Frame("braintree-hosted-field-number");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4242424242424242");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-expirationDate");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-cvv");
            driver.FindElement(By.Id("cvv")).Click();
            driver.FindElement(By.Id("cvv")).SendKeys("123");
            driver.SwitchTo().ParentFrame();

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("response")));

            Assert.IsTrue(driver.FindElement(By.TagName("h1")).GetAttribute("innerText").Contains("Transaction Failed"));

            Assert.IsTrue(driver.FindElement(
                By.XPath("//*[contains(@class, 'response')]/div/section[1]/p"))
                    .GetAttribute("innerText")
                    .Contains("Your test transaction has a status of processor_declined.")
            );
        }

        [TestMethod]
        public void TestStaysOnCheckoutPageIfTransactionFails()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("NaN");

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-option__card")));
            driver.FindElement(By.ClassName("braintree-option__card")).Click();

            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("braintree-form__hosted-field")));
            driver.SwitchTo().Frame("braintree-hosted-field-number");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4242424242424242");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-expirationDate");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");
            driver.SwitchTo().ParentFrame();

            driver.SwitchTo().Frame("braintree-hosted-field-cvv");
            driver.FindElement(By.Id("cvv")).Click();
            driver.FindElement(By.Id("cvv")).SendKeys("123");
            driver.SwitchTo().ParentFrame();

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

           wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("notice-message")));

            Assert.IsTrue(driver.FindElement(By.Id("payment-form")).Displayed);
            Assert.IsTrue(driver.FindElement(
                By.XPath("//*[contains(@class, 'notice-message')]"))
                    .GetAttribute("innerText")
                    .Contains("Error: 81503: Amount is an invalid format.")
            );
        }
    }
}
