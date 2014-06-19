using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit;
using NUnit.Framework;

using FluentAssertions;

using MusicCollection.Infra;


namespace MusicCollectionTest.Infra
{
    public enum TestEnum { Un, [System.ComponentModel.Description("Two")] Deux, Trois };
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Infra")]
    public class EnumExtenderTester
    {

        [Test]
        public void Test()
        {
            string res = TestEnum.Un.GetDescription();
            res.Should().Be("Un");

            res = TestEnum.Deux.GetDescription();
            res.Should().Be("Two");

            Enum nullen = null;
            nullen.GetDescription().Should().Be(string.Empty);
        }
    }
}
