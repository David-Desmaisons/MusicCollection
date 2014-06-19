using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicCollection.ToolBox;
using FluentAssertions;
using NSubstitute;
using System.Threading;
using NUnit.Framework;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class EnumerableThreadingExtensionTest
    {
        private IEnumerable<int> GetCollection(CancellationTokenSource ct, Action<CancellationTokenSource, int> Ac)
        {
            for (int i = 0; i < 20; i++)
            {
                Ac(ct, i);
                yield return i;
            }
        }

        [Test]
        public void Test_Basic()
        {
            CancellationTokenSource ct = new CancellationTokenSource();
            var res = GetCollection(ct, (_, __) => { }).CancelableToList(ct.Token);
            res.Count().Should().Be(20);
        }

         [Test]
        public void Test_Basic_Cancelled()
        {
            CancellationTokenSource ct = new CancellationTokenSource();
            var res = GetCollection(ct, (t, i) => { if (i == 10) ct.Cancel(); }).CancelableToList(ct.Token);
            res.Count().Should().Be(10);
        }

        //public void Test_Basic()
        //{
        //    CancellationToken ct = new CancellationToken();
        //    var res = GetCollection(ct, (_, __) => { }).CancelableToList(ct);
        //    res.Count().Should().Be(20);
        //}
    }
}
