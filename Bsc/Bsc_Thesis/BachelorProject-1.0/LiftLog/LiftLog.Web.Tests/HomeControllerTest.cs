using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Web.Controllers.Web;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace LiftLog.Web.Tests
{
    
    /// <summary>
    /// This class tests the controller logic of the HomeController and tests how it behaves based on valid and invalid inputs.
    /// This includes the following controller responsibilities:
    /// 1) Verify ModelState.IsValid
    /// 2) Return error response if Modelstate is invalid
    /// 3) Retrieve a business entity from persistence 
    /// 4) Perform action on business entity
    /// 5) Save business entity to persistence
    /// 6) Return appropriate IActionResult
    /// Note that 3-5 are tested in the service layer. 
    /// </summary>
    public class HomeControllerTest
    {

        /// <summary>
        /// Test Index method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Index returns a ViewResult")]
        public void Index_ReturnsAViewResult_WhenIndex()
        {
            // Arrange 
            var controller = new HomeController();
            var message = "StatusMessage";

            // Act 
            var result = controller.Index(message);
            
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test About method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "About returns a ViewResult")]
        public void About_ReturnsAViewResult_WhenAbout()
        {
            // Arrange
            var controller = new HomeController();

            // Act 
            var result = controller.About();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test Contact method by verifying that a ViewResult is returned. 
        /// </summary>
        /// <returns> True if a view result is returned. </returns>
        [Fact(DisplayName = "Contact returns a ViewResult")]
        public void Contact_ReturnsAViewResult_WhenContact()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Contact();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

    }
}
