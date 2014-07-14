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

using MusicCollection.Fundation;
using MusicCollectionWPF.Windows;


namespace MusicCollectionWPF.UserControls
{
    /// <summary>
    /// Interaction logic for SettingEditor.xaml
    /// </summary>
    public partial class SettingEditor : UserControl
    {
        public SettingEditor()
        {

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
            }
        }  
    }
}
