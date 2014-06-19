using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using NUnit.Framework;

using MusicCollectionTest.TestObjects;
using MusicCollection.Infra;
using MusicCollection.ToolBox.StringExpression;

namespace MusicCollectionTest.Infra
{
    [TestFixture]
    [NUnit.Framework.Category("Infra")]
    [NUnit.Framework.Category("Unitary")]
    class GetSubstringTestor
    {
        [Test]
        public void SubstringTestor()
        {
            string toto = "a";

            List<string> ls = toto.GetSubstring().ToList();

            Assert.That(ls.Count, Is.EqualTo(2));
            Assert.That(ls.Contains("a"), Is.True);
            Assert.That(ls.Contains(""), Is.True);

            toto = "";
            ls = toto.GetSubstring().ToList();
            Assert.That(ls.Count, Is.EqualTo(1));
            Assert.That(ls.Contains(""), Is.True);

            toto = "ab";
            ls = toto.GetSubstring().ToList();
            Assert.That(ls.Count, Is.EqualTo(4));
            Assert.That(ls.Contains("a"), Is.True);
            Assert.That(ls.Contains("ab"), Is.True);
            Assert.That(ls.Contains("b"), Is.True);
            Assert.That(ls.Contains(""), Is.True);

            toto = "anticonstitutionellement";
            ls = toto.GetSubstring().ToList();

            var same = ls.Distinct().ToList();
            Assert.That(ls.Count, Is.EqualTo(same.Count));

            Random r = new Random();
            int Max = toto.Length-1;

            for (int i=0; i<50;i++)
            {

                int Deb = r.Next(0,Max);
                int length = r.Next(1, Max-Deb+1);
                string test = toto.Substring(Deb, length);

                Assert.That(ls.Contains(test), Is.True);

            }

        }

        [Test]
        public void SubstringsTestor()
        {
            string toto = "agfaff";

            List<string> ls = toto.GetSubstrings(3).ToList();

            Assert.That(ls.Count, Is.LessThanOrEqualTo(15));
            Assert.That(ls.Contains("a"), Is.True);
            Assert.That(ls.Contains("gf"), Is.True);
            Assert.That(ls.Contains("aff"), Is.True);
        }


    }
}
