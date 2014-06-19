using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.PyBindingTest
{
    [TestFixture, RequiresSTA]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("PyBinding")]
    public class PyBindingTest
    {
        private Slider _slider;
        private Window _window;
        private WPFTester _wpfTester;
        private TextBlock _textBlock;
        private PyBinding.PyBinding _target;
        
        [SetUp]
        [STAThread]
        public void Setup()
        {
            _wpfTester = new WPFTester();

            _window = new Window();
            _textBlock = new TextBlock();
            StackPanel stackPanel = new StackPanel();
            NameScope.SetNameScope(_window, new NameScope());
            
            _window.Content = stackPanel;
            
            _slider = new Slider();
            _slider.Name = "TheSlider";
            _window.RegisterName(_slider.Name, _slider);

            stackPanel.Children.Add(_textBlock);
            stackPanel.Children.Add(_slider);

            _target = new PyBinding.PyBinding();

            _wpfTester.ShowWindow(_window);
        }
        
        [TearDown]
        public void TearDown()
        {
            BindingOperations.ClearBinding(_textBlock, TextBox.TextProperty);
            _wpfTester.Close();
        }

        [Test]
        public void TextBlockText_BoundToSliderValueByName_DisplaysValue()
        {
            _target.Script = "$[TheSlider.Value]";

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual("4", _textBlock.Text);
        }

        [Test]
        public void TextBlockText_Conditional_DisplaysValue()
        {
            _target.Script = "'Greater' if $[TheSlider.Value] > 5 else 'LessThan'";

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 6;
            Assert.AreEqual("Greater", _textBlock.Text);

            _slider.Value = 5;
            Assert.AreEqual("LessThan", _textBlock.Text);
        }

        [Test]
        public void TextBlockText_DataContext_DisplaysValue()
        {
            _target.Script = "$[.]";

            _textBlock.SetBinding(TextBlock.TextProperty, _target);


            //NOTE: if you set the datacontext after you set the binding you must call DoEvents.
            //      The standard MultiBinding behaves the same way.
            
            _window.DataContext = 4;
            WPFTester.DoEvents();
            Assert.AreEqual("4", _textBlock.Text);
        }

        [Test]
        public void TextBlockText_MultipleValues_DisplaysValue()
        {
            _slider.Value = 6;
            _window.DataContext = 4;

            _target.Script = "$[TheSlider.Value] + $[.]";
 
            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            Assert.AreEqual("10", _textBlock.Text);

            _window.DataContext = 5;
            Assert.AreEqual("11", _textBlock.Text);
        }

        [Test]
        public void TextBlockText_ValueIsNull_ReturnsDefaultValue()
        {
            _target.Script = "$[.]";

            _slider.SetBinding(Slider.ValueProperty, _target);

            Assert.AreEqual(0, _slider.Value);
        }

        [Test]
        public void TextBlockText_PropertyOnSelf_DisplaysValue()
        {
            _target.Script = "$[{Self}.Tag]";

            _textBlock.Tag = "Foo";
            
            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            Assert.AreEqual("Foo", _textBlock.Text);
        }

        [Test]
        public void TextBlockText_BoundToSliderValueByName_Toway_DisplaysValue()
        {
            _target.Mode = BindingMode.TwoWay;
            _target.Script = "$[TheSlider.Value]";
            _target.ScriptBack = "Double.Parse(var_0)";
             
            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual(4.0, _slider.Value);
            Assert.AreEqual("4", _textBlock.Text);

            _textBlock.Text = "5";
            Assert.AreEqual("5", _textBlock.Text); 
            Assert.AreEqual(5, _slider.Value);
        }

        [Test]
        public void TextBlockText_BoundToSliderValueByName_Toway_DisplaysValue2()
        {          
            _target.Script = "$[TheSlider.Value]";
            _target.ScriptBack = "Double.Parse(var_0)";
            _target.Mode = BindingMode.TwoWay;

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual(4.0, _slider.Value);
            Assert.AreEqual("4", _textBlock.Text);

            _textBlock.Text = "5";
            Assert.AreEqual("5", _textBlock.Text);
            Assert.AreEqual(5, _slider.Value);
        }

        [Test]
        public void TextBlockText_BoundToSliderValueByName_Toway_Complex()
        {
            _target.Script = "String.Format('{0}-{1}',$[TheSlider.Value],$[TheSlider.Maximum])";
           _target.ScriptBack = "Test(var_0)";
                   
            _target.Mode = BindingMode.TwoWay;

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual(4.0, _slider.Value);
            Assert.AreEqual("4-10", _textBlock.Text);

            _textBlock.Text = "5-9";
            Assert.AreEqual("5-9", _textBlock.Text);
            Assert.AreEqual(5, _slider.Value);
            Assert.AreEqual(9, _slider.Maximum);

        }

        [Test]
        public void TextBlockText_BoundToSliderValueByName_Toway_Complex_NoExplicitTwoWay()
        {
            _target.Script = "String.Format('{0}-{1}',$[TheSlider.Value],$[TheSlider.Maximum])";
            _target.ScriptBack = "Test(var_0)";

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual(4.0, _slider.Value);
            Assert.AreEqual("4-10", _textBlock.Text);

            _textBlock.Text = "5-9";
            Assert.AreEqual("5-9", _textBlock.Text);
            Assert.AreEqual(5, _slider.Value);
            Assert.AreEqual(9, _slider.Maximum);

        }


        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void TextBlockText_BoundToSliderValueByName_Toway_Exception()
        {
            _target.Script = "String.Format('{0}-{1}',$[TheSlider.Value],$[TheSlider.Maximum])";
            _target.Mode = BindingMode.TwoWay;

            _textBlock.SetBinding(TextBlock.TextProperty, _target);

            _slider.Value = 4;
            Assert.AreEqual(4.0, _slider.Value);
            Assert.AreEqual("4-10", _textBlock.Text);

            _textBlock.Text = "5-9";
           
        }
    }
}
