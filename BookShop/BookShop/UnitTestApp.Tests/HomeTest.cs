using BookShop.Areas.Admin.Controllers;
using BookShop.Areas.Customer.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Xunit;

namespace UnitTestApp.Tests
{
    
    public class HomeTest
    {

        [Fact]
        public void IndexViewDataMessage()
        {
            // Arrange
            TESTController controller = new TESTController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.Equal("For testing", result?.ViewBag.Message);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            TESTController controller = new TESTController();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void IndexViewNameEqualIndex()
        {
            // Arrange
            TESTController controller = new TESTController();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.Equal("Index", result?.ViewName);
        }
    }
}
