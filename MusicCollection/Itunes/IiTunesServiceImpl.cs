using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

using System.Collections.Specialized;
using System.Collections.ObjectModel;

using MusicCollection.Fundation;
using MusicCollection.Implementation;
using MusicCollection.FileImporter;
using MusicCollection.ToolBox;
using MusicCollection.DataExchange;


namespace MusicCollection
{
    namespace Itunes
    {
        internal sealed class IiTunesImporter : IImporter
        {

            private string _MyXMLFile;
           
            IImportContext _IIC;
            bool _ImportBrokenTrack;

            IImportContext IImporter.Context { set { _IIC = value; } get { return _IIC; } }

            IImporter IImporter.Action(IEventListener iel)
            {
                ITracksDescriptorBuilder TD = TrackDescriptor.GetItunesBuilder();
                  
                try
                {
                    FileInfo iTunesFile = new FileInfo(_MyXMLFile);

                    if (!iTunesFile.Exists)
                    {
                        iel.OnError(new FileNotFoundArgs("Itunes Library"));
                        return null;
                    }

                    iel.OnProgress(new ImportProgessEventArgs("Itunes Library"));

                    int Count = Convert.ToInt32(iTunesFile.Length / 1500);

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Ignore;

                    using (XmlReader reader = XmlReader.Create(_MyXMLFile, settings))
                    {

                        if (!reader.Read())
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        if (!reader.ReadToFollowing(iTunesKeys.List))
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        if (!reader.ReadToDescendant(iTunesKeys.dict))
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        if (!reader.ReadToDescendant(iTunesKeys.dict))
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        XmlReader reader2 = reader.ReadSubtree();
                        if (!reader2.Read())
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        if (!reader2.ReadToDescendant(iTunesKeys.dict))
                        {
                            iel.OnError(new ItunesImportError());
                            return null;
                        }

                        int Compt = 0;
                        do
                        {

                            if ((Compt % 50) == 0)
                                iel.OnProgress(new ImportingTrack(Compt + 1));

                            IAttributeObjectBuilder<TrackDescriptor> TuD = TD.DescribeNewTrack();

                            XmlReader inner = reader2.ReadSubtree();

                            while (inner.ReadToFollowing(iTunesKeys.Key))
                            {
                                 TuD.DescribeAttribute(inner.ReadString(), () => { inner.Read(); return inner.ReadString(); });
                            }

                            ITrackDescriptor itd = TuD.Mature();

                            if ((_ImportBrokenTrack) || (File.Exists(itd.Path)))
                                Track.GetTrackFromTrackDescriptor(itd, false, _IIC);

                            Compt++;

                        } while (reader2.ReadToFollowing(iTunesKeys.dict));

                    }
                }
                catch (Exception e)
                {
                    iel.OnError(new ItunesImportError());
                    Trace.WriteLine(e.ToString());
                }

                return null;
            }

            ImportType IImporter.Type
            {
                get { return ImportType.ItunesImport; }
            }

            internal IiTunesImporter(bool ImportBrokenTrack, string Directory)
            {
                _ImportBrokenTrack = ImportBrokenTrack;
                _MyXMLFile = Path.Combine(Directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes"), "iTunes Music Library.xml");
            }
        }
    }
}
