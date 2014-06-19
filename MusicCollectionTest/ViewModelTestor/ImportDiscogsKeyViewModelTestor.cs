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
    public class ImportDiscogsKeyViewModelTestor
    {
        private ImportKeyViewModel _Target;
        private IDiscogsAuthentificationProvider _IWebServicesSettings;
        private IInfraDependencies _Infra;
        private IWindow _IWindow;

        [SetUp]
        public void SetUp()
        {
            _IWindow = Substitute.For<IWindow>();
            _IWebServicesSettings = Substitute.For<IDiscogsAuthentificationProvider>();
            _Infra = Substitute.For<IInfraDependencies>();
            _Target = new ImportKeyViewModel(_IWebServicesSettings, _Infra) { Window = _IWindow };
        }

        [TearDown]
        public void TD()
        {
            _Target.Dispose();
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
        public void Test_File_2()
        {
            _Infra.File.FileExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;
            commit.MonitorEvents();
            _Target.FilePath = "toto";


            _Infra.File.Received().FileExists("toto");
            commit.CanExecute(null).Should().BeTrue();
            commit.ShouldRaise("CanExecuteChanged");
        }

        [Test]
        public void Test_Commit_OK()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";
            _Target.FilePath = "fp";

            _Infra.Zip.UnZipp("fp", "asticot").Returns(new string[] { "key", "Value" });
            _IWebServicesSettings.ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false).Returns(true);

            commit.Execute(null);

            _Infra.Zip.Received().UnZipp("fp", "asticot");
            _IWebServicesSettings.Received().ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false);
            _IWindow.Received().Close();
            _IWindow.Received().ShowMessage("Keys Imported successfully!", "Importing Discogs Key", false);
        }

    
        [Test]
        public void Test_Commit_KO_IWebServicesSettings_False()
        {
            //IMusicSession ims = Substitute.For<IMusicSession>();
            //ims.Setting.WebUserSettings.Returns(_IWebServicesSettings);
            //ims.Dependencies.Returns(_Infra);

            //IInfraDependencies iifd = Substitute.For<IInfraDependencies>();

            _Target = new ImportKeyViewModel(_IWebServicesSettings, _Infra) { Window = _IWindow };

            //_Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";
            _Target.FilePath = "fp";

            _Infra.Zip.UnZipp("fp", "asticot").Returns(new string[] { "key", "Value" });
            _IWebServicesSettings.ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false).Returns(false);
            commit.Execute(null);

            _Infra.Zip.Received().UnZipp("fp", "asticot");
            _IWebServicesSettings.Received().ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false);
            _IWindow.DidNotReceive().Close();
            _IWindow.Received().ShowMessage("Unable to import keys!", "Importing Discogs Key", false);
           
        }

        [Test]
        public void Test_Commit_KO_2()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";
            _Target.FilePath = "fp";

            _Infra.Zip.UnZipp("fp", "asticot").Returns(new string[] { "key" });
            commit.Execute(null);

            _Infra.Zip.Received().UnZipp("fp", "asticot");
            _IWebServicesSettings.DidNotReceive().ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false);
            _IWindow.DidNotReceive().Close();
            _IWindow.Received().ShowMessage("Unable to import keys!", "Importing Discogs Key", false);
        }

        [Test]
        public void Test_Commit_KO_Exception()
        {
            _Infra.File.DirectoryExists(Arg.Any<string>()).Returns(true);
            var commit = _Target.Commit;

            _Target.Directory = "toto";
            _Target.Password = "asticot";
            _Target.FilePath = "fp";

            _Infra.Zip.UnZipp("fp", "asticot").Returns(new string[] { "key", "Value" });
            _IWebServicesSettings.ImportPrivateKeys(Arg.Any<IDictionary<string, string>>(), false).Returns(x => { throw new Exception(); });
            commit.Execute(null);

            _Infra.Zip.Received().UnZipp("fp", "asticot");
            _IWindow.DidNotReceive().Close();
            _IWindow.Received().ShowMessage("Unable to import keys!", "Importing Discogs Key", false);
        }

  
    }
}
