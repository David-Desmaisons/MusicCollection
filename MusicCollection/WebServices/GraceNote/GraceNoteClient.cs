using MusicCollection.ToolBox.Web;
using MusicCollection.WebServices.GraceNote.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MusicCollection.WebServices.GraceNote
{
    class GraceNoteClient
    {
        private string _GraceNoteID;
        private readonly bool _useGzip;
        private Uri _Uri;

        public GraceNoteClient(string iGraceNoteID, bool useGzip=true)
        {
            _GraceNoteID = iGraceNoteID;
            _useGzip = useGzip;
            _Uri = new System.Uri(GetClientEndpointUrl(_GraceNoteID));
        }

        private string GetClientEndpointUrl(string clientId)
        {
            string replacement = Regex.Match(clientId, @"(\d*)-.*").Groups[1].ToString();
            return string.Format("https://c{0}.web.cddbp.net/webapi/xml/1.0/", replacement);
        }

        private IHttpWebRequest CreateHttpRequest()
        {
            IHttpWebRequest request = InternetProvider.InternetHelper.CreateHttpRequest(_Uri);
            request.Method = "POST";
            request.ContentType = "text/xml";
            if (this._useGzip)
            {
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Headers.Add("accept-encoding", "gzip");
            }
            return request;
        }


        public Responses Post(Queries queries)
        {
            Responses responses = null;
            IHttpWebRequest request = this.CreateHttpRequest();
            using (Stream requestStream = request.GetRequestStream())
            {
                queries.Serialize(requestStream);

                IHttpWebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    { 
                        response.Close(); 
                        return new Responses();
                    }

                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string xml = reader.ReadToEnd();
                        responses = Responses.Deserialize(xml);
                    }

                    response.Close();
                }
            }
           
            return responses;
        }

    }
}
