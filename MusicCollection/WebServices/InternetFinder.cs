using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Concurrency;

using MusicCollection.Infra.Tasks;
using MusicCollection.Fundation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using MusicCollection.ToolBox.Event;
using System.Threading;
using MusicCollection.ToolBox.Web;


namespace MusicCollection.WebServices
{
    internal class InternetFinder : IInternetFinder
    {
        private WebResult _Res = null;
        private IWebUserSettings _WSM;
        private bool _Computing = false;
        private IDisposable _Subscribed;
        private ManualResetEvent _MRE;

        internal InternetFinder(IWebUserSettings iic, IWebQuery iQuery)
        {
            _OnResult = new UISafeEvent<InternetFinderResultEventArgs>(this);
            _OnInternetError = new UISafeEvent<InternetFailedArgs>(this);

            _WSM = iic;
            Query = iQuery;

            //CancellationSource = new CancellationTokenSource();

            _Res = new WebResult();
        }

        //private CancellationTokenSource CancellationSource
        //{
        //    get;
        //    set;
        //}

        static IDictionary<WebProvider, Type> _InternetProviders;

        static InternetFinder()
        {
            _InternetProviders = AttributeExtender.GetMarkedTypeInTheAssembly<WebServicesInfoProviderAttribute>()
                .Where(o => o.Item2.GetInterface("IInternerInformationProvider") != null).ToDictionary(o => o.Item1.WebProvider, o => o.Item2);
        }


        #region Events

        //private UISafeEvent<OnInternetFinderProgressEventArgs> _OnSearchProgress;
        private UISafeEvent<InternetFinderResultEventArgs> _OnResult;
        private UISafeEvent<InternetFailedArgs> _OnInternetError;

        //public event EventHandler<OnInternetFinderProgressEventArgs> OnSearchProgress
        //{
        //    add { _OnSearchProgress.Event += value; }
        //    remove { _OnSearchProgress.Event -= value; }
        //}

        public event EventHandler<InternetFinderResultEventArgs> OnResult
        {
            add { _OnResult.Event += value; }
            remove { _OnResult.Event -= value; }
        }

        public event EventHandler<InternetFailedArgs> OnInternetError
        {
            add { _OnInternetError.Event += value; }
            remove { _OnInternetError.Event -= value; }
        }

        //private void SearchProgress(string connection,string What)
        //{
        //    _OnSearchProgress.Fire(new OnInternetFinderProgressEventArgs(What,connection, Query), false);
        //}

        private void ConnectionDown(InternetFailedArgs ifa, WebProvider? Provider = null)
        {
            ifa.WebService = Provider;
            _OnInternetError.Fire(ifa, true);
        }

        //private void Results()
        //{
        //    _OnResult.Fire(new InternetFinderResultEventArgs(Query, Result), true);
        //}

        #endregion

        private List<Tuple<WebProvider, IInternerInformationProvider>> GetInternetProviders()
        {
                return _InternetProviders.Select(ip => new Tuple<WebProvider, IInternerInformationProvider>(ip.Key, Activator.CreateInstance(ip.Value, _WSM) as IInternerInformationProvider)).ToList();
        }


        //private IEnumerable<WebMatch<AlbumDescriptor>> FromProviders(Tuple<WebProvider, IInternerInformationProvider> provider)
        //{
        //    try
        //    {
        //        EventHandler<InternetFailedArgs> Error = (o, e) => ConnectionDown( e, provider.Item1);
        //        provider.Item2.OnInternetError += Error;

        //        var res = provider.Item2.Search(Query, CancellationSource.Token).Select(m => new WebMatch<AlbumDescriptor>(m.FindItem, m.Precision, provider.Item1));

        //        provider.Item2.OnInternetError -= Error;
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(ex.ToString());
        //        ConnectionDown(InternetFailedArgs.InternetDown(ex), provider.Item1);
        //        return Enumerable.Empty<WebMatch<AlbumDescriptor>>();
        //    }
        //}


        private void Search(CancellationToken iCancellationToken)
        {
            _MRE = new ManualResetEvent(false); 

            if ((!IsValid) || (_Computing))
            {
                _MRE.Set();
                _OnResult.Fire(null, true);
                return;
            }
           
            _Computing = true;

            if (!InternetProvider.InternetHelper.GetIsNetworkAvailable())
            {
                ConnectionDown(InternetFailedArgs.InternetDown());
                _OnResult.Fire(null, true);
                _MRE.Set();
            }
            else
            {
                List<IEnumerable<WebMatch<IFullAlbumDescriptor>>> listofsolution = new List<IEnumerable<WebMatch<IFullAlbumDescriptor>>>();
                GetInternetProviders().Apply(provider => listofsolution.Add(
                    provider.Item2.Search(Query, iCancellationToken)
                    .Select(m => new WebMatch<IFullAlbumDescriptor>(m.FindItem, m.Precision, provider.Item1))));


                var observables = listofsolution.Select(ie => ie.ToObservable(NewThreadScheduler.Default)
                                .Catch((WebServicesException e) => { ConnectionDown(e.Event); return Observable.Empty<WebMatch<IFullAlbumDescriptor>>(); }));

                _Subscribed = observables.Merge().SubscribeOn(NewThreadScheduler.Default).Subscribe(
                    item => _Res.Found.Add(item),
                    () => { _OnResult.Fire(new InternetFinderResultEventArgs(Query, Result), true); _MRE.Set(); });    
             }
        }

        public void Compute(CancellationToken iCancellationToken)
        {
            //if (!Sync)
            //{
            Search(iCancellationToken);
            _MRE.WaitOne();
            //}
            //else
            //{
            //    Search();
            //    _MRE.WaitOne();
            //}
        }

        public IWebQuery Query { get; private set;}

        public IWebResult Result
        {
            get { return _Res; }
        }

        public WebResult RawResult
        {
            get { return _Res; }
        }

        public bool IsValid
        {
            get { return (Query != null); }
        }

        public void Cancel()
        {
            if (_Subscribed != null)
            {
                _Subscribed.Dispose();
                _Subscribed = null;
            }
            //CancellationSource.Cancel();
        }

        //public void Compute()
        //{
        //    Search();
        //    _MRE.WaitOne();
        //}

        public Task ComputeAsync(CancellationToken iCancellationToken)
        {
            Search(iCancellationToken);
            return _MRE.AsTask();
        }
    }
}
