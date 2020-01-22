using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Adapter;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Adapter.CarbonQueryDatabase
{
    public partial class CarbonQueryDatabaseAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public fields                             ****/
        /***************************************************/

        public static string AdapterID = "CarbonQueryDatabaseAdapter";

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Description goes here. Description goes here.")]
        [Input("username", "Provide EC3 Username")]
        [Input("password", "Provide EC3 Password")]
        [Output("adapter", "adapter results")]

        public CarbonQueryDatabaseAdapter(string username = "", string password = "")
        {
            //Insert login auth request and return token to m_bearerKey

            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;
        }

        /***************************************************/
        /*** Private Fields                              ***/
        /***************************************************/

        private static string m_bearerKey = null;
    }
}