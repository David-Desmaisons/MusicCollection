using MusicCollectionWPF.Infra;
using MusicCollectionWPF.ViewModel;
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

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ItemizedListWithOptions.xaml
    /// </summary>
    public partial class AttributeListWithOptionsEditor : UserControl
    {
        public AttributeListWithOptionsEditor()
        {
            InitializeComponent();
        }

        public string AtributeName
        {
            get { return (string)GetValue(AtributeNameProperty); }
            set { SetValue(AtributeNameProperty, value); }
        }

        public static readonly DependencyProperty AtributeNameProperty =
            DependencyProperty.Register("AtributeName", typeof(string), typeof(AttributeListWithOptionsEditor));

        public IList Value
        {
            get { return (IList)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(IList), typeof(AttributeListWithOptionsEditor));


        public static readonly DependencyProperty SearchableFactoryProperty = DependencyProperty.Register("SearchableFactory", typeof(ISearchableFactory),
           typeof(AttributeListWithOptionsEditor));

        public ISearchableFactory SearchableFactory
        {
            get { return (ISearchableFactory)GetValue(SearchableFactoryProperty); }
            set { SetValue(SearchableFactoryProperty, value); }
        }

        public IEnumerable ItemsOptionsSource
        {
            get { return (IEnumerable)GetValue(ItemsOptionsSourceProperty); }
            set { SetValue(ItemsOptionsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsOptionsSourceProperty =
            DependencyProperty.Register("ItemsOptionsSource", typeof(IEnumerable), typeof(AttributeListWithOptionsEditor));
    }
}
