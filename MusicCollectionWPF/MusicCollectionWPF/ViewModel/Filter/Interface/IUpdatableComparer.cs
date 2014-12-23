using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicCollectionWPF.ViewModel.Interface
{
    /// <summary>
    /// Class that respresents an event linked to corresponding Element
    /// </summary>
    public class ElementChangedArgs<T> : EventArgs
    {
        private T _Element;
        public ElementChangedArgs(T element)
        {
            _Element = element;
        }

        public T Element { get { return _Element; } }

        public static ElementChangedArgs<T> From(T element)
        {
            return new ElementChangedArgs<T>(element);
        }
    }

    /// <summary>
    ///  interface to compute update comparison between T instances
    /// </summary>
    public interface IUpdatableComparer<T> : IComparer<T>, IDisposable
    {
        /// <summary>
        /// Raised when the value of all objects may change
        /// </summary> 
        event EventHandler OnChanged;

        /// <summary>
        /// Raised when the comparaison value of an object may change
        /// </summary> 
        event EventHandler<ElementChangedArgs<T>> OnElementChanged;
    }
}
