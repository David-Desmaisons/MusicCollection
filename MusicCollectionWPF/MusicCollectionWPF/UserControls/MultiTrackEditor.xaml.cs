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

using MusicCollectionWPF.Infra;
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel;


namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for MultiTrackEditor.xaml
    /// </summary>
    public partial class MultiTrackEditor : UserControl, IDisposable
    {
        public MultiTrackEditor()
        {
            InitializeComponent();

            var Same = FactoryBuilder.Instanciate((n) => n);

            NameCombo.Factory = Same;
            AuthorCombo.Factory = Same;

            YearCombo.Factory = FactoryBuilder.Instanciate((n) => IntConvert(n));
        }

        //private object Same(string Entry)
        //{
        //    return Entry;
        //}

        private object IntConvert(string Entry)
        {
            int res = 0;
            int.TryParse(Entry, out res);
            return res;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            End(false);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            End(true);
        }

        internal event EventHandler<EndEvent> Finalize;

        private void End(bool OK)
        {
            Finalize(this, new EndEvent(OK));
        }

        private IEnumerable<IDisposable> ToBecleaned
        {
            get
            {
                return Root.Children.Cast<UIElement>().Select(c => c as IDisposable).Where(d => d != null);
            }
        }

        public void Dispose()
        {
            ToBecleaned.Apply(t => t.Dispose());
        }

    }
}
