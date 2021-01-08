using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CargaInicial
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string pathName in new string[] { "f", "hf", "ff" })
            {
                string[] files = System.IO.Directory.GetFiles("c:\\nff\\"+pathName+"\\");
                foreach (string filePath in files)
                {
                    
                    Dictionary<string, string> postParameters = new Dictionary<string, string>();
                    postParameters.Add("userToken", System.Configuration.ConfigurationManager.AppSettings["userToken"]);
                    postParameters.Add("description", "");
                    postParameters.Add("category", pathName == "f" ? "0" : pathName == "hf" ? "2" : "3");
                    try
                    {
                        SendFile("https://nff.construcodeapp.com/Post/CreatePost", postParameters, filePath, "https://www.nudesforfree.com/", keepAlive: true);                    
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    catch (Exception ex){ Console.WriteLine(ex.Message); }
                }
            }
        }

        public static void SendFile(string url,Dictionary<string,string> postParameters,string file, string referrer = null, string method = "POST", string contentType = "application/x-www-form-urlencoded", bool keepAlive = false)
        {           
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.ContinueTimeout = 9999999;
            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in postParameters.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, postParameters[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                
            }
            catch (Exception ex)
            {
                
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

    }
}
