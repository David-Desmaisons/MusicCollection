using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Infra;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class ThreadPriorityTest
    {

        [Test]
        public void FromCurrentThread_Test()
        {
            ThreadProperties target = ThreadProperties.FromCurrentThread();
            target.Should().NotBeNull();
            target.ProcessPriorityClass.Should().Be(Process.GetCurrentProcess().PriorityClass);
            target.ThreadPriority.Should().Be(Thread.CurrentThread.Priority);
        }
    }
}
