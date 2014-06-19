using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit;
using NUnit.Framework;

using MusicCollection.Implementation;
using MusicCollection.Utilies;

namespace MusicCollectionTest.Implementation
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("Implementation")]

    public class StringParserTester
    {
        [Test]
        public void StringAlbumParserTester()
        {
            string test = @"Anthony Braxton - Six Compositions (Quartet) 1984";
            StringAlbumParser sap = StringAlbumParser.FromString(test);
            Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);
            Assert.That(sap.AlbumAuthor, Is.EqualTo("Anthony Braxton"));
            Assert.That(sap.AlbumName, Is.EqualTo("Six Compositions (Quartet) 1984"));
            Assert.That(sap.AlbumYear, Is.EqualTo(1984));

            test = @"Anthony Braxton - Studio Rivbea - 1976-02-21";

            sap = StringAlbumParser.FromString(test);
            Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);
            Assert.That(sap.AlbumAuthor, Is.EqualTo("Anthony Braxton"));
            Assert.That(sap.AlbumName, Is.EqualTo("Studio Rivbea - 1976-02-21"));
            Assert.That(sap.AlbumYear, Is.EqualTo(1976));

            test = @"7k.Oaks.-.Entelechy.(2011).part1.rar";
            sap = StringAlbumParser.FromRarZipName(test);
            Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);
            Assert.That(sap.AlbumAuthor, Is.EqualTo("7k.Oaks."));
            Assert.That(sap.AlbumName, Is.EqualTo(".Entelechy.(2011)"));
            Assert.That(sap.AlbumYear, Is.EqualTo(2011));

            test = @"Anthony Braxton - Studio Rivbea - 1976-02-21";
            sap = StringAlbumParser.FromDirectory(test);
            Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);
            Assert.That(sap.AlbumAuthor, Is.EqualTo("Anthony Braxton"));
            Assert.That(sap.AlbumName, Is.EqualTo("Studio Rivbea - 1976-02-21"));
            Assert.That(sap.AlbumYear, Is.EqualTo(1976));
        }


        [Test]
        public void StringTrackParserTester()
        {
            string test = @"Anthony Braxton - Six Compositions (Quartet) 1984";
            test = @"02 Bauer Houtkamp Reijsegger Bennink Tilburg 1984.10.25.flac";
            StringTrackParser stp = new StringTrackParser(test);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.False);
            Assert.That(stp.TrackName, Is.EqualTo("Bauer Houtkamp Reijsegger Bennink Tilburg 1984.10.25"));
            Assert.That(stp.TrackNumber, Is.EqualTo(2));
      
            test = @"01-Primo.mp3";
            stp = new StringTrackParser(test);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.False);
            Assert.That(stp.TrackName, Is.EqualTo("Primo"));
            Assert.That(stp.TrackNumber, Is.EqualTo(1));
   

            test = @"Track 02.flac";
            stp = new StringTrackParser(test);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.True);
            Assert.That(stp.TrackNumber, Is.EqualTo(2));
   

            test = @"02.flac";
            stp = new StringTrackParser(test);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.True);
            Assert.That(stp.TrackNumber, Is.EqualTo(2));

            test = "06 - songeene";
            stp = new StringTrackParser(test);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.False);
            Assert.That(stp.FounSomething, Is.True);
            Assert.That(stp.TrackNumber, Is.EqualTo(6));
            Assert.That(stp.TrackName, Is.EqualTo("songeene"));


            test = "01.Krube. - Des Zunichtewerdens I";
            stp = new StringTrackParser(test,false);
            Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);
            Assert.That(stp.IsDummy, Is.False);
            Assert.That(stp.FounSomething, Is.True);
            Assert.That(stp.TrackNumber, Is.EqualTo(1));
            Assert.That(stp.TrackName, Is.EqualTo("Krube. - Des Zunichtewerdens I"));



            





        }


    }
}
