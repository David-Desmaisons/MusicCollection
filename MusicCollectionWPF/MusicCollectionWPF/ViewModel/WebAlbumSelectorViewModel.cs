using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection.Observable;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class WebAlbumSelectorViewModel : ViewModelBase
    {
        private IInternetFinder _IInternetFinder;
        private IModifiableAlbum _IModifiableAlbum;
        private IMusicSession _IMS;

        public  WebAlbumSelectorViewModel(IMusicSession ims,  IModifiableAlbum imad, int iMaxResult=5)
        {
            _IModifiableAlbum = imad;
            _IMS = ims; 
            
            _IsLoading = true;
            _FoundSomething = true;
     
            _MergeStategy = MergeStategy.OnlyCover;

            var lookfor = imad.GetAlbumDescriptor();

            IWebQuery wb = ims.WebQueryFactory.FromAlbumDescriptor(lookfor);
            wb.NeedCoverArt = true;
            wb.MaxResult = iMaxResult;

            _OriginalAlbumName = lookfor.ToString();
            _IInternetFinder = ims.GetInternetFinder(wb);

            SelectCover = Register(RelayCommand.InstanciateAsync(()=>DoSelect(), () => SelectedInfos.Count>0));
            Cancel = Register(RelayCommand.Instanciate(DoCancel, ()=>IsLoading));
            Close = RelayCommand.Instanciate(DoClose);

            LoadResult().DoNotWaitSafe();
        }

        private async Task LoadResult()
        {
            try
            {
                await _IInternetFinder.ComputeAsync();
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Problem loading internet information {0}",e));
            }
            IsLoading = false;

            switch (_IInternetFinder.Result.Found.Count)
            {
                case 0:
                    FoundSomething = false;
                    break;

                case 1:
                    SelectedInfos.Add(_IInternetFinder.Result.Found[0]);
                    break;
            }
        }

        private void DoCancel()
        {
            if (IsLoading)
            {
                _IInternetFinder.Cancel();
                IsLoading = false;
            }
        }

        private async Task DoSelect()
        {

           IMergeStrategy imf = _IMS.Strategy.Get(MergeStategy);

           if ((imf.AlbumMetaData != IndividualMergeStategy.Never) && (SelectedInfos.Count > 1))
           {
               string Res = string.Format("Warning only data from {0} will be imported", SelectedInfos[0].FindItem.ToString());
               if (!Window.ShowConfirmationMessage("Do you want to proceed?", Res))
                   return;
           }
           
           DoClose();

           foreach (var alinf in SelectedInfos)
           {
               await _IModifiableAlbum.MergeFromMetaDataAsync(alinf.FindItem, imf);
               imf = _IMS.Strategy.Get(MergeStategy.OnlyCover);
           }
        }

        private void DoClose()
        {
            DoCancel();
            Window.Close();
        }

        public override void Dispose()
        {
            DoCancel();
            //Window.Close();
            base.Dispose();
        }

        #region Commands

        public ICommand SelectCover { get; private set; }
 
        public ICommand Cancel {get;private set;}

        public ICommand Close {get;private set;}
       
        #endregion

        #region Properties

        private MergeStategy _MergeStategy;
        public MergeStategy MergeStategy
        {
            get { return _MergeStategy; }
            set { this.Set(ref _MergeStategy, value); }
        }

        private WrappedObservableCollection<WebMatch<IFullAlbumDescriptor>> _SelectedInfos = 
            new WrappedObservableCollection<WebMatch<IFullAlbumDescriptor>>();
        public IList<WebMatch<IFullAlbumDescriptor>> SelectedInfos
        {
            get { return _SelectedInfos; }
        }

        public bool AtLeastOne
        {
            get { return this.Get<WebAlbumSelectorViewModel, bool>(()=>t => t.CDInfos.Count >= 1); }
        }


        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set { this.Set(ref _IsLoading, value); }
        }

        private bool _FoundSomething;
        public bool FoundSomething
        {
            get { return _FoundSomething; }
            set { this.Set(ref _FoundSomething, value); }
        }

        private string _OriginalAlbumName;
        public string OriginalAlbumName
        {
            get { return _OriginalAlbumName; }
        }

        public IList<WebMatch<IFullAlbumDescriptor>> CDInfos
        {
            get { return _IInternetFinder.Result.Found; }
        }

        #endregion

    }
}
