using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollection.Infra.Communication
{
    public class IPCClient<T>  where T : class
    {
        private ChannelFactory<T> _PipeFactory;

        public IPCClient(string iAdress = "MusicCollection"):this(typeof(T).Name,iAdress)
        {
        }

        public IPCClient(string iName, string iAdress )
        {
            _PipeFactory = new ChannelFactory<T>(new NetNamedPipeBinding(),
                    new EndpointAddress(string.Format(@"net.pipe://localhost/{0}/{1}", iAdress, iName)));
        }

        public T GetService()
        {
            return _PipeFactory.CreateChannel();
        }
    }
}
