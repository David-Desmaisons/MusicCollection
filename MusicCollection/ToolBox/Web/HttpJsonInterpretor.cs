using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace MusicCollection.ToolBox.Web
{
    internal class HttpJsonInterpretor
    {
        private IHttpWebRequest _Request;

        internal HttpJsonInterpretor(IHttpWebRequest request)
        {
            if (request == null)
                throw new ArgumentNullException();

            _Request = request;

        }

        //internal HttpJsonInterpretor(HttpWebRequest request):this(request.ToInterface())
        //{
        //}

        private IHttpWebResponse GetResponse(IInternetServiceListener Listener)
        {
            try
            {
                return _Request.GetResponse();
            }
            catch (WebException e)
            {
                if (Listener != null)
                    Listener.OnWebExeption(e);

                Trace.WriteLine(e);
                return null;
            }
        }

        internal dynamic GetObjectResponse(IInternetServiceListener Listener = null)
        {
            IHttpWebResponse response = GetResponse(Listener);
            if (response == null)
                return null;

            HttpStatusCode sc = response.StatusCode;

            if (sc != HttpStatusCode.OK)
            {
                if (Listener != null)
                    Listener.OnStatusCodeKO(sc);

                return null;
            }

            Stream readStream = response.GetResponseStream();
            if (readStream == null)
            {
                if (Listener != null)
                    Listener.OnUnExpectedUnreadableResult();

                return null;
            }

            Stream toberead = null;

            if (response.ContentEncoding == "gzip")
            {
                toberead = new MemoryStream();

                using (Stream unzip = new System.IO.Compression.GZipStream(readStream, System.IO.Compression.CompressionMode.Decompress))
                {
                    unzip.CopyTo(toberead); 
                }

                toberead.Position = 0;
                readStream.Dispose();
            }
            else
                toberead = readStream;


            string sr = null;
            using (toberead)
            {
                using (StreamReader reader = new StreamReader(toberead))
                {
                    sr = reader.ReadToEnd();
                }

            }


            return DynamicJsonConverter.DynamicDeSerialize(sr);
        }

    }
}
