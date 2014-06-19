using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.WebServices.Discogs2;


using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;
using MusicCollection.Fundation;
using MusicCollection.Infra;
using MusicCollection.FileConverter;

using FluentAssertions;
using MusicCollection.DataExchange.Cue;
using System.ComponentModel;

namespace MusicCollectionTest.DataExchange
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("DataExchange")]
    class TimeCueIndexerTestor
    {
        [Test]
        public void Test_Incoherent_Const_Minute_should_be_less_than_100()
        {
            TimeCueIndexer target = null;
            Action Const = () => target = new TimeCueIndexer(100, 0, 0);
            Const.ShouldThrow<Exception>();
        }

        [Test]
        public void Test_Incoherent_Const_Second_should_be_less_than_60()
        {
            TimeCueIndexer target = null;
            Action Const = () => target = new TimeCueIndexer(0, 60, 0);
            Const.ShouldThrow<Exception>();
        }

        [Test]
        public void Test_Incoherent_Const_Second_should_be_less_than_75()
        {
            TimeCueIndexer target = null;
            Action Const = () => target = new TimeCueIndexer(0, 0, 75);
            Const.ShouldThrow<Exception>();
        }

        [Test]
        public void Test_Const_Default()
        {
            TimeCueIndexer target = new TimeCueIndexer();
            target.ToString().Should().Be("0:0:0");
        }

        [Test]
        public void Test_Compare_Same()
        {
            TimeCueIndexer target1 = new TimeCueIndexer(20,20,4);
            TimeCueIndexer target2 = new TimeCueIndexer(20, 20, 4);
            (target2 >= target1).Should().BeTrue();
            (target1 >= target2).Should().BeTrue();

            (target2 <= target1).Should().BeTrue();
            (target1 <= target2).Should().BeTrue();
        }

        [Test]
        public void Test_Compare_NotSame()
        {
            TimeCueIndexer target1 = new TimeCueIndexer(20, 0, 4);
            TimeCueIndexer target2 = new TimeCueIndexer(20, 20, 4);
            (target2 >= target1).Should().BeTrue();
            (target1 >= target2).Should().BeFalse();

            (target2 <= target1).Should().BeFalse();
            (target1 <= target2).Should().BeTrue();
        }

        [Test]
        public void TimeCueIndexerConverter_Converter_Basic()
        {
            TimeCueIndexerConverter tcic = new TimeCueIndexerConverter();
            tcic.CanConvertFrom(typeof(string)).Should().BeTrue();
            tcic.CanConvertTo(typeof(string)).Should().BeTrue();

            TypeDescriptor.GetConverter(typeof(TimeCueIndexer)).GetType().Should().Be(typeof(TimeCueIndexerConverter));
        }

        [Test]
        public void TimeCueIndexerConverter_Converter_Convertion()
        {
            TimeCueIndexerConverter tcic = new TimeCueIndexerConverter();

            TimeCueIndexer tci = tcic.ConvertFromString("05:43:67") as TimeCueIndexer;
            tci.Should().NotBeNull();
            tci.Frames.Should().Be(67);
            tci.Minutes.Should().Be(5);
            tci.Seconds.Should().Be(43);

            TimeCueIndexer res = (TimeCueIndexer)tcic.ConvertFrom(new object());
            res.Should().BeNull();

            res = (TimeCueIndexer)tcic.ConvertFrom(null);
            res.TotalFrames.Should().Be(0);

            res = (TimeCueIndexer)tcic.ConvertFrom(string.Empty);
            res.TotalFrames.Should().Be(0);

            Action wt = () => res = (TimeCueIndexer)tcic.ConvertFrom("dddd");
            wt.ShouldThrow<Exception>();
        }

        [Test]
        public void TimeCueIndexerConverter_Converter_TimeSpan()
        {
            TimeCueIndexerConverter tcic = new TimeCueIndexerConverter();

            TimeCueIndexer tci = tcic.ConvertFromString("05:43:00") as TimeCueIndexer;
            tci.Should().NotBeNull();
            TimeSpan ts = new TimeSpan(0, 5, 43);
            tci.ToTimeSpan().Should().Be(ts);
        }

       

        [Test]
        public void TimeCueIndexerConverter_Converter_Convertion_To_String()
        {
            TimeCueIndexerConverter tcic = new TimeCueIndexerConverter();

            TimeCueIndexer tci = new TimeCueIndexer(1, 2, 3);
            string res = tcic.ConvertToString(tci);
            res.Should().Be("01:02:03");

            TimeCueIndexer tci2 = new TimeCueIndexer(10, 20, 30);
            string res2 = tcic.ConvertToString(tci2);
            res2.Should().Be("10:20:30");

            object Myres = tcic.ConvertTo(null, null, null, typeof(int));
            Myres.Should().BeNull();

            Myres = tcic.ConvertTo(null, null, null, typeof(string));
            Myres.Should().Be(string.Empty);

            Action wt = () => tcic.ConvertTo(null, null, new object(), typeof(string));
            wt.ShouldThrow<Exception>();
        }

        [Test]
        public void TimeCueIndexer_Converter_TotalFrameConst()
        {

            TimeCueIndexer expected = new TimeCueIndexer(10, 20, 30);

            TimeCueIndexer target = new TimeCueIndexer(expected.TotalFrames);
            target.Frames.Should().Be(30);
            target.Minutes.Should().Be(10);
            target.Seconds.Should().Be(20);
        }
    }
}
