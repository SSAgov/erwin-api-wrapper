using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERwinApiWrapper
{
    public static class ModelDirectoryExtensions
    {
        public static Model ToModel(this SCAPI.ModelDirectoryUnit moDirUnit, Library parent = null)
        {
            Model m = new Model(moDirUnit.Locator, parent, moDirUnit.Name);
            return m;
        }
    }
}
