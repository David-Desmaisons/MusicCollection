using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using MusicCollection.FileConverter;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;
using MusicCollection.Implementation;
using MusicCollection.Fundation;

namespace MusicCollection.FileImporter
{
    static internal class CollectorFactory
    {
        private interface IImageProvider
        {
            List<string> Image
            {
                get;
            }
        }

        private class RarCollector : ICollector
        {
            private string _Rar = null;
            private IInternalMusicSession _IMusicConverter;

            internal RarCollector(IInternalMusicSession imc, string name)
            {
                _IMusicConverter = imc;
                _Rar = name;
            }

            public ImporterConverterAbstract Importer
            {
                get { return new RarImporter(_IMusicConverter,_Rar); }
            }

            public bool IsSealed
            {
                get { return true; }
            }

            public ICollector Merge(ICollector coll)
            {
                throw new InvalidProgramException("Algorithmic error");
            }

            public bool IsMergeable(ICollector coll)
            {
                return false;
            }
        }

        private class ImageCollector : ICollector, IImageProvider
        {
            private List<string> _ListImage = null;

            internal ImageCollector(List<string> Images)
            {
                _ListImage = Images;
            }

            List<string> IImageProvider.Image
            {
                get { return _ListImage; }
            }

            public ImporterConverterAbstract Importer
            {
                get { return null; }
            }

            public bool IsSealed
            {
                get { return false; }
            }

            public ICollector Merge(ICollector coll)
            {
                if (!this.IsMergeable(coll))
                    throw new InvalidProgramException("Algorithmic error");

                IImageProvider ic = coll as IImageProvider;

                if (ic != null)
                {
                    List<string> Images = new List<string>(_ListImage);
                    Images.AddRange(ic.Image);
                    return new ImageCollector(Images);
                }

                return coll.Merge(this);
            }

            public bool IsMergeable(ICollector coll)
            {
                IImageProvider ic = coll as IImageProvider;

                if (ic != null)
                    return true;

                return coll.IsMergeable(this);
            }
        }

        private abstract class BaseCollectorForMusic : ICollector
        {
            protected List<string> _ListImage = null;
            protected IImportHelper _ClueName;

            protected internal BaseCollectorForMusic(List<string> Image, IImportHelper ClueName)
            {
                _ListImage = Image;
                _ClueName = ClueName;
            }

            protected abstract BaseCollectorForMusic Clone(List<string> Image);

            public abstract ImporterConverterAbstract Importer
            {
                get;
            }

            public bool IsSealed
            {
                get
                {
                    return _ListImage.Count > 0;
                }
            }

            public ICollector Merge(ICollector coll)
            {
                if (!this.IsMergeable(coll))
                    throw new InvalidProgramException("Algorithmic error");

                IImageProvider mp = coll as IImageProvider;

                if (mp == null)
                    throw new InvalidProgramException("Algorithmic error");

                return Clone(mp.Image);

            }

            public bool IsMergeable(ICollector coll)
            {
                if (IsSealed)
                    return false;

                IImageProvider mp = coll as IImageProvider;

                return (mp != null);
            }

        }

        private abstract class CollectorForMusic : BaseCollectorForMusic, ICollector
        {
            protected List<string> _ListMusic = null;

            internal CollectorForMusic(List<string> Music, List<string> Image, IImportHelper ClueName)
                : base(Image, ClueName)
            {
                _ListMusic = Music;
            }

        }

        private class CollectorForImportMusic : CollectorForMusic
        {
            internal CollectorForImportMusic(List<string> Music, List<string> Image, IImportHelper ClueName)
                : base(Music, Image, ClueName)
            {
            }


            override public ImporterConverterAbstract Importer
            {
                get
                {
                    return new MusicImporter(_ListMusic, _ListImage, _ClueName);
                }
            }

            override protected BaseCollectorForMusic Clone(List<string> Image)
            {
                return new CollectorForImportMusic(_ListMusic, Image, _ClueName);
            }
        }

        private class CollectorForCueConvertMusic : BaseCollectorForMusic
        {
            private List<Tuple<string, AlbumDescriptor>> _MusicandCue = null;
            private IMusicConverter _IMusicConverter;

            internal CollectorForCueConvertMusic(IMusicConverter imc, List<Tuple<string, AlbumDescriptor>> MusicandCue, 
                        List<string> Image, IImportHelper ClueName)
                : base(Image, ClueName)
            {
                _IMusicConverter = imc;
                _MusicandCue = MusicandCue;
            }

            internal CollectorForCueConvertMusic(IMusicConverter imc, string Music, List<string> Image, AlbumDescriptor Cue, IImportHelper ClueName)
                : base(Image, ClueName)
            {
                _IMusicConverter = imc;
                _MusicandCue = new List<Tuple<string, AlbumDescriptor>>();
                _MusicandCue.Add(new Tuple<string, AlbumDescriptor>(Music, Cue));
            }


            override public ImporterConverterAbstract Importer
            {
                get
                {
                    return new MusicCueConverterImporter(_IMusicConverter,_MusicandCue, _ListImage, _ClueName);
                }
            }

            override protected BaseCollectorForMusic Clone(List<string> Image)
            {
                return new CollectorForCueConvertMusic(_IMusicConverter,_MusicandCue, Image, _ClueName);
            }
        }

        private class CollectorForConvertMusic : CollectorForMusic
        {
            private IMusicConverter _IMusicConverter;
            internal CollectorForConvertMusic(IMusicConverter iMusicConverter, List<string> Music, 
                List<string> Image, IImportHelper ClueName)
                : base(Music, Image, ClueName)
            {
                _IMusicConverter = iMusicConverter;
            }


            override public ImporterConverterAbstract Importer
            {
                get
                {
                    return new MusicConverterImporter(_IMusicConverter,_ListMusic, _ListImage, _ClueName);
                }
            }

            override protected BaseCollectorForMusic Clone(List<string> Image)
            {
                return new CollectorForConvertMusic(_IMusicConverter,_ListMusic, Image, _ClueName);
            }
        }


        static internal ICollector CollectorForMusicToImport(List<string> Music, List<string> Image, IImportHelper ClueName)
        {
            return new CollectorForImportMusic(Music, Image, ClueName);
        }

        private class CuePairingHelper
        {
            private IList<string> _Music;
            private IEnumerable<AlbumDescriptor> _Cues;
            private List<Tuple<string, AlbumDescriptor>> _Result = null;

            internal CuePairingHelper(IList<string> Music, IEnumerable<AlbumDescriptor> Cues)
            {
                _Music = Music;
                _Cues = Cues;
            }

            private Func<string, string> _MusicGlue;
            internal CuePairingHelper SetMusicGlue(Func<string, string> mg)
            {
                _MusicGlue = mg;
                return this;
            }

            private Func<AlbumDescriptor, string> _CueGlue;
            internal CuePairingHelper SetCueGlue(Func<AlbumDescriptor, string> cg)
            {
                _CueGlue = cg;
                return this;
            }

            internal CuePairingHelper Compute()
            {
                var dic = _Cues.ToLookup(_CueGlue);

                HashSet<AlbumDescriptor> cues = new HashSet<AlbumDescriptor>();

                _Result = (from M in _Music
                               let junc = _MusicGlue(M)
                               let C = (dic.Contains(junc)) ? dic[junc].First(c=>!cues.Contains(c)) : null
                               let ok = (C==null)  ? true : cues.Add(C)
                           select new Tuple<string, AlbumDescriptor>(M, C)).ToList();

                return this;
            }

            internal bool FullMatch
            {
                get
                {
                    return (_Result == null) ? false : Matches.Count == _Music.Count;
                }
            }

            internal List<Tuple<string, AlbumDescriptor>> Matches
            {
                get
                {
                    return (_Result == null) ? null : (from r in _Result where r.Item2!=null select r).ToList();
                }
            }

            internal List<string> Remaining
            {
                get
                {
                    return (_Result == null) ? null : (from r in _Result where r.Item2 == null select r.Item1).ToList();
                }
            }

        }

        static internal ICollector CollectorForMusicToConvert(IMusicConverter iMusicConverter,
            List<string> Music, List<string> Image, List<string> Cue, IImportHelper ClueName, IEventListener IEL)
        {
            int ccont = Cue.Count;
            int mcount =Music.Count;
            if (ccont >= mcount)
            {
                if ((ccont == 1) && (mcount==1))
                {
                    AlbumDescriptor ad = AlbumDescriptor.FromCUESheet(Cue[0]);

                    if (ad != null)
                    {

                        string mus = Path.GetFileNameWithoutExtension(Music[0]).RemoveInvalidCharacters().ToLower();

                        if ((Path.GetFileNameWithoutExtension(ad.CUEFile).RemoveInvalidCharacters().ToLower() == mus) ||
                            (mus == Path.GetFileNameWithoutExtension(Cue[0]).RemoveInvalidCharacters().ToLower()))
                        {

                           // var length = ad.TrackDescriptors[ad.TrackDescriptors.Count-1].CueIndex01

                            if ((ad.CheckCueConsistency() == false) || (iMusicConverter.GetFileLengthInSeconds(Music[0]) <= ad.GetCueMinLengthInseconds()))
                            //|| ad.TrackDescriptors[ad.TrackDescriptors.Count-1])
                            {
                                CueWillbeDiscarded cwd = new CueWillbeDiscarded(Cue[0],Music[0]);
                                IEL.Report(cwd);

                                if (cwd.Continue == false)
                                    return null;
                            }
                            else
                            {
                                return new CollectorForCueConvertMusic(iMusicConverter, Music[0], Image, ad, ClueName);
                            }
                        }
                    }

                }
                else
                {
                    var cues = Cue.Select(c=>AlbumDescriptor.FromCUESheet(c)).Where(c=>c!=null);

                    //Cherchons les matchs parfaits
                    CuePairingHelper cph = new CuePairingHelper(Music, cues).SetCueGlue(c => Path.GetFileName(c.CUEFile).RemoveInvalidCharacters().ToLower())
                        .SetMusicGlue(M => Path.GetFileName(M).RemoveInvalidCharacters().ToLower()).Compute();

                    List<Tuple<string, AlbumDescriptor>> OK = cph.Matches;

                    if (cph.FullMatch)
                    {
                        return new CollectorForCueConvertMusic(iMusicConverter,OK, Image, ClueName);
                    }

                    //Cherchons les matchs imparfaits pour les cue restants             
                    cph = new CuePairingHelper(cph.Remaining, cues).SetCueGlue(c => Path.GetFileNameWithoutExtension(c.CUEFile).RemoveInvalidCharacters().ToLower())
                        .SetMusicGlue(M => Path.GetFileNameWithoutExtension(M).RemoveInvalidCharacters().ToLower()).Compute();

                    OK.AddRange(cph.Matches);


                    if (cph.FullMatch)
                    {
                        return new CollectorForCueConvertMusic(iMusicConverter,OK, Image, ClueName);
                    }

                    //Derniere chance    
                    cph = new CuePairingHelper(cph.Remaining, cues).SetCueGlue(c => Path.GetFileNameWithoutExtension(c.CUESheetFileName).RemoveInvalidCharacters().ToLower())
                        .SetMusicGlue(M => Path.GetFileNameWithoutExtension(M).RemoveInvalidCharacters().ToLower()).Compute();

                    if (cph.FullMatch)
                    {
                        OK.AddRange(cph.Matches);
                        return new CollectorForCueConvertMusic(iMusicConverter,OK, Image, ClueName);
                    }

                }


            }

            return new CollectorForConvertMusic(iMusicConverter,Music, Image, ClueName);
        }

        static internal ICollector CollectorImages(List<string> Image)
        {
            return new ImageCollector(Image);
        }

        static internal IEnumerable<ICollector> CollectorRar(IInternalMusicSession iMusicConverter, IEnumerable<string> rar)
        {
            HashSet<string> rarFiles = new HashSet<string>();

            foreach (string file in rar)
            {
                string filename = Regex.Replace(file, @"\.part([0-9]+)\.rar", m =>  new StringBuilder(".part").Append('0',m.Groups[1].Value.Length-1).Append("1.rar").ToString());
                if (!File.Exists(filename))
                    continue;

                //mesh name
                if (rarFiles.Add(filename))
                    yield return new RarCollector(iMusicConverter, filename);
            }


            yield break;

        }
    }
}
