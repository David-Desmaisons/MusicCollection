#region COPYRIGHT (c) 2004 by Brian Weeres
/* Copyright (c) 2004 by Brian Weeres
 * 
 * Email: bweeres@protegra.com; bweeres@hotmail.com
 * 
 * Permission to use, copy, modify, and distribute this software for any
 * purpose is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * If you modify it then please indicate so. 
 *
 * The software is provided "AS IS" and there are no warranties or implied warranties.
 * In no event shall Brian Weeres and/or Protegra Technology Group be liable for any special, 
 * direct, indirect, or consequential damages or any damages whatsoever resulting for any reason 
 * out of the use or performance of this software
 * 
 */
#endregion
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections.Generic;

//using MusicCollection.Fundation;
using MusicCollection.DataExchange;
using MusicCollection.ToolBox.Web;

namespace MusicCollection.WebServices.Freedb
{
    /// <summary>
    /// Summary description for FreedbHelper.
    /// </summary>
    public class FreedbHelper
    {
        //public const string MAIN_FREEDB_ADDRESS = "freedb.freedb.org";
        private const string MAIN_FREEDB_ADDRESS = "us.freedb.org:80";


        public const string DEFAULT_ADDITIONAL_URL_INFO = "/~cddb/cddb.cgi";
        private Site m_mainSite = new Site(MAIN_FREEDB_ADDRESS, "http", DEFAULT_ADDITIONAL_URL_INFO);
        private string m_UserName;
        private string m_Hostname;
        private string m_ClientName;
        private string m_Version;
        private string m_ProtocolLevel = "6"; // default to level 6 support
        //private Site m_CurrentSite = null;


        #region Constants for Freedb commands
        public class Commands
        {
            public const string CMD_HELLO = "hello";
            public const string CMD_READ = "cddb+read";
            public const string CMD_QUERY = "cddb+query";
            public const string CMD_SITES = "sites";
            public const string CMD_PROTO = "proto";
            public const string CMD_CATEGORIES = "cddb+lscat";
            public const string CMD = "cmd="; // will never use without the equals so put it here
            public const string CMD_TERMINATOR = ".";
        }
        #endregion

        #region Constants for Freedb ResponseCodes
        public class ResponseCodes
        {
            public const string CODE_210 = "210"; // Okay // or in a query multiple exact matches
            public const string CODE_401 = "401"; // sites: no site information available
            public const string CODE_402 = "402"; // Server Error

            public const string CODE_500 = "500"; // Invalid command, invalid parameters, etc.
            //query codes
            public const string CODE_200 = "200"; // Exact match 
            public const string CODE_211 = "211"; // InExact matches found - list follows
            public const string CODE_202 = "202"; // No match 
            public const string CODE_403 = "403"; // Database entry is corrupt
            public const string CODE_409 = "409"; // No Handshake

            // our own code
            public const string CODE_INVALID = "-1"; // Invalid code 
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Property Version (string)
        /// </summary>
        //public string Version
        //{
        //    get
        //    {
        //        return this.m_Version;
        //    }
        //    set
        //    {
        //        this.m_Version = value;
        //    }
        //}






        /// <summary>
        /// Property MainSite(string)
        /// </summary>
        public string MainSiteAdress
        {
            get
            {
                return this.m_mainSite.SiteAddress;
            }
            set
            {
                this.m_mainSite.SiteAddress = value;
            }
        }

        /// <summary>
        /// Property ClientName (string)
        /// </summary>
        public string ClientName
        {
            get
            {
                return this.m_ClientName;
            }
            set
            {
                this.m_ClientName = value;
            }
        }

        ///// <summary>
        ///// Property Hostname (string)
        ///// </summary>
        //public string Hostname
        //{
        //    get
        //    {
        //        return this.m_Hostname;
        //    }
        //    set
        //    {
        //        this.m_Hostname = value;
        //    }
        //}

        ///// <summary>
        ///// Property UserName (string)
        ///// </summary>
        //public string UserName
        //{
        //    get
        //    {
        //        return this.m_UserName;
        //    }
        //    set
        //    {
        //        this.m_UserName = value;
        //    }
        //}

        ///// <summary>
        ///// Property ProtocolLevel (string)
        ///// </summary>
        //public string ProtocolLevel
        //{
        //    get
        //    {
        //        return this.m_ProtocolLevel;
        //    }
        //    set
        //    {
        //        this.m_ProtocolLevel = value;
        //    }
        //}

        /// <summary>
        /// Property CurrentSite (Site)
        /// </summary>
        //public Site CurrentSite
        //{
        //    get
        //    {
        //        return this.m_CurrentSite;
        //    }
        //    set
        //    {
        //        this.m_CurrentSite = value;
        //    }
        //}




        public FreedbHelper(string iUserName, string iHostName, string iClientName, string iVersion)
        {
            m_UserName = iUserName;
            m_Hostname = iHostName;
            m_ClientName = iClientName;
            m_Version = iVersion;

            m_ProtocolLevel = "6"; // default it
        }



        ///// <summary>
        ///// Retrieve all Freedb servers from the main server site
        ///// </summary>
        ///// <param name="sites">SiteCollection that is populated with the site information</param>
        ///// <returns>Response Code</returns>
        //public List<Site> GetSites()
        //{
        //    return GetSites(Site.PROTOCOLS.ALL);
        //}

        #endregion


        ///// <summary>
        ///// Get the Freedb sites
        ///// </summary>
        ///// <param name="protocol"></param>
        ///// <param name="sites">SiteCollection that is populated with the site information</param>
        ///// <returns>Response Code</returns>
        ///// 
        //public List<Site> GetSites(string protocol )
        //{
        //    if (protocol != Site.PROTOCOLS.CDDBP  && protocol != Site.PROTOCOLS.HTTP)
        //        protocol = Site.PROTOCOLS.ALL;

        //    List<string> coll;

        //    try
        //    {
        //        coll= Call(Commands.CMD_SITES,m_mainSite.GetUrl());
        //    }

        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error retrieving Sites." + ex.Message);
        //        throw new Exception("FreedbHelper.GetSites: Error retrieving Sites.", ex);
        //    }


        //    // check if results came back
        //    if (coll.Count < 0)
        //    {
        //        string msg = "No results returned from sites request.";
        //        throw new Exception(msg,null);
        //    }

        //    string code = GetCode(coll[0]);
        //    if (code == ResponseCodes.CODE_INVALID)
        //    {
        //        string msg = "Unable to process results Sites Request. Returned Data: " + coll[0];
        //        throw new Exception(msg,null);

        //    }

        //    switch (code)
        //    {
        //        case ResponseCodes.CODE_500:
        //        case ResponseCodes.CODE_401:
        //            return null;

        //        case ResponseCodes.CODE_210:
        //        {
        //            coll.RemoveAt(0);
        //            List<Site> sites = new List<Site>();
        //            foreach (String line in coll)
        //            {
        //                Debug.WriteLine("line: " + line);
        //                Site site = new Site(line);
        //                if (protocol == Site.PROTOCOLS.ALL)
        //                    sites.Add(new Site(line));
        //                else if (site.Protocol == protocol)
        //                    sites.Add(new Site(line));
        //            }

        //            return sites;
        //        }

        //        default: 
        //            return null;
        //    }

        //}


        /// <summary>
        /// Read Entry from the database. 
        /// </summary>
        /// <param name="qr">A QueryResult object that is created by performing a query</param>
        /// <param name="cdEntry">out parameter - CDEntry object</param>
        /// <returns></returns>
        //public CDEntry Read(QueryResult qr)
        //{
        //    Debug.Assert(qr != null);

        //    List<string> coll = null;
        //    StringBuilder builder = new StringBuilder(FreedbHelper.Commands.CMD_READ);
        //    builder.Append("+");
        //    builder.Append(qr.Category);
        //    builder.Append("+");
        //    builder.Append(qr.Discid);

        //    //make call
        //    try
        //    {
        //        coll = Call(builder.ToString());
        //    }

        //    catch (Exception ex)
        //    {
        //        string msg = "Error performing cddb read.";
        //        throw new Exception(msg,ex);
        //    }

        //    // check if results came back
        //    if (coll.Count < 0)
        //    {
        //        string msg = "No results returned from cddb read.";
        //        throw new Exception(msg,null);
        //    }


        //    string code = GetCode(coll[0]);
        //    if (code == ResponseCodes.CODE_INVALID)
        //    {
        //        string msg = "Unable to process results for cddb read. Returned Data: " + coll[0];
        //        throw new Exception(msg,null);
        //    }


        //    switch (code)
        //    {
        //        case ResponseCodes.CODE_500: // query problem
        //        case ResponseCodes.CODE_401: // entry not found
        //        case ResponseCodes.CODE_402: // server error
        //        case ResponseCodes.CODE_403: // Database entry is corrupt
        //        case ResponseCodes.CODE_409: // No handshake
        //            return null;

        //        case ResponseCodes.CODE_210: // good 
        //        {
        //            coll.RemoveAt(0); // remove the 210
        //            return new CDEntry(coll);
        //        }
        //        default:
        //            return null;
        //    }
        //}

        /// <summary>
        /// Read Entry from the database. 
        /// </summary>
        /// <param name="qr">A QueryResult object that is created by performing a query</param>
        /// <param name="cdEntry">out parameter - CDEntry object</param>
        /// <returns></returns>
        internal AlbumDescriptor ConvertToAlbumDescriptor(QueryResult qr, IInternetServiceListener listener)
        {
            Debug.Assert(qr != null);

            List<string> coll = null;
            StringBuilder builder = new StringBuilder(FreedbHelper.Commands.CMD_READ);
            builder.Append("+");
            builder.Append(qr.Category);
            builder.Append("+");
            builder.Append(qr.Discid);

            //make call
            try
            {
                coll = Call(builder.ToString());
            }

            catch (WebException ex)
            {
                if (listener != null)
                    listener.OnWebExeption(ex);
                return null;
            }
            catch (Exception)
            {
                return null;
            }

            // check if results came back
            if (coll.Count == 0)
            {
                return null;
            }


            string code = GetCode(coll[0]);
            if (code == ResponseCodes.CODE_INVALID)
            {
                if (listener != null)
                    listener.OnUnExpectedUnreadableResult("Unable to process results for cddb read.");
                return null;
            }


            switch (code)
            {
                case ResponseCodes.CODE_500: // query problem
                case ResponseCodes.CODE_401: // entry not found
                case ResponseCodes.CODE_402: // server error
                case ResponseCodes.CODE_403: // Database entry is corrupt
                case ResponseCodes.CODE_409: // No handshake
                    return null;

                case ResponseCodes.CODE_210: // good 
                    {
                        coll.RemoveAt(0); // remove the 210
                        return AlbumDescriptor.FromFreeDBInfo(coll);
                    }
                default:
                    if (listener != null)
                        listener.OnStatusCodeKO();
                    return null;
            }
        }


        /// <summary>
        /// Query the freedb server to see if there is information on this cd
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="queryResult"></param>
        /// <param name="queryResultsColl"></param>
        /// <returns></returns>
        public List<QueryResult> Query(string querystring)
        {
            List<string> coll = null;

            StringBuilder builder = new StringBuilder(FreedbHelper.Commands.CMD_QUERY);
            builder.Append("+");
            builder.Append(querystring);

            //make call
            try
            {
                coll = Call(builder.ToString());
            }

            catch (Exception ex)
            {
                string msg = "Unable to perform cddb query.";
                Exception newex = new Exception(msg, ex);
                throw newex;
            }

            // check if results came back
            if (coll.Count == 0)
            {
                string msg = "No results returned from cddb query.";
                Exception ex = new Exception(msg, null);
                throw ex;
            }

            string code = GetCode(coll[0]);
            if (code == ResponseCodes.CODE_INVALID)
            {
                string msg = "Unable to process results returned for query: Data returned: " + coll[0];
                Exception ex = new Exception(msg, null);
                throw ex;
            }


            switch (code)
            {
                case ResponseCodes.CODE_500:
                case ResponseCodes.CODE_202:
                //Database entry is corrupt
                case ResponseCodes.CODE_403:
                //no handshake
                case ResponseCodes.CODE_409:
                    return null;

                // Multiple results were returned
                // Put them into a queryResultCollection object
                case ResponseCodes.CODE_211:
                case ResponseCodes.CODE_210:
                    {
                        List<QueryResult> queryResults = new List<QueryResult>();
                        //remove the 210 or 211
                        coll.RemoveAt(0);
                        foreach (string line in coll)
                        {
                            QueryResult result = new QueryResult(line, true);
                            queryResults.Add(result);
                        }

                        return queryResults;
                    }


                // exact match 
                case ResponseCodes.CODE_200:
                    {
                        List<QueryResult> queryResults = new List<QueryResult>();
                        queryResults.Add(new QueryResult(coll[0]));
                        return queryResults;
                    }


                //not found


                default:
                    return null;

            } // end of switch


        }


        ///// <summary>
        ///// Retrieve the categories
        ///// </summary>
        ///// <param name="strings"></param>
        ///// <returns></returns>
        //public string GetCategories(out List<string> strings)
        //{

        //    List<string> coll;
        //    strings = null;

        //    try
        //    {
        //        coll = Call(FreedbHelper.Commands.CMD_CATEGORIES);
        //    }

        //    catch (Exception ex)
        //    {
        //        string msg = "Unable to retrieve Categories.";
        //        Exception newex = new Exception(msg,ex);
        //        throw newex;
        //    }

        //    // check if results came back
        //    if (coll.Count < 0)
        //    {
        //        string msg = "No results returned from categories request.";
        //        Exception ex = new Exception(msg,null);
        //        throw ex;
        //    }

        //    string code = GetCode(coll[0]);
        //    if (code == ResponseCodes.CODE_INVALID)
        //    {
        //        string msg = "Unable to retrieve Categories. Data Returned: " + coll[0];
        //        Exception ex = new Exception(msg,null);
        //        throw ex;
        //    }

        //    switch (code)
        //    {
        //        case ResponseCodes.CODE_500:
        //            return ResponseCodes.CODE_500;

        //        case ResponseCodes.CODE_210:
        //        {
        //            strings = coll;
        //            coll.RemoveAt(0);
        //            return ResponseCodes.CODE_210;
        //        }

        //        default:
        //        {
        //            string msg = "Unknown code returned from GetCategories: " + coll[0];
        //            Exception ex = new Exception(msg,null);
        //            throw ex;
        //        }


        //    }

        //}


        /// <summary>
        /// Call the Freedb server using the specified command and the current site
        /// If the current site is null use the default server
        /// </summary>
        /// <param name="command">The command to be exectued</param>
        /// <returns>StringCollection</returns>
        private List<string> Call(string command)
        {
            //if (m_CurrentSite != null)
            //    return Call(command,m_CurrentSite.GetUrl());
            //else
            //    
            return Call(command, m_mainSite.GetUrl());
        }

        /// <summary>
        /// Call the Freedb server using the specified command and the specified url
        /// The command should not include the cmd= and hello and proto parameters.
        /// They will be added automatically
        /// </summary>
        /// <param name="command">The command to be exectued</param>
        /// <param name="url">The Freedb server to use</param>
        /// <returns>StringCollection</returns>
        private List<string> Call(string commandIn, string url)
        {
            StreamReader reader = null;
            IHttpWebResponse response = null;
            List<string> coll = new List<string>();

            try
            {
                //create our HttpWebRequest which we use to call the freedb server
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                IHttpWebRequest req = InternetProvider.InternetHelper.CreateHttpRequest(url);
                req.ContentType = "text/plain";
                // we are using th POST method of calling the http server. We could have also used the GET method
                req.Method = "POST";
                //add the hello and proto commands to the request
                string command = BuildCommand(Commands.CMD + commandIn);
                //using Unicode
                byte[] byteArray = Encoding.UTF8.GetBytes(command);
                //get our request stream
                Stream newStream = req.GetRequestStream();
                //write our command data to it
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                //Make the call. Note this is a synchronous call
                response = req.GetResponse();
                //put the results into a StreamReader
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                // add each line to the StringCollection until we get the terminator
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(Commands.CMD_TERMINATOR))
                        break;
                    else
                        coll.Add(line);
                }
            }

            catch
            //(Exception ex)
            {
                throw;
            }

            finally
            {
                if (response != null)
                    response.Close();
                if (reader != null)
                    reader.Close();
            }

            return coll;
        }





        /// <summary>
        /// Given a specific command add on the hello and proto which are requied for an http call
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string BuildCommand(string command)
        {
            StringBuilder builder = new StringBuilder(command);
            builder.Append("&");
            builder.Append(Hello());
            builder.Append("&");
            builder.Append(Proto());
            return builder.ToString();
        }

        /// <summary>
        /// Build the hello part of the command 
        /// </summary>
        /// <returns></returns>
        public string Hello()
        {
            StringBuilder builder = new StringBuilder(Commands.CMD_HELLO);
            builder.Append("=");
            builder.Append(m_UserName);
            builder.Append("+");
            builder.Append(this.m_Hostname);
            builder.Append("+");
            builder.Append(this.ClientName);
            builder.Append("+");
            builder.Append(this.m_Version);
            return builder.ToString();
        }

        /// <summary>
        /// Build the Proto part of the command
        /// </summary>
        /// <returns></returns>
        public string Proto()
        {
            StringBuilder builder = new StringBuilder(Commands.CMD_PROTO);
            builder.Append("=");
            builder.Append(m_ProtocolLevel);
            return builder.ToString();
        }


        /// <summary>
        /// given the first line of a result set return the CDDB code
        /// </summary>
        /// <param name="firstLine"></param>
        /// <returns></returns>
        private string GetCode(string firstLine)
        {
            firstLine = firstLine.Trim();

            //find first white space after start
            int index = firstLine.IndexOf(' ');
            if (index != -1)
                firstLine = firstLine.Substring(0, index);
            else
            {
                return ResponseCodes.CODE_INVALID;
            }

            return firstLine;
        }



        ///// <summary>
        ///// If a different default site address is preferred over "freedb.freedb.org"
        ///// set it here
        ///// NOTE: Only set the ip address
        ///// </summary>
        ///// <param name="ipAddress"></param>
        //public void SetDefaultSiteAddress(string siteAddress)
        //{
        //    //sanity check on the url
        //    if (siteAddress.IndexOf("http") != -1 ||
        //        siteAddress.IndexOf("cgi") != -1)
        //        throw new Exception("Invalid Site Address specified");

        //    this.m_mainSite.SiteAddress = siteAddress;
        //}

    }
}
