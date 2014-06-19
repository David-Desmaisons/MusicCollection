using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using MusicCollection.Infra;
using MusicCollection.ToolBox;
using MusicCollection.Utilies;
using MusicCollection.Implementation;

namespace MusicCollectionTest
{
       //Updater<IArtist> u = new Updater<IArtist>(@"E:\sql\musictest_tracks.sql",1);
       //     u.Update();

       //     Updater<IAlbum> ud = new Updater<IAlbum>(@"E:\sql\musictest_discs.sql", 0);
       //     ud.Update();

    internal class Accentor<T> where T : class,ISessionPersistentObject
    {
        private Updater _Up;
        private  Func<T, IDictionary<string, string>, DBOperation> _BL;
        private Func<Func<T, DBOperation>, DataBaseUpdater<T>> _Factory;
   

        internal Accentor(string Path, IEnumerable<string> Attributes, Func<T, IDictionary<string, string>, DBOperation> Updater, Func<Func<T, DBOperation>, DataBaseUpdater<T>> Factory)
        {
            _Up = new Updater(Path, Attributes);
            _BL = Updater;
            _Factory = Factory;
        }

        internal void Fire()
        {
            _Up.Update();
            DataBaseUpdater<T> DBU = _Factory((t) => _BL(t, _Up.GetItemfromID(t.ID)));
            DBU.UpdateDataBase();
            //IDBPersistentObject
        }

    }


    internal class Updater
    {
        private string _FN;
        //private uint _Junp;
        private Regex _IDGuesser;

        private Dictionary<int,Dictionary<string,string>> _Objects= new  Dictionary<int,Dictionary<string,string>>();

        private List<string> _attribute;
        private IEnumerable<string> attribute
        {
            get { return _attribute; }
            set { _attribute = value.ToList(); }
        }


        private List<int> _Indexes;
        private IEnumerable<int> Indexes
        {
            get { return _Indexes; }
            set { _Indexes = value.ToList(); }
        }


        internal IDictionary<string, string> GetItemfromID(int ID)
        {
            Dictionary<string, string> res = null;
            if (_Objects.TryGetValue(ID, out res))
            {
                return res;
            }
            return null;

        }


        internal Updater(string fn, IEnumerable<string> iattribute)
        {
            _FN = fn;
           attribute = iattribute;
        }

        //INSERT INTO `tracks` (`ID`, `DiscID`, `Name`, `TrackNumber`, `Duration`, `Path`, `Rating`, `LastPlay`, `PlayCount`, `DateAdded`, `Hashkey`, `ISRC`) VALUES (1,1,'Jawa Jawa',1,3442370000,'c:\\users\\david\\music\\music collection\\files\\akalewube_320\\akalewube_320\\akalé wubé - akalé wubé [l''arôme productions, 2011] (@320)a1 - jawa jawa.mp3',5,'2011-10-06 00:02:06',6,'2011-10-01 08:55:17','b1247284141fc0f5e13d9c4f07cc11229cd36661',NULL);     


        private void UpdateLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            if (_IDGuesser == null)
            {
                Regex depioter = new Regex(@"^INSERT INTO `\w*?` \((?:`(\w*?)`, )*?`(\w*?)`\)");
                var result = (from el in depioter.Match(line).Groups.Cast<Group>() where el.Success from m in el.Captures.Cast<Capture>() select m.Value).Skip(1).ToList();

                Indexes = from at in attribute let id=result.IndexOf(at) orderby id select id ;
  
                if (_Indexes.Contains(-1))
                    throw new Exception();

                _attribute = (from a in _attribute orderby result.IndexOf(a) select a).ToList();
                
                StringBuilder sb = new StringBuilder(@"VALUES \((\d+),");

                int mamax = result.Count;
                int max = _Indexes[_Indexes.Count-1];
                int pn=0;

                for (int i = 1; i <= max; i++)
                {
                    if (i == _Indexes[pn])
                    {
                        sb.Append(@"'(.*?)'");
                        if (i == mamax-1)
                            sb.Append(@"\)");
                        else
                            sb.Append(@",");
                        pn++;
                    }
                    else
                        sb.Append(@"(?:(?:[^'].*?,)|(?:'.*',))");
                }
       

                _IDGuesser = new Regex(sb.ToString());
            }


            Dictionary<string, string> res = new Dictionary<string, string>();

            Match ma = _IDGuesser.Match(line);

            res.Add("ID",ma.Groups[1].Value);
            int ID = int.Parse(ma.Groups[1].Value);

            int i2 = 2;
            foreach (string at in attribute)
            {
                res.Add(at, ma.Groups[i2].Value.Replace("''", "'"));
                i2++;
            }

            _Objects.Add(ID, res);
        }


        internal void Update()
        {
            StreamReader sr = new StreamReader(_FN);

            while (sr.Peek() >= 0)
            {
                 UpdateLine(sr.ReadLine());
            }

        }


    }


}
