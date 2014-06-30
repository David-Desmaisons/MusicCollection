using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollectionWPF.ViewModelHelper;
using NUnit.Framework;
using FluentAssertions;
using NSubstitute;
using MusicCollectionTest.TestObjects;
using MusicCollectionWPF.ViewModel;

namespace MusicCollectionTest.ViewModelHelper
{
    public class vm : ViewModelBase
    {
    }

    public class vm2 : vm
    {
    } 
    
    public class vm3 : ViewModelBase
    {
    }

    public class vm4 : ViewModelBase
    {
    }

    [ViewModelBinding(typeof(vm))]
    public class MyWindowBinded : IWindow
    {
        public bool? ShowDialog()
        {
            throw new NotImplementedException();
        }

        public bool ShowConfirmationMessage(string iMessage, string iTitle)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string iMessage, string iTitle, bool iBlocking)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IWindow CreateFromViewModel(ViewModelBase iModelViewBase)
        {
            throw new NotImplementedException();
        }

        public ViewModelBase ModelView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string ChooseFile(string iTitle, string Extension)
        {
            throw new NotImplementedException();
        }


        public IWindow LogicOwner
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool ShowInTaskbar
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CenterScreenLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event System.Windows.RoutedEventHandler Loaded { add { } remove { } }

        public event System.ComponentModel.CancelEventHandler Closing { add { } remove { } }

        public bool? ShowDialog(bool AddEffect = true)
        {
            throw new NotImplementedException();
        }


        public void Show()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<string> ChooseFiles(string iTitle, string Extension,string Directory=null)
        {
            throw new NotImplementedException();
        }
    }

   
    [ViewModelBinding(typeof(vm3))]
    public class MyWindowBinded2 : IWindow
    {
        public bool? ShowDialog()
        {
            throw new NotImplementedException();
        }

        public bool ShowConfirmationMessage(string iMessage, string iTitle)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string iMessage, string iTitle, bool iBlocking)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IWindow CreateFromViewModel(ViewModelBase iModelViewBase)
        {
            throw new NotImplementedException();
        }

        public ViewModelBase ModelView
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string ChooseFile(string iTitle, string Extension)
        {
            throw new NotImplementedException();
        }


        public IWindow LogicOwner
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool ShowInTaskbar
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CenterScreenLocation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event System.Windows.RoutedEventHandler Loaded { add { } remove { } }

        public event System.ComponentModel.CancelEventHandler Closing { add { } remove { } }

        public bool? ShowDialog(bool AddEffect = true)
        {
            throw new NotImplementedException();
        }


        public void Show()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<string> ChooseFiles(string iTitle, string Extension, string Directory = null)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ViewModel")]
    public class ViewModelBinderTestor
    {
        private ViewModelBinder _Target;
        [SetUp]
        public void SetUp()
        {
            _Target = new ViewModelBinder(this.GetType().Assembly);
        }

        [Test]
        public void Test_OK_Simple()
        {
            var myvm = new vm();
            IWindow res = _Target.Solve(myvm);
            res.Should().NotBeNull();
            (res is MyWindowBinded).Should().BeTrue();
        }

        [Test]
        public void Test_OK_Simple_Inheritence()
        {
            var myvm = new vm2();
            IWindow res = _Target.Solve(myvm);
            res.Should().NotBeNull();
            (res is MyWindowBinded).Should().BeTrue();
        }

        [Test]
        public void Test_OK_Simple_NoFound()
        {
            var myvm = new vm4();
            IWindow res = _Target.Solve(myvm);
            res.Should().BeNull();
        }

        [Test]
        public void Test_KO_Argument()
        {
            IWindow res = null;
            Action wt = () => res = _Target.Solve(null);
            wt.ShouldThrow<ArgumentNullException>();
        }
    }
}
