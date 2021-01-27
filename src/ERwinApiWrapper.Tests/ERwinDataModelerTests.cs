using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace ERwinApiWrapper.Tests
{
    [TestFixture]
    public class ErwinApplicationTests
    {
        [Test]
        public void ERwinApplication_GivenNew_ExpectNoErrors()
        {
            ERwinApiWrapper a = new ERwinApiWrapper();
        }


    }
}
