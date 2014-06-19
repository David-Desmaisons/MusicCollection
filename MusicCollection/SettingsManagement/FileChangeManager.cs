using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
using MusicCollection.Properties;
using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using System.Diagnostics;


namespace MusicCollection.SettingsManagement
{
    internal abstract class FileChangeManager
    {
        private IImportContext _Context;

        private Dictionary<Tuple<string, string>, string> _Corr = new Dictionary<Tuple<string, string>, string>();

        protected FileChangeManager(IImportContext Ms)
        {
            _Context = Ms;
        }

        protected IImportContext Context
        {
            get { return _Context; }
        }

        protected Tuple<string, string> FromClue(IImportHelper Cue)
        {
            return new Tuple<string, string>(Cue.AlbumArtistClue, Cue.AlbumNameClue);
        }

        protected string FindDirectory(Tuple<string, string> Tu)
        {
            string Dir = null;
            _Corr.TryGetValue(Tu, out Dir);
            return Dir;
        }

        protected void RegisterDirectory(Tuple<string, string> Tu, string Direct)
        {
              _Corr.Add(Tu, Direct);
        }

        internal string ComputeName(IImportHelper Cue)
        {
            Tuple<string, string> CT = FromClue(Cue);

            string Directory = FindDirectory(CT);

            if (Directory != null)
                return Directory;

            Directory = Context.Folders.CreateMusicalFolder(Cue);// FileHelper.CreateMusicalFolder(Cue);
            RegisterDirectory(CT, Directory);

            return Directory;
        }
           

        protected bool Copy(string FileName, string dirOut)
        {
            string Original = Path.GetFileName(FileName);
            string Final = Path.Combine(dirOut, Original);

            try
            {
                File.Move(FileName, Final);
            }
            catch (DirectoryNotFoundException e)
            {
                Trace.WriteLine("Problem moving file" + e.ToString());
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem moving file" + ex.ToString());
                Context.OnFactorisableError<UnableToCopyFile2>(FileName);
            }

            return true;
        }

        protected bool Delete(string FileName, bool Rever=true)
        {
            try
            {
                FileExtender.RevervibleFileDelete(FileName, Rever);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Problem Deleting file" + ex.ToString());
                Context.OnFactorisableError<UnableToDeleteFile>(FileName);
            }

            return true;
        }
    }
}
