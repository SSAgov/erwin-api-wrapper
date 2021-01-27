using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERwinApiWrapper
{
    public class ModelMartConfiguration
    {
        public ModelMartConfiguration(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public ModelMartConfiguration(string serverUrl, string userName, string password)
        {
            ServerUrl = serverUrl;
            Username = userName;
            Password = password;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ServerUrl { get; set; }

        /// <summary>
        /// Default Port of 18170 is set
        /// </summary>
        public string ServerPort { get; set; } = "18170";

        /// <summary>
        /// Default ServiceName of MartServer is set
        /// </summary>
        public string ServiceName { get; set; } = "MartServer";

        /// <summary>
        /// Default MartName is set to Mart
        /// </summary>
        public string MartName { get; set; } = "Mart";

        /// <summary>
        /// Specifies if this mart uses SSL
        /// </summary>
        public bool IsSSL { get; set; } = false;

        /// <summary>
        /// Specifies if this mart uses IIS
        /// </summary>
        public bool IsIIS { get; set; } = false;


        /// <summary>
        /// TBD: Don't know what this means
        /// </summary>
        public bool IsTRC { get; set; } = false;

        public string ConnectionString
        {
            get
            {
                string cnString = "";
                cnString += "SRV=" + ServerUrl;
                cnString += IsSSL ? ";SSL=YES" : ";SSL=NO";
                cnString += IsIIS ? ";IIS=YES" : ";IIS=NO";
                cnString += IsTRC ? ";TRC=YES" : ";TRC=NO";
                cnString += ";UID=" + Username;
                cnString += ";PSW=" + Password;
                cnString += ";ASR=" + ServiceName;
                cnString += ";PRT=" + ServerPort;
                return cnString;
            }
        }

        
        public string MartLocator
        {
            get
            {
                return string.Format("mart://{0}?", MartName);
            }
        }


    }
}
