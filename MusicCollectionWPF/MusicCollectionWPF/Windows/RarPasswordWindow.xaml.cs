﻿using System;
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
using MusicCollectionWPF.ViewModelHelper;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionWPF.Windows
{
    /// <summary>
    /// Interaction logic for RarPasswordWindow.xaml
    /// </summary>
    ///  
    [ViewModelBinding(typeof(RarPasswordViewModel))]
    public partial class RarPasswordWindow : CustomWindow
    {
        public RarPasswordWindow()
        {
            InitializeComponent();
        }
    }
}
