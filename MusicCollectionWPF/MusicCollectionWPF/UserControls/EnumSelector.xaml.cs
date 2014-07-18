using PyBinding;
using System;
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
    /// Interaction logic for EnumSelector.xaml
    /// </summary>
    public partial class EnumSelector : UserControl
    {
        public EnumSelector()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                PythonEvaluator pe = new PythonEvaluator();
            }
            InitializeComponent();
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EnumSelector));


        public Array ItemsSource
        {
            get { return (Array)GetValue(ItemsProperty); }
            private set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("ItemsSource", typeof(Array), typeof(EnumSelector));


        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumSelector), new PropertyMetadata(EnumTypeChangedCallback));


        private static void EnumTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumSelector ec = d as EnumSelector;
            ec.ItemsSource = Enum.GetValues(e.NewValue as Type);
        }

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(EnumSelector), new PropertyMetadata(50D));


    }
}
