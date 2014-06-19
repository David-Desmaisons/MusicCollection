using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using MusicCollection.Fundation;
using MusicCollection.Infra;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumDirectoryCopy.xaml
    /// </summary>
    public partial class AlbumDirectoryCopy : UserControl
    //, IObjectAttribute, INotifyPropertyChanged, IObjectBuildAttributeListener
    {
        //private IResultListener<bool> _IsValid;

        public AlbumDirectoryCopy()
        {
            InitializeComponent();
            //_IsValid = _IsValidFactory.CreateListener(this);
        }
    }
}

        //#region ListenerInfra

        ////private ObjectModifiedArgs _Changing;

        //protected void PropertyHasChanged<Tat>(string PropertyName, Tat iold, Tat iNew)
        //{
        //    if (object.Equals(iold, iNew))
        //        return;

        //    EventHandler<ObjectModifiedArgs> o = _ObjectChanged;
        //    if (o != null)
        //    {
        //        //var Changing = new ObjectAttributeChangedArgs<Tat>(this, PropertyName, iold, iNew);
        //        o(this, new ObjectAttributeChangedArgs<Tat>(this, PropertyName, iold, iNew));

        //        //_Changing = new ObjectAttributeChangedArgs<Tat>(this, PropertyName, iold, iNew);
        //        //o(this, _Changing);
        //        //_Changing = null;
        //    }

        //    PropertyChangedEventHandler pc = PropertyChanged;
        //    if (pc != null)
        //        pc(this, new PropertyChangedEventArgs(PropertyName));

        //}

        //protected void PropertyHasChangedUIOnly(string PropertyName)
        //{
        //    PropertyChangedEventHandler pc = PropertyChanged;
        //    if (pc != null)
        //        pc(this, new PropertyChangedEventArgs(PropertyName));
        //}

        ////ObjectModifiedArgs IObjectAttribute.this[string iAttributeName]
        ////{
        ////    get
        ////    {
        ////        if (_Changing == null)
        ////            return null;

        ////        if (_Changing.AttributeName == iAttributeName)
        ////            return _Changing;

        ////        return null;
        ////    }
        ////}


        //private event EventHandler<ObjectModifiedArgs> _ObjectChanged;
        //public event EventHandler<ObjectModifiedArgs> ObjectChanged
        //{
        //    add { _ObjectChanged += value; }
        //    remove { _ObjectChanged -= value; }
        //}



        //public event PropertyChangedEventHandler PropertyChanged;

        //public void AttributeChanged<T>(string AttributeName, T oldv, T newv)
        //{
        //    PropertyHasChanged(AttributeName, oldv, newv);
        //}

        //#endregion

        //private const string _AlbumsProperty = "AllAlbums";
        //private const string _SelectedAlbumsProperty = "SelectedAlbums";
        //private const string _DirectoryTargetProperty = "Directory";
        //private const string _SizeCheckerProperty = "SizeChecker";

        //private List<IAlbum> _Albums;
        //public IEnumerable<IAlbum> AllAlbums
        //{
        //    get { return _Albums; }
        //    set
        //    {
        //        var old = _Albums;
        //        _Albums = (value == null) ? null : value.ToList();
        //        PropertyHasChanged(_AlbumsProperty, old, _Albums);
        //    }
        //}

        //private IList<IAlbum> _SelectedAlbums;
        //public IEnumerable<IAlbum> SelectedAlbums
        //{
        //    set
        //    {
        //        var old = _SelectedAlbums;
        //        _SelectedAlbums = value.ToList();


        //        PropertyHasChanged(_SelectedAlbumsProperty, old, _SelectedAlbums);
        //        SizeChecker.Albums = value;
        //    }
        //    get
        //    {
        //        return _SelectedAlbums;
        //    }
        //}

        //private string _DirectoryTargets;
        //public string Directory
        //{
        //    get { return _DirectoryTargets; }
        //    set
        //    {
        //        if (_DirectoryTargets == value)
        //            return;

        //        var old = _DirectoryTargets;

        //        _DirectoryTargets = value;
        //        SizeChecker.DirectoryName = value;

        //        PropertyHasChanged(_DirectoryTargetProperty, old, _DirectoryTargets);
        //    }
        //}

        //private void ItemSourceChanged(object sender, DataTransferEventArgs e)
        //{
        //    if (AlbumSelector.ItemsSource == null)
        //        return;

        //    //foreach (IAlbum al in AlbumSelector.ItemsSource)
        //    //    AlbumSelector.SelectedItems.Add(al);

        //    foreach (object al in AlbumSelector.ItemsSource)
        //        AlbumSelector.SelectedItems.Add(al);
        //}

        //private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (AlbumSelector.SelectedItems == null)
        //        return;

        //    SelectedAlbums = AlbumSelector.SelectedItems.Cast<IAlbum>();
        //}

        //static IResultListenerFactory<AlbumDirectoryCopy, bool> _IsValidFactory;

        //static AlbumDirectoryCopy()
        //{
        //    _IsValidFactory =
        //        ListenerFunctionBuilder.BuildFunctionListenerFactory<AlbumDirectoryCopy, bool>
        //        (t => ((t.SizeChecker.SpaceCheck != null) && (t.SizeChecker.SpaceCheck.OK)), "IsValid");
        //}


        //public bool IsValid
        //{
        //    get
        //    {
        //        return _IsValid.Value;
        //    }
        //}

        //private AlbumListSizeChecker _ALSC;
        //public AlbumListSizeChecker SizeChecker
        //{
        //    get
        //    {
        //        if (_ALSC == null)
        //        {
        //            _ALSC = new AlbumListSizeChecker(Directory);
        //        }
        //        return _ALSC;
        //    }
        //}
//    }
//}
