using System;
using System.Collections.Generic;
using SCAPI;
using PrimitiveExtensions;
using System.Data;

namespace ERwinApiWrapper
{
    public class Table
    {
        public Table(Model m)
        {
            Model = m;

        }

        public Table(Model m, ModelObject mo)
        {
            Model = m;
            ModelObject = mo;
        }

        public Model Model { get; private set; }

        public ModelObject ModelObject { get; private set; }
        public string ERwinObjectID
        {
            get
            {
                return ModelObject.ObjectId;
            }
        }

        //TODO extract these out
        public bool IsLogical { get; set; }
        public bool IsPhysical { get; set; }

        public string LogicalName
        {
            get
            {
                return ModelObject.NzPropertyString("Name");
            }

            set
            {
                ModelObject.Properties["Name"].Value = value;
            }
        }
        public string LogicalDefinition
        {
            get
            {
                return ModelObject.NzPropertyString("Definition");
            }

            set
            {
                ModelObject.Properties["Definition"].Value = value;
            }
        }

        public string PhysicalName

        {
            get
            {
                return ModelObject.NzPropertyString("Physical_Name");
            }

            set
            {
                ModelObject.Properties["Physical_Name"].Value = value;
            }
        }


        public string PhysicalDefinition
        {
            get
            {
                return ModelObject.NzPropertyString("Comment");
            }

            set
            {
                ModelObject.Properties["Comment"].Value = value;
            }
        }
        public bool IsPhysicalNameDerived
        {
            get
            {
                return ModelObject.Properties.NzPropertyValue("Physical_Name").ToUpper() == "%ENTITYNAME";
            }
        }

        public List<Column> GetColumns()
        {
            List<Column> columns = new List<Column>();
            foreach (ModelObject mo in Model.ModelObjects.Collect(ERwinObjectID, "Attribute"))
            {
                columns.Add(new Column(this, mo));
            }
            return columns;
        }

        public Column AddNewColumn()
        {
            return new Column(this, Model.ModelObjects.Collect(ERwinObjectID).Add("Attribute"));
        }


    }
}
