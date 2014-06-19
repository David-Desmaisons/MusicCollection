using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;
using FluentAssertions;

using MusicCollectionTest.TestObjects;
using MusicCollection.ToolBox;
using MusicCollection.ToolBox.Collection;
using MusicCollection.Infra;
using MusicCollection.ToolBox.Maths;

namespace MusicCollectionTest.ToolBox.Maths
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class SecondDegreeSolverTest
    {
        [Test]
        public void Test0()
        {
            SecondDegreeSolver sds= new SecondDegreeSolver(a: 1, b: 1, c: -12);
            sds.GetMaxSolution().Should().Be(3);

            sds = new SecondDegreeSolver(a: 1, b: 1, c: -30);
            sds.GetMaxSolution().Should().Be(5);

            sds = new SecondDegreeSolver(a: 1, b: 0, c:1);
            sds.GetMaxSolution().Should().NotHaveValue();
        }
    }
}
