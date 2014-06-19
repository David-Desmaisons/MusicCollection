using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PyBinding;
using System.Windows.Data;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.PyBindingTest
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("PyBinding")]
    public class BindingPathParserTest
    {
        private WPFTester _wpfTester;

        [SetUp]
        public void Setup()
        {

            _wpfTester = new WPFTester();
        }
    
        [Test]
        public void BuildBinding_DirectToDataContext_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding(".");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);
            Assert.IsNull(binding.Path);
            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_DataContextWithProperty_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding(".Foo");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);

            Assert.AreEqual("Foo", binding.Path.Path);

            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_DataContextWithTwoLevelProperty_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding(".Foo.Bar");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);

            Assert.AreEqual("Foo.Bar", binding.Path.Path);

            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_DataContextWithIndexedProperty_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding(".Foo[3].Bar");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);

            Assert.AreEqual("Foo[3].Bar", binding.Path.Path);

            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_TemplateParent_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("{TemplatedParent}");

            Assert.IsNull(binding.Source);
            Assert.AreEqual(RelativeSource.TemplatedParent, binding.RelativeSource);

            Assert.IsNull(binding.Path);

            Assert.IsNull(binding.ElementName);
        }


        [Test]
        public void BuildBinding_Indexer()
        {
            Binding binding = BindingPathParser.BuildBinding(".Images[0]");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);
            Assert.AreEqual(binding.Path.Path, "Images[0]");
            Assert.IsNull(binding.ElementName);

        }


        [Test]
        public void BuildBinding_CurrentItem()
        {
            Binding binding = BindingPathParser.BuildBinding(".Images/");

            Assert.IsNull(binding.Source);
            Assert.IsNull( binding.RelativeSource);
            Assert.AreEqual(binding.Path.Path, "Images/");
            Assert.IsNull(binding.ElementName);


            binding = BindingPathParser.BuildBinding("Album.Images/");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);
            Assert.AreEqual(binding.Path.Path, "Images/");
            Assert.AreEqual(binding.ElementName, "Album");



            binding = BindingPathParser.BuildBinding("Album.Images/.Size");

            Assert.IsNull(binding.Source);
            Assert.IsNull(binding.RelativeSource);
            Assert.AreEqual(binding.Path.Path, "Images/.Size");
            Assert.AreEqual(binding.ElementName, "Album");

        }


        [Test]
        public void BuildBinding_FindAncestor_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("{FindAncestor[System.Int32]}");

            Assert.IsNull(binding.Source);
            Assert.AreEqual(RelativeSourceMode.FindAncestor, binding.RelativeSource.Mode);
            Assert.AreEqual(typeof(int), binding.RelativeSource.AncestorType);
            Assert.AreEqual(1, binding.RelativeSource.AncestorLevel);
            Assert.IsNull(binding.Path);
            Assert.IsNull(binding.ElementName);


            binding = BindingPathParser.BuildBinding("{FindAncestor[ MusicCollectionTest.PyBindingTest.BindingPathParserTest,3]}");

            Assert.IsNull(binding.Source);
            Assert.AreEqual(RelativeSourceMode.FindAncestor, binding.RelativeSource.Mode);
            Assert.AreEqual(this.GetType(), binding.RelativeSource.AncestorType);
            Assert.AreEqual(3, binding.RelativeSource.AncestorLevel);
            Assert.IsNull(binding.Path);
            Assert.IsNull(binding.ElementName);
        
        }

        [Test]
        public void BuildBinding_Self_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("{Self}");

            Assert.IsNull(binding.Source);
            Assert.AreEqual(RelativeSource.Self, binding.RelativeSource);
            
            Assert.IsNull(binding.Path);

            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_SelfWithProperty_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("{Self}.Foo");

            Assert.IsNull(binding.Source);
            Assert.AreEqual(RelativeSource.Self, binding.RelativeSource);

            Assert.AreEqual("Foo", binding.Path.Path);

            Assert.IsNull(binding.ElementName);
        }

        [Test]
        public void BuildBinding_ElementName_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("ControlName");

            Assert.IsNull(binding.Source);
            
            Assert.IsNull(binding.Path);

            Assert.AreEqual("ControlName", binding.ElementName);
        }

        [Test]
        public void BuildBinding_ElementNameWithProperty_ReturnsCorrectBinding()
        {
            Binding binding = BindingPathParser.BuildBinding("ControlName.Foo");

            Assert.IsNull(binding.Source);

            Assert.AreEqual("Foo", binding.Path.Path);

            Assert.AreEqual("ControlName", binding.ElementName);
        }

        [Test]
        public void SetupPythonScriptWithReplacement_OneBindingPaths_ReplacesPaths()
        {
            string result = BindingPathParser.SetupPythonScriptWithReplacement("$[.]", PythonEvaluator.VariablePrefix);
            Assert.AreEqual("var_0", result);
        }

        [Test]
        public void SetupPythonScriptWithReplacement_ConditionalStatement_ReplacesPaths()
        {
            string result = BindingPathParser.SetupPythonScriptWithReplacement("$[.] if $[.Age] > 100 else $[Foo]", PythonEvaluator.VariablePrefix);
            Assert.AreEqual("var_0 if var_1 > 100 else var_2", result);
        }
    }
}