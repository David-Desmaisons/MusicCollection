using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;

using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Web;


namespace MusicCollection.ToolBox.Buffer
{
    internal class InternalBufferFactory
    {

        private interface IprivatePersistance : IDisposable
        {
            Stream GetBuffer();

            byte[] RawData
            {
                get;
            }

            bool IsOK
            {
                get;
            }

            long Length
            {
                get;
            }

            string PersistedPath
            {
                get;
            }

            IprivatePersistance Save(string FileName);

            bool CopyTo(string FileName);

            IprivatePersistance Clone();

        }

        private class BufferWrapper : IPersistentBufferProvider
        {
            IprivatePersistance _IPP;
            bool _IsPersistent;

            internal BufferWrapper(IprivatePersistance IPP)
            {
                _IPP = IPP;
                _IsPersistent = false;
            }

            bool IBufferProvider.IsOK { get { return _IPP.IsOK; } }

            int IBufferProvider.GetContentHashCode()
            {
                int res = 0;
                using (Stream str = _IPP.GetBuffer())
                {
                    SHA1KeyComputer skc = SHA1KeyComputer.FromStream(str);
                    res = skc.ComputeKey().GetHashCode();
                }
                return res;
            }


            bool IBufferProvider.Compare(IBufferProvider Other)
            {
                if (Other == null)
                    return false;

                if (object.ReferenceEquals(this, Other))
                    return true;

                BufferWrapper autre = Other as BufferWrapper;

                if (autre == null)
                    return false;

                if (object.ReferenceEquals(_IPP, autre._IPP))
                    return true;

                using (Stream st = _IPP.GetBuffer())
                {
                    using (Stream st2 = autre._IPP.GetBuffer())
                    {
                        if ((st2 == null) || (st == null))
                            return object.ReferenceEquals(st2, st);

                        return st.Compare(st2);
                    }
                }
            }

            string IBufferProvider.PersistedPath
            {
                get { return _IPP.PersistedPath; }
            }

            bool IBufferProvider.CopyTo(string FileName)
            {
                return _IPP.CopyTo(FileName);
            }

            private const string _Bin = ".bin";

            private string _Ext = null;
            public string DefaultExtension
            {
                get { return _Ext == null ? _Bin : _Ext; }
                set { if (!value.StartsWith("."))return; _Ext = value; }
            }


            bool IPersistentBufferProvider.IsPersistent { get { return _IsPersistent; } set { _IsPersistent = value; } }

            Stream IBufferProvider.GetBuffer()
            {
                return _IPP.GetBuffer();
            }

            long IBufferProvider.Length
            {
                get { return _IPP.Length; }
            }

            IBufferProvider IBufferProvider.Clone()
            {
                return new BufferWrapper(_IPP.Clone());
            }

            byte[] IBufferProvider.RawData
            {
                get { return _IPP.RawData; }
            }

            private string FinalFileName(string fn)
            {
                return (Path.GetExtension(fn) == string.Empty) ? fn + DefaultExtension : fn;
            }

            bool IPersistentBufferProvider.Save(string FileName)
            {
                if (FileName == null)
                    return false;

                IprivatePersistance New = null;

                try
                {
                    New = _IPP.Save(FinalFileName(FileName));
                    if (New != null)
                    {
                        _IPP.Dispose();
                        _IPP = New;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving a stream " + e.ToString() + " " + _IPP.ToString());
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return _IPP.GetHashCode();
            }

            public override bool Equals(object iCompTo)
            {
                BufferWrapper bf = iCompTo as BufferWrapper;
                if (bf == null)
                    return false;

                return (_IPP.Equals(bf._IPP));
            }

            public void Dispose()
            {
                if (_IPP != null)
                {
                    _IPP.Dispose();
                    _IPP = null;
                }
            }

        }

        private class BasicBuffer : IprivatePersistance
        {
            private byte[] _Buffer;

            string IprivatePersistance.PersistedPath
            {
                get { return null; }
            }

            long IprivatePersistance.Length
            {
                get { return _Buffer.Length; }
            }


            bool IprivatePersistance.IsOK
            {
                get { return (_Buffer != null); }
            }

            Stream IprivatePersistance.GetBuffer()
            {
                return new MemoryStream(_Buffer, false);
            }

            byte[] IprivatePersistance.RawData
            {
                get { return _Buffer; }
            }

            internal BasicBuffer(byte[] iBuffer)
            {
                _Buffer = iBuffer;
            }

            IprivatePersistance IprivatePersistance.Save(string FileName)
            {
                try
                {
                    File.WriteAllBytes(FileName, _Buffer);
                    return new BufferFromFile(FileName);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving a byte[] " + e.ToString());
                    throw e;
                }
            }

            bool IprivatePersistance.CopyTo(string FileName)
            {
                try
                {
                    File.WriteAllBytes(FileName, _Buffer);
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving a byte[] " + e.ToString());
                    return false;
                }
            }


            IprivatePersistance IprivatePersistance.Clone()
            {
                return new BasicBuffer(_Buffer);
            }

            public override int GetHashCode()
            {
                return _Buffer.Length.GetHashCode();
            }

            public override bool Equals(object iCompTo)
            {
                if (iCompTo == null)
                    return false;

                if (object.ReferenceEquals(this, iCompTo))
                    return true;

                BasicBuffer BFF = iCompTo as BasicBuffer;
                if (BFF == null)
                    return false;

                return _Buffer.SequenceEqual(BFF._Buffer);
            }

            public void Dispose()
            {
                _Buffer = null;
            }


        }



        private class BufferFromFile : IprivatePersistance
        {

            //private PathDecomposeur _PD; 
            private string _Name;
            //{
            //    get { return (_PD == null) ? null : _PD.FullName; }
            //    set{ _PD = PathDecomposeur.FromName(value); }
            //}


            private bool? _IsOK;

            string IprivatePersistance.PersistedPath
            {
                get { return _Name; }
            }

            long IprivatePersistance.Length
            {
                get { return new FileInfo(_Name).Length; }
            }

            bool IprivatePersistance.IsOK
            {
                get { return (IsOK == true); }
            }

            private bool IsOK
            {
                get
                {
                    if (_IsOK == null)
                    {
                        _IsOK = File.Exists(_Name);
                    }

                    return (bool)_IsOK;
                }
            }

            public Stream GetBuffer()
            {
                return IsOK ? new FileStream(_Name, FileMode.Open, FileAccess.Read) : null;
            }

            byte[] IprivatePersistance.RawData
            {
                get
                {
                    if (!IsOK)
                        return null;

                    byte[] res = null;
                    using (Stream b = GetBuffer())
                    {
                        res = new byte[b.Length];
                        b.Read(res, 0, res.Length);
                    }

                    return res;
                }
            }

            IprivatePersistance IprivatePersistance.Clone()
            {
                if (IsOK)
                    return new BasicBuffer((this as IprivatePersistance).RawData);

                return new BufferFromFile(_Name);
            }

            internal BufferFromFile(string iName)
            {
                _Name = iName;
            }

            IprivatePersistance IprivatePersistance.Save(string FileName)
            {
                try
                {
                    if (IsOK)
                    {
                        File.Copy(_Name, FileName);
                        _Name = FileName;
                    }
                    else
                    {
                        //DEM breaking change
                        throw new Exception("Cannot save not found file.");
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving a byte[] " + e.ToString());
                    throw;
                }
            }

            bool IprivatePersistance.CopyTo(string FileName)
            {
                if (IsOK == false)
                    return false;

                try
                {
                    File.Copy(_Name, FileName);
                    return true; ;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Problem saving a byte[]" + e.ToString());
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return _Name.GetHashCode();
            }

            public override bool Equals(object iCompTo)
            {
                if (iCompTo == null)
                    return false;

                if (object.ReferenceEquals(this, iCompTo))
                    return true;

                BufferFromFile BFF = iCompTo as BufferFromFile;
                if (BFF == null)
                    return false;

                return _Name == BFF._Name;
            }

            public void Dispose()
            {
            }

        }



        static internal IPersistentBufferProvider GetBufferProviderFromStream(Stream iBuffer)
        {
            if (iBuffer.CanSeek)
                iBuffer.Position = 0;

            byte[] res = null;

            using (MemoryStream MS = new MemoryStream())
            {
                iBuffer.CopyTo(MS);
                MS.Position = 0;
                res = MS.ToArray();
            }

            return new BufferWrapper(new BasicBuffer(res));
        }

        static internal IPersistentBufferProvider GetBufferProviderFromArray(byte[] iBuffer)
        {
            return new BufferWrapper(new BasicBuffer(iBuffer));
        }

        static internal IPersistentBufferProvider GetBufferProviderFromFile(string FileName)
        {
            return new BufferWrapper(new BufferFromFile(FileName));
        }

        static internal IPersistentBufferProvider GetBufferProviderFromURI(Uri filename, IHttpContextFurnisher Context = null)
        {
            if (filename == null)
                return null;

            IHttpWebRequest request = InternetProvider.InternetHelper.CreateHttpRequest(filename);

            if (Context != null)
            {
                HttpContext c = Context.Context;
                request.UserAgent = c.UserAgent;
                if (c.Referer != null)
                    request.Referer = c.Referer;
            }

            return GetBufferProviderFromHttpRequest(request);
        }


        static internal IPersistentBufferProvider GetBufferProviderFromHttpRequest(IHttpWebRequest request)
        {
            if (request == null)
                return null;

            try
            {
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ReadWriteTimeout = 900000;
                request.Timeout = 300000;

                using (var response = request.GetResponse())
                {
                    return GetBufferProviderFromStream(response.GetResponseStream());
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return null;
            }

        }

    }
}