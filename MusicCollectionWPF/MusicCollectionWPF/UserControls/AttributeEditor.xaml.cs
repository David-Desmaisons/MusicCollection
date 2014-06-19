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
    public partial class AttributeEditor : UserControl
    {
        public AttributeEditor()
        {
            InitializeComponent();
        }

        public string AtributeName
        {
            get { return (string)GetValue(AtributeNameProperty); }
            set { SetValue(AtributeNameProperty, value); }
        }

        public static readonly DependencyProperty AtributeNameProperty =
            DependencyProperty.Register("AtributeName", typeof(string), typeof(AttributeEditor));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(AttributeEditor));

    }
}
