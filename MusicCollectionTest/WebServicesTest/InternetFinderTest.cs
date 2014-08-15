using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using System.Net;
using System.IO;
using FluentAssertions;
using MusicCollection.WebServices;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox.Web;
using System.Threading;
using MusicCollectionWPF.ViewModelHelper;


namespace MusicCollectionTest.WebServicesTest
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("WebServices")]
    public class InternetFinderTest
    {
        private IInternetProvider _Std;

        [SetUp]
        public void SetUp()
        {
            _Std = InternetProvider.InternetHelper;
        }

        [TearDown]
        public void TearDown()
        {
            InternetProvider.InternetHelper = _Std;
        }

        [Test]
        public void InternetFinderTest_Test()
        {
            IWebUserSettings wsm = Substitute.For<IWebUserSettings>();
            IWebQuery wq = Substitute.For<IWebQuery>();
            IInternetProvider iip = Substitute.For<IInternetProvider>();
            iip.GetIsNetworkAvailable().Returns(false);
            InternetProvider.InternetHelper = iip;

            InternetFinder ifi = new InternetFinder(wsm, wq);
            IInternetFinder iifif = ifi;
            iifif.MonitorEvents();

            bool receivederror = false;

            var p = new WPFSynchroneProgress<InternetFailed>((e) => receivederror=true);


            ifi.Compute(CancellationToken.None,p);

            //iifif.ShouldRaise("OnInternetError");
            receivederror.Should().BeTrue();
            ifi.Result.Found.Should().BeEmpty();

        }
    }
}
