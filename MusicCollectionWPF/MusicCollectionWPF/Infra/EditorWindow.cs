using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.Windows;
using System.Windows;
using System.ComponentModel;
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.Infra
{
    internal static class EditorViewModelFactory
    {
        private static bool CheckAlbums(IEnumerable<IAlbum> alls)
        {
            return alls.All(al => al.IsModifiable);
        }

        private static ViewModelBase BusyMV()
        {
            return new InfoViewModel()
                        {
                            Title="Impossible to Edit Disc",
                            Message="Album is currently being modified. Please try laster."
                        };
        }

        internal static ViewModelBase FromEntities(IEnumerable<IObjectAttribute> entities, IMusicSession ims)
        {
            int count = entities.Count();

            if (count == 0)
                return null;

            IObjectAttribute ent = entities.First();

            if (count == 1)
            {
                IAlbum al = ent as IAlbum;
                if (al != null)
                {
                    IModifiableAlbum IAM = al.GetModifiableAlbum();
                    if (IAM != null)
                    {
                        return new AlbumEditorViewModel(ims, IAM);
                    }

                    return BusyMV();
                }
            }

      
            if (ent is IAlbum)
            {
                var all = entities.Cast<IAlbum>();
                if (!CheckAlbums(all))
                    return BusyMV();

                return new MusicEntitiesEditorViewModel(ims, all);
            }

            if (ent is ITrack)
            { 
                var trcs = entities.Cast<ITrack>();
                if (!CheckAlbums(trcs.Select(t=>t.Album).Distinct()))
                    return BusyMV();

                return new MusicEntitiesEditorViewModel(ims, trcs);
            }

            return null;
        }

    }
}
