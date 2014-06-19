using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MusicCollection.Utilies
{

    public class StringTrackParser
    {
        private string _Name;
        private uint? _TrackNumber;
        private bool _IsDummy = false;
        private bool _FoundSomething = false;

        private static readonly Regex _TrackDummyParser;
        private static readonly Regex _TrackSimpleParser;


        public StringTrackParser(string NameClue, bool IsFileName = true)
        {
            string NC = (IsFileName) ? Path.GetFileNameWithoutExtension(NameClue) : NameClue;

            if (string.IsNullOrEmpty(NameClue))
            {
                _IsDummy = true;
                _FoundSomething = true;
                return;
            }


            if (_TrackDummyParser.IsMatch(NC))
            {

                Match m = _TrackDummyParser.Match(NC);
                _TrackNumber = uint.Parse(m.Groups[1].Success ? m.Groups[1].Value : m.Groups[4].Value);
                _IsDummy = true;
                _Name = NC;
                _FoundSomething = true;
                return;
            }

            if (_TrackSimpleParser.IsMatch(NC))
            {
                _TrackNumber = uint.Parse(_TrackSimpleParser.Match(NC).Groups[1].Value);
                _IsDummy = false;
                _Name = _TrackSimpleParser.Match(NC).Groups[2].Value;
                _FoundSomething = true;
                return;
            }

            _Name = NC;
        }

        static StringTrackParser()
        {
            _TrackDummyParser = new Regex(@"(?i)(^\d{1,2}$)|((faixa|traccia|track|morceau)\s*(\d{1,2}))");
            // _TrackDummyParser = new Regex(@"(?i)(faixa|traccia|track|morceau)\s*(\d{1,2})");
            //_TrackDummyParser = new Regex(@"(?i)[faixa|track|morceau]\s*[(0\d)|(\d{1,2})]");
            _TrackSimpleParser = new Regex(@"^\s*(\d{1,2})\s*[\.|-]?\s*(\S.*\S)\s*$");
        }

        public string TrackName
        {
            get { return _Name; }
        }

        public uint? TrackNumber
        {
            get { return _TrackNumber; }
        }

        public bool IsDummy
        {
            get { return _IsDummy; }
        }

        public bool FounSomething
        {
            get { return _FoundSomething; }
        }
    }
}
