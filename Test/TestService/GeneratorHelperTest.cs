using NUnit.Framework;
using PostmanExporter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.TestService
{
    public class GeneratorHelperTest
    {
        [Test]
        public void GetControllersTest()
        {
            //arrange
            var types = new Type[] { };

            //act
            var result = GeneratorHelper.GetControllers(types);

            //assert
            Assert.AreEqual(result, types);
        }

        [Test]
        public void GetControllerNameTest()
        {
            //arrange
            var controller = new Type[] { }.FirstOrDefault();

            //act
            var result = GeneratorHelper.GetControllerName(controller);

            //assert
            Assert.AreEqual(result, string.Empty);
        }
    }
}
