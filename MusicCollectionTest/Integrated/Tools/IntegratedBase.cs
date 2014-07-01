using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

using NUnit;
using NUnit.Framework;

using SevenZip;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.Infra;
using MusicCollection.ToolBox;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollection.DataExchange;
using MusicCollectionTest.TestObjects;

namespace MusicCollectionTest.Integrated.Tools
{
    public abstract class IntegratedBase : AlbumComparer
    {
        //public enum AlbumDescriptorCompareMode { AlbumMD, AlbumandTrackMD };


        protected string DirectoryIn;

        private  MusicFolderHelper _MFH;
  
        protected internal IntegratedBase()
        {
           
            new Preparator().GlobalSetUp();    
            string realpath = Path.GetFullPath(Path.Combine(@".\7z.dll"));
            SevenZipExtractor.SetLibraryPath(realpath);
        }

        protected string GetFileOutName(string iName)
        {
            return Path.Combine(DirectoryOut, iName);
        }

        protected IList<IList<ExportAlbum>> OldAlbums;
        protected IList<IList<AlbumDescriptor>> Albums;

        private string InputDirectorySimpleName
        {
            get
            {
                return this.GetType().Name; ;
            }
        }

     

        private string _DI;
        protected string RootDirectoryIn
        {
            get
            {
                if (_DI == null)
                {
                    _DI = Path.GetFullPath(@"..\..\TestFolders\InFolder");
                }
                return _DI;
            }
        }

        private string _DO;
        protected string DirectoryOut
        {
            get
            {
                if (_DO == null)
                {
                    _DO = Path.GetFullPath(@"..\..\TestFolders\OutFolder");
                }
                return _DO;
            }
        }

        private string _TempIn;
        protected string TempDirectoryIn
        {
            get
            {
                if (_TempIn == null)
                {
                    _TempIn = Path.Combine(DirectoryIn, "TempDir");

                }

                if (!Directory.Exists(_TempIn))
                    Directory.CreateDirectory(_TempIn);

                return _TempIn;
            }
        }

        private string _TempOut;
        protected string TempDirectoryOut
        {
            get
            {
                if (_TempOut == null)
                {
                    _TempOut = Path.Combine(DirectoryOut, "TempDir");
                }

                if (!Directory.Exists(_TempOut))
                    Directory.CreateDirectory(_TempOut);

                return _TempOut;
            }
        }

        protected virtual Nullable<bool> OpenClean
        {
            get { return true; }
        }

        private string CopyName
        {
            get;
            set;
        }

        private string CopyNameDB
        {
            get;
            set;
        }     

        protected virtual SessionKeys GetKeys()
        {
            return new SessionKeys(DirectoryOut, OpenClean,true);
        }

       

        private void CopyDBIfNeeded()
        {
            string DDBCopy = Path.Combine(Path.GetFullPath(@"..\..\TestFolders\DBToBeCopied"), this.CopyNameDB);

             DirectoryInfo dif = new DirectoryInfo(DDBCopy);

            if (!dif.Exists)
                return;

            foreach(FileInfo fi in dif.GetFiles())
            {
                fi.CopyTo(Path.Combine(Root, fi.Name), true);
            }  
         }

        protected void Init()
        {

            _MFH = new MusicFolderHelper(DirectoryOut);
            //_SK = GetKeys();
            DirectoryIn = Path.Combine(RootDirectoryIn, InputDirectorySimpleName);
 
            object[] PIs = this.GetType().GetCustomAttributes(typeof(TestFolderAttribute), false);

            if ((PIs != null) && (PIs.Length > 0))
            {
                TestFolderAttribute tfa = PIs[0] as TestFolderAttribute;
                if (!Directory.Exists(DirectoryIn))
                {
                    Directory.CreateDirectory(DirectoryIn);
                }

                CopyName = tfa.InFolderName ?? InputDirectorySimpleName;
                CopyNameDB = tfa.DBFolderName ?? InputDirectorySimpleName;


                DirectoryInfo dif = new DirectoryInfo(Path.Combine(Path.GetFullPath(@"..\..\TestFolders\InToBeCopied"), CopyName));
                dif.Copy(DirectoryIn);


                dif = new DirectoryInfo(DirectoryIn);
                dif.RemoveReadOnly();


                //IsOverride = true;
            }
            else
            {
                CopyName = InputDirectorySimpleName;
                CopyNameDB = InputDirectorySimpleName;
            }

            CopyDBIfNeeded();

            _SK = GetKeys();
            
            OldAlbums = new List<IList<ExportAlbum>>();

            bool continu = true;
            int i = 0;

            while (continu)
            {
                string iPath = Path.Combine(DirectoryIn, string.Format("AlbumRef{0}.xml", i++));
                if (File.Exists(iPath))
                {
                    OldAlbums.Add(ExportAlbums.Import(iPath, false, String.Empty, null));
                }
                else
                    continu = false;
            }


            Albums = new List<IList<AlbumDescriptor>>();

            continu = true;
            i = 0;

            while (continu)
            {
                string iPath = Path.Combine(DirectoryIn, string.Format("Album{0}.xml", i++));
                if (File.Exists(iPath))
                {
                    Albums.Add(AlbumDescriptorExchanger.Import(iPath, false, String.Empty, null));
                }
                else
                    continu = false;
            }

            Albums.SelectMany(o => o).SelectMany(u => u.RawTrackDescriptors.Select(t => new Tuple<AlbumDescriptor, TrackDescriptor>(u, t))).Apply(o => Assert.That(o.Item1, Is.EqualTo(o.Item2.AlbumDescriptor)));

        }


        protected internal SessionKeys _SK;



      


        protected virtual string Cache
        {
            get { return _MFH.Cache; }
        }

        protected virtual string Root
        {
            get { return _MFH.Root; }
        }

        protected void CleanDirectories(bool tot = true)
        {
            DirectoryInfo di = new DirectoryInfo(Cache);
            di.RemoveReadOnly();
            di.Empty(true);

           

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (tot)
            {
                DirectoryInfo diroot = new DirectoryInfo(Root);
                diroot.RemoveReadOnly();
                diroot.Empty(true);
                //Directory.Delete(Root, true);

                //while (diroot.Exists)
                //{
                //    System.Threading.Thread.Sleep(200);
                //    di.Refresh();
                //}
            }
        }

        protected void BigClean()
        {

            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                if (ms.AllAlbums.Count != 0)
                {
                    IMusicRemover imr = ms.GetMusicRemover();
                    imr.IncludePhysicalRemove = false;
                    imr.AlbumtoRemove.AddCollection(ms.AllAlbums);
                    imr.Comit(true);
                }

                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
            }
            Reinitialize();
        }


        protected void LoadTest(IEnumerable<IFullAlbumDescriptor> @ref = null)
        {
            using (IMusicSession ms = MusicSessionImpl.GetSession(_SK.Builder))
            {
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(0));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                IMusicImporter imi = ms.GetDBImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();
                Assert.That(ms.AllAlbums.Count, Is.EqualTo(0));
                Assert.That(ms.AllGenres.Count, Is.EqualTo(25));
                Assert.That(ms.AllArtists.Count, Is.EqualTo(0));

                Console.WriteLine("Importing Music Folder");
                IDirectoryImporterBuilder imib = ms.GetImporterBuilder(MusicImportExportType.Directory) as IDirectoryImporterBuilder;
                Assert.That(imib, Is.Not.Null);
                imib.Directory = DirectoryIn;
                imib.DefaultAlbumMaturity = AlbumMaturity.Discover;

                Assert.That(imib.IsValid, Is.True);
                imi = imib.BuildImporter();
                Assert.That(imi, Is.Not.Null);
                imi.Load();

                if (@ref != null)
                    AssertAlbums(ms, @ref, AlbumDescriptorCompareMode.AlbumandTrackMD);

            }
        }

        protected void Reinitialize()
        {
            CleanDirectories(false);

            DirectoryInfo root = new DirectoryInfo(Root);
            root.RemoveReadOnly();
            root.Empty(true);

            DirectoryInfo In = new DirectoryInfo(DirectoryIn);
            In.RemoveReadOnly();
            In.Empty(true);

            string Pathentry = Path.Combine(Path.GetFullPath(@"..\..\TestFolders\InToBeCopied"), this.CopyName);

            DirectoryInfo dif = new DirectoryInfo(Pathentry);
            dif.Copy(DirectoryIn);

            In = new DirectoryInfo(DirectoryIn);
            In.RemoveReadOnly();

            _SK = GetKeys();

            CopyDBIfNeeded();


        }
    }
}
