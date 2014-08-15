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
            //_OnResult = new UISafeEvent<InternetFinderResultEventArgs>(this);
            //_OnInternetError = new UISafeEvent<InternetFailedArgs>(this);

            _WSM = iic;
            Query = iQuery;

            _Res = new WebResult();
        }

        static IDictionary<WebProvider, Type> _InternetProviders;

        static InternetFinder()
        {
            _InternetProviders = AttributeExtender.GetMarkedTypeInTheAssembly<WebServicesInfoProviderAttribute>()
                .Where(o => o.Item2.GetInterface("IInternerInformationProvider") != null).ToDictionary(o => o.Item1.WebProvider, o => o.Item2);
        }


        #region Events

        //private UISafeEvent<InternetFinderResultEventArgs> _OnResult;
        //private UISafeEvent<InternetFailedArgs> _OnInternetError;

        //public event EventHandler<InternetFinderResultEventArgs> OnResult
        //{
        //    add { _OnResult.Event += value; }
        //    remove { _OnResult.Event -= value; }
        //}

        //public event EventHandler<InternetFailedArgs> OnInternetError
        //{
        //    add { _OnInternetError.Event += value; }
        //    remove { _OnInternetError.Event -= value; }
        //}

        private void ConnectionDown(InternetFailed ifa, IProgress<InternetFailed> iInternetFailedArgs, WebProvider? Provider = null)
        {
            ifa.WebService = Provider;
            //_OnInternetError.Fire(ifa, true);
            iInternetFailedArgs.SafeReport(ifa);
        }

        #endregion

        private List<Tuple<WebProvider, IInternerInformationProvider>> GetInternetProviders()
        {
                return _InternetProviders.Select(ip => new Tuple<WebProvider, IInternerInformationProvider>(ip.Key, Activator.CreateInstance(ip.Value, _WSM) as IInternerInformationProvider)).ToList();
        }


        private void Search(CancellationToken iCancellationToken, IProgress<InternetFailed> iInternetFailedArgs)
        {
            _MRE = new ManualResetEvent(false); 

            if ((!IsValid) || (_Computing))
            {
                _MRE.Set();
                //_OnResult.Fire(null, true);
                return;
            }
           
            _Computing = true;

            if (!InternetProvider.InternetHelper.GetIsNetworkAvailable())
            {
                ConnectionDown(InternetFailed.InternetDown(), iInternetFailedArgs);
                //_OnResult.Fire(null, true);
                _MRE.Set();
            }
            else
            {
                List<IEnumerable<WebMatch<IFullAlbumDescriptor>>> listofsolution = new List<IEnumerable<WebMatch<IFullAlbumDescriptor>>>();
                GetInternetProviders().Apply(provider => listofsolution.Add(
                    provider.Item2.Search(Query, iCancellationToken)
                    .Select(m => new WebMatch<IFullAlbumDescriptor>(m.FindItem, m.Precision, provider.Item1))));


                var observables = listofsolution.Select(ie => ie.ToObservable(NewThreadScheduler.Default)
                                .Catch((WebServicesException e) => { ConnectionDown(e.Event, iInternetFailedArgs); return Observable.Empty<WebMatch<IFullAlbumDescriptor>>(); }));

                _Subscribed = observables.Merge().SubscribeOn(NewThreadScheduler.Default).Subscribe(
                    item => _Res.Found.Add(item),
                     () => {  _MRE.Set(); }); 
                    //() => { _OnResult.Fire(new InternetFinderResultEventArgs(Query, Result), true); _MRE.Set(); });    
             }
        }

        public void Compute(CancellationToken iCancellationToken, IProgress<InternetFailed> iInternetFailedArgs)
        {
            Search(iCancellationToken, iInternetFailedArgs);
            _MRE.WaitOne();
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
        }

        public Task ComputeAsync(CancellationToken iCancellationToken, IProgress<InternetFailed> iInternetFailedArgs)
        {
            Search(iCancellationToken, iInternetFailedArgs);
            return _MRE.AsTask();
        }
    }
}
