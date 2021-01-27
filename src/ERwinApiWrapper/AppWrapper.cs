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
    public class AppWrapper
    {
        Application _app;

        public AppWrapper(Application app)
        {
            _app = app;
        }

        public Application App
        {
            get
            {
                return _app;
            }
        }
    }
}
