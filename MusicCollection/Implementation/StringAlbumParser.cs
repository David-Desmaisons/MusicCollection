using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using MusicCollection.ToolBox;

namespace MusicCollection.Implementation
{
    internal class StringAlbumParser
    {
        private string _Name=null;
        private string _Authour=null;
        private int? _Year=null;
        private static readonly Regex _DateParser;
        private static readonly Regex _NameParser;

        private bool _FoundSomething = false;

        private StringAlbumParser(string NameClue)
        {
            if (string.IsNullOrEmpty(NameClue))
            {
                _Authour = "";
                _Name = "";
                return;
            }

            if (_DateParser.IsMatch(NameClue))
            {
                _Year = int.Parse(_DateParser.Match(NameClue).Value);
            }

            if (!_NameParser.IsMatch(NameClue))
            {
                _Authour = NameClue;
                _Name = NameClue;
                return;
            }
                
            Match m = _NameParser.Match(NameClue);

            _Authour = m.Groups[1].Value;
            _Name = m.Groups[2].Value;

            _FoundSomething = true;
      
        }

        private StringAlbumParser()
        {
             _Authour = "";
             _Name = "";
            _Year = null;
        }

        private StringAlbumParser(string iAuthor, string iName,int? iYear)
        {
             _Authour = iAuthor;
             _Name = iName;
            _Year = iYear;
        }

        static internal StringAlbumParser FromString(string Name)
        {
            return new StringAlbumParser(Name);
        }

        static internal StringAlbumParser FromDirectory(string Name)
        {
            return new StringAlbumParser(Path.GetFileName(Name));
        }

        static internal StringAlbumParser FromRarZipName(string Name)
        {

            return new StringAlbumParser(FileInternalToolBox.RawRarName(Name));
        }

        static internal StringAlbumParser FromDirectoryHelper(DirectoryHelper  dh)
        {
            if ((dh == null) || (dh.Count==0))
                return new StringAlbumParser();

            if(dh.Count==1)
                return new StringAlbumParser(dh.Path);


            int? Year = null;

            string last =dh[dh.Count-1];

            if (_DateParser.IsMatch(last))
            {
                Year = int.Parse(_DateParser.Match(last).Value);
            }

            return new StringAlbumParser(dh[dh.Count-2],last,Year);
        }

        static StringAlbumParser()
        {
            _DateParser = new Regex(@"(19\d{2})|(2\d{3})");
            _NameParser = new Regex(@"^\s*(\w[^-]*?)\s*-\s*(.*?)\s*$");
           // _RarParser = new Regex(@"^(\w.*?)(\.part\d)?\.\w{3}$");
        }

        internal string AlbumName
        {
            get { return _Name; }
        }

        internal string AlbumAuthor
        {
            get { return _Authour; }
        }

        internal int? AlbumYear
        {
            get{ return _Year;}
        }

        internal bool FounSomething
        {
            get { return _FoundSomething; }
        }
    }

}
