using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
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
        private TimeSpan timeoutForgiving = TimeSpan.FromSeconds(60.0);
        private TimeSpan timeoutStrict = TimeSpan.FromSeconds(0.05);
        private string _applicationName = "BraintreeASPExample";
        private Process _iisProcess;
        IWebDriver driver = new FirefoxDriver();

        [TestInitialize]
        public void TestInitialize()
        {
            StartIIS();
            driver.Manage().Timeouts().SetPageLoadTimeout(timeoutForgiving);
            driver.Manage().Timeouts().ImplicitlyWait(timeoutForgiving);
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

        private void FillCvvIfPresent()
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(timeoutStrict);
                IWebElement cvvField = driver.FindElement(By.Id("cvv"));
                cvvField.Click();
                cvvField.SendKeys("123");
            }
            catch (NoSuchElementException){ }
            finally
            {
                driver.Manage().Timeouts().ImplicitlyWait(timeoutForgiving);
            }

        }

        [TestMethod]
        public void TestCheckoutsPageRenders()
        {
            driver.Navigate().GoToUrl(GetAbsoluteUrl("/"));
            Assert.IsTrue(driver.Title.Equals(_applicationName));
            Assert.IsTrue(driver.FindElement(By.Id("payment-form")).Displayed);
            Assert.IsTrue(driver.FindElement(By.Id("amount")).Displayed);
            Assert.IsTrue(driver.FindElement(By.XPath("//button[@type='submit']")).Displayed);

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
            FillCvvIfPresent();

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

            driver.SwitchTo().Frame("braintree-dropin-frame");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4111111111111111");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");
            FillCvvIfPresent();

            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

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

            driver.SwitchTo().Frame("braintree-dropin-frame");
            driver.FindElement(By.Id("credit-card-number")).Click();
            driver.FindElement(By.Id("credit-card-number")).SendKeys("4111111111111111");
            driver.FindElement(By.Id("expiration")).Click();
            driver.FindElement(By.Id("expiration")).SendKeys("1020");
            FillCvvIfPresent();

            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            
            Assert.IsTrue(driver.FindElement(By.Id("payment-form")).Displayed);
            Assert.IsTrue(driver.FindElement(
                By.XPath("//*[contains(@class, 'notice-message')]"))
                    .GetAttribute("innerText")
                    .Contains("Error: 81503: Amount is an invalid format.")
            );
        }
    }
}
