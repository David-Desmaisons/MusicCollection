using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

using NUnit.Framework;

using MusicCollection.ToolBox;
using MusicCollection.Implementation;

namespace MusicCollectionTest.ToolBox
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("ToolBox")]
    class stringParserTester
    {

        [Test]
        public void DirectoryHelperTester()
        {
            string test = @"Aziz Sahmaoui\University Of Gnawa";
            string test2 = @"Aziz Sahmaoui";
            string test3 = @"Aziz Sahmaoui\University Of Gnawa\Image";
            string test4 = @"Aziz Sahmaoui\University Of Gnawa\Musique";

            DirectoryHelper dh = new DirectoryHelper(test);
            Assert.That(dh.Count, Is.EqualTo(2));
  
            DirectoryHelper dh2 = new DirectoryHelper(test2);
            Assert.That(dh2.Count, Is.EqualTo(1));

            DirectoryHelper dh3 = dh2.GetCommunRoot(dh);
            Assert.That(dh3, Is.Not.Null);
            Assert.That(dh3.Path, Is.EqualTo(@"Aziz Sahmaoui"));

            dh3 = dh.GetCommunRoot(dh2);
            Assert.That(dh3, Is.Not.Null);
            Assert.That(dh3.Path, Is.EqualTo(@"Aziz Sahmaoui"));

            dh2 = new DirectoryHelper("toto");
            Assert.That(dh2.Count, Is.EqualTo(1));
   
            dh3 = dh.GetCommunRoot(dh2);
            Assert.That(dh3, Is.Null);
   
            dh = new DirectoryHelper(test3);
            Assert.That(dh.Count, Is.EqualTo(3));

            dh2 = new DirectoryHelper(test4);
            Assert.That(dh2.Count, Is.EqualTo(3));

            dh3 = dh.GetCommunRoot(dh2);
            Assert.That(dh3, Is.Not.Null);
            Assert.That(dh3.Count, Is.EqualTo(2));
            Assert.That(dh3.Path, Is.EqualTo(test));

        }

        [Test]
        public void StringExtenderTitleLike()
        {
            string Name = "AAAAA nIIIME";
            Assert.That(Name.TitleLike(), Is.EqualTo("Aaaaa Niiime"));

            Name = "An Time Good";
            Assert.That(Name.TitleLike(), Is.EqualTo(Name));

            Name = "An Time GooDD";
            Assert.That(Name.TitleLike(), Is.EqualTo("An Time Goodd"));

            string test = "toto";
            test.TitleLike().Should().Be("Toto");

            test = "T";
            test.TitleLike().Should().Be("T");

            test = "";
            test.TitleLike().Should().Be("");

            test = null;
            Action getname = () => test.TitleLike();
            getname.ShouldThrow<ArgumentNullException>();

            test = "       ";
            test.TitleLike().Should().BeEmpty();

            test = "  4     ";
            test.TitleLike().Should().Be("4");

            test = "  A  tt   ";
            test.TitleLike().Should().Be("A Tt");

            test = "jean-jacques  van-damme";
            test.TitleLike().Should().Be("Jean-Jacques Van-Damme");

            test = "j.j cale";
            test.TitleLike().Should().Be("J.J Cale");

            test = "Dr. prapancham Sitaram";
            test.TitleLike().Should().Be("Dr. Prapancham Sitaram");

            test = "Dr.      prapancham Sitaram";
            test.TitleLike().Should().Be("Dr. Prapancham Sitaram");

            test = "D'avid Desmaisons";
            test.TitleLike().Should().Be("D'Avid Desmaisons");
       }

        [Test]
        public void StringExtenderNormalizeSpace()
        {
            string Name = "AAAAA nIIIME";
            Assert.That(Name.NormalizeSpace(), Is.EqualTo(Name));

            Name = "An  Time  Good";
            Assert.That(Name.NormalizeSpace(), Is.EqualTo("An Time Good"));

            Name = " An  Time  Good ";
            Assert.That(Name.NormalizeSpace(), Is.EqualTo("An Time Good"));
        }

        [Test]
        public void StringExtenderToMaxLength()
        {
            string Name = "on old time kept";
            Assert.That(Name.ToMaxLength(6), Is.EqualTo("on old"));

            Assert.That(Name.ToMaxLength(7), Is.EqualTo("on old"));

            Assert.That(Name.ToMaxLength(8), Is.EqualTo("on old t"));

            Assert.That(Name.ToMaxLength(39), Is.EqualTo(Name));

            Name = null;
            Assert.That(Name.ToMaxLength(39), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Test_TitleLike_Fail()
        {
            string test = null;
            string res = null;
            Action wll = () => res = test.TitleLike();

            wll.ShouldThrow<Exception>();
        }

        [Test]
        public void Test_FormatFileName()
        {
            string test = "NORMALname.mp3";
            test.FormatFileName().Should().Be("NORMALname.mp3");

            test = "NORMA;+Lname.mp3";
            test.FormatFileName().Should().Be("NORMALname.mp3");

            test = string.Empty;
            test.FormatFileName().Should().Be("Unknown");


            test = "    ";
            test.FormatFileName().Should().Be("Unknown");

            test = "1234567890";
            test.FormatFileName(2).Should().Be("1234567");
        }

        [Test]
        public void Test_FormatForDirectoryName()
        {
             string test = "NORMALname.mp3";
             test.FormatForDirectoryName().Should().Be("NORMALname mp3");

             test = "NORMALnameffffmp3";
             test.FormatForDirectoryName().Should().Be("NORMALnameffffmp3");

            
        }

        
        [Test]
        public void Test_FormatExistingRelativeDirectoryName()
        {
             string test = "NORMALname.mp3";
             test.FormatExistingRelativeDirectoryName().Should().Be("NORMALname.mp3");

             test = "NORMALnameffffmp3";
             test.FormatExistingRelativeDirectoryName().Should().Be("NORMALnameffffmp3");

             test = @"NORMAL.name\fff+fmp3";
             test.FormatExistingRelativeDirectoryName().Should().Be(@"NORMAL.name\ffffmp3");

             test = null;
             test.FormatExistingRelativeDirectoryName().Should().BeNull();
        }

        
        
        //[Test]
        //public void InternetFormat()
        //{
        //    string Name = "on old time kept";
        //    Assert.That(Name.InternetFormat(), Is.EqualTo("on+old+time+kept"));


        //    Name = " on old time kep t ";
        //    Assert.That(Name.InternetFormat(), Is.EqualTo("on+old+time+kep+t"));

        //    Name = "aaaaaaaaaaaaaaaa";
        //    Assert.That(Name.InternetFormat(), Is.EqualTo("aaaaaaaaaaaaaaaa"));
        //}

        

        [Test]
        public void StringExtenderRemoveInvalidCharacters()
        {
            string Name = "on old::: time kept";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("on old time kept"));

            //string reres = new string(Name.Where(c => !char.IsPunctuation(c)).Select(c=>(char.IsSeparator(c) || char.IsWhiteSpace(c))?' ':c).ToArray());

            Name = "on old???? time kept";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("on old time kept"));

            Name = @"|a";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("a"));

            Name = "on old\t time kept";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("on old  time kept"));

            Name = "on old\n time kept?";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("on old  time kept"));

            Name = @"""a"":b/c\d,e<f>g|h01";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("abcdefgh01"));

            Name = @"toto.mp3";
            Assert.That(Name.RemoveInvalidCharacters(), Is.EqualTo("toto.mp3"));


            //return fname.Replace(@"?", "").Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"""", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
 
          
            //FormatForDirectoryName(this string fname, int Limit = -1)
            //FormatExistingRelativeDirectoryName(this string fname)
            //FormatFileName

        }

       
    }
}
