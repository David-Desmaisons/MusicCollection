//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using Un4seen.Bass;
//using Un4seen.Bass.Misc;
//using Un4seen.Bass.AddOn.Cd;
//using System.Reflection;
//using System.Threading;
//using System.Diagnostics;

//using MusicCollection.Fundation;
//using MusicCollection.Implementation;
//using MusicCollection.ToolBox;
//using MusicCollection.Infra;
//using MusicCollection.Properties;
//using MusicCollection.DataExchange;
//using MusicCollection.DataExchange.Cue;

//namespace MusicCollection.FileConverter
//{
//    internal static class MusicFileConverter
//    {
//        private static int _APEPlug = 0;
//        private static int _FLACPlug = 0;
//        private static int _AAClug = 0;
//        private static int _WVPlug = 0;
//        private static bool _Init = false;

//        static internal void Clean()
//        {
//            if (_APEPlug != 0)
//            {
//                Bass.BASS_PluginFree(_APEPlug);
//                _APEPlug = 0;
//            }

//            if (_FLACPlug != 0)
//            {
//                Bass.BASS_PluginFree(_FLACPlug);
//                _FLACPlug = 0;
//            }

//            if (_AAClug != 0)
//            {
//                Bass.BASS_PluginFree(_AAClug);
//                _AAClug = 0;
//            }

//            if (_WVPlug != 0)
//            {
//                Bass.BASS_PluginFree(_WVPlug);
//                _WVPlug = 0;
//            }       

//            if (_Init)
//                Bass.BASS_Free();

//            _Init = false;
//        }

//        static readonly string _DirectoryExe;

//        static MusicFileConverter()
//        {
//            string DirectoryExe = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MusicFileConverter)).CodeBase);
//            const string Files = @"file:\";

//            if (DirectoryExe.StartsWith(Files))
//                DirectoryExe = DirectoryExe.Substring(Files.Length);

//            _DirectoryExe = DirectoryExe;
//        }

//        static internal int  GetDriverNumber(char DriverName)
//        {
//            InitAndValidate(null);
//            return (from dr in (BassCd.BASS_CD_GetInfos().Select((o,r)=>new {D=o,R=r})) where (dr.D.DriveLetter==DriverName) select dr.R).ToList()[0];

//                //.Where(r => r.DriveLetter == DriverName).Select((o, r) => r)).ToList()[0];
//        }


//        static internal double GetFileLengthInSeconds(string iFileName)
//        {
//            if (!InitAndValidate(iFileName))
//                return -1;

//            int stream = Bass.BASS_StreamCreateFile(iFileName, 0, 0, BASSFlag.BASS_STREAM_DECODE);
//            double time = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
//            Bass.BASS_StreamFree(stream);

//            return time;
//        }


//        static private bool InitAndValidate(string FileName)
//        {
//            if (!File.Exists(FileName) && (FileName!=null))
//                return false;

//           if (_Init == false)
//            {
//                Un4seen.Bass.BassNet.Registration(Settings.Default.BassUser, Settings.Default.BassPassword);
   
//                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
//                {
//                    Thread.Sleep(5000);
//                    if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
//                    {
//                        Thread.Sleep(15000);
//                        if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
//                        {
//                            Trace.WriteLine("Doeu");
//                            throw new InvalidProgramException("Doeu");
//                        }
//                    }
//                }


//                _Init = true;
//            }

//            if (FileName == null)
//                return true;

//            string Ext = Path.GetExtension(FileName).ToLower();

//            if (Ext == FileServices.APE)
//            {
//                if (_APEPlug == 0)
//                    _APEPlug = Bass.BASS_PluginLoad("bass_ape.dll");
//                return true;
//            }

//            if (Ext == FileServices.FLAC)
//            {
//                if (_FLACPlug == 0)
//                    _FLACPlug = Bass.BASS_PluginLoad("bassflac.dll");
//                return true;
//            }

//            if ((Ext == FileServices.AAC) || (Ext == FileServices.M4A) || (Ext == FileServices.MP4) )
//            {
//                if (_AAClug == 0)
//                    _AAClug = Bass.BASS_PluginLoad("bass_aac.dll");
//                return true;
//            }

//            if (Ext == FileServices.WV)
//            {
//                if (_WVPlug == 0)
//                    _WVPlug = Bass.BASS_PluginLoad("basswv.dll");
//                return true;
//            }           

//            if ((Ext == FileServices.OGG) || (Ext == FileServices.WAV) || (Ext == FileServices.AIFF))
//            {
//                return true;
//            }

//            return false;
//        }

//        static private EncoderLAME GetLameEndoder(int stream = 0)
//        {
//            EncoderLAME Lame = new EncoderLAME(stream);
//            Lame.LAME_Bitrate = (int)EncoderLAME.BITRATE.kbps_320;
//            Lame.LAME_Mode = EncoderLAME.LAMEMode.Default;
//            Lame.LAME_TargetSampleRate = (int)EncoderLAME.SAMPLERATE.Hz_44100;
//            Lame.LAME_Quality = EncoderLAME.LAMEQuality.Quality;

//            Lame.EncoderDirectory = _DirectoryExe;

//            return Lame;

//        }

//        static private IEnumerable<EncoderLAME> LameFromCD(int Cdnumber)
//        {
//            int TrackNumber = BassCd.BASS_CD_GetTracks(0);
//            for (int i = 0; i < TrackNumber; i++)
//            {
//                int stream = BassCd.BASS_CD_StreamCreate(Cdnumber, i, BASSFlag.BASS_STREAM_DECODE);
//                EncoderLAME Enc = GetLameEndoder(stream);
//                yield return Enc;
//                Bass.BASS_StreamFree(stream);
//                Enc.Dispose();
//            }

//            yield break;

//        }

//        static internal IMusicfilesConverter GetMusicConverter(string FileName, List<TrackDescriptor> Cues, string Outdir, string temp)
//        {
//            if (InitAndValidate(FileName) == false)
//                return null;

//            return new CUEConverter(GetLameEndoder(), FileName, Cues, Outdir,temp);
//        }

//        static internal IMusicFileConverter GetMusicConverter(string FileName, string Outdir, string temp)
//        {
//            if (InitAndValidate(FileName) == false)
//                return null;

//            return new SimpleConverter(GetLameEndoder(), FileName, Outdir,temp);
//        }

//        static internal IMusicfilesConverter GetCDMusicConverter(AlbumDescriptor ICDInfo, string Outdir, bool BigBrute=false, int Cdnumber = 0)
//        {
//            bool res = BassCd.BASS_CD_IsReady(Cdnumber);

//            if ((!BigBrute) && (res==false))
//                return null;

//            if (InitAndValidate(null) == false)
//                return null;

//            return new CDConverter(ICDInfo, LameFromCD(Cdnumber), Outdir, Cdnumber);
//        }

//        private class FileChecker
//        {
//            private string _Real;
//            private string _Temp;

//            internal string ToBeConsidered
//            {
//                get { return _Temp ?? _Real; }
//            }

//            internal string Desired
//            {
//                get { return _Real; }
//            }

//            internal FileChecker(string In,string TempDir)
//            {
//                _Real = In;

//                string NewName = _Real.WithoutAccent();
//                if (NewName != _Real)
//                {
//                    _Temp = FileInternalToolBox.CreateNewAvailableName(TempDir, Path.GetFileNameWithoutExtension(NewName), Path.GetExtension(NewName));
//                }
//            }

//            internal void Temporize()
//            {
//                if (_Temp == null)
//                    return;

//                try
//                {
//                    File.Move(_Real, _Temp);
//                }
//                catch (IOException)
//                {
//                    throw ImportExportException.FromError(new FileInUse(_Real));
//                }
//            }

//            internal void Finalise()
//            {
//                if (_Temp == null)
//                    return;

//                File.Move(_Temp, _Real);
//            }
//        }

//        private class SimpleConverter : IMusicFileConverter
//        {
//            private FileChecker _FileName;
//            private EncoderLAME _Lame;
//            private FileChecker _Out;

//            internal SimpleConverter(EncoderLAME Lame, string FileName, string Outdir,string TempDir)
//            {
//                _FileName = new FileChecker(FileName, TempDir);
//                _Lame = Lame;

//                _FileName.Temporize();

//                _Lame.InputFile = _FileName.ToBeConsidered;

//                if (!Directory.Exists(Outdir))
//                    throw new InvalidDataException("Should provide a valid folder.");

//                string FinalName = FileInternalToolBox.CreateNewAvailableName(Outdir,Path.GetFileNameWithoutExtension(_FileName.Desired), FileServices.MP3);

//                _Out = new FileChecker(FinalName, TempDir);

//                _Lame.OutputFile = _Out.ToBeConsidered;

//            }

//            string IMusicFileConverter.ConvertName
//            {
//                get { return _Out.Desired; }
//            }

//            bool IMusicFileConverter.TagetFileAlreadyExist { get { return File.Exists(_Out.Desired); } }

//            bool IMusicFileConverter.ConvertTomp3(bool DeleteIfAlreadyExist, bool deleteIfSucceed)
//            {

//                if (!_Lame.EncoderExists)
//                {
//                    throw new InvalidDataException("Lame encoder not found.");
//                }

//                bool res = BaseEncoder.EncodeFile(_Lame, null, DeleteIfAlreadyExist, deleteIfSucceed, true);

//                _FileName.Finalise();


//                if (File.Exists(_Out.ToBeConsidered))
//                {
//                    _Out.Finalise();
//                }

//                res = File.Exists(_Out.Desired);

//                return res;
//            }

//            void IDisposable.Dispose()
//            {
//                _Lame.Dispose();
//            }
//        }

      


//        private class CUEConverter : IMusicfilesConverter
//        {
//            private FileChecker _FileName;
//            private string _Output;
//            private EncoderLAME _Lame;
//            private EventHandler<TrackConvertedArgs> _TC;
//            private List<TrackDescriptor> _Cues;
//            private string _TempDir;

//            internal CUEConverter(EncoderLAME Lame, string FileName, List<TrackDescriptor> Cues, string Outdir, string TempDir)
//            {
//                // private FileChecker _Out;
//                _Lame = Lame;
//                _Cues = Cues;
//                _TempDir = TempDir;
//                _FileName = new FileChecker(FileName, _TempDir); 
//                _FileName.Temporize();
//                _Output = Outdir;
                        
//            }

//            event EventHandler<TrackConvertedArgs> IMusicfilesConverter.TrackHandled
//            {
//                add { _TC += value; }
//                remove { _TC -= value; }
//            }

//            private void OnTrackhandled(TrackConvertedArgs TCA)
//            {
//                if (_TC != null)
//                    _TC(this, TCA);
//            }


//            bool IMusicfilesConverter.ConvertTomp3(bool deleteIfSucceed)
//            {
//                if (!_Lame.EncoderExists)
//                {
//                    throw new InvalidDataException("Lame encoder not found.");
//                }

//                double From = 0;
//                double To = 0;

//                int stream = Bass.BASS_StreamCreateFile(_FileName.ToBeConsidered, 0, 0, BASSFlag.BASS_STREAM_DECODE);
//                double time = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
//                Bass.BASS_StreamFree(stream);

//                bool finalres = true;
//                int Count = _Cues.Count;

//                for (int i = 0; i < Count; i++)
//                {
//                    TrackDescriptor Tr = _Cues[i];

//                    if (i == 0)
//                        From = -1;
//                    else
//                    {
                        
//                        //From = Tr.Indices[0].TotalSeconds;
//                        From = Tr.CueIndex01.TotalSeconds;
//                    }


//                    if (i + 1 < Count)
//                    {
//                        TimeCueIndexer tci = _Cues[i + 1].CueIndex00 ?? _Cues[i + 1].CueIndex01;
//                        To = tci.TotalSeconds;
//                    }
//                    else
//                    { 
//                        To = (long)time;
//                    }


//                    //string Name = FileInternalToolBox.CreateNewAvailableName(String.Format("{0}-{1}", Tr.TrackNumber, Tr.Name + FileServices.MP3).FormatFileName(150));
 
//                    //string OutPut = Path.Combine(_Output, Name);
//                    string OutPut = FileInternalToolBox.CreateNewAvailableName(_Output, String.Format("{0}-{1}", Tr.TrackNumber, Tr.Name).FormatFileName(150), FileServices.MP3);
 

//                    FileChecker Out = new FileChecker(OutPut, _TempDir);

//                    bool OK = BaseEncoder.EncodeFile(_FileName.ToBeConsidered, Out.ToBeConsidered, _Lame, null, false, false, true, From, To);
                   
//                    if (OK)
//                    {
//                        if (File.Exists(Out.ToBeConsidered))
//                            Out.Finalise();
//                        else
//                            OK = false;
//                    }

//                    if (OK == false)
//                        finalres = false;

//                    Tr.Path =OutPut;
//                    double realfrom = (From == -1) ? 0 : From;
//                    Tr.Duration = TimeSpan.FromSeconds(To - realfrom);

//                    OnTrackhandled(new TrackConvertedArgs(Tr, OK));
//                        //,false));
//                }

//                _FileName.Finalise();

//                if (finalres && deleteIfSucceed)
//                    File.Delete(_FileName.Desired);

//                return finalres;
//            }

//            void IDisposable.Dispose()
//            {
//                _Lame.Dispose();
//            }
//        }



//        private class CDConverter : IMusicfilesConverter
//        {
//            private string _Output;
//            private IEnumerable<EncoderLAME> _Lames;
//            private EventHandler<TrackConvertedArgs> _TC;
//            private bool finalres = false;
//            private AlbumDescriptor _ICI;
//            private int _CD = 0;

//            internal CDConverter(AlbumDescriptor ICDInfo, IEnumerable<EncoderLAME> Lame, string Outdir, int CD)
//            {
//                _CD = CD;
//                _Lames = Lame;
//                _ICI = ICDInfo;
               
//                if (Outdir == null)
//                    throw new InvalidDataException();

//                _Output = Path.Combine(Outdir, _ICI.Artist.FormatForDirectoryName(), _ICI.Name.FormatForDirectoryName());

//                Directory.CreateDirectory(_Output);
//            }

//            event EventHandler<TrackConvertedArgs> IMusicfilesConverter.TrackHandled
//            {
//                add { _TC += value; }
//                remove { _TC -= value; }
//            }

//            private void OnTrackhandled(TrackConvertedArgs TCA)
//            {
//                if (_TC != null)
//                    _TC(this, TCA);
//            }


//            bool IMusicfilesConverter.ConvertTomp3(bool deleteIfSucceed)
//            {
//                finalres = true;
//                int i = 0;

//               // IEnumerator<TrackDescriptor> enu = _ICI.RawTrackDescriptors;

//                if (_Lames.Count() != _ICI.RawTrackDescriptors.Count)
//                {
//                    Trace.WriteLine("CD track number mismatch. May appear when itunes has an open window for a cd (choose correct name).");
//                    return false;
//                }
              
//                foreach (EncoderLAME AL in _Lames)
//                {  
//  //                  enu.MoveNext();
//                    TrackDescriptor itd = _ICI.RawTrackDescriptors[i++];
//                        //enu.Current;
                   
//                    if (!AL.EncoderExists)
//                    {
//                        throw new InvalidDataException("Lame encoder not found.");
//                    }

//                    int stream = AL.ChannelHandle;

//                    double time = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));


//                    string OutPut = FileInternalToolBox.CreateNewAvailableName(_Output, string.Format("{0}-{1}", itd.TrackNumber,itd.Name.FormatFileName(100)), FileServices.MP3);  
//                     //string OutPut = Path.Combine(_Output, string.Format("{0}-{1}.mp3", itd.TrackNumber,itd.Name.FormatFileName(150)));

//                    AL.OutputFile = OutPut;

//                    AL.Start(null, IntPtr.Zero, false);
//                    // decode the stream (if not using a decoding channel, simply call "Bass.BASS_ChannelPlay" here)
//                    byte[] encBuffer = new byte[65536]; // our dummy encoder buffer
//                    while (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
//                    {
//                        // getting sample data will automatically feed the encoder
//                        int len = Bass.BASS_ChannelGetData(stream, encBuffer, encBuffer.Length);
//                    }
//                    AL.Stop();  // finish

                    
//                    itd.Path = AL.OutputFile;
//                    itd.Duration = TimeSpan.FromSeconds(time);

//                    OnTrackhandled(new TrackConvertedArgs(itd, File.Exists(itd.Path)));
//                }

//                BassCd.BASS_CD_Release(_CD);

//                return finalres;
//            }

//            void IDisposable.Dispose()
//            {
//            }
//        }


//    }

//}
