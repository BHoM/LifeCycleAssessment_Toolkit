using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.LifeCycleAnalysis
{
    public static partial class Compute
    {
        [Description("Returns a bearer token for the CarbonQueryDatabase system from the provided username and password")]
        [Input("username", "Your username for the system")]
        [Input("password", "Your password for the system - case sensitive, do not share scripts with this saved")]
        [Input("apiAddress", "The API address to connect to, default to ETL-API")]
        [Output("bearerToken", "The bearer token to use the database system or the full response string if there was an error")]
        public static string BearerToken(string username, string password, string apiAddress = "https://etl-api.cqd.io/api/rest-auth/login")
        {
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;

            HttpClient client = new HttpClient();

            //Add headers per api auth requirements
            client.DefaultRequestHeaders.Add("accept", "application/json");

            //Post login auth request and return token to m_bearerKey
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiAddress);

            string loginString = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
            request.Content = new StringContent(loginString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.SendAsync(request).Result;

            string responseAuthString = response.Content.ReadAsStringAsync().Result;
            if(responseAuthString.Split('"').Length >= 3)
                return responseAuthString.Split('"')[3];
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("We did not receive the response we expected. Returned is the full response result for you to manually examine and extract a bearer key from");
                return responseAuthString;
            }
        }
    }
}
