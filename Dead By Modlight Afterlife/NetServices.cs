using System.IO;
using System.Net;

namespace Dead_By_Modlight_Afterlife
{
    class NetServices
    {
        public static string REQUEST_GET(string URL, string variable)
        {
            try
            {
                if (variable != "")
                    URL = URL + $"?{variable}";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.UserAgent = "Dead By Modlight AfterLife";
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch { return "ERROR"; }
        }
    }
}
