using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.Infra;
using System.Diagnostics;
using System.Threading;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]

    class ThreadPropertiesExtenderTestor
    {
        [Test]
        public void FromCurrentThread_Test()
        {
         

            ThreadPriority or = Thread.CurrentThread.Priority;

            if (or == ThreadPriority.Highest)
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }  
            
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            
            ThreadProperties target = ThreadProperties.FromCurrentThread();
            target.Should().NotBeNull();

            or = Thread.CurrentThread.Priority;
            or.Should().NotBe(ThreadPriority.Highest);

            ProcessPriorityClass orproc = Process.GetCurrentProcess().PriorityClass;
            orproc.Should().NotBe(ProcessPriorityClass.High);

            target.ProcessPriorityClass.Should().Be(orproc);
            target.ThreadPriority.Should().Be(or);


            ThreadProperties targetprop = new ThreadProperties(ThreadPriority.Highest, null);

            using (targetprop.GetChanger())
            {
                Thread.CurrentThread.Priority.Should().Be(ThreadPriority.Highest);
                Process.GetCurrentProcess().PriorityClass.Should().Be(orproc);
            }

            Thread.CurrentThread.Priority.Should().Be(or);
            Process.GetCurrentProcess().PriorityClass.Should().Be(orproc);
   
        }
    }
}
