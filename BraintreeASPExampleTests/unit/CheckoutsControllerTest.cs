using System.Web.Mvc;
using Braintree;
using BraintreeASPExample;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BraintreeASPExampleTests
{
    [TestClass]
    public class CheckoutsControllerTest
    {
        [TestMethod]
        public void TestForClientToken()
        {
            var clientTokenMock = new Mock<IClientTokenGateway>();
            clientTokenMock.Setup(mock => mock.Generate(null)).Returns("CLIENT_TOKEN");
            var braintreeGatewayMock = new Mock<BraintreeGateway>();
            braintreeGatewayMock.Setup(mock => mock.ClientToken).Returns(clientTokenMock.Object);
            var braintreeConfigurationMock = new Mock<IBraintreeConfiguration>();
            braintreeConfigurationMock.Setup(mock => mock.GetGateway()).Returns(braintreeGatewayMock.Object);

            var controller = new BraintreeASPExample.Controllers.CheckoutsController();
            controller.config = braintreeConfigurationMock.Object;
            var result = controller.New() as ViewResult;

            Assert.AreEqual("CLIENT_TOKEN", result.ViewData["ClientToken"]);
        }
    }
}
