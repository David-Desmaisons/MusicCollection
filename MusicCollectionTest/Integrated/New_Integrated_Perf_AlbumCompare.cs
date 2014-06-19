using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;


using NUnit;
using NUnit.Framework;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Utilies.Edition;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollectionTest.Integrated.Tools;
using MusicCollectionTest.TestObjects;

using FluentAssertions;
using System.Diagnostics;
using System.Threading;
using MusicCollectionWPF.UserControls.AlbumPresenter;
using MusicCollectionWPF.ViewModel.Element;
using MusicCollectionWPF.ViewModel.Interface;

namespace MusicCollectionTest.Integrated
{
    [TestFixture]
    [NUnit.Framework.Category("Integrated")]
    [TestFolder(null, "SQLiteClean")]
    class New_Integrated_Perf_AlbumCompare : IntegratedTestImprovedBase
    {

        public New_Integrated_Perf_AlbumCompare()
        {
        }

        #region test helpers

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            baseTestFixtureSetUp();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            baseTestFixtureTearDown();
        }

        [SetUp]
        public void SetUp()
        {
            baseSetUp();
            //ThreadProperties TP = new ThreadProperties(ProcessPriorityClass.High, ThreadPriority.Highest);
            //TP.SetCurrentThread();
        }

       

        [TearDown]
        public void TearDown()
        {
            baseTearDown();
        }
        #endregion

        [Test]
        public void TestPef()
        {

            _IMusicSession.AllAlbums.Should().HaveCount(2860);


            for (int k = 0; k < 10; k++)
            {
                AlbumDistanceComparerFactory adcf = new AlbumDistanceComparerFactory(_IMusicSession);
   
                TimeTracer tr2 = TimeTracer.TimeTrack("100 Building Comparator Cache");
                List<IUpdatableComparer<IAlbum>> cts = new List<IUpdatableComparer<IAlbum>>();
                using (tr2)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        IUpdatableComparer<IAlbum> ct = adcf.GetComparer(_IMusicSession.AllAlbums[10]);
                        cts.Add(ct);
                    }
                }
            }
        }

    }
}
