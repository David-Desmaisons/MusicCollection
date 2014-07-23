using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using NUnit.Framework;
using MusicCollection.WebServices.GraceNote.DTO;
using System.IO;

namespace MusicCollectionTest.GraceNoteTest
{
    [TestFixture]
    [NUnit.Framework.Category("Unitary")]
    [NUnit.Framework.Category("GraceNote")]
    public class GraceNoteTest
    {

        [Test]
        public void Test_TOC()
        {
            Queries q = new Queries()
            {
                Auth = new Auth("client_id_string","user_id_string"),
                Query = new Query()
                {
                    Command = "ALBUM_TOC",
                    Mode = "SINGLE_BEST_COVER",
                    Toc= new Toc(150,20512,30837,50912,64107,78357,90537,110742,126817,144657,153490,160700,175270,186830,201800,218010,237282,244062,262600,272929)
                }
            };

            MemoryStream memory = new MemoryStream();
            q.Serialize(memory);
            memory.Seek(0,SeekOrigin.Begin);

            StreamReader reader = new StreamReader(memory);
            string text = reader.ReadToEnd();

            string expected = "<?xml version=\"1.0\"?>\r\n<QUERIES>\r\n  <AUTH>\r\n    <CLIENT>client_id_string</CLIENT>\r\n    <USER>user_id_string</USER>\r\n  </AUTH>\r\n  <COUNTRY>usa</COUNTRY>\r\n  <LANG>eng</LANG>\r\n  <QUERY CMD=\"ALBUM_TOC\">\r\n    <MODE>SINGLE_BEST_COVER</MODE>\r\n    <TOC>\r\n      <OFFSETS>150 20512 30837 50912 64107 78357 90537 110742 126817 144657 153490 160700 175270 186830 201800 218010 237282 244062 262600 272929</OFFSETS>\r\n    </TOC>\r\n  </QUERY>\r\n</QUERIES>";
            text.Should().Be(expected);
        }
    }
}
