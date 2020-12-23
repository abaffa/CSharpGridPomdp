using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;

namespace System.Net
{
    public class CookieAwareWebClient : WebClient
    {
        private static byte[] DecompressGzip(Stream streamInput)
        {
            Stream streamOutput = new MemoryStream();
            int iOutputLength = 0;
            try
            {
                byte[] readBuffer = new byte[4096];

                /// read from input stream and write to gzip stream

                using (GZipStream streamGZip = new GZipStream(streamInput, CompressionMode.Decompress))
                {

                    int i;
                    while ((i = streamGZip.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        streamOutput.Write(readBuffer, 0, i);
                        iOutputLength = iOutputLength + i;
                    }
                }
            }
            catch (Exception ex)
            {
                // todo: handle exception
            }

            /// read uncompressed data from output stream into a byte array

            byte[] buffer = new byte[iOutputLength];
            streamOutput.Position = 0;
            streamOutput.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        private static byte[] DecompressDeflate(Stream streamInput)
        {
            Stream streamOutput = new MemoryStream();
            int iOutputLength = 0;
            try
            {
                byte[] readBuffer = new byte[4096];

                /// read from input stream and write to gzip stream

                using (DeflateStream streamGZip = new DeflateStream(streamInput, CompressionMode.Decompress))
                {

                    int i;
                    while ((i = streamGZip.Read(readBuffer, 0, readBuffer.Length)) != 0)
                    {
                        streamOutput.Write(readBuffer, 0, i);
                        iOutputLength = iOutputLength + i;
                    }
                }
            }
            catch (Exception ex)
            {
                // todo: handle exception
            }

            /// read uncompressed data from output stream into a byte array

            byte[] buffer = new byte[iOutputLength];
            streamOutput.Position = 0;
            streamOutput.Read(buffer, 0, buffer.Length);

            return buffer;
        }



        public string ConvertNameValueCollection(NameValueCollection loginData)
        {
            string ret = "";

            foreach (String key in loginData.Keys)
            {
                ret += "&" + key + "=" + loginData[key];
            }

            return ret.Trim('&');

        }

        private void sendPost(HttpWebRequest request, NameValueCollection loginData)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var buffer = Encoding.ASCII.GetBytes(ConvertNameValueCollection(loginData));
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();
        }

        private void sendGet(HttpWebRequest request)
        {
            request.Method = "GET";
            request.ContentType = "text/html";
        }

        public String Login(string loginPageAddress, NameValueCollection loginData)
        {
            string result = "";

            CookieContainer container;

            //var request = (HttpWebRequest)WebRequest.Create(loginPageAddress);
            var request = (HttpWebRequest)GetWebRequest(new Uri(loginPageAddress, UriKind.Absolute));

            sendPost(request, loginData);

            container = request.CookieContainer = new CookieContainer();

            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            response.Close();

            CookieContainer = container;
            /*
            CookieCollection c = container.GetCookies(request.RequestUri);
            foreach (Cookie c2 in c)
            {
                Console.WriteLine(c2.ToString());
            }
             * */

            return result;
        }

        public CookieAwareWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        public CookieAwareWebClient()
            : this(new CookieContainer())
        { }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);

            //WebProxy proxyObject = new WebProxy("http://127.0.0.1:8888/");
            //request.Proxy = proxyObject;

            request.CookieContainer = CookieContainer;

            WebHeaderCollection myWebHeaderCollection = request.Headers;
            myWebHeaderCollection.Add("Cache-Control", "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            myWebHeaderCollection.Add("Accept-Encoding", "gzip, deflate");
            myWebHeaderCollection.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";

            return request;
        }

        public String RequestPost(string loginPageAddress, NameValueCollection loginData)
        {
            string result = "";

            var request = (HttpWebRequest)GetWebRequest(new Uri(loginPageAddress, UriKind.Absolute));

            sendPost(request, loginData);

            CookieContainer container = request.CookieContainer;

            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            response.Close();

            CookieContainer = container;

            /*
            CookieCollection c = container.GetCookies(request.RequestUri);
            foreach (Cookie c2 in c)
            {
                Console.WriteLine(c2.ToString());
            }
             */

            return result;
        }

        public String RequestGet(string loginPageAddress)
        {
            string result = "";

            HttpWebRequest request = (HttpWebRequest)GetWebRequest(new Uri(loginPageAddress, UriKind.Absolute));

            sendGet(request);

            CookieContainer container = request.CookieContainer;

            WebResponse response = request.GetResponse();

            if (response.Headers.AllKeys.Contains("Content-Encoding") && response.Headers["Content-Encoding"].Contains("gzip"))
            {

                byte[] b = DecompressGzip(response.GetResponseStream());
                result = System.Text.Encoding.GetEncoding(((HttpWebResponse)response).CharacterSet).GetString(b);
            }
            else if (response.Headers.AllKeys.Contains("Content-Encoding") && response.Headers["Content-Encoding"].Contains("deflate"))
            {
                byte[] b = DecompressDeflate(response.GetResponseStream());
                result = System.Text.Encoding.GetEncoding(((HttpWebResponse)response).CharacterSet).GetString(b);
            }
            else
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
            response.Close();

            CookieContainer = container;

            /*
            CookieCollection c = container.GetCookies(request.RequestUri);
            foreach (Cookie c2 in c)
            {
                Console.WriteLine(c2.ToString());
            }
             */

            return result;
        }
    }
}