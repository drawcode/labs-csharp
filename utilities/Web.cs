using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Labs.Utility {

    public class Web {

        public static bool logEnabled = false;

        public static void Log(string key, object data) {
            Log(key, data, false);
        }

        public static void Log(string key, object data, bool tojson) {
            if (!logEnabled)
                return;

            HttpContext.Current.Response.Write("\r\n\r\n<br><br>" + key + "<br>\r\n");

            string result = JsonConvert.SerializeObject(data, Formatting.Indented);

            HttpContext.Current.Response.Write(data);
        }

        public static void Stop() {
            Stop(false);
        }

        public static void Stop(bool force) {
            if (!logEnabled && !force)
                return;

            HttpContext.Current.Response.End();
        }

        public static bool WriteFileResponse(string path, bool endResponse, string contentType) {
            if (File.Exists(path)) {
                HttpContext.Current.Response.ContentType = contentType;
                HttpContext.Current.Response.WriteFile(path);
                HttpContext.Current.Response.End();
                return true;
            }
            return false;
        }

        public static string GetParamValue(string key) {
            // check params in order

            string typeUrlParam = Paths.GetPathParamValue(
                HttpContext.Current.Request.RawUrl, key);

            string typeForm = HttpContext.Current.Request.Form[key];

            string typeQuery = HttpContext.Current.Request.QueryString[key];

            if (!string.IsNullOrEmpty(typeUrlParam)) {
                return typeUrlParam;
            }
            else if (!string.IsNullOrEmpty(typeQuery)) {
                return typeQuery;
            }
            else if (!string.IsNullOrEmpty(typeForm)) {
                return typeForm;
            }

            return null;
        }

        public static string GetRequest(string url) {

            Web.Log("WebData::GetRequest:url: ", url);

            try {
                System.Net.HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Credentials = CredentialCache.DefaultCredentials;
                WebResponse res = req.GetResponse();
                Stream stream = res.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string content = sr.ReadToEnd();
                sr.Close();
                stream.Close();
                Web.Log(url, content);

                return content;
            }
            catch (Exception e) {

                Web.Log("WebData::GetRequest:e: ", e.Message + e.ToString());
            }
            finally {
            }
            return null;
        }


        public static void GetRequestAsync(string url) {

            Web.Log("WebData::GetRequestAsync:url: ", url);

            try {
                System.Net.HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Credentials = CredentialCache.DefaultCredentials;
                req.BeginGetResponse(null, null);
            }
            catch (Exception e) {

                Web.Log("WebData::GetRequestAsync:e: ", e.Message + e.ToString());
            }
            finally {
            }
        }

        public static void SaveFile(string url, string path) {

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect) &&
                    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) {

                    // if the remote file was found, download oit
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.OpenWrite(path)) {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                    }
                }
            }
            catch (Exception e) {
                Web.Log("WebData::WriteFile:e: ", url + " " + e.Message + e.ToString());
            }
            finally {

            }
        }

        private async Task<byte[]> GetURLContentsAsync(string url) {
            // The downloaded resource ends up in the variable named content. 
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL. 
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for 
            // the response.                 
            using (WebResponse response = await webReq.GetResponseAsync())

            // The previous statement abbreviates the following two statements. 

            //Task<WebResponse> responseTask = webReq.GetResponseAsync(); 
            //using (WebResponse response = await responseTask)
            {
                // Get the data stream that is associated with the specified url. 
                using (Stream responseStream = response.GetResponseStream()) {
                    // Read the bytes in responseStream and copy them to content. 
                    await responseStream.CopyToAsync(content);

                    // The previous statement abbreviates the following two statements. 

                    // CopyToAsync returns a Task, not a Task<T>. 
                    //Task copyTask = responseStream.CopyToAsync(content); 

                    // When copyTask is completed, content contains a copy of 
                    // responseStream. 
                    //await copyTask;
                }
            }
            // Return the result as a byte array. 
            return content.ToArray();
        }
    }
}
