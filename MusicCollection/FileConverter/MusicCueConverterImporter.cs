using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;
using MusicCollection.Infra;
using System.Threading;

namespace MusicCollection.FileConverter
{
    internal class MusicCueConverterImporter : FileConverterAbstract, IImporter
    {
        private List<string> _ListImage = null;
        private List<Tuple<string, AlbumDescriptor>> _MusicandCueFile = null;
        private IImportHelper _ClueName = null;
        private bool _Done = false;
        private List<string> _MusicConverted = new List<string>();
        private IMusicConverter _IMusicConverter;

        internal MusicCueConverterImporter(IMusicConverter iIMusicConverter, List<Tuple<string, AlbumDescriptor>> MusicandCue, List<string> Image, IImportHelper ClueName)
        {
            _IMusicConverter = iIMusicConverter;
            _MusicandCueFile = MusicandCue;
            _ListImage = Image;
            _ClueName = ClueName;
        }

        private bool Analyse()
        {
            foreach (Tuple<string, AlbumDescriptor> MAC in _MusicandCueFile)
            {
                if (
                    (MAC.Item2.RawTrackDescriptors.Count == 0) ||
                    ((Path.GetFileNameWithoutExtension(MAC.Item2.CUESheetFileName).RemoveInvalidCharacters().ToLower() != Path.GetFileNameWithoutExtension(MAC.Item1).RemoveInvalidCharacters().ToLower()) &&
                    (Path.GetFileNameWithoutExtension(MAC.Item2.CUEFile).RemoveInvalidCharacters().ToLower() != Path.GetFileNameWithoutExtension(MAC.Item1).RemoveInvalidCharacters().ToLower())))
                {
                    //Cue does not match.
                    return false;
                }
            }

            return true;
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            if (_Done)
                throw new InvalidOperationException("reentrance");

            _Done = true;

            if (!Analyse())
            {
                //Cue does not match..let's do the easy way.
                return new MusicConverterImporter(_IMusicConverter, _MusicandCueFile.Select(i => i.Item1).ToList(), _ListImage, _ClueName);
            }

            foreach (Tuple<string, AlbumDescriptor> MAC in _MusicandCueFile)
            {
                _MusicConverted.Add(MAC.Item1);
                _MusicConverted.Add(MAC.Item2.CUESheetFileName);
            }

            List<TrackConverted> tracks = new List<TrackConverted>();

            SpaceChecker sc = new SpaceChecker(Context.ConvertManager.PathFromOutput(_MusicandCueFile[0].Item1, _ClueName),
                from mac in _MusicandCueFile select mac.Item1);

            if (!sc.OK)
            {
                iel.Report(new NotEnougthSpace(sc.ToString()));
                return null;
            }

            if (iCancellationToken.IsCancellationRequested)
            {
                return null;
            }

            foreach (Tuple<string, AlbumDescriptor> MAC in _MusicandCueFile)
            {
                string MusicPath = MAC.Item1;
                AlbumDescriptor Cs = MAC.Item2;

                int Current = 1;
                iel.Report(new ConvertProgessEventArgs(_ClueName.DisplayName, Current, Cs.RawTrackDescriptors.Count));

                bool OK = false;

                using (IMusicfilesConverter imcc = _IMusicConverter.GetMusicConverter(MusicPath, Cs.RawTrackDescriptors, Context.ConvertManager.PathFromOutput(MusicPath, _ClueName), Context.Folders.Temp))
                {

                    IProgress<TrackConverted> progress = new SimpleProgress<TrackConverted>
                    ( (e) =>
                    {
                        iel.Report(new ConvertProgessEventArgs(_ClueName.DisplayName, ++Current, Cs.RawTrackDescriptors.Count));
                        if (e.OK)
                        {
                            tracks.Add(e);
                            AddConvertedFiles(MusicPath, e.Track.Path);
                        }
                        else
                            iel.OnFactorisableError<UnableToConvertFile>(e.Track.Name);
                    });

                    OK = imcc.ConvertTomp3(progress, iCancellationToken);
                }
            }

            if (iCancellationToken.IsCancellationRequested)
            {
                return null;
            }

            return new MusicWithMetadaImporter((from t in tracks select t.Track).ToArray<ITrackDescriptor>(), _ListImage, _ClueName); ;
        }

        protected override IEnumerable<string> InFiles
        {
            get { return _MusicConverted; }
        }

    }
}