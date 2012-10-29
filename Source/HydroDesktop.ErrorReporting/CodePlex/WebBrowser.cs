using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HydroDesktop.ErrorReporting.CodePlex
{
    internal class WebBrowser
    {
        private readonly string _userAgent = string.Empty;
        private CookieCollection m_cookies = new CookieCollection();

        static WebBrowser()
        {
            ServicePointManager.Expect100Continue = false;
        }
      
        public WebBrowser(string userAgent)
        {
            _userAgent = userAgent;
        }

        public string Get(string url, HtmlForm form = null, string referer = null)
        {
            return Send(url, "GET", form, referer);
        }

        public string Post(string url, HtmlForm form, string referer = null, bool isMultiPart = false)
        {
            return Send(url, "POST", form, referer, isMultiPart);
        }

        private string Send(string url, string method, HtmlForm form, string referer, bool isMultiPart = false)
        {
            if (form != null && string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
            {
                url = url + "?" + form;
            }
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = _userAgent;
            httpWebRequest.Accept = "*/*";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.CookieContainer = new CookieContainer();
            if (m_cookies.Count > 0)
            {
                httpWebRequest.CookieContainer.Add(m_cookies);
            }
            httpWebRequest.Method = method;
            if (referer != null)
            {
                httpWebRequest.Referer = referer;
            }
            if (form != null && string.Equals(httpWebRequest.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes;
                if (!isMultiPart)
                {
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    var text = form.ToString();
                    bytes = Encoding.UTF8.GetBytes(text);
                }
                else
                {
                    var formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                    var contentType = "multipart/form-data; boundary=" + formDataBoundary;
                    httpWebRequest.ContentType = contentType;
                    bytes = GetMultipartFormData(form.Elements, formDataBoundary);
                }

                httpWebRequest.ContentLength = bytes.Length;

                // Send the form data to the request.
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
            }
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var text2 = GetText(httpWebResponse.GetResponseStream());
            if (httpWebResponse.Cookies.Count > 0)
            {
                m_cookies.Add(httpWebResponse.Cookies);
            }
            httpWebResponse.Close();
            return text2;
        }

        private static string GetText(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private static byte[] GetMultipartFormData(IEnumerable<HtmlElement> postParameters, string boundary)
        {
            var encoding = Encoding.UTF8;
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (param.Name == null) continue;

                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.IsFile)
                {
                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header =
                        string.Format(
                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                            boundary,
                            param.Name,
                            param.FileName ?? param.Name,
                            param.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    if (param.File != null)
                    {
                        formDataStream.Write(param.File, 0, param.File.Length);
                    }
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                                                    boundary,
                                                    param.Name,
                                                    param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
