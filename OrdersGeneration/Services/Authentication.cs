using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGeneration.Services
{
    public class Authentication
    {
        public static HttpWebRequest CreateAuthorization()
        {
            var url = "https://api.baselinker.com/connector.php";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Headers["X-BLToken"] = "3005808-3025752-RYXG71QNXRS42JN9Q80ZTLSVA07X3ACKFFINPHFMZHH63A2H20CFFQC5E2R1QEZ5";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            return httpRequest;
        }

        public static HttpWebResponse GetResponse(string method)
        {
            var httpRequest = CreateAuthorization();

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(method);
            }
            var response = (HttpWebResponse)httpRequest.GetResponse();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
                return response;
            else
                return null;
        }
    }
}
