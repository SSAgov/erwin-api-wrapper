using Newtonsoft.Json.Linq;
using SCAPI;
using PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERwinApiWrapper
{
    public static class ModelObjectExtensions
    {
        public static Dictionary<string, string> GetProperties(this ModelObject mo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            ModelProperties props = mo.CollectProperties();
            foreach (ModelProperty prop in props)
            {
                dict.Add(prop.ClassName, prop.FormatAsString());
            }
            return dict;
        }

        public static string NzPropertyString(this ModelObject mo, string propertyName)
        {
            ModelProperties mprops = mo.CollectProperties();
            if (mprops.HasProperty(propertyName))
            {
                return mprops[propertyName].FormatAsString();
            }
            else
            {
                return "";
            }
        }
        public static string NzPropertyValue(this ModelObject mo, string propertyName)
        {
            ModelProperties mprops = mo.CollectProperties();
            if (mprops.HasProperty(propertyName))
            {
                return mprops[propertyName].ToString();
            }
            else
            {
                return "";
            }
        }
        public static int NzPropertyInteger(this ModelObject mo, string propertyName)
        {
            ModelProperties mprops = mo.CollectProperties();
            if (mprops.HasProperty(propertyName))
            {
                return int.Parse(mprops[propertyName].FormatAsString());
            }
            else
            {
                return 0;
            }
        }



        public static bool GetBoolProperty(this ModelObject modelObject, string propertyName)
        {
            if (modelObject.Properties.HasProperty(propertyName))
            {
                try
                {
                    if (modelObject.Properties[propertyName].FormatAsString().ToUpper() == "TRUE")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                catch (Exception)
                {

                }
            }
            return false;
        }

        private static IEnumerable<string> GetPropertyNameStringArray(IEnumerable<string> props)
        {
            List<string> properties = new List<string>();
            foreach (var property in props)
            {
                var fields = property.Split('.');
                string propertyName = fields[0];
                properties.Add(propertyName);
            }
            return properties.Distinct();
        }

        public static DataTable ModelObjectsToDataTable(SCAPI.ModelObjects mObjects, string collectionName, string tableName = "")
        {
            DataTable dt = new DataTable();
            DataView dv = new DataView();
            if (tableName == "")
            {
                tableName = "ERwin " + collectionName;
            }

            //Build Datatable Collection for Logical/Physical Elements

            switch (collectionName)
            {
                case "Attribute":
                case "Columns":
                    dt.Columns.Add("ENTITY_NAME");
                    dt.Columns.Add("TABLE_NAME");
                    dt.Columns.Add("ATTRIBUTE_NAME");
                    dt.Columns.Add("COLUMN_NAME");
                    dt.Columns.Add("DATATYPE_FAMILY");
                    dt.Columns.Add("LOGICAL_DATATYPE_NAME");
                    dt.Columns.Add("DATATYPE_NAME");
                    dt.Columns.Add("MAX_CHARACTERS", typeof(Int32));
                    dt.Columns.Add("LOGICAL_MAX_CHARACTERS", typeof(Int32));
                    dt.Columns.Add("IS_NULLABLE");
                    dt.Columns.Add("MAX_BYTES", typeof(Int32));
                    dt.Columns.Add("COLUMN_DEFAULT");
                    dt.Columns.Add("REMARKS");
                    dt.SetBestGuessPrimaryKey();

                    foreach (SCAPI.ModelObject attributeObject in mObjects)
                    {
                        ModelObject entityObject = attributeObject.Context;
                        ModelProperties entityProperties = entityObject.CollectProperties();
                        ModelProperties attributeProperties = attributeObject.CollectProperties();

                        //if (entityProperties["Is_Logical_Only"].FormatAsString() != "true")
                        //{

                        //if (attributeProperties["Is_Logical_Only"].FormatAsString() != "true")
                        //{
                        string sTemp = "";
                        DataRow dr;
                        dr = dt.NewRow();
                        dr["ENTITY_NAME"] = attributeObject.Context.Name.ToString();
                        dr["ATTRIBUTE_NAME"] = attributeObject.Name.ToString();

                        if (entityProperties.HasProperty("Physical_Name"))
                        {
                            dr["TABLE_NAME"] = entityProperties["Physical_Name"].FormatAsString();
                        }

                        if (attributeProperties.HasProperty("Physical_Name"))
                        {
                            dr["COLUMN_NAME"] = attributeProperties["Physical_Name"].FormatAsString();
                        }

                        if (attributeProperties.HasProperty("Physical_Data_Type"))
                        {
                            sTemp = attributeProperties["Physical_Data_Type"].FormatAsString();
                            dr["DATATYPE_NAME"] = "";// StringHelper.RemoveDatatypeLength(sTemp.ExtractNumberFromString());
                            dr["MAX_CHARACTERS"] = sTemp.ExtractNumberFromString();
                        }
                        if (attributeProperties.HasProperty("Logical_Data_Type"))
                        {
                            sTemp = attributeProperties["Logical_Data_Type"].FormatAsString();
                            dr["LOGICAL_DATATYPE_NAME"] = "";//StringHelper.RemoveDatatypeLength(sTemp);
                            dr["LOGICAL_MAX_CHARACTERS"] = sTemp.ExtractNumberFromString();

                        }
                        if (attributeProperties.HasProperty("Null_Option_Type"))
                        {
                            switch (attributeProperties["Null_Option_Type"].FormatAsString().ToUpper())
                            {
                                case "NULL":
                                    sTemp = "YES";
                                    break;
                                default:
                                    sTemp = "NO";
                                    break;
                            }
                            dr["IS_NULLABLE"] = sTemp;
                        }
                        if (attributeProperties.HasProperty("Comments"))
                        {
                            dr["REMARKS"] = attributeProperties["Comments"].FormatAsString();
                        }
                        try
                        {
                            dt.Rows.Add(dr);
                        }
                        catch (Exception ex)
                        {
                            //throw ex;
                        }
                    }

                    dv = dt.DefaultView;
                    dv.Sort = "[Entity_Name]";
                    dt = dv.ToTable();
                    DataColumn[] keys = new DataColumn[2];
                    keys[0] = dt.Columns["TABLE_NAME"];
                    keys[1] = dt.Columns["COLUMN_NAME"];
                    dt.PrimaryKey = keys;

                    break;
                default:
                    break;
            }

            //Remove Columns based on collection type


            dt.TableName = tableName;
            return dt;
        }


    }
}
