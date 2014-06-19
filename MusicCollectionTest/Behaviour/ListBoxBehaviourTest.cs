using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;

using MusicCollectionWPF.Infra;
using MusicCollectionWPF.Infra.Behaviour;
using System.Windows;
using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox.Collection.Observable;
using System.Windows.Controls;

namespace MusicCollectionTest.Behaviour
{

    [TestFixture, RequiresSTA]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("MusicCollectionWPF.Infra.Behaviour")]
    class ListBoxBehaviourTest
    {
        private Window _window;
        private WPFTester _wpfTester;
        private ListBox _listBlock;
        private WrappedObservableCollection<string> _Binded;

        [SetUp]
        [STAThread]
        public void Setup()
        {
            _wpfTester = new WPFTester();

            _window = new Window();
        
            StackPanel stackPanel = new StackPanel();
            NameScope.SetNameScope(_window, new NameScope());

            _window.Content = stackPanel;

            _listBlock = new ListBox();
            _listBlock.Name = "theleb";
            _listBlock.SelectionMode = SelectionMode.Multiple;  
            _listBlock.ItemsSource = new string[] { "1", "2", "3", "4" };
   
            _window.RegisterName(_listBlock.Name, _listBlock);
            stackPanel.Children.Add(_listBlock);

            _Binded = new WrappedObservableCollection<string>();

            _wpfTester.ShowWindow(_window);
        }

        [TearDown]
        public void TearDown()
        {
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, null);
            _wpfTester.Close();
        }

        [Test]
        public void SelectionShoulBeSynchronizedOnAdd()
        {
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(0);

            _listBlock.SelectedItem = "1";

            _Binded.Should().HaveCount(1);
            _Binded.Should().Equal(new string[]{"1"});

        }

        [Test]
        public void SelectionShoulBeSynchronizedOnRemove()
        {
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(0);

            _listBlock.SelectedItem = "1";

            _Binded.Should().HaveCount(1);
            _Binded.Should().Equal(new string[] { "1" });

            _listBlock.SelectedItem = null;

            _Binded.Should().HaveCount(0);
        }

        [Test]
        public void SelectionShoulBeSynchronizedOnInit()
        {
            _Binded.Add("3");
            _Binded.Add("4");
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(2);

            _listBlock.SelectedItems.Cast<string>().Should().HaveCount(2);
            _listBlock.SelectedItems.Cast<string>().Should().Equal(new string[] { "3","4" });
        }

        [Test]
        public void Selection_TwoWayBinding()
        {
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(0);

            _Binded.Add("3");

            _Binded.Should().HaveCount(1);
            _Binded.Should().Equal(new string[] { "3" });

        }

        [Test]
        public void SelectionShoulBeSynchronized_TwoWay_Clear()
        {
           
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(0); 
            
            _Binded.Add("3");
            _Binded.Add("4");

            _listBlock.SelectedItems.Cast<string>().Should().HaveCount(2);
            _listBlock.SelectedItems.Cast<string>().Should().Equal(new string[] { "3", "4" });

            _Binded.Clear();

            _listBlock.SelectedItems.Cast<string>().Should().BeEmpty();
        }

        [Test]
        public void Selection_TwoWayBinding_WithRemove()
        {
            ListBoxBehaviour.SetSelectedItemsSource(_listBlock, _Binded);
            _Binded.Should().HaveCount(0);

            _Binded.Add("3");

            _Binded.Should().HaveCount(1);
            _Binded.Should().Equal(new string[] { "3" });

            _Binded.Remove("3");

            _Binded.Should().HaveCount(0);
        }

    }
}
