using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace CodeGetTool
{
    class Http
    {
        public static string DoGet(string url, string strParams, CookieContainer cookie)
        {
            string strRet = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (strParams == "" ? "" : "?" + strParams));
            request.Method = "GET";
            request.ContentType = "text/html; charset=UTF-8";
            request.CookieContainer = cookie;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader strReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                strRet = strReader.ReadToEnd();
                strReader.Close();
                response.Close();
            }
            catch(Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("error.txt", true, Encoding.UTF8))
                {
                    sw.Write(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff] ") + "Http.DoGet Exception:" + ex.Message + "\r\n");
                }
            }
            finally
            {
                request = null;
            }

            return strRet;
        }

        public static string DoPost(string url, string strParams, ref CookieContainer cookie)
        {
            string strRet = null;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            try
            {
                byte[] postData = Encoding.UTF8.GetBytes(strParams);
                request.ContentLength = postData.Length;
                request.CookieContainer = cookie;
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                reqStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                StreamReader strReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                strRet = strReader.ReadToEnd();
                strReader.Close();
                response.Close();
            }
            catch(Exception ex)
            {
                using (StreamWriter sw = new StreamWriter("error.txt", true, Encoding.UTF8))
                {
                    sw.Write(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff] ") + "Http.DoPost Exception:" + ex.Message + "\r\n");
                }
            }
            finally
            {
                request = null;
            }

            return strRet;
        }

        public static void GetOpenCode()
        {
            WebBrowser web = new WebBrowser();
            web.Navigate("http://www.opencai.net/open/");   
            web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler( web_DocumentCompleted ); 
        }
        public static void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)   
        {   
            WebBrowser web = (WebBrowser)sender;   
            // HtmlElementCollection ElementCollection = web.Document.GetElementsByTagName("table");

            HtmlElementCollection ElementCollection = web.Document.All;

            foreach (HtmlElement item in ElementCollection)   
            {   
                 File.AppendAllText("Kaijiang_xj.txt", item.InnerText);   
            }   
        }  

    }
}
