using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using NUnit;
using NUnit.Framework;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;

using MusicCollectionTest.Integrated.Session_Accessor;
using MusicCollection.DataExchange;
using MusicCollectionTest.Integrated.Tools;

namespace MusicCollectionTest.Integrated
{
    [SetUpFixture]
    public class Preparator
    //: IntegratedBase
    {
        public Preparator()
        {
        }

 
        private MusicFolderHelper _MFH;
        private static bool _EnvironmentInitialised = false;

        //[SetUp]
        public void GlobalSetUp()
        {
            if (_EnvironmentInitialised == false)
            {

                DirectoryInfo difo = new DirectoryInfo(Path.GetFullPath(@"..\..\TestFolders\InFolder"));
                if (difo.Exists)
                {
                    difo.RemoveReadOnly();
                    difo.Empty(true);
                }

                difo = new DirectoryInfo(Path.GetFullPath(@"..\..\TestFolders\OutFolder"));
                if (difo.Exists)
                {
                    difo.RemoveReadOnly();
                    difo.Empty(true);
                }

                DirectoryInfo dif = new DirectoryInfo(Path.GetFullPath(@"..\..\TestFolders\InToBeCopied"));
                dif.Copy(Path.GetFullPath(@"..\..\TestFolders\InFolder"));

                difo = new DirectoryInfo(Path.GetFullPath(@"..\..\TestFolders\InFolder"));
                difo.RemoveReadOnly();

                _MFH = new MusicFolderHelper(Path.GetFullPath(@"..\..\TestFolders\OutFolder"));
                CleanDirectories(_MFH);

                _MFH = new MusicFolderHelper(Path.GetFullPath(@"..\..\TestFolders\OutFolder"));
                _EnvironmentInitialised = true;
            }
        }

        private void CleanDirectories(MusicFolderHelper imfh)
        {
            DirectoryInfo di = new DirectoryInfo(imfh.Cache);
            di.RemoveReadOnly();
            di.Empty(true);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            DirectoryInfo diroot = new DirectoryInfo(imfh.Root);
            diroot.RemoveReadOnly();
            diroot.Empty(true);
        }

        [TearDown]
        public void teardown()
        {
            if (_EnvironmentInitialised)
            {
                DirectoryInfo dif = new DirectoryInfo(Path.GetFullPath(@"..\..\TestFolders\InFolder"));
                dif.Empty(true);
            }
        }
    }
}
