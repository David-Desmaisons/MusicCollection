﻿using System;
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
    /// Interaction logic for PathChooser.xaml
    /// </summary>
    public partial class PathChooser : UserControl
    {
        public PathChooser()
        {
            InitializeComponent();
        }

        public string[] FilesPath
        {
            get { return (string[])GetValue(FilesPathProperty); }
            set { SetValue(FilesPathProperty, value); }
        }

        public static readonly DependencyProperty FilesPathProperty = DependencyProperty.Register("FilesPath", typeof(string[]), typeof(PathChooser));

        public string File
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty = DependencyProperty.Register("File", typeof(string), typeof(PathChooser));


        public bool Multiselection
        {
            get { return (bool)GetValue(MultiselectionProperty); }
            set { SetValue(MultiselectionProperty, value); }
        }

        public static readonly DependencyProperty MultiselectionProperty = DependencyProperty.Register("Multiselection", typeof(bool), typeof(PathChooser));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(PathChooser));

        public string OriginalDirectory
        {
            get { return (string)GetValue(OriginalDirectoryProperty); }
            set { SetValue(OriginalDirectoryProperty, value); }
        }

        public static readonly DependencyProperty OriginalDirectoryProperty = DependencyProperty.Register("OriginalDirectory", typeof(string), typeof(PathChooser));

    }
}
