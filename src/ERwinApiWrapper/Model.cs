using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SCAPI;
using PrimitiveExtensions;
using System.Data;

namespace ERwinApiWrapper
{
    public class Model
    {
        // Common Properties:
        public SCAPI.Application Application { get; private set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Definition { get; set; }

        public Model(string location)
        {
        }

        public int GetTableCount(string tableName = "Entity")
        {
            return ModelObjects.Collect(RootObjectID, tableName).Count;
        }

        public List<Table> GetTables(string tableName = "Entity")
        {
            List<Table> tables = new List<Table>();
            foreach (ModelObject mo in ModelObjects.Collect(RootObjectID, tableName))
            {
                tables.Add(new Table(this, mo));
            }
            return tables;
        }

        /// <summary>
        /// Property names should be in format of ClassName.Property.AsString or ClassName.Property.Value
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            DataSet ds = new DataSet(this.GetModelObject().Name);

            foreach (ModelObject tableMo in ModelObjects.Collect(RootObjectID, "Entity"))
            {
                DataTable dt = ds.AddNewDataTableWithERwinExtendedProperties(tableMo.Name,
                    tableMo.NzPropertyString("User_Formatted_Physical_Name"),
                    tableMo.NzPropertyString("Definition"));

                dt.ExtendedProperties.Add("Long_Id", tableMo.NzPropertyString("Long_Id"));

                foreach (ModelObject columnMo in ModelObjects.Collect(tableMo, "Attribute"))
                {
                    DataColumn c = dt.AddNewDataColumnWithERwinExtendedProperties(columnMo.Name,
                    columnMo.NzPropertyString("User_Formatted_Physical_Name"),
                    columnMo.NzPropertyString("Physical_Data_Type"),
                    (columnMo.NzPropertyString("Null_Option_Type") == "Null"),
                    columnMo.NzPropertyString("Definition"));
                    c.ExtendedProperties.Add("Long_Id", columnMo.NzPropertyString("Long_Id"));
                }
            }

            //add key groups
            foreach (ModelObject tableMo in ModelObjects.Collect(RootObjectID, "Entity"))
            {
                string relationName = "";
                DataTable dtParent = ds.Tables[tableMo.NzPropertyString("User_Formatted_Physical_Name")];
                
                foreach (ModelObject keyGroupMo in ModelObjects.Collect(tableMo, "Key_Group"))
                {
                    string keyGroupType = keyGroupMo.NzPropertyString("Key_Group_Type");

                    Dictionary<string,int> keyValuePairs = new Dictionary<string,int>();
                    foreach (ModelObject keygroupMember in ModelObjects.Collect(keyGroupMo, "Key_Group_Member"))
                    {
                        //DataTable dtChild = ds.Tables[tableMo.NzPropertyString("User_Formatted_Physical_Name")];
                        int orderNum = (keygroupMember.NzPropertyInteger("Key_Group_Member_Order"));
                        string colName = keygroupMember.NzPropertyString("User_Formatted_Physical_Name");
                        string tableName = keygroupMember.Context.Context.NzPropertyString("User_Formatted_Physical_Name");
                        try
                        {
                            keyValuePairs.Add($"{tableName}.{colName}", orderNum);
                        }
                        catch (Exception)
                        {
                           
                        }
                    }

                    relationName = keyGroupMo.NzPropertyString("User_Formatted_Physical_Name");



                    if (keyGroupType == "PK")
                    {
                        List<DataColumn> cols = new List<DataColumn>();
                        foreach (var item in keyValuePairs.OrderBy(a=>a.Value))
                        {
                            cols.Add(dtParent.Columns[item.Key.Split('.')[1]]);
                        }
                        dtParent.PrimaryKey = cols.ToArray();
                    }
                    else
                    {
                      //  ds.Relations.Add(new DataRelation(relationName, "", "", "", "", false));
                    }
                }

            }
            return ds;
        }

        public ModelObject GetModelObject()
        {
            return this.ModelObjects[this.RootObjectID];
        }

        public bool HasUnsavedChanged { get; private set; }

        //API Properties
        public ModelObjects ModelObjects { get; private set; }



        public PersistenceUnit ModelPersistenceFile { get; private set; }

        public string RootObjectID { get; private set; }
        public bool IsOpen { get; private set; }
        private ModelDirectoryUnit _moDirUnit;
        public ModelDirectoryUnit MoDirUnit { get => _moDirUnit; }
        public Library ParentCatalogObject { get; private set; }



        private string _Locator;


        public string Locator
        {
            get
            {
                if (_moDirUnit != null)
                {
                    return _moDirUnit.Locator;
                }
                else
                {
                    return _Locator;
                }

            }
            set => _Locator = value;
        }

        public Table AddNewTable()
        {
            return new Table(this, ModelObjects.Add("Entity"));
        }

        private Session _ModelingSession;
        public Session ModelingSession
        {
            get
            {
                if (_ModelingSession == null)
                {
                    _ModelingSession = Application.Sessions.Add();
                }
                return _ModelingSession;
            }
            private set => _ModelingSession = value;
        }





        public Model(string locator, Library parent = null, string name = "") : this(locator)
        {
            Locator = locator;
            ParentCatalogObject = parent;
            Name = name;
        }

        public Model(string locator, AppWrapper scApp, Library parent = null, string name = "") : this(locator)
        {
            Application = scApp.App;
            Locator = locator;
            ParentCatalogObject = parent;
            Name = name;
        }

        [Obsolete("use the AppWrapper signature")]
        public Model(string locator, ERwinApiWrapper scApp, Library parent = null, string name = "") : this(locator)
        {
            Application = scApp.ScapiApplication;
            Locator = locator;
            ParentCatalogObject = parent;
            Name = name;
        }

        public Model(ModelDirectoryUnit moDirUnit, ERwinApiWrapper scApp, Library parent = null) : this("")
        {
            _moDirUnit = moDirUnit;
            Name = _moDirUnit.Name;
            //_Locator = moDirUnit.Locator;
            ParentCatalogObject = parent;
            Application = scApp.ScapiApplication;
            HasUnsavedChanged = false;
        }


        public void Create(ERwinApiWrapper wrapper = null)
        {
            if (wrapper != null) 
                Create(wrapper.AppWrapper);
            else
                Create(new AppWrapper(Application));
        }
        public void Create(AppWrapper app)
        {
            PersistenceUnits pus = app.App.PersistenceUnits;
            PropertyBag propertyBag = new PropertyBag();

            //propertyBag.Add("Long_Id", new Guid().ToString());
            //propertyBag.Add("Name", name);
            //propertyBag.Add("ModelType", modelType);
            PersistenceUnit pu = pus.Create(propertyBag);
            ModelPersistenceFile = pu;
            ModelingSession.Open(ModelPersistenceFile.ModelSet());
            ModelObjects = ModelingSession.ModelObjects;
            RootObjectID = ModelObjects.Root.ObjectId;


        }

        public void Open(object app = null)
        {
            //if (Locator.ToLower().StartsWith("mart://") && app.ModelMartConfiguration.Username == null )
            //{
            //    //scApp.FillModelMartCredentials(LoginForm.ShowDialogAndReturnCredential("Enter your ERwin Model Mart Credentials"));
            //}
            if (!IsOpen)
            {

                //string lockingOption = "OVL=Yes";
                try
                {
                    if (!Locator.ToLower().StartsWith("mart://"))
                    {
                        FileInfo f = new FileInfo(Locator);
                        f.IsReadOnly = false;
                    }
                    ModelPersistenceFile = Application.PersistenceUnits.Add(Locator);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to open model: " + ex.Message);
                }
                ModelingSession.Open(ModelPersistenceFile.ModelSet());

                ModelObjects = ModelingSession.ModelObjects;

                RootObjectID = ModelObjects.Root.ObjectId;
                Name = ModelObjects.Root.Name;
                Author = ModelObjects.Root.NzPropertyString("Author");
                Definition = ModelObjects.Root.NzPropertyString("Definition");
                IsOpen = true;
                //PopulateSubjectAreas();
                //OnModelOpened();
            }
            else
            {
                //OnNotifyUser("Model already open");
            }

        }

        public ModelTransaction BeginTransaction()
        {
            return new ModelTransaction(this);
        }

        //public void CommitTransaction(object transactionID)
        //{
        //    ModelingSession.CommitTransaction(transactionID);
        //}

        public void Close(ERwinApiWrapper app = null)
        {
            if (app != null)
                Close(app.AppWrapper);
            else
                Close(new AppWrapper(Application));
        }
        public void Close(AppWrapper app)
        {
            // ModelingSession.Close();
            if (app != null)
            {
                app.App.PersistenceUnits.Remove(ModelPersistenceFile);
                //TODO:is this needed?
                //ModelingSession.Close();


                //app.ScapiApplication.Sessions.Remove(ModelingSession.i;
            }

            IsOpen = false;
        }

        public void Save()
        {
            this.ModelPersistenceFile.Save(Locator);
            HasUnsavedChanged = false;
            //   OnModelSaved();
        }
        public void Save(string locator)
        {
            this.ModelPersistenceFile.Save(locator);
            Locator = locator;
        }

    }
}