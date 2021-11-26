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
        [
            Test,
            TestCase("abcd1234", false),
            TestCase("irf@uni-corvinus", false),
            TestCase("irf.uni-corvinus.hu", false),
            TestCase("irf@uni-corvinus.hu", true)
        ]
        public void TestValidateEmail(string email, bool expectedResult) 
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            var result = accountController.ValidateEmail(email);

            // Assert
            Assert.AreEqual(result, expectedResult);
        }

        [
            Test,
            TestCase("abcd1234", false),
            TestCase("ABCD1234", false),
            TestCase("abcdABCD", false),
            TestCase("abCD12", false),
            TestCase("abCD1234", true),
            TestCase("abCDEF12", true)
        ]
        public void TestValidatePassword(string password, bool expectedResult)
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            var result = accountController.ValidatePassword(password);

            // Assert
            Assert.AreEqual(result, expectedResult);
        }
    }
}
