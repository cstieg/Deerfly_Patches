using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Deerfly_Patches.Controllers.ModelControllers;
using System.Web.Mvc;

namespace Deerfly_Patches.Tests.Controllers
{
    [TestClass]
    public class RetailerMapControllerTest
    {
        RetailersController controller = new RetailersController();

        [TestMethod]
        public void TestIndex()
        {
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestListJson()
        {
            JsonResult jsonResult = controller.ListJson();

            Assert.IsNotNull(jsonResult);
        }
    }
}
