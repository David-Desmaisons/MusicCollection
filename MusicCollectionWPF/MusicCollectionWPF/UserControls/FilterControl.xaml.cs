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

using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        public FilterControl()
        {
            InitializeComponent();
        }

        public FilterControl(ISharpFilterTypeIndependant filter)
            : this()
        {
            Filter = filter;
        }

        private ISharpFilterTypeIndependant _Filter;
        public ISharpFilterTypeIndependant Filter
        {
            get { return _Filter; }
            set { _Filter = value; DataContext = value; }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Filter != null)
                Filter.Reset();
        }
    }
}
