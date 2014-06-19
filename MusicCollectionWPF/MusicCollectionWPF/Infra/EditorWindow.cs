using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Fundation;

using MusicCollectionWPF.Windows;
using System.Windows;
using System.ComponentModel;

namespace MusicCollectionWPF.Infra
{
    internal interface IWindowEditor
    {
        bool? ShowDialog();

        bool IsEditing { get; }

        event EventHandler<EventArgs> EndEdit;

        event EventHandler<ImportExportErrorEventArgs> Error;

        event RoutedEventHandler Loaded;

        event CancelEventHandler Closing;
    }

    class NoEditWindow : CustoMessageBox, IWindowEditor
    {
        public NoEditWindow()
            : base("Album is currently being modified. Please try laster.","Impossible to Edit Disc",false)
        {
        }

        public bool IsEditing
        {
            get { return false; }
        }

        public event EventHandler<EventArgs> EndEdit
        {
            add { } remove { }
        }


        public event EventHandler<ImportExportErrorEventArgs> Error
        {
            add { } remove{}
        }
    }




    


    internal class EditorWindow
    {
        internal static IWindowEditor FromEntities(IEnumerable<IObjectAttribute> entities, IMusicSession ims, Window main)
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
                        var res =  new DiscEditor(IAM, ims);
                        res.Owner = main;
                        return res;
                    }

                    var resnw = new NoEditWindow();
                    resnw.Owner = main;
                    return resnw;
                }
                //return null;
            }

            if (ent is IAlbum)
            {
                 var resm = new MultiTrackEditorWindow(ims, entities.Cast<IAlbum>());
                 resm.Owner = main;
                 return resm;
            }

            if (ent is ITrack)
            {
                var resmt = new MultiTrackEditorWindow(ims, entities.Cast<ITrack>());
                resmt.Owner = main;
                return resmt;
            }

            return null;
        }
    }
}
