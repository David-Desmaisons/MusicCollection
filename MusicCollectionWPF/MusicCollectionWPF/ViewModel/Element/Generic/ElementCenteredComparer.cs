using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using MusicCollectionWPF.ViewModel.Interface;
using MusicCollection.Infra;

namespace MusicCollectionWPF.ViewModel.Element
{
    public class ElementCenteredComparer<T> : IUpdatableComparer<T> where T : class, INotifyPropertyChanged
    {
        private IList<T> _Elements;
        private IDistanceEvaluator<T> _DistantComputer;
        private IDictionary<T, int> _Dic;
        private bool _First = true;
 
        public ElementCenteredComparer(IList<T> iElements, IDistanceEvaluator<T> iDistantComputer)
        {
            _DistantComputer = iDistantComputer;
            _Elements = iElements;

            Bufferize();
        }

        private void Bufferize()
        {
            _Dic = new Dictionary<T, int>(_Elements.Count);

            Parallel.ForEach(_Elements,
                () => new Dictionary<T, int>(),
                (el, lc, localdic) => { localdic.Add(el, RawCompare(el)); return localdic; },
                (ldic) => { lock (_Dic) _Dic.Import(ldic); });

            if (_First)
            {
                _Dic.Keys.Apply(al => al.PropertyChanged += al_PropertyChanged);
                _First = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int RawCompare(T element)
        {
            return Math.Abs(_DistantComputer.EvaluateDistance(element));
        }

        public int Compare(T xx, T yy)
        {
            if (object.ReferenceEquals(xx, yy))
                return 0;

            return Value(xx) - Value(yy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Value(T al)
        {
            var trytofind = _Dic.FindOrCreate(al, c => RawCompare(c));
            if (trytofind.CollectionStatus == CollectionStatus.Create)
            {
                al.PropertyChanged += al_PropertyChanged;
            }
            return trytofind.Item;
        }

        private bool ResetAlbum(T al)
        {
            _Dic.Remove(al);
            _Dic.Add(al, RawCompare(al));

            return object.ReferenceEquals( al , _DistantComputer.Reference);
        }

        private void al_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var album = sender as T;
            if (ResetAlbum(album))
            {
                _DistantComputer.UpdateCacheData();
                Bufferize();
                FireChanged();
            }
            else
            {
                FireElementChanged(album);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FireChanged()
        {
            if (OnChanged != null) OnChanged(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FireElementChanged(T element)
        {
            if (OnChanged != null) OnElementChanged(this, ElementChangedArgs<T>.From(element));
        }

        public event EventHandler OnChanged;
        public event EventHandler<ElementChangedArgs<T>> OnElementChanged;

        public void Dispose()
        {
            foreach (T al in _Dic.Keys)
            {
                al.PropertyChanged -= al_PropertyChanged;
            }
            _Dic.Clear();
        }
    }
}
