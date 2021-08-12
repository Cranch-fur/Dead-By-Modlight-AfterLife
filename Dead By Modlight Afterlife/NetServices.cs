using System.IO;
using System.Net;

namespace Dead_By_Modlight_Afterlife
{
    class NetServices
    {
        public static string REQUEST_GET(string URL, string bhvrSession, string user_agent)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.UserAgent = user_agent;
                request.Headers.Add("Cookie", "bhvrSession=" + bhvrSession);
                request.Headers.Add("x-kraken-client-platform", "steam");
                request.Headers.Add("x-kraken-client-version", "5.1.0");
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        return "ERROR " + ((int)response.StatusCode).ToString();
                    } else {
                        return "ERROR"; }

                } 
                else 
                { 
                    return "ERROR";
                }
            }
        }
    }
}
