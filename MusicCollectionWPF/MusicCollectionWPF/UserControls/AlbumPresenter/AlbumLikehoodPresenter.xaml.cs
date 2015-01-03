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
using System.Collections;
using System.Windows.Threading;

using MusicCollection.Fundation;
using MusicCollection.Infra;

using MusicCollectionWPF.CustoPanel;
using MusicCollectionWPF.Infra;
using System.Threading.Tasks;
using MusicCollection.Infra.Collection;
using MusicCollectionWPF.ViewModel.Element;

namespace MusicCollectionWPF.UserControls.AlbumPresenter
{
    /// <summary>
    /// Interaction logic for AlbumLikehoodPresenter.xaml
    /// </summary>
    /// 

    public partial class AlbumLikehoodPresenter : AlbumPresenterBase
    {
        public AlbumLikehoodPresenter()
        {
            InitializeComponent();
        }
    }
}
