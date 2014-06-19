using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using NSubstitute;
using NUnit;
using NUnit.Framework;

using MusicCollectionWPF.ViewModel;
using MusicCollection.Fundation;
using MusicCollection.ToolBox.ZipTools;
using System.Windows.Input;
using MusicCollection.Infra.File;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionTest.ViewModelTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    public class ExportDiscogsKeyViewModelTestor
    {
        private ExportKeyViewModel _Target;
        private IDiscogsAuthentificationProvider _IWebServicesSettings;
        private IInfraDependencies _Infra;
        private IWindow _IWindow;

        [SetUp]
        public void SetUp()
        {
            _IWindow = Substitute.For<IWindow>();
            _IWebServicesSettings = Substitute.For<IDiscogsAuthentificationProvider>();
            _Infra = Substitute.For<IInfraDependencies>();
            _Target = new ExportKeyViewModel(_IWebServicesSettings, _Infra) { Window = _IWindow };
        }

        [TearDown]
        public void TD()
        {
            _Target.Dispose();
        }

        [Test]
        public void TestCommand()
        {
            _IWebServicesSettings = Substitute.For<IDiscogsAuthentificationProvider>();
            _Infra = Substitute.For<IInfraDependencies>();
            FileSystem fs = new FileSystem();
            _Infra.File.Returns(fs);

            _Target = new ExportKeyViewModel(_IWebServicesSettings, _Infra);
    
            ICommand com = _Target.Commit;
            com.MonitorEvents();

            com.Should().NotBeNull();
            com.CanExecute(null).Should().BeTrue();

            com.ShouldNotRaise("CanExecuteChanged");

            _Target.Directory = "24";

            com.CanExecute(null).Should().BeFalse();
            com.ShouldRaise("CanExecuteChanged");
   
            _Target.Directory = @"C:\";

            com.CanExecute(null).Should().BeTrue();
            com.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public void TestPassword()
        {
            _Target.MonitorEvents();
            _Target.Password.Should().BeEmpty();

            _Target.Password = "alibaba";
            _Target.Password.Should().Be("alibaba");
            _Target.ShouldRaisePropertyChangeFor(t => t.Password);
        }

        [Test]
        public void Test_Directory_2()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;
            commit.MonitorEvents();
            _Target.Directory = "toto";


            _Infra.File.Received().DirectoryExists("toto");
            commit.CanExecute(null).Should().BeTrue();
            commit.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public void Test_Commit_OK()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password="asticot";

            Dictionary<string,string> d = new Dictionary<string,string>();
            _Infra.File.CreateNewAvailableName("toto" , "MusicCollectionkeys", Arg.Any<string>(), true).Returns("mam23");
            _IWebServicesSettings.GetPrivateKeys().Returns(d);
            _Infra.Zip.ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot").Returns(Task.FromResult<bool>(true));
            commit.Execute(null);

            _IWindow.Received().Close();
            _Infra.File.Received().CreateNewAvailableName("toto", "MusicCollectionkeys", Arg.Any<string>(), true);
            _IWebServicesSettings.Received().GetPrivateKeys();
            _Infra.Zip.Received().ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot");
            _IWindow.Received().ShowMessage("Keys exported successfully to mam23!", "Exporting Discogs Key", false);
        }

        [Test]
        public void Test_Commit_OK_2()
        {
            //IMusicSession ims = Substitute.For<IMusicSession>();
            //ims.Setting.WebUserSettings.Returns(_IWebServicesSettings);
            //ims.Dependencies.Returns(_Infra);

            _Target = new ExportKeyViewModel(_IWebServicesSettings, _Infra) { Window = _IWindow };

            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";

            Dictionary<string, string> d = new Dictionary<string, string>();
            _Infra.File.CreateNewAvailableName("toto", "MusicCollectionkeys", Arg.Any<string>(), true).Returns("mam23");
            _IWebServicesSettings.GetPrivateKeys().Returns(d);
            _Infra.Zip.ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot").Returns(Task.FromResult<bool>(true));
            commit.Execute(null);

            _IWindow.Received().Close();
            _Infra.File.Received().CreateNewAvailableName("toto", "MusicCollectionkeys", Arg.Any<string>(), true);
            _IWebServicesSettings.Received().GetPrivateKeys();
            _Infra.Zip.Received().ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot");
            _IWindow.Received().ShowMessage("Keys exported successfully to mam23!", "Exporting Discogs Key", false);
        }

        [Test]
        public void Test_Commit_KO()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";

            Dictionary<string, string> d = new Dictionary<string, string>();
            _Infra.File.CreateNewAvailableName("toto", "MusicCollectionkeys", Arg.Any<string>(), true).Returns("mam23");
            _IWebServicesSettings.GetPrivateKeys().Returns(d);
            _Infra.Zip.ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot").Returns(Task.FromResult<bool>(false));
            commit.Execute(null);

            _IWindow.Received().Close();
            _Infra.File.Received().CreateNewAvailableName("toto", "MusicCollectionkeys", Arg.Any<string>(), true);
            _IWebServicesSettings.Received().GetPrivateKeys();
            _Infra.Zip.Received().ZippAsync(Arg.Any<IEnumerable<string>>(), "mam23", "asticot");
            _IWindow.Received().ShowMessage("Unable to export keys!", "Exporting Discogs Key", false);
        }

           //bool res = await _Infra.Zip.SerializeSafeAsync(_IWebServicesSettings.GetPrivateKeys(), filepath, Password);

           // string Message = res ? string.Format("Keys exported successfully to {0}!",filepath) : "Unable to export keys!";
           // window.ShowMessage(Message, "Exporting Discogs Key", false);

    }
}
