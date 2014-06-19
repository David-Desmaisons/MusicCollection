using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using NUnit.Framework;

using MusicCollection.SettingsManagement;

namespace MusicCollectionTest.MusicSetting
{
    [TestFixture]
    [NUnit.Framework.Category("SettingsManagement")]
    [NUnit.Framework.Category("Unitary")]
    public class UIGridManagementTestor
    {

         [SetUp]
        public void SetUp()
        {
            XmlSerializer xs = new XmlSerializer(typeof(PersistentColumns));

            PersistentColumns pc = new PersistentColumns();
            pc.Add(new PersistentColumn(1) {DisplayIndex=1,Visibility=true,Width=30 });
            
            string iPath = Path.GetTempFileName();
            using (Stream s = File.Create(iPath))
                xs.Serialize(s, pc);

            PersistentColumns pcc = null;
            using (Stream s = File.OpenRead(iPath))
                pcc = xs.Deserialize(s) as PersistentColumns;

            Assert.That(pcc, Is.Not.Null);
            Assert.That(pcc.Count, Is.EqualTo(1));

            PersistentColumn pec = pcc[0];

            Assert.That(pec, Is.Not.Null);

            Assert.That(pec.Index, Is.EqualTo(1));
            Assert.That(pec.DisplayIndex, Is.EqualTo(1));
            Assert.That(pec.Visibility, Is.True);
            Assert.That(pec.Width, Is.EqualTo(30));

            File.Delete(iPath);
        }


         [Test]
         public void Test()
         {
         }
    }
}
