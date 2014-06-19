using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for AttributeListEditor.xaml
    /// </summary>
    public partial class AttributeListEditor : UserControl
    {
        public AttributeListEditor()
        {
            InitializeComponent();
        }

        public string AtributeName
        {
            get { return (string)GetValue(AtributeNameProperty); }
            set { SetValue(AtributeNameProperty, value); }
        }

        public static readonly DependencyProperty AtributeNameProperty =
            DependencyProperty.Register("AtributeName", typeof(string), typeof(AttributeListEditor));

        public IList Value
        {
            get { return (IList)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(IList), typeof(AttributeListEditor));

        //public IEnumerable ItemsSource
        //{
        //    get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //}

        //public static readonly DependencyProperty ItemsSourceProperty =
        //    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AttributeListEditor));

        public static readonly DependencyProperty SearchableFactoryProperty = DependencyProperty.Register("SearchableFactory", typeof(ISearchableFactory),
           typeof(AttributeListEditor));

        public ISearchableFactory SearchableFactory
        {
            get { return (ISearchableFactory)GetValue(SearchableFactoryProperty); }
            set { SetValue(SearchableFactoryProperty, value); }
        }
    }
}
