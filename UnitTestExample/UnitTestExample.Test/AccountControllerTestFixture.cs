using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestExample.Controllers;

namespace UnitTestExample.Test
{
    class AccountControllerTestFixture
    {
        [Test]
        public void TestValidateEmail(string email, bool expectedResult) 
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            var result = accountController.ValidateEmail(email);

            // Assert
            Assert.AreEqual(ResultStateException, expectedReasult);
        }
    }
}
