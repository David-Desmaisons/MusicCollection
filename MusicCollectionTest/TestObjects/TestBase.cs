using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

using MusicCollection.Infra;
using MusicCollection.DataExchange;
using MusicCollection.Fundation;
using MusicCollectionTest.Integrated;

namespace MusicCollectionTest.TestObjects
{
    internal abstract class TestBase : AlbumComparer
    {
        protected string MYIn;

        private string _MYOut;


        protected string MYOut
        {
            get 
            {
                if (_MYOut == null)
                {
                    _MYOut = Path.Combine(Path.GetFullPath(@"..\..\TestFolders\OutFolder"), this.GetType().Name);
                    Directory.CreateDirectory(MYOut);
                }

                return _MYOut;

            }
        }

        protected IList<IList<IFullAlbumDescriptor>> AlbumsOld;
        protected IList<IList<AlbumDescriptor>> Albums;


        protected string GetFileInName(string iName)
        {
            return Path.Combine(MYIn, iName);
        }

        protected string GetFileOutName(string iName)
        {
            return Path.Combine(MYOut, iName);
        }

        protected string GetFileOutName(string iName,string Ext)
        {
            return Path.Combine(MYOut, iName + Ext);
        }

        protected string GetFileOutNameRandom(string Ext)
        {
            return GetFileOutName(Guid.NewGuid().ToString(), Ext);
        }


        protected TestBase()
        {
            new Preparator().GlobalSetUp();

            MYIn = Path.Combine(Path.GetFullPath(@"..\..\TestFolders\InToBeCopied"), this.GetType().Name);
           

            AlbumsOld = new List<IList<IFullAlbumDescriptor>>();

            bool continu = true;
            int i = 0;

            while (continu)
            {
                string iPath = Path.Combine(MYIn, string.Format("AlbumRef{0}.xml", i++));
                if (File.Exists(iPath))
                {
                    AlbumsOld.Add(ExportAlbums.Import(iPath, false, String.Empty, null).ToList<IFullAlbumDescriptor>());
                }
                else
                    continu = false;
            }


            Albums = new List<IList<AlbumDescriptor>>();

            continu = true;
            i = 0;

            while (continu)
            {
                string iPath = Path.Combine(MYIn, string.Format("Album{0}.xml", i++));
                if (File.Exists(iPath))
                {
                    Albums.Add(AlbumDescriptorExchanger.Import(iPath, false, String.Empty, null));
                }
                else
                    continu = false;
            }

            Albums.SelectMany(o => o).SelectMany(u => u.RawTrackDescriptors.Select(t => new Tuple<AlbumDescriptor, TrackDescriptor>(u, t))).Apply(o => Assert.That(o.Item1, Is.EqualTo(o.Item2.AlbumDescriptor)));

        }


    }
}
