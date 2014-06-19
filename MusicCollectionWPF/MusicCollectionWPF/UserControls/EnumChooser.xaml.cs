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
using System.Diagnostics;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for EnumChooser.xaml
    /// </summary>
    public partial class EnumChooser : UserControl
    {
        public EnumChooser()
        {
            InitializeComponent();
        }

        #region Dependency Property

        public object EnumChoosed
        {
            get { return (object)GetValue(EnumChoosedColorProperty); }
            set { SetValue(EnumChoosedColorProperty, value); }
        }
        public static readonly DependencyProperty EnumChoosedColorProperty = DependencyProperty.Register("EnumChoosed", typeof(object), typeof(EnumChooser), new PropertyMetadata(EnumValueChangedCallback));


        public Array ItemsSource
        {
            get { return (Array)GetValue(ItemsProperty); }
            private set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("ItemsSource", typeof(Array), typeof(EnumChooser));

       
        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumChooser), new PropertyMetadata(EnumTypeChangedCallback));


        private static void EnumTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumChooser ec = d as EnumChooser;
            ec.ChangedType(e.NewValue as Type);
        }

        private void ChangedType(Type enumtype)
        {
            if (!enumtype.IsEnum)
            {
                Trace.WriteLine("EnumChooser error should be an enum type!!");
                return;
            }

            ItemsSource = Enum.GetValues(enumtype);

        }

        private static void EnumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumChooser ec = d as EnumChooser;
            ec.ChangedValue(e.NewValue);
        }

        private void ChangedValue(object enumtype)
        {
            if (EnumType!=null)
                return;

            EnumType = enumtype.GetType();

        }


        

        #endregion

    }
}
