using SCAPI;

namespace ERwinApiWrapper
{
    public class Column
    {
        public Column(Table t)
        {
            Table = t;
        }

        public Column(Table t, ModelObject mo)
        {
            Table = t;
            ModelObject = mo;
        }


        public Table Table {get; private set;}

        public ModelObject ModelObject { get; private set; }

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

        public string LogicalDatatype
        {
            get
            {
                return ModelObject.NzPropertyString("Logical_Data_Type");
            }

            set
            {
                ModelObject.Properties["Logical_Data_Type"].Value = value;
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


        public string PhysicalDatatype
        {
            get
            {
                return ModelObject.NzPropertyString("Physical_Data_Type");
            }

            set
            {
                ModelObject.Properties["Physical_Data_Type"].Value = value;
            }
        }


    }
}