using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using MusicCollection.Infra;
using System.Diagnostics;


namespace MusicCollection.ToolBox.Collection.Observable
{
    internal interface IIsolatedMofiableList<T> where T : class
    {
        ObservableCollection<T> MofifiableCollection { get; }

        event EventHandler<EventColllectionChangedArgs<T>> OnBeforeChangedCommited;

        void CommitChanges();

        void CancelChanges();

        bool Changed { get; }

        event EventHandler<EventArgs> OnCommit;
    }



    internal class EventColllectionChangedArgs<T> : EventArgs
    {
        internal NotifyCollectionChangedAction What
        { get; private set; }

        internal T Who
        { get; private set; }

        internal EventColllectionChangedArgs(T o, NotifyCollectionChangedAction what)
        {
            Who = o;
            What = what;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    internal class SafeCollectionModifierConverter<T, Tconverted> : IIsolatedMofiableList<Tconverted>
        where T : class
        where Tconverted : class
    {
        private ObservableCollection<Tconverted> _ObsList;
        private IList<T> _Original;
        private Queue<Tuple<Tconverted, Action<Tconverted, IList<T>>>> _Changes;

        public event EventHandler<EventColllectionChangedArgs<Tconverted>> OnBeforeChangedCommited;


        static internal SafeCollectionModifierConverter<Ti, Tc> GetSafeCollectionModifierConverterDerived<Ti, Tc>(IList<Ti> items)
            where Ti : class,Tc
            where Tc : class
        {
            return new SafeCollectionModifierConverter<Ti, Tc>(items, t => t, t => t as Ti);
        }

        private void OnBeforeChanges(Tconverted o, NotifyCollectionChangedAction what)
        {
            EventHandler<EventColllectionChangedArgs<Tconverted>> BeforeChangedCommited = OnBeforeChangedCommited;

            if (BeforeChangedCommited != null)
                BeforeChangedCommited(this, new EventColllectionChangedArgs<Tconverted>(o, what));
        }


        #region exposed methods

        public event EventHandler<EventArgs> OnCommit;


        internal SafeCollectionModifierConverter(IList<T> items, Func<T, Tconverted> Conv1, Func<Tconverted, T> Conv2)
        {
            _Original = items;
            ConvertModelToObserved = Conv1;
            ConvertObservedToModel = Conv2;
        }

        private Func<T, Tconverted> ConvertModelToObserved;
        private Func<Tconverted, T> ConvertObservedToModel;

        ObservableCollection<Tconverted> IIsolatedMofiableList<Tconverted>.MofifiableCollection
        {
            get
            {
                if (_ObsList == null)
                {
                    _ObsList = new UISafeObservableCollection<Tconverted>(from o in _Original select ConvertModelToObserved(o));
                    _ObsList.CollectionChanged += OnCollectionChanged;
                }

                return _ObsList;
            }
        }

        void IIsolatedMofiableList<Tconverted>.CancelChanges()
        {
            _Changes = null;
            _ObsList.Clear();
            _ObsList.AddCollection(from o in _Original select ConvertModelToObserved(o));
        }

        bool IIsolatedMofiableList<Tconverted>.Changed
        {
            get {return (_Changes!=null);}
        }

        void IIsolatedMofiableList<Tconverted>.CommitChanges()
        {
            if (_Changes == null)
                return;

            int C = _Changes.Count;

            for (int i = 0; i < C; i++)
            {
                Tuple<Tconverted, Action<Tconverted, IList<T>>> action = _Changes.Dequeue();
                action.Item2(action.Item1, _Original);
            }

            EventHandler<EventArgs> Commit = OnCommit;

            if ((C > 0) && (Commit != null))
                Commit(this, EventArgs.Empty);
        }

        #endregion

        #region private method

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_Changes == null)
                _Changes = new Queue<Tuple<Tconverted, Action<Tconverted, IList<T>>>>();

            int i = 0;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    i = e.NewStartingIndex;
                    foreach (Tconverted Al in e.NewItems)
                    {
                        _Changes.Enqueue(new Tuple<Tconverted, Action<Tconverted, IList<T>>>(Al, (a, l) =>
                           {
                               T aa = ConvertObservedToModel(a);
                               if ((a != null) && (aa == null)) //@report
                                   throw new Exception("Collection Album Modifier");

                               OnBeforeChanges(a, e.Action);
                               l.Insert(i++, aa);
                           }
                           ));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    i = e.OldStartingIndex;
                    foreach (Tconverted Al in e.OldItems)
                    {
                        T AI = ConvertObservedToModel(Al);
                        if ((AI == null) && (Al!=null))
                            throw new Exception("Collection Album Modifier");

                        _Changes.Enqueue(new Tuple<Tconverted, Action<Tconverted, IList<T>>>(Al, ((a, l) =>
                        {
                            OnBeforeChanges(a, e.Action);
                            l.RemoveAt(i++);

                        })));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new Exception("Album Modifier");

                case NotifyCollectionChangedAction.Replace:
                    
                    Tconverted ConvRemove = e.OldItems[0] as Tconverted; 
                    Tconverted ConvAdd = e.NewItems[0] as Tconverted;

                    if (ConvRemove == ConvAdd)
                        return;
                    
                    int IndexFroozen = e.OldStartingIndex;
 
                    _Changes.Enqueue(new Tuple<Tconverted, Action<Tconverted, IList<T>>>(ConvRemove, ((a, l) =>
                    {
                        OnBeforeChanges(a, NotifyCollectionChangedAction.Remove);
                        l.RemoveAt(IndexFroozen);
                    })));
                   
                    T OrAdd = ConvertObservedToModel(ConvAdd);
                    if ((OrAdd == null) && (ConvAdd!=null))
                        throw new Exception("Collection Album Modifier");

                    _Changes.Enqueue(new Tuple<Tconverted, Action<Tconverted, IList<T>>>(ConvAdd, ((a, l) =>
                    {
                        OnBeforeChanges(a, NotifyCollectionChangedAction.Add);
                        l.Insert(IndexFroozen, OrAdd);
                    })));
                    break;



                case NotifyCollectionChangedAction.Reset:
                    int size = _Original.Count - 1;
                    for (int k = size; k >= 0; k--)
                    {
                        T AI = _Original[k];
                        if (AI == null)
                            throw new Exception("Collection Album Modifier");

                        Tconverted Al = ConvertModelToObserved(AI);

                        int Index = k;

                        _Changes.Enqueue(new Tuple<Tconverted, Action<Tconverted, IList<T>>>(Al, ((a, l) =>
                        {
                            OnBeforeChanges(a, e.Action);
                            l.RemoveAt(Index);

                        })));
                    }
                    break;
            }
        }

        #endregion

    }

}
