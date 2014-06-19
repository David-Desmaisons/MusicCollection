using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using MusicCollection.Infra.Collection;
using MusicCollection.Infra;
using MusicCollectionTest.TestObjects;
using System.Threading;

namespace MusicCollectionTest.Infra.TaskTest
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    class TaskExtenderTester
    {
        //[SetUp]
        //public void SetUp()
        //{
        //}

        //[TearDown]
        //public void TearDown()
        //{
        //}

        [Test]
        public void Test_Cancelled()
        {
            Task t = Task.Run(() => { Thread.Sleep(5000); throw new Exception(); });
            CancellationTokenSource cts = new CancellationTokenSource();
            var res = t.WithTimeout(1000, cts);
            res.Wait();

            cts.IsCancellationRequested.Should().BeTrue();
            Thread.Sleep(6000);
        }

        [Test]
        public void Test_OK()
        {
            bool ok = false;
            Task t = Task.Run(() => { ok = true; });
            CancellationTokenSource cts = new CancellationTokenSource();
            var res = t.WithTimeout(1000, cts);
            res.Wait();

            ok.Should().BeTrue();
        }
    }
}
