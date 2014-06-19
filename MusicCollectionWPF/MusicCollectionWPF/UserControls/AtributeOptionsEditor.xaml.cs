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
    /// Interaction logic for AtributeOptionsEditor.xaml
    /// </summary>
    public partial class AtributeOptionsEditor : UserControl
    {
        public AtributeOptionsEditor()
        {
            InitializeComponent();
        }

        public string AtributeName
        {
            get { return (string)GetValue(AtributeNameProperty); }
            set { SetValue(AtributeNameProperty, value); }
        }

        public static readonly DependencyProperty AtributeNameProperty =
            DependencyProperty.Register("AtributeName", typeof(string), typeof(AtributeOptionsEditor));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(AtributeOptionsEditor));


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AtributeOptionsEditor));

        public IFactory Factory
        {
            get { return (IFactory)GetValue(FactoryProperty); }
            set { SetValue(FactoryProperty, value); }
        }
        public static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(IFactory), typeof(AtributeOptionsEditor));

    }
}
