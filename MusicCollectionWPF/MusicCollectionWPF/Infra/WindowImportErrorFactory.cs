using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using MusicCollection.Fundation;
using MusicCollectionWPF.Windows;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.ViewModelHelper;


namespace MusicCollectionWPF.Infra
{
    static internal class  ViewModelFactory
    {
        //internal static IWindow GetWindowFromImporterror(ImportExportErrorEventArgs Ev, IMusicSession ims)
        //{
        //    CorruptedRarOrMissingPasswordArgs cmp = Ev as CorruptedRarOrMissingPasswordArgs;
        //    if (cmp != null)
        //    {
        //        return new RarPasswordWindow(cmp);
        //    }

        //    AmbigueousCDInformationArgs acdia = Ev as AmbigueousCDInformationArgs;
        //    if (acdia != null)
        //    {
        //        var cdinfos = new CDAlbumDescriptorCreatorViewModel(acdia,ims);
        //        return new CDImportInfoEditor() { ModelView = cdinfos };
        //    }

        //    CDCoverInformationArgs cdia = Ev as CDCoverInformationArgs;
        //    if (cdia != null)
        //    {
        //        WebAlbumFoundSelectorViewModel waw = new WebAlbumFoundSelectorViewModel(cdia, ims.Strategy);
        //        return new InternetResultWindow() { ModelView = waw };
        //    }

        //    return new CustoMessageBox(Ev);
        //}

        internal static ViewModelBase GetViewModelBaseFromImporterror(ImportExportError Ev, IMusicSession ims)
        {
            CorruptedRarOrMissingPasswordArgs cmp = Ev as CorruptedRarOrMissingPasswordArgs;
            if (cmp != null)
            {
                return new RarPasswordViewModel(cmp);
            }

            AmbigueousCDInformationArgs acdia = Ev as AmbigueousCDInformationArgs;
            if (acdia != null)
            {
                return new CDAlbumDescriptorCreatorViewModel(acdia, ims);
            }

            CDCoverInformationArgs cdia = Ev as CDCoverInformationArgs;
            if (cdia != null)
            {
               return new WebAlbumFoundSelectorViewModel(cdia, ims.Strategy);
            }

            return new ImportExportErrorEventArgsViewModel(Ev);
        }
    }
}
