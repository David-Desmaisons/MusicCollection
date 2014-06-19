using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.Communication
{
    public class IPCServer<T> : IDisposable where T : class
    {
        private ServiceHost _ServiceHost;

        public IPCServer(T serviceinstance, string Adress = "MusicCollection"):this(typeof(T).Name,serviceinstance,Adress)
        {
        }

        public IPCServer(string iName, T serviceinstance, string Adress="MusicCollection")
        {
            _ServiceHost = new ServiceHost(serviceinstance, new Uri[] { new Uri(string.Format(@"net.pipe://localhost/{0}", Adress)) });
            _ServiceHost.AddServiceEndpoint(typeof(T), new NetNamedPipeBinding(),iName);
            _ServiceHost.Open();
        }

        public void Dispose()
        {
            if (_ServiceHost!=null)
            {
                _ServiceHost.Close();
                IDisposable id =  _ServiceHost;
                id.Dispose();
                _ServiceHost = null;
            }
        }
    }
}
