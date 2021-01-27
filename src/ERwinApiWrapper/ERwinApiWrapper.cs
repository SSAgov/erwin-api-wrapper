using SCAPI;
using PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
//using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERwinApiWrapper
{
    public class ERwinApiWrapper
    {

        #region constructors
        public ERwinApiWrapper()
        {
            if (ScapiApplication == null)
            {
                ScapiApplication = new SCAPI.Application();
            }
        }

        public ERwinApiWrapper(ModelMartConfiguration configuration) : this()
        {
            ModelMartConnection = new ModelMartConnection(AppWrapper, configuration);
        }
        #endregion

        /// <summary>
        /// The underlying Script Client Application class from the ERwin API
        /// </summary>
        public SCAPI.Application ScapiApplication { get; set; }


        public ModelMartConnection ModelMartConnection { get; set; }


        private Session _ErwinSession;

        public AppWrapper AppWrapper
        {
            get
            {
                return new AppWrapper(ScapiApplication);
            }
        }

        
        /// <summary>
        /// Creates a new model
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Model AddNewModel(string path)
        {
            //if (name.IsNullOrEmptyString()) throw new Exception("Name can't be empty");
            Model m = new Model(path, AppWrapper);
            _ErwinSession = ScapiApplication.Sessions.Add();
            m.Create();
            return m;
        }


        /// <summary>
        /// Opens a Model
        /// </summary>
        /// <param name="path">Path to the *.erwin file. For example: C:\Models\Model.erwin</param>
        /// <returns></returns>
        public Model OpenModel(string path)
        {
            Model m = new Model(path, AppWrapper);
            _ErwinSession = ScapiApplication.Sessions.Add();

            m.Open(this);
            return m;
        }

    }
}
