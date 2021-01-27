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
    public class ModelMartConnectionTests
    {

        [Test]
        public void OpenModelMartModel_Given_NoConnection_ExpectException()
        {
            ERwinApiWrapper app = new ERwinApiWrapper();

            Assert.Throws<NullReferenceException>(() => app.ModelMartConnection.OpenModel(""));
        }

        [Test]
        public void Open_ExpectSuccessfulConnection()
        {
            //Arrange
            ModelMartConfiguration configuration = GetTestModelMartConfiguration();
            ERwinApiWrapper app = new ERwinApiWrapper(configuration);
            
            //Act
            app.ModelMartConnection.Open();
            var martIsOpen = app.ModelMartConnection.IsConnected;

            //Assert
            Assert.IsTrue(martIsOpen);
            if (martIsOpen)
            {
                app.ModelMartConnection.Close();
            }
        }

        [Test]
        public void Close_ExpectSuccessfulConnectionClosure()
        {
            //Arrange
            ModelMartConfiguration configuration = GetTestModelMartConfiguration();
            ERwinApiWrapper app = new ERwinApiWrapper(configuration);

            //Act
            app.ModelMartConnection.Open();
            app.ModelMartConnection.Close();
            var martIsOpen = app.ModelMartConnection.IsConnected;

            //Assert
            Assert.IsFalse(martIsOpen);
        }


        [Test]
        public void FillModelMartCredentials_WithUserIdAndPassword_ExpectSuccessfulConnection()
        {
            //Arrange
            ModelMartConfiguration configuration = GetTestModelMartConfiguration();
            ERwinApiWrapper app = new ERwinApiWrapper(configuration);

            //Act
            var successfullConnected = app.ModelMartConnection.TestModelMartConnection();

            //Assert
            Assert.IsTrue(successfullConnected);
        }

        private static ModelMartConfiguration GetTestModelMartConfiguration()
        {
            ModelMartConfiguration c = new ModelMartConfiguration("");
            c.Username = "";
            c.Password = "";
            return c;
        }

    }
}
