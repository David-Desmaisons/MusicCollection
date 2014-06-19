//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Collections.ObjectModel;

//using NHibernate;


//using MusicCollection.Fundation;
//using MusicCollection.Implementation;
//using MusicCollection.Itunes;
//using MusicCollection.FileConverter;
//using MusicCollection.FileImporter;
////using MusicCollection.InternetCDInfoProvider;
//using MusicCollection.Nhibernate.Session;
//using MusicCollection.Nhibernate.Mapping;
//using MusicCollection.Infra;
//using MusicCollection.DataExchange;
//using MusicCollection.ToolBox;
//using MusicCollection.Utilies;


//namespace MusicCollectionTest
//{
//    static internal class sessionextender
//    {
//        static internal IMusicImporterFactory GetImporterFactory(this IMusicSession sess)
//        {
//            return LazyLoadingMusicImporter.GetFactory(sess as MusicSessionImpl);
//        }
//    }

//    class Program
//    {


//        //static private void SetUpData()
//        //{
//        //    //DirectoryInfo din = new DirectoryInfo(@"..\..\Input");
//        //    //if (!din.Exists)
//        //    //    throw new Exception("Missing input  directory");

//        //    //CleanUp();

//        //    //DirectoryInfo dou = new DirectoryInfo(@"..\..\..\Output");
//        //    //if (!dou.Exists)
//        //    //    throw new Exception("Missing output directory");

//        //    //foreach (FileInfo fi in din.GetFiles())
//        //    //{
//        //    //    fi.CopyTo(Path.Combine(dou.FullName, fi.Name), true);
//        //    //}

//        //    //FileHelper.SetRootApplicationDirectoty(dou.FullName);

//        //}

//        //static private void CleanUp()
//        //{

//        //    DirectoryInfo dou = new DirectoryInfo(@"..\..\..\Output");
//        //    if (!dou.Exists)
//        //        throw new Exception("Missing output directory");

//        //    foreach (FileInfo fi in dou.GetFiles())
//        //    {
//        //        fi.Delete();
//        //    }

//        //    foreach (DirectoryInfo di in dou.GetDirectories())
//        //    {
//        //        di.Delete(true);
//        //    }
//        //}

//        static void testParser()
//        {
//            string it = "DES010900133";
//            ISRC ob = ISRC.Fromstring(it);
//            if (ob == null)
//                throw new Exception("No good");

//            if (ob.Name != it)
//                throw new Exception("No good");

//            //string unicode = @"ドラゴンクエストI・II・III(復刻版攻略本「ファミコン神拳」(書籍全130ページ)他同梱) 初";
//            //string trans = unicode.WithoutAccent();

//            //Console.WriteLine("{0} comp {1}", unicode, trans);

//            //unicode = @"6 Third Piano Sonata III. Doïna.flac";
//            //trans = unicode.WithoutAccent();

//            //Console.WriteLine("{0} comp {1}", unicode, trans);


//            //unicode = "ґ";
//            //trans = unicode.WithoutAccent();

//           // Console.WriteLine("{0} comp {1}", unicode, trans);

//            it = "000000000000";
//            ob = ISRC.Fromstring(it);
//            if (ob != null)
//                throw new Exception("No good");

//            Regex _VA = new Regex(@"(?i)^v\.?a\.?$");


//            //string test = @"Anthony Braxton - Six Compositions (Quartet) 1984";
//            //StringAlbumParser sap = StringAlbumParser.FromString(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);


//            //test = @"Anthony Braxton - Studio Rivbea - 1976-02-21";

//            //sap = StringAlbumParser.FromString(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);

//            //test = @"Anthony Braxton - Studio Rivbea - 1976-02-21";
//            //sap = StringAlbumParser.FromString(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);

//            //test = @"7k.Oaks.-.Entelechy.(2011).part1.rar";
//            //sap = StringAlbumParser.FromRarZipName(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);

//            //test = @"Anthony Braxton - Studio Rivbea - 1976-02-21";
//            //sap = StringAlbumParser.FromDirectory(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);


//            //test = @"Ornette Coleman Quartet - Live in Genoa '10_mp3";
//            //sap = StringAlbumParser.FromDirectory(test);
//            //Console.WriteLine("Original: {3} Match Name:{0}, authour {1}, Year {2}", sap.AlbumName, sap.AlbumAuthor, sap.AlbumYear, test);



//            //test = @"02 Bauer Houtkamp Reijsegger Bennink Tilburg 1984.10.25.flac";
//            //StringTrackParser stp = new StringTrackParser(test);
//            //Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);

//            //test = @"01-Primo.mp3";
//            //stp = new StringTrackParser(test);
//            //Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);

//            //test = @"Track 02.flac";
//            //stp = new StringTrackParser(test);
//            //Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);

//            //test = @"02.flac";
//            //stp = new StringTrackParser(test);
//            //Console.WriteLine("Original: {3} Match Dummy:{0}, Name {1}, TrackNumber {2}", stp.IsDummy, stp.TrackName, stp.TrackNumber, test);



//            //test = @"Aziz Sahmaoui\University Of Gnawa";
//            //string test2 = @"Aziz Sahmaoui";
//            //string test3 = @"Aziz Sahmaoui\University Of Gnawa\Image";
//            //string test4 = @"Aziz Sahmaoui\University Of Gnawa\Musique";

//            //DirectoryHelper dh = new DirectoryHelper(test);
//            //if (dh.Count != 2)
//            //    throw new Exception("DirectoryHelper");

//            //DirectoryHelper dh2 = new DirectoryHelper(test2);
//            //if (dh2.Count != 1)
//            //    throw new Exception("DirectoryHelper");

//            //DirectoryHelper dh3 = dh2.GetCommunRoot(dh);
//            //if (dh3 == null)
//            //    throw new Exception("DirectoryHelper");

//            //if (dh3.Path != @"Aziz Sahmaoui")
//            //    throw new Exception("DirectoryHelper");

//            //dh3 = dh.GetCommunRoot(dh2);
//            //if (dh3 == null)
//            //    throw new Exception("DirectoryHelper");

//            //if (dh3.Path != @"Aziz Sahmaoui")
//            //    throw new Exception("DirectoryHelper");

//            //dh2 = new DirectoryHelper("toto");
//            //if (dh2.Count != 1)
//            //    throw new Exception("DirectoryHelper");

//            //dh3 = dh.GetCommunRoot(dh2);
//            //if (dh3 != null)
//            //    throw new Exception("DirectoryHelper");

//            //dh = new DirectoryHelper(test3);
//            //if (dh.Count != 3)
//            //    throw new Exception("DirectoryHelper");
//            //dh2 = new DirectoryHelper(test4);
//            //if (dh2.Count != 3)
//            //    throw new Exception("DirectoryHelper");
//            //dh3 = dh.GetCommunRoot(dh2);
//            //if (dh3.Count != 2)
//            //    throw new Exception("DirectoryHelper");
//            //if (dh3.Path != test)
//            //    throw new Exception("DirectoryHelper");

//            string lath = @"..\..\Input\bftaqe01.rar";

//            IImportHelper rih = new RarImporterHelper(new SevenZip.SevenZipExtractor(lath));

//            Console.WriteLine("Artist {0}, Name {1} Year {2}", rih.AlbumArtistClue, rih.AlbumNameClue, rih.Year);

//        }


//        private class ThumbleGenerator
//        {
//            private int _view = 0;
//            private int _ok = 0;

//            internal DBOperation Albumkanalyser(Album tr)
//            {
//                DBOperation status = DBOperation.Update;
//                _view++;
//                _ok++;

//                if (_view % 100 == 0)
//                    Console.WriteLine("View: {0}, udpatted: {1}", _view, _ok);

//                return status;
//            }


//        }

//        private class iscrcompteur
//        {
//            private int _view = 0;
//            private int _ok = 0;

//            internal DBOperation Trackanalyser(Track tr)
//            {
//                DBOperation status = DBOperation.Ignore;
//                _view++;

//                tr.UpdateISRC();
//                ISRC res = tr.ISRC;
//                if (res != null)
//                {
//                    _ok++;
//                    Console.WriteLine("               Match Number {0} value: {1}", _ok, res);
//                    status = DBOperation.Update;
//                }

//                if (_view % 100 == 0)
//                    Console.WriteLine("View: {0}, with tag: {1}", _view, _ok);

//                return status;
//            }


//        }

//        private static void testcollect2()
//        {
//            //ObservableCollection<Tuple<string, int>> l = new ObservableCollection<Tuple<string, int>>();

//            //Tuple<string, int> un = new Tuple<string, int>("blo cou", 1);
//            //l.Add(un);
//            //l.Add(new Tuple<string, int>("blob coub", 2));
//            //l.Add(new Tuple<string, int>("bla- cou", 0));
//            //l.Add(new Tuple<string, int>("aa cou o", -1));

//            //Tuple<string, int> deux = new Tuple<string, int>("aab cou o", -2);
//            //l.Add(deux);

//            //FilteredTransformReadOnlyObservableCollection<Tuple<string, int>, string, int> f =
//            //    new FilteredTransformReadOnlyObservableCollection<Tuple<string, int>, string, int>(l, li => li.Item1.Contains("bl"), li => li.Item1, li => li.Item2);

//            //Console.WriteLine("-----------------------");

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.CollectionChanged += (o, e) => Console.WriteLine("Changes {0} old:{1} new:{2}", e.Action, e.OldItems == null ? null : string.Join("&&", e.OldItems.Cast<string>()), e.NewItems == null ? null : string.Join("&&", e.NewItems.Cast<string>()));

//            //f.Filter = li => li.Item1.Contains("blo");

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => true;

//            //foreach (string re in f)
//            //    Console.WriteLine(re);


//            //Console.WriteLine("-----------------------");

//            //f.Filter = null;

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => li.Item1.Contains("bl");

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => li.Item1.Contains("a");

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Add(new Tuple<string, int>("a", -1));

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Remove(un);

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Remove(deux);

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = null;

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//            //l.Add(new Tuple<string, int>("aa cou o", -1));
//            //l.Add(new Tuple<string, int>("aa cou o", -1));

//            //f.Filter = fi => fi.Item1.StartsWith("aa");

//            //foreach (string re in f)
//            //    Console.WriteLine(re);

//        }

//        private static void testcollect()
//        {
//            //ObservableCollection<Tuple<string, int>> l = new ObservableCollection<Tuple<string, int>>();

//            //Tuple<string, int> un = new Tuple<string, int>("blo cou", 1);
//            //l.Add(un);
//            //l.Add(new Tuple<string, int>("blob coub", 2));
//            //l.Add(new Tuple<string, int>("bla- cou", 0));
//            //l.Add(new Tuple<string, int>("aa cou o", -1));

//            //Tuple<string, int> deux = new Tuple<string, int>("aab cou o", -2);
//            //l.Add(deux);


//            //var f = l.FilterAndOrder(li => li.Item1.Contains("bl"), li => li.Item2);

//            ////FilteredReadOnlyObservableCollection<Tuple<string, int>, int> f =
//            ////    new FilteredReadOnlyObservableCollection<Tuple<string, int>, int>(l, li => li.Item1.Contains("bl"),li=>li.Item2);

//            //Console.WriteLine("-----------------------");

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.CollectionChanged += (o, e) => Console.WriteLine("Changes {0} old:{1} new:{2}", e.Action, e.OldItems == null ? null : string.Join("&&", e.OldItems.Cast<Tuple<string, int>>()), e.NewItems == null ? null : string.Join("&&", e.NewItems.Cast<Tuple<string, int>>()));

//            //f.Filter = li => li.Item1.Contains("blo");

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => true;

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);


//            //Console.WriteLine("-----------------------");

//            //f.Filter = null;

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => li.Item1.Contains("bl");

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = li => li.Item1.Contains("a");

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Add(new Tuple<string, int>("a", -1));

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Remove(un);

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //l.Remove(deux);

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);

//            //Console.WriteLine("-----------------------");

//            //f.Filter = null;

//            //foreach (Tuple<string, int> re in f)
//            //    Console.WriteLine(re);




//        }

//        //public static void Main(string[] args)
//        //{
//        //    //AppDomain.CurrentDomain.ExecuteAssembly(
//        //    //  @"C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe",
//        //    //  null,
//        //    //  new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });

            

//        //    string[] realsargs = (new List<string>(System.Reflection.Assembly.GetExecutingAssembly().Location.SingleItemCollection()).AddCollection(args)).ToArray();

//        //    AppDomain.CurrentDomain.ExecuteAssembly(
//        //     @"C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe",
//        //     realsargs);

//        //     //new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });
//        //}

//        static void Main2(string[] args)
//        {
//            List<string> tracks = new List<string>();
//            tracks.Add("Name");

//            //Accentor<Album> aa = new Accentor<Album>
//            //    (@"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\SQL\insert\hehhehe\musictest_discs.sql", tracks,
//            //    (A, at) => { if (at == null) return DBOperation.Ignore; string n = at["Name"]; if (n == n.WithoutAccent()) return DBOperation.Ignore; A.Name = n; return DBOperation.Update; }, (f) => new DataBaseAlbumUpdater(f));


//            //aa.Fire();

//             //Accentor<Artist> aa = new Accentor<Artist>
//             //    (@"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\SQL\insert\hehhehe\musictest_artists.sql",tracks,
//             //    (A, at) => { if (at == null) return DBOperation.Ignore; string n = at["Name"]; if (n == n.WithoutAccent()) return DBOperation.Ignore; A.Name = n; return DBOperation.Update; }, (f) => new DataBaseArtistUpdater(f));


//            // aa.Fire();
            
//            //tracks.Add("Path");


//            //Accentor<Track> aa = new Accentor<Track>
//            //    (@"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\SQL\insert\hehhehe\musictest_tracks.sql", tracks,
//            //    (t, at) => 
//            //    { 
//            //        if (at == null) return DBOperation.Ignore; 
//            //        string n = at["Name"];
//            //        string p = at["Path"];
//            //        bool needupdaten=(n != n.WithoutAccent());
//            //        bool needupdatep=(p != p.WithoutAccent());
//            //        if ((!needupdaten) && (!needupdatep))
//            //            return DBOperation.Ignore; 
                    
//            //        if (needupdaten)
//            //            t.Name = n;

//            //        if (needupdatep)
//            //            t.Path = p;

//            //        return DBOperation.Update; 
//            //    }
//            //    , (f) => new DataBaseTrackUpdater(f));



//            //aa.Fire();

//            Updater u = new Updater (@"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\SQL\insert\hehhehe\musictest_tracks.sql", tracks);
//            u.Update();

//            tracks.RemoveAt(1);
//            Updater ud = new Updater (@"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\SQL\insert\hehhehe\musictest_discs.sql", tracks);
//            ud.Update();


//            testcollect2();

//            testcollect();

//            testParser();

//            //SetUpData();

//            try
//            {
//                {
//                    //IList<Genre> Genres = null;

//                    //using (IDBSession session = NhibernateSession.CreateorGetCurrentSession())
//                    //{
//                    //    IQuery query = session.Session.CreateQuery("from Genre");
//                    //    Genres = query.List<Genre>();
//                    //}


//                    //DataBaseAlbumUpdater dbtu = new DataBaseAlbumUpdater(
//                    //    tr =>
//                    //    {  if (tr.Author!="V.a")
//                    //        return DBOperation.Ignore;

//                    //    tr.UpdateArist();


//                    //        return DBOperation.Update;
//                    //    });
//                    //dbtu.UpdateDataBase();

//                    ThumbleGenerator acic = new ThumbleGenerator();

//                    DataBaseAlbumUpdater dbtu2 = new DataBaseAlbumUpdater(acic.Albumkanalyser);
//                    dbtu2.UpdateDataBase();


//                    iscrcompteur ic = new iscrcompteur();

//                    DataBaseTrackUpdater dbtu3 = new DataBaseTrackUpdater(ic.Trackanalyser);
//                    dbtu3.UpdateDataBase();

//                    //DataBaseTrackUpdater dbtu = new DataBaseTrackUpdater(
//                    //    tr => { //tr.UpdatePath(); if (SHA1KeyComputer.IsKeyDummy(tr.MD5HashKey)) tr.UpdateKey(); 
//                    //        return DBOperation.Update;
//                    //    });
//                    //dbtu.UpdateDataBase();




//                    //using (IMusicSession session = MusicSession.GetSession())
//                    //{
//                    //    session.GetImporterFactory().GetDBImporter().Load(true);

//                    //    List<ITrack> tracks = new List<ITrack>();

//                    //    foreach (ITrack tr in from al in session.AllAlbums from tr in al.Tracks select tr)
//                    //    {
//                    //        if (tr.Path.Contains(@"..\"))
//                    //        {
//                    //            tracks.Add(tr);
//                    //            Console.WriteLine(tr.Path);
//                    //        }
//                    //    }

//                    //    using (IDBSession session2 = NhibernateSession.CreateorGetCurrentSession( (session as MusicSessionImpl).GetNewSessionContext()))
//                    //    {

//                    //        TrackDAO td = new TrackDAO(session2.Session);

//                    //        using (ITransaction tran = session2.Session.BeginTransaction())
//                    //        {

//                    //            foreach (Track tr in tracks)
//                    //            {
//                    //                td.MakeTransient(tr);
//                    //            }

//                    //            tran.Commit();
//                    //        }
//                    //    }


//                    //}


//                    DateTime agora;// = DateTime.Now;


//                    using (IMusicSession session = MusicSession.GetSession())
//                    {
//                        session.GetDBImporter().Load(true);

//                        agora = DateTime.Now;

//                        int i = 1;

//                        foreach (ITrack tr in from al in session.AllAlbums from tr in al.Tracks select tr)
//                        {
//                            string md5 = tr.MD5HashKey;

//                            if (i++ % 100 == 0)
//                            {
//                                DateTime agora2D = DateTime.Now;
//                                TimeSpan deltaD = agora2D.Subtract(agora);
//                                Console.WriteLine("Time to calucate 100 Hkey: {0}", deltaD);
//                                agora = agora2D;
//                            }
//                        }

//                    }

//                    // DateTime agora;

//                    using (IMusicSession session = MusicSession.GetSession())
//                    {

//                        session.GetImporterFactory().GetDBImporter().Load(true);

//                        agora = DateTime.Now;

//                        IAlbum al = session.AllAlbums[0];

//                        using (IMusicRemover imr = session.GetMusicRemover())
//                        {
//                            imr.AlbumtoRemove.Add(al);
//                            imr.Comit(true);
//                        }

//                        session.GetImporterFactory().GetITunesService(true, null,AlbumMaturity.Discover).Load(true);
//                    }


//                    TimeSpan delta = DateTime.Now - agora;
//                    Console.WriteLine("Imported itune collection in {0}", delta);


//                    DateTime agora2 = DateTime.Now;

//                    using (IMusicSession session = MusicSession.GetSession())
//                    {

//                        //IMusicImporter Imu = session.GetImporterFactory().GetDBImporter();

//                        //Imu.Load(true);

//                        //IList<IAlbum> als = new List<IAlbum>(Imu.ImportedAlbums);
//                    }


//                    TimeSpan delta2 = DateTime.Now - agora2;
//                    Console.WriteLine("Data opened in {0}", delta2);

//                }


//                {

//                    FileStatus fs = FileServices.GetFileStatus(@"C:\Documents and Settings\DEM\My Documents\test\Jailbreack__w._corsano__-_The_Rocker.zip");
//                }

//                {
//                    //IMusicImporter MCI = new MusicCDImporter(0, MusicSession.GetSession() as MusicSessionImpl);
//                    //MCI.Load(true);
//                }

//                {
//                    //CueSheet cs = new CueSheet(@"..\..\..\Output\Postango.cue");

//                    //RarImporterHelper RIH = new RarImporterHelper(null);
//                    //List<Tuple<string, CueSheet>> l = new List<Tuple<string, CueSheet>>();
//                    //l.Add(new Tuple<string, CueSheet>(@"..\..\..\Output\Postango.ape", cs));
//                    //IImporter CI = new MusicCueConverterImporter(l, new List<string>(), RIH);

//                    //CI.Action(null);

//                }

//                {
//                    //CDInfoHandler cih = new CDInfoHandler(0, null);

//                    //ICDInfos[] cinfs = CDInfoProvider.GetInfoFromCD(cih);//todo refactorize
//                    //foreach (ICDInfos cdi in cinfs)
//                    //{
//                    //    Console.WriteLine("Name {0}, Artist {1}", cdi.Name, cdi.Artist);
//                    //}

//                }

//                {
//                    //IMusicFileConverter MFC = MusicFileConverter.GetMusicConverter(@"..\..\..\Output\CD_LR_556.ogg", @"..\..\..\Output\");

//                    //bool res = MFC.ConvertTomp3();
//                    //if (res == false)
//                    //    throw new Exception("MusicFileConverter");

//                    //if (!File.Exists(MFC.ConvertName))
//                    //    throw new Exception("MusicFileConverter");


//                    //MFC = MusicFileConverter.GetMusicConverter(@"..\..\..\Output\Track 02.flac", @"..\..\..\Output\");

//                    //res = MFC.ConvertTomp3();

//                    //if (res == false)
//                    //    throw new Exception("MusicFileConverter");

//                    //if (!File.Exists(MFC.ConvertName))
//                    //    throw new Exception("MusicFileConverter");

//                    //string filename = @"..\..\..\Output\Postango.ape";

//                    //MFC = MusicFileConverter.GetMusicConverter(filename, Path.GetDirectoryName(filename));

//                    //res = MFC.ConvertTomp3(true);

//                    //if (res == false)
//                    //    throw new Exception("MusicFileConverter");


//                    //if (!File.Exists(MFC.ConvertName))
//                    //    throw new Exception("MusicFileConverter");

//                    //if (File.Exists(filename))
//                    //    throw new Exception("MusicFileConverter");

//                }


//                {
//                    //ITrack tr = Track.GetTrackFromPath(@"..\..\..\Output\2 - Coalescence 1.mp3",null,MusicSessionImpl.Session).Result;
//                    //string md5 = tr.MD5HashKey;

//                    //IModifiableAlbum alb = tr.Album.GetModifiableAlbum();
//                    //IModifiableTrack tra = alb.Tracks[0];

//                    //tra.Name = "rasga minha retaguarda";
//                    //tra.Artist = "toto cutogno";
//                    //alb.Name = "Uma noite romantica no pronctologista";

//                    //alb.CommitChanges(true);

//                    //ITrack tr2 = Track.GetTrackFromPath(@"..\..\..\Output\2 - Coalescence 1.mp3",null, MusicSessionImpl.Session).Result;

//                    //string md52 = tr2.MD5HashKey;

//                    //if (md52 != md5)
//                    //    throw new Exception("Md5");

//                    //if (tr2.Album.Name != "Uma noite romantica no pronctologista")
//                    //    throw new Exception("ModifiableAlbum");

//                    //if (tr2.Artist != "toto cutogno")
//                    //    throw new Exception("ModifiableTrack");

//                    //if (tr2.Name != "rasga minha retaguarda")
//                    //    throw new Exception("ModifiableTrack");
//                }





//                {
//                    string path = @"..\..\..\Output\Other Dimensions In Music-Live At The Sunset-2007.part1.rar";

//                    List<string> Path = new List<string>();
//                    Path.Add(@"..\..\..\Output\DHumair_Quatre.part2.rar");
//                    Path.Add(@"..\..\..\Output\DorLean-Inta320.rar");
//                    IMusicImporter mu = MusicSession.GetSession().GetImporterFactory().GetMultiRarImporter(Path, AlbumMaturity.Discover);
//                    mu.Load(true);


//                    IMusicImporter ara = MusicSession.GetSession().GetImporterFactory().GetRarImporter(path, AlbumMaturity.Discover);
//                    ara.Load(true);
//                }

//                {
//                    DateTime agora = DateTime.Now;
//                    MusicSession.GetSession().GetImporterFactory().GetITunesService(true,null, AlbumMaturity.Discover).Load(true);
//                    TimeSpan delta = DateTime.Now - agora;
//                    Console.WriteLine("Imported itune collection in {0}", delta);


//                    var Tracks = (from Al in MusicSession.GetSession().AllAlbums from track in Al.Tracks select track).ToList();




//                    int Count = Tracks.Count;
//                    int Current = 0;

//                    //MusicSession.Session.AllTracks.AsParallel().Select(tr => tr.MD5HashKey).ForAll(Console.WriteLine);


//                    DateTime agora2 = DateTime.Now;
//                    HashSet<string> HSS = new HashSet<string>();
//                    int coll = 0;

//                    foreach (ITrack tra in Tracks)
//                    {
//                        string md5 = tra.MD5HashKey;
//                        if ((md5 != null) && !HSS.Add(md5))
//                            coll++;
//                        Console.WriteLine("Processed {0} file(s) collisions: {1}", ++Current, coll);
//                    }

//                    TimeSpan delta2 = DateTime.Now - agora2;
//                    Console.WriteLine("Compute mda in {0}", delta2);

//                    Console.WriteLine("Imported itune collection in {0} for {1} tracks", delta2, Count);
//                }
//            }
//            finally
//            {
//                //CleanUp();
//            }


//        }
//    }
//}
