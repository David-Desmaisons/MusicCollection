using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using NUnit;
using NUnit.Framework;
using FluentAssertions;

using FakeItEasy;
using NSubstitute;

using MusicCollection.Infra;
using MusicCollection.Fundation;
using MusicCollectionTest.TestObjects;
using MusicCollection.Infra.Tasks;
using MusicCollection.Infra.Communication;


namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class WaitHandleExtenderTest
    {
        private ManualResetEvent _MRE;
        private bool _RunnedOK = false;

        [SetUp]
        public void SetUp()
        {
            _RunnedOK = false;
        }

        [TearDown]
        public void CleanUp()
        {
            _RunnedOK = false;
            if (_MRE != null)
            {
                _MRE.Dispose();
                _MRE = null;
            }
        }

        [Test]
        public void Test_OK_No_TimeOut()
        {
            _MRE = new ManualResetEvent(false);
            Thread nThread = new Thread(() => ThreadReleaser(2000));
            nThread.Start();
            Task t = _MRE.AsTask();
            t.Should().NotBeNull();
            t.Wait();
            _RunnedOK.Should().BeTrue();
        }

        [Test]
        public void Test_Cancelled_TimeOut()
        {
            _MRE = new ManualResetEvent(false);
            Thread nThread = new Thread(() => ThreadReleaser(5000));
            nThread.Start();
            Task t = _MRE.AsTask(TimeSpan.FromSeconds(1));
            t.Should().NotBeNull();
            t.ContinueWith(tt => tt.IsCanceled.Should().BeTrue());
            _RunnedOK.Should().BeFalse();
        }

        private void ThreadReleaser(int iTimems)
        {
            Thread.Sleep(iTimems);
            _RunnedOK = true;
            if (_MRE != null)
            {
                _MRE.Set();
            }
        }
    }
}
