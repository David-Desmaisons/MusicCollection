using System;
using System.Collections;
using System.Linq;
using System.Text;

using MusicCollection.Infra;


namespace MusicCollectionWPF.ViewModel
{
    public interface IComposedObservedCollection : IDisposable
    {
        IList Collection { get; }
    }


    public class ComposedObservedCollectionAdapter<T> : IComposedObservedCollection
    {
        private IExtendedObservableCollection<T> _Coll;
        public ComposedObservedCollectionAdapter(IExtendedObservableCollection<T> coll)
        {
            _Coll = coll;
        }

        public IList Collection
        {
            get { return _Coll; }
        }

        public void Dispose()
        {
            _Coll.Dispose();
        }
    }
}
