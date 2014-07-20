using System;
using System.Collections;
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
using MusicCollection.Infra;
using MusicCollectionWPF.ViewModel;
using MusicCollectionWPF.Infra;

namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ArtistFinder.xaml
    /// </summary>
    public partial class ArtistFinder : UserControl
    {
        public ArtistFinder()
        {
            InitializeComponent();
        }

        public ISearchableFactory SF
        {
            get;
            set;
        }

        public event EventHandler<ObjectModifiedArgs> ObjectChanged;

        public IList ItemOptions
        {
            get { return (IList)GetValue(ItemOptionsProperty); }
            private set { SetValue(ItemOptionsProperty, value); }
        }
        public static readonly DependencyProperty ItemOptionsProperty = DependencyProperty.Register("ItemOptions", typeof(IList), typeof(ArtistFinder), new PropertyMetadata(null));

        #region SelectItem

        public static readonly DependencyProperty SelectItemProperty = DependencyProperty.Register("SelectItem", typeof(object), typeof(ArtistFinder), new PropertyMetadata(SelectItemPropertyCallback));
        public object SelectItem
        {
            get { return (object)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        public static readonly DependencyProperty OptionMaxWidthProperty = DependencyProperty.Register("OptionMaxWidth", typeof(double), typeof(ArtistFinder), new PropertyMetadata(double.PositiveInfinity));
        public double OptionMaxWidth
        {
            get { return (double)GetValue(OptionMaxWidthProperty); }
            set { SetValue(OptionMaxWidthProperty, value); }
        }

        static private void SelectItemPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ArtistFinder).ArtistChanged(e);
        }

        private void ArtistChanged(DependencyPropertyChangedEventArgs e)
        {
            Texter.Text = string.Empty;
            if (ObjectChanged != null)
            {
                ObjectChanged(this, new ObjectModifiedArgs(this, "SelectItem", e.OldValue, e.NewValue));
            }
        }

        #endregion

        public void Commit()
        {
            Commit(Texter.Text);
        }

        private void Commit(string t, string inewstring =null)
        {
            if (string.IsNullOrWhiteSpace(t))
                return;

            SelectItem = this.SF.CreateFromName(t);

            Texter.Text = inewstring??string.Empty;
        }


        private void Texter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string t = this.Texter.Text;
            if (string.IsNullOrEmpty(t))
                return;

            if (t.Length == 1)
                return;

            this.Options.SelectedIndex = -1;
            ItemOptions = SF.PossibilitiesFromClue(t);
        }

        private void Texter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    Commit(Texter.Text);
                    e.Handled = true;
                    break;

                case Key.Oem1:
                case Key.Oem2:
                    e.Handled = true;

                    if (Texter.CaretIndex == Texter.Text.Length)
                        return;

                    string Fs = Texter.Text.Remove(Texter.CaretIndex);
                    if (string.IsNullOrWhiteSpace(Fs))
                        return;

                    int startsegondstring = (Texter.SelectionStart == Texter.CaretIndex) ? Texter.CaretIndex + Texter.SelectionLength : Texter.CaretIndex;

                    string Ls = Texter.Text.Substring(startsegondstring);
                    if (string.IsNullOrWhiteSpace(Ls))
                        return;

                    Commit(Fs, Ls);

                    break;

            }
        }

        private void Texter_LostFocus(object sender, RoutedEventArgs e)
        {
            //GetWindow
            Window parentWindow = Window.GetWindow(this);

            //Get new object with focus
            var element = FocusManager.GetFocusedElement(parentWindow) as ListBoxItem;

            //check if element is a listboxitem from popup
            if (element != null)
            {
                ListBox lb = element.FindAncestor<ListBox>();

                //Check if an element has been checked from Options listbox
                if (lb == this.Options)
                    return;
            }

            //if we are here then the focus is elsewhere in the window I can commit
            Commit();
        }
    }
}
