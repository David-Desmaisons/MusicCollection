﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using MusicCollection.FileImporter;
using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.ToolBox;
using MusicCollection.Infra;
using System.Threading;

namespace MusicCollection.FileConverter
{
    class MusicConverterImporter : FileConverterAbstract, IImporter
    {
        private List<string> _ListMusic = null;
        private List<string> _ListImage = null;
        private IImportHelper _ClueName = null;
        private List<string> _TargetConvertedMusic = null;
        private IMusicConverter _IMusicConverter;

        internal MusicConverterImporter(IMusicConverter iIMusicConverter, List<string> Music, List<string> Image, IImportHelper ClueName)
        {
            _IMusicConverter = iIMusicConverter;
            _ListMusic = Music;
            _ListImage = Image;
            _ClueName = ClueName;
        }

        private IEnumerable<string> ConvertMusic(IEventListener iel, CancellationToken iCancellationToken)
        {
            int Current = 0;

            if (iCancellationToken.IsCancellationRequested)
                yield break;

            foreach (string Music in _ListMusic)
            {
                iel.Report(new ConvertProgessEventArgs(_ClueName.DisplayName, ++Current, _ListMusic.Count));

                string OutDirectory = Context.ConvertManager.PathFromOutput(Music, _ClueName);
                string TempDir = Context.Folders.Temp;

                using (IMusicFileConverter imf = _IMusicConverter.GetMusicConverter(Music, OutDirectory, TempDir))
                {

                    if (iCancellationToken.IsCancellationRequested)
                        yield break;

                    bool res = imf.ConvertTomp3();

                    if (res)
                    {
                        AddConvertedFiles(Music, imf.ConvertName);

                        yield return imf.ConvertName;
                    }
                    else
                    {
                        Trace.WriteLine("Unable to convert file"+Music);
                        iel.OnFactorisableError<UnableToConvertFile>(Music);
                    }
                }
            }
        }

        protected override ImporterConverterAbstract GetNext(IEventListener iel, CancellationToken iCancellationToken)
        {
            if (iCancellationToken.IsCancellationRequested)
            {
                return null;
            }

            SpaceChecker sc = new SpaceChecker(Context.ConvertManager.PathFromOutput(_ListMusic[0], _ClueName),
              _ListMusic);

            if (!sc.OK)
            {
                iel.Report(new NotEnougthSpace(sc.ToString()));
                return null;
            }

            try
            {
                _TargetConvertedMusic = ConvertMusic(iel, iCancellationToken).ToList();
            }
            catch (ImportExportException iee)
            {
                throw iee;
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Exception during music convestion, could be ok if this a cancellation exception {0}", e));
                return null;
            }

            if (iCancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (_TargetConvertedMusic.Count == 0)
            {
                return null;
            }

            return new MusicImporter(_TargetConvertedMusic, _ListImage, _ClueName);
        }

        protected override IEnumerable<string> InFiles
        {
            get { return _ListMusic; }
        }
    }
}
