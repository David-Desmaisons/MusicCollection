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
    /// Interaction logic for AttributeEditor.xaml
    /// </summary>
    public partial class AttributeEnumEditor : UserControl
    {
        public AttributeEnumEditor()
        {
            InitializeComponent();
        }

        public string AtributeName
        {
            get { return (string)GetValue(AtributeNameProperty); }
            set { SetValue(AtributeNameProperty, value); }
        }

        public static readonly DependencyProperty AtributeNameProperty =
            DependencyProperty.Register("AtributeName", typeof(string), typeof(AttributeEnumEditor));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(AttributeEnumEditor));

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(AttributeEnumEditor));

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(AttributeEnumEditor), new PropertyMetadata(50D));

    }
}
