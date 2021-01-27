using Newtonsoft.Json.Linq;
using SCAPI;

namespace ERwinApiWrapper
{
    public static class ModelPropertiesExtensions
    {
        public static string NzPropertyString(this ModelProperties mprops, string propertyName)
        {
            if (mprops.HasProperty(propertyName))
            {
                return mprops[propertyName].FormatAsString();
            }
            else
            {
                return "";
            }
        }
        public static string NzPropertyValue(this ModelProperties mprops, string propertyName)
        {
            if (mprops.HasProperty(propertyName))
            {
                return mprops[propertyName].ToString();
            }
            else
            {
                return "";
            }
        }

     

        public static int NzPropertyInteger(this ModelProperties mprops, string propertyName)
        {
            if (mprops.HasProperty(propertyName))
            {
                return int.Parse(mprops[propertyName].FormatAsString());
            }
            else
            {
                return 0;
            }
        }

    }
}
