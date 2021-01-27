using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using PrimitiveExtensions;
using System.Data;
//using Newtonsoft.Json;

namespace ERwinApiWrapper.Tests
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void ToDataSet_GivenScapiERwinModel_ExpectValidDataSet()
        {
            //try
            //{
                ERwinApiWrapper app = new ERwinApiWrapper();
                Model m = app.OpenModel(GetLocalModel());
                //string[] classNames = { "Entity.Name.AsString", "Domain.Name.AsString", "Attribute.Name.AsString", "Key_Group.Name.AsString", "Key_Group_Member.Name.AsString", "Relationship.Name.AsString" };
                DataSet dataSet = m.ToDataSet();
                m.Close();
                Assert.AreEqual(11, dataSet.Tables.Count);

            //}
            //catch (Exception e)
            //{
            //    Assert.Fail(e.Message);
            //}
        }

        [Test]
        public void OpenLocalERwinModelWith16Tables_Expect16Tables()
        {
            try
            {
                ERwinApiWrapper app = new ERwinApiWrapper();
                Model m = app.OpenModel(GetLocalModel());
                var tables = m.GetTables();
                Assert.AreEqual(11, tables.Count);
                m.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void CreateNewModel_ExpectNoErrors()
        {
            try
            {
                ERwinApiWrapper app = new ERwinApiWrapper();
                Model m = app.AddNewModel(GetTestFileName());
                m.Save();
                m.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void AddTablesToModelAndSave_ExpectNoErrors()
        {
            try
            {
                ERwinApiWrapper app = new ERwinApiWrapper();
                Model m = app.AddNewModel(GetTestFileName());
                m.Save();

                var trans = m.BeginTransaction();

                Table t = m.AddNewTable();
                t.LogicalName = "MyNewTable";
                t.LogicalDefinition = "my logical def";

                Column c = t.AddNewColumn();
                c.LogicalName = "My Col";
                c.LogicalDatatype = "Char(123)";
                c.LogicalDefinition = "my deifnition";
                                
                trans.Commit();
                m.Save();
                m.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void OpenLocalERwinModel_GivenBadFileName_ExpectException()
        {
            //Arrange
            string modelLocation = @"DOESNT_EXIST.erwin";

            //Act
            ERwinApiWrapper app = new ERwinApiWrapper();

            try
            {
                Model m = app.OpenModel(modelLocation);
            }
            catch (Exception ex)
            {
                File.Delete(modelLocation);
                Assert.True(ex.Message.Contains("Unable to open model"));
            }
        }

        [Test]
        public void OpenLocalERwinModel_GivenGoodFileName_ExpectOpenModel()
        {
            string localLocation = GetLocalModel();

            try
            {
                ERwinApiWrapper app = new ERwinApiWrapper();
                Model m = app.OpenModel(localLocation);

                Assert.AreEqual(m.Name, "UnitTestModel");
                Assert.AreEqual(m.Author, "Unit Tester");
                Assert.AreEqual(m.Definition, "The definition of this model for unit tests.");
                m.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public void OpenModelMartERwinModel_GivenGoodFileName_ExpectOpenModel()
        {
            string localLocation = GetLocalModel();

            try       
            {
                ERwinApiWrapper app = new ERwinApiWrapper();
                string modelPath = @"mart://Mart/API Testing/ora-model mart";
                ModelMartConfiguration config = GetTestModelMartConfiguration();
                ModelMartConnection cn = new ModelMartConnection(app.AppWrapper, config);
                cn.Open();
                Model m = cn.OpenModel(modelPath);
                var x = m.BeginTransaction();
                m.AddNewTable();
                x.Commit();
                m.Save();

                m.Close();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        public static string ToFilePath(string martPath, string prefix = "")
        {
            return prefix + martPath.Substring(12).Replace("/", "(!)");
        }

        [Test]
        public void OpenModelMartERwinModel_DownloadModel()
        {
            const string DOWNLOAD_DIR = @"C:\Temp";
            Assert.True(Directory.Exists(DOWNLOAD_DIR));
            try
            {
                ModelMartConfiguration configuration = GetTestModelMartConfiguration();
                ERwinApiWrapper app = new ERwinApiWrapper(configuration);
                app.ModelMartConnection.Open();
                var martIsOpen = app.ModelMartConnection.IsConnected;
                if (martIsOpen)
                {
                    string modelPath = @"mart://Mart/API Testing/ora-model mart";
                    Model m = app.ModelMartConnection.OpenModel(modelPath);
                    app.ModelMartConnection.SaveMMModelToLocal(m, DOWNLOAD_DIR + @"\", ToFilePath(modelPath) + ".erwin");
                    app.ModelMartConnection.Close();

                    Assert.True(File.Exists(DOWNLOAD_DIR + @"\" + ToFilePath(modelPath) + ".erwin"));
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private static string GetLocalModel()
        {
            string modelLocation = Path.Combine(TestContext.CurrentContext.WorkDirectory, "UnitTestModel.erwin");
            string localLocation = Path.Combine(TestContext.CurrentContext.WorkDirectory, "UnitTestModel_"+ReflectionHelper.GetThisCallersMethodName()+".erwin");
            try { File.Copy(modelLocation, localLocation, true); }
            catch (Exception)
            {
            }

            return localLocation;
        }
        private static string GetTestFileName()
        {
            
            Random r = new Random();
            string filename = DateTime.UtcNow.ToString("o").Replace("-", "").Replace(":", "") + "_" + ReflectionHelper.GetThisCallersMethodName() ;
            string localLocation = Path.Combine(TestContext.CurrentContext.WorkDirectory, filename + ".erwin");

            return localLocation;
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
