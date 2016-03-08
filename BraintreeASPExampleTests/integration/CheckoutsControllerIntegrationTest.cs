using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace BraintreeASPExampleTests.integration
{
    [TestClass]
    public class CheckoutsControllerIntegrationTest
    {
        // Setup of IIS server with Selenium from http://stephenwalther.com/archive/2011/12/22/asp-net-mvc-selenium-iisexpress
        private int iisPort = 8080;
        private string _applicationName = "BraintreeASPExample";
        private Process _iisProcess;
        IWebDriver driver = new FirefoxDriver();

        [TestInitialize]
        public void TestInitialize()
        {
            StartIIS();
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
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
            Assert.IsTrue(driver.FindElement(By.Id("checkout")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
            Assert.IsTrue(driver.FindElement(By.XPath("//input[@type='submit' and @value='Pay Now']")).Displayed);

            driver.SwitchTo().Frame("braintree-dropin-frame");
            Assert.IsTrue(driver.FindElement(By.ClassName("inline-frame")).Displayed);
            driver.FindElement(By.Id("credit-card-number")).Click();
            Assert.IsTrue(driver.FindElement(By.Id("credit-card-number")).Displayed);
            driver.FindElement(By.Id("expiration")).Click();
            Assert.IsTrue(driver.FindElement(By.Id("expiration")).Displayed);
        }

        [TestMethod]
        public void TestProcessesTransactionAndDisplaysDetails()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("10.00");

            driver.SwitchTo().Frame("braintree-dropin-frame");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4242424242424242");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");

            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Pay Now']")).Click();

            Assert.IsTrue(driver.FindElement(By.TagName("h1")).Text.Contains("Transaction"));
            Assert.IsTrue(driver.FindElement(By.XPath("//h2[2]")).Text.Contains("Credit Card Details"));
            Assert.IsTrue(driver.FindElement(By.XPath("//h2[3]")).Text.Contains("Customer Details"));
        }

        [TestMethod]
        public void TestTransactionProcessorDeclined()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("2000.00");

            driver.SwitchTo().Frame("braintree-dropin-frame");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4111111111111111");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");

            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Pay Now']")).Click();

            Assert.IsTrue(driver.FindElement(By.TagName("h1")).Text.Contains("Transaction"));
            Assert.IsTrue(driver.FindElement(By.XPath("//div[1]")).Text.Contains("Transaction status - processor_declined"));

        }

        [TestMethod]
        public void TestStaysOnCheckoutPageIfTransactionFails()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            driver.FindElement(By.Id("amount")).Clear();
            driver.FindElement(By.Id("amount")).SendKeys("NaN");

            driver.SwitchTo().Frame("braintree-dropin-frame");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4111111111111111");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");

            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Pay Now']")).Click();

            Assert.IsTrue(driver.FindElement(By.Id("checkout")).Displayed);
            Assert.IsTrue(driver.FindElement(By.XPath("//div[1]")).Text.Contains("Error: 81503: Amount is an invalid format."));
        }
    }
}
