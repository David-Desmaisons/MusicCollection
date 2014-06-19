using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using NUnit;
using NUnit.Framework;

using MusicCollectionTest.TestObjects;


using MusicCollection.ToolBox;
using MusicCollection.Infra;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    public class PathDecomposeurTestor
    {
        private string _Path0;
        private string _Path1;
        private string _Path2;
        private string _Path3;
        private string _Path4;
        private string[] _Path5;


        [SetUp]
        public void SetUp()
        {
            _Path0 = @"C:\Documents and Settings\DEM\My Documents\My Music\Music Collection\Files\insen_live_AUDIO\insen_live_AUDIO\ALVA NOTO & RYUICHI SAKAMOTO - insen live AUDIO (raster-noton, 2006)08 - Trioon.mp3";
            _Path1 = @"C:\Documents and Settings\DEM\My Documents\My Music\Music Collection\Files\insen_live_AUDIO\insen_live_AUDIO\ALVA NOTO & RYUICHI SAKAMOTO - insen live AUDIO (raster-noton, 2006)09 - Xerox.mp3";
            _Path2 = @"C:\Documents and Settings\DEM\My Documents\My Music\Music Collection";
            _Path3 = @"C:\Documents and Settings\DEM\My Documents\My Music";
            _Path4 = @"C:\Documents and Settings\DEM\My Documents\My Music\My Music";

            _Path0 = _Path0.ToLower(); _Path1 = _Path1.ToLower(); _Path2 = _Path2.ToLower(); _Path3 = _Path3.ToLower();
            _Path4 = _Path4.ToLower();

            _Path5 = new string[10];
            for (int i = 0; i < 10; i++)
            {
                _Path5[i] = string.Format(@"C:\Documents and Settings\DEM\My Documents\My Music\Music Collection\Files\insen_live_AUDIO\{0}", i).ToLower();
            }
        }

        [Test]
        public void Test1()
        {
            PathDecomposeur pd = PathDecomposeur.FromName(_Path0);
            Assert.That(pd.FullName, Is.EqualTo(_Path0));

            PathDecomposeur pd2 = PathDecomposeur.FromName(_Path0);
            Assert.That(pd2.FullName, Is.EqualTo(_Path0));

            Assert.That(object.ReferenceEquals(pd, pd2),Is.True);

            PathDecomposeur pd3 = PathDecomposeur.FromName(_Path1);
            Assert.That(pd3.FullName, Is.EqualTo(_Path1));

            PathDecomposeur pd4 = PathDecomposeur.FromName(_Path2);
            Assert.That(pd4.FullName, Is.EqualTo(_Path2));

            PathDecomposeur pd5 = PathDecomposeur.FromName(_Path3);
            Assert.That(pd5.FullName, Is.EqualTo(_Path3));

            PathDecomposeur pd6 = PathDecomposeur.FromName(_Path4);
            Assert.That(pd6.FullName, Is.EqualTo(_Path4));

            PathDecomposeur f = null;

            foreach (string s in _Path5)
            {
                PathDecomposeur pd2d = PathDecomposeur.FromName(s);
                Assert.That(pd2d.FullName, Is.EqualTo(s));

                if (f == null)
                {
                    f = pd2d.Father;
                }

                Assert.That(object.ReferenceEquals(f, pd2d.Father), Is.True);
            }


            PathDecomposeur pd2e = PathDecomposeur.FromName(_Path0);
            Assert.That(pd2e.FullName, Is.EqualTo(_Path0));

            Assert.That(object.ReferenceEquals(pd, pd2e), Is.True);
        }
    }
}
