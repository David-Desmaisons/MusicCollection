using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.ToolBox.Collection;

namespace MusicCollection.Infra
{
    internal class ObjectDynamicAtributes
    {
        private IDictionary<string, IRawResultListener> _InstanceAttribute = new PolyMorphDictionary<string, IRawResultListener>();
        private bool _Registering = false;
        private bool _Disposed = false;

        internal ObjectDynamicAtributes()
        {
        }

        internal bool IsListening { get { return _Registering; } }

        internal IRawResultListener GetOrCreate<T>(string iName, Func<string, IRawResultListener> Factory)
        {
            var foc = _InstanceAttribute.FindOrCreate(iName, Factory);
            if ((foc.CollectionStatus == CollectionStatus.Create) && (_Registering))
            {
                foc.Item.Register();
            }
            return foc.Item;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IEnumerable<EventHandler<ObjectModifiedArgs>> ObservedAttribute
        {
            get
            {
                return _InstanceAttribute.Values.Select(o => o.ListenedObject).Where(obj => obj != null);
            }
        }

        public void Register()
        {
            if (_Disposed || _Registering)
                return;

            _Registering = true;
            _InstanceAttribute.Values.Apply(f => f.Register());
        }

        public void UnRegister()
        {
            if (_Disposed || !_Registering)
                return;

            _Registering = false;
            _InstanceAttribute.Values.Apply(f => f.UnRegister());
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            _InstanceAttribute.Values.Apply(f => f.Dispose());
        }

    }
}
