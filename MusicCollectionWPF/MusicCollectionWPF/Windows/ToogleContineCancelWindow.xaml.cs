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
using System.Windows.Shapes;

using MusicCollection.Fundation;
using MusicCollectionWPF.Infra;


namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for ToogleContineCancelWindow.xaml
    /// </summary>
    public partial class ToogleContineCancelWindow : CustomWindow
    {
        private IToogleProvider _IMR;

        public ToogleContineCancelWindow()
        {
            InitializeComponent();
        }

        public ToogleContineCancelWindow(string Name, string ToogleMessage, IToogleProvider imr,string iIntro=null)
        {
            InitializeComponent();
            _IMR = imr;
            DataContext = _IMR;
            Title = Name;
            CheckBoxMessage.Text = ToogleMessage;
            if (iIntro != null)
            {
                Intro.Text = iIntro;
            }
            else
                Intro.Visibility = Visibility.Collapsed;

        }

        public bool? ToogleResult
        { get { return this.CheckBox1.IsChecked; } }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            if (_IMR.IsValid)
            {
                DialogResult = true;
                this.Close();
            }
        }


        private void ToogleContineCancelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeToContent = SizeToContent.Height;
        }

 
    }
}
