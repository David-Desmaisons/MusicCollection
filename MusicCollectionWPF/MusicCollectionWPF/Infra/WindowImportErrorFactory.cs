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
    static internal class  WindowFactory
    {
        internal static IWindow GetWindowFromImporterror(ImportExportErrorEventArgs Ev, IMusicSession ims)
        {
            CorruptedRarOrMissingPasswordArgs cmp = Ev as CorruptedRarOrMissingPasswordArgs;
            if (cmp != null)
            {
                return new RarPasswordWindow(cmp);
            }

            AmbigueousCDInformationArgs acdia = Ev as AmbigueousCDInformationArgs;
            if (acdia != null)
            {
                var cdinfos = new CDAlbumDescriptorCreatorViewModel(acdia,ims);
                //return new CDImportInfoEditor(cdinfos);
                return new CDImportInfoEditor() { ModelView = cdinfos };
            }

            CDCoverInformationArgs cdia = Ev as CDCoverInformationArgs;
            if (cdia != null)
            {
                WebAlbumFoundSelectorViewModel waw = new WebAlbumFoundSelectorViewModel(cdia, ims.Strategy);
                return new InternetResultWindow() { ModelView = waw };
                //return new InternetResultWindow(cdia);
            }

            return new CustoMessageBox(Ev);
        }
    }
}
