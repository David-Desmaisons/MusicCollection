using MusicCollection.Fundation;
using MusicCollection.ToolBox.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using MusicCollection.ToolBox;
using System.Diagnostics;

namespace MusicCollection.WebServices.Rovi
{
    internal class RoviClient
    {
        private static string _SearchUrl = "http://api.rovicorp.com/search/v2.1/music/search?";
        private Dictionary<string,string> _Atributes = new Dictionary<string,string>();


        private string _Apikey;
        private string _SharedKey;
        internal RoviClient(string apikey, string sharedkey)
        {
            _Atributes.Add("apikey",apikey);
            _Atributes.Add("offset","0");
            _Atributes.Add("language","en");
            _Atributes.Add("country","US");
            _Atributes.Add("format","json");
            _Atributes.Add("size", "20");
            _SharedKey = sharedkey;
            _Apikey = apikey;
        }

        public RoviClient SearchAlbum(IAlbumDescriptor iIAlbumDescriptor)
        {
            _Atributes.Add("query", 
                string.Format("{0}+{1}",iIAlbumDescriptor.Name,iIAlbumDescriptor.Artist).Trim().Replace(' ','+') );
            _Atributes.Add("entitytype","album");
            _Atributes.Add("include", "images,tracks");
            return this;
        }
 
        public RoviClient Limit(int iLimit)
        {
            if (iLimit > 0)
                _Atributes["size"] = iLimit.ToString();
            return this;
        }

        private IHttpWebRequest BuildRequest()
        {
            IHttpWebRequest request = InternetProvider.InternetHelper.CreateHttpRequest(BuildUrl());
            request.Method = "GET";
            request.Accept = "application/json";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Headers.Add("accept-encoding", "gzip,deflate");
            return request;      
        }

        //A calculated, 32-hex-digit authorization code. To perform the calculation, execute the MD5 function on the concatenation of the following three ASCII strings:
        //    Your API key.
        //    The secret key you received with your API key.
        //    The Unix time. Unix time is a timestamp supported in most development environments, and is generally defined as the number of seconds since January 1, 1970 00:00:00 GMT.
        //Express the alpha hex digits as lower case.
        //Perform the calculation at the time of each request to be sure it's within a five-minute window of the server time. If you're testing the call in a browser, use our online signature generator to perform the calculation.
        private string ComputeSig()
        {
            return MD5Computer.ComputeMD5(
                                string.Format("{0}{1}{2}",_Apikey,_SharedKey,DateTimeExtension.NowUtcUnixTime()))
                                .ToLower();
        }

        private string BuildUrl()
        {
            StringBuilder sb = new StringBuilder(_SearchUrl);
            _Atributes.Add("sig",ComputeSig());
            sb.Append(string.Join("&",_Atributes.Select(kvp=>string.Format("{0}={1}",kvp.Key,kvp.Value))));
            return sb.ToString();
        }
       
        internal dynamic GetAnswer()
        {
            try
            { 
                return new HttpJsonInterpretor(BuildRequest()).GetObjectResponse();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("Problem during Rovi request {0}",ex));
                return null;
            }
        }
    }
}
