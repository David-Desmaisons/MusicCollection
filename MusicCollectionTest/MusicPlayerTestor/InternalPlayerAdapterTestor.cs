using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;
using NSubstitute;
using FluentAssertions;

using MusicCollection.MusicPlayer;
using System.Threading;
using MusicCollectionTest.TestObjects;
using System.Windows.Threading;

namespace MusicCollectionTest.MusicPlayerTestor
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("MusicPlayer")]
    internal class InternalPlayerAdapterTestor : WPFThreadingHelper
    {

        private class InternalPlayerAdapterFake : InternalPlayerAdapter
        {
            public InternalPlayerAdapterFake(double interval, Dispatcher iDispatcher)
                : base(interval, iDispatcher)
            {
                Timed = 0;
                Stopped = false;
                Played = false;
                Paused = false;
                Closed = false;
            }

            public bool Stopped { get; private set; }
            protected override void DoStop()
            {
                Stopped = true;
            }

            public bool Played { get; private set; }
            protected override void DoPlay()
            {
                Played = true;
            }

            public bool Paused { get; private set; }
            protected override void DoPause()
            {
                Paused = true;
            }

            public bool Closed { get; private set; }
            protected override void DoClose()
            {
                Closed = true;
            }

            public int Timed { get; private set; }
            protected override void OnTimer()
            {
                Timed++;
            }
        }

        private InternalPlayerAdapterFake _Target;

        [SetUp]
        public void Setup()
        {
            base.BaseSetUp();
            //arrange
            _Target = new InternalPlayerAdapterFake(100, MainWindow.Dispatcher);
        }

        [TearDown]
        public void TearDown()
        {
            base.BaseTearDown();
        }

        [Test]
        public void InternalPlayerAdapter_Play()
        {
            //arrange
            //InternalPlayerAdapterFake ipa = new InternalPlayerAdapterFake(100);

            //act
            _Target.Play();

            Thread.Sleep(300);

            //assert
            _Target.Played.Should().BeTrue();
            _Target.Timed.Should().BeGreaterOrEqualTo(2);

            _Target.Dispose();

        }

        [Test]
        public void InternalPlayerAdapter_Play_ThenPause()
        {
            

            //act
            _Target.Play();

            Thread.Sleep(300);
            int counted = _Target.Timed;
            _Target.Pause();
            Thread.Sleep(400);

            //assert
            _Target.Played.Should().BeTrue();
            _Target.Paused.Should().BeTrue();
            counted.Should().BeGreaterOrEqualTo(2);
            _Target.Timed.Should().Be(counted);

            _Target.Dispose();

        }

        [Test]
        public void InternalPlayerAdapter_Pause()
        {
            //arrange
            //InternalPlayerAdapterFake ipa = new InternalPlayerAdapterFake(1);

            //act
            _Target.Pause();

            //assert
            _Target.Paused.Should().BeTrue();
            _Target.Timed.Should().Be(0);

            _Target.Dispose();

        }

        [Test]
        public void InternalPlayerAdapter_Close()
        {
            //arrange
            //InternalPlayerAdapterFake ipa = new InternalPlayerAdapterFake(1);

            //act
            _Target.Close();

            //assert
            _Target.Closed.Should().BeTrue();

            _Target.Dispose();

        }

        [Test]
        public void InternalPlayerAdapter_Stop()
        {
            //arrange
            //InternalPlayerAdapterFake ipa = new InternalPlayerAdapterFake(1);

            //act
            _Target.Stop();

            //assert
            _Target.Stopped.Should().BeTrue();
            _Target.Timed.Should().Be(0);

            _Target.Dispose();

        }
    }
}
