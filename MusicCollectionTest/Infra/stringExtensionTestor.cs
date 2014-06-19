using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit;
using NUnit.Framework;
using FluentAssertions;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Collection;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Infra")]
    [NUnit.Framework.Category("Unitary")]
    public class stringExtensionTestor
    {
        [SetUp]
        public void SU()
        {
        }

        [Test]
        public void Test()
        {
            string toto = "toto";
            Assert.AreEqual(0, toto.GetDamerauLevenshteinDistance("toto"));
            Assert.AreEqual(1, toto.GetDamerauLevenshteinDistance("oto"));
            Assert.AreEqual(1, toto.GetDamerauLevenshteinDistance("voto"));
            Assert.AreEqual(1, toto.GetDamerauLevenshteinDistance("tpto"));
            Assert.AreEqual(1, toto.GetDamerauLevenshteinDistance("otto"));
            Assert.AreEqual(3, toto.GetDamerauLevenshteinDistance("o"));
            Assert.AreEqual(4, toto.GetDamerauLevenshteinDistance(""));
            Assert.AreEqual(4, toto.GetDamerauLevenshteinDistance("yu"));
            Assert.AreEqual(3, toto.GetDamerauLevenshteinDistance("t"));

            string.Empty.GetDamerauLevenshteinDistance(string.Empty).Should().Be(0);
            "1234567890".GetDamerauLevenshteinDistance(string.Empty).Should().Be(10);
            string.Empty.GetDamerauLevenshteinDistance("1234567890").Should().Be(10);
        }

        [Test]
        public void Test2()
        {
            string[] lines = System.IO.File.ReadAllLines(@"Resources\words.txt");

            List<string> Dict = lines.ToList();

            ItemFinder<string> MyDic = new ItemFinder<string>(Dict, n => n);

            var res = MyDic.FindSimilarMatches("bayb", 1).ToList();
            Console.WriteLine(res);

            res.Should().Contain(new string[]{"baby","bay","bays"});

            string r = MyDic.FindExactMatches("babyy").FirstOrDefault();
            r.Should().BeNull();


            r = MyDic.FindExactMatches("Baby").FirstOrDefault();
            r.Should().Be("baby");

            r = MyDic.FindExactMatches("BABY").FirstOrDefault();
            r.Should().Be("baby");


        }
    }
}
