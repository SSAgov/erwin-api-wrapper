using SCAPI;
using PrimitiveExtensions;
using System;
using System.Data;
using System.Collections.Generic;

namespace ERwinApiWrapper
{
    public class ModelMartConnection : IDbConnection
    {

        private ModelDirectory _scModelMart;
        private ConnectionState _State;

        private SCAPI.Application Application;
        private string MartLocator;

        public ModelMartConnection(ERwinApiWrapper app, ModelMartConfiguration config)
        : this(new AppWrapper(app.ScapiApplication), config)
        {

        }
        public ModelMartConnection(AppWrapper app, ModelMartConfiguration config)
        {
            Application = app.App;

            MartLocator = config.MartLocator;
            ConnectionString = config.ConnectionString;
        }


        public StateChangeEventHandler StateChange;
        private void OnStateChange(ConnectionState newState)
        {
            StateChange?.Invoke(this, new StateChangeEventArgs(_State, newState));
        }

        public void Open()
        {
            if (_State != ConnectionState.Open)
            {
                Application.Sessions.Add();
                bool success = Application.ConnectMMart(ConnectionString, "", true);
                if (success)
                {
                    ModelDirectories mDirs = Application.ModelDirectories;
                    _scModelMart = mDirs.Add(MartLocator, "");
                    OnStateChange(ConnectionState.Open);
                }
            }
        }

        public void Close()
        {
            //_scApp.PersistenceUnits.Clear();
            _scModelMart = null;

            Application.DisconnectMMart();
            //OnStateChange(ConnectionState.Closed);
        }

        public bool IsConnected
        {
            get
            {
                if (_scModelMart != null)
                {
                    return true;
                }
                return false;
            }
        }

        public string ConnectionString         { get;set; }
                

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State => throw new NotImplementedException();

        public bool TestModelMartConnection()
        {
            bool result = false;

            if (ConnectionString.IsNullOrEmptyString()) return false;

            try
            {
                Open();
                if (IsConnected)
                {
                    result = true;
                }
                Close();
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

        /// <summary>
        /// Example: mart://Mart/Directory/ModelName
        /// </summary>
        /// <param name="fullyQualifiedModelPath"></param>
        /// <returns></returns>
        public Model OpenModel(string fullyQualifiedModelPath)
        {
            if (IsConnected)
            {
                string separator = fullyQualifiedModelPath.Contains("?") ? "&" : "?";
                Model m = new Model(fullyQualifiedModelPath.Trim() + separator + ConnectionString, 
                    new AppWrapper(Application));
                //_ErwinSession = _scApp.Sessions.Add();
                m.Open();
                return m;
            }
            else
            {
                throw new Exception("Not connected to ModelMart");
            }
        }

        public void SaveMMModelToLocal(Model model, string filePath, string fileName)
        {
            PersistenceUnits pus = Application.PersistenceUnits;
            try
            {
                PersistenceUnit pu = pus.Add(model.Locator);
                pu.Save(filePath + fileName, "RDO=YES;OVF=YES;");
                Application.PersistenceUnits.Clear();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(filePath, fileName, ex.Message);
            }
        }

        public List<string> ListMartDir(string dirName)
        {
            List<string> entries = new List<string>();

            ModelDirectory md = _scModelMart.LocateDirectory("MART://MART/" + dirName);
            string name = md.Name;

            ModelDirectoryUnit mdu= md.LocateDirectoryUnit("*");
            while (true)
            {
                if (mdu == null) break;
                name = mdu.Name;
                entries.Add(name);

                mdu = md.LocateDirectoryUnitNext();
            }

            return entries;

        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();          
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }



        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}