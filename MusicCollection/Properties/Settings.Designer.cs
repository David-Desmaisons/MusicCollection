﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MusicCollection.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DoNothing")]
        public global::MusicCollection.Fundation.CompleteFileBehaviour RarZipFileAfterSuccessfullExtract {
            get {
                return ((global::MusicCollection.Fundation.CompleteFileBehaviour)(this["RarZipFileAfterSuccessfullExtract"]));
            }
            set {
                this["RarZipFileAfterSuccessfullExtract"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SameFolder")]
        public global::MusicCollection.Fundation.ConvertFileBehaviour FileCreatedByConvertion {
            get {
                return ((global::MusicCollection.Fundation.ConvertFileBehaviour)(this["FileCreatedByConvertion"]));
            }
            set {
                this["FileCreatedByConvertion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DoNothing")]
        public global::MusicCollection.Fundation.PartialFileBehaviour ConvertedFileExtractedFromRar {
            get {
                return ((global::MusicCollection.Fundation.PartialFileBehaviour)(this["ConvertedFileExtractedFromRar"]));
            }
            set {
                this["ConvertedFileExtractedFromRar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Yes")]
        public global::MusicCollection.Fundation.BasicBehaviour ImportBrokenItunesTrack {
            get {
                return ((global::MusicCollection.Fundation.BasicBehaviour)(this["ImportBrokenItunesTrack"]));
            }
            set {
                this["ImportBrokenItunesTrack"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DirForPermanentCollection {
            get {
                return ((string)(this["DirForPermanentCollection"]));
            }
            set {
                this["DirForPermanentCollection"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ExportCollectionFiles {
            get {
                return ((bool)(this["ExportCollectionFiles"]));
            }
            set {
                this["ExportCollectionFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DoNothing")]
        public global::MusicCollection.Fundation.CompleteFileBehaviour RarZipFileAfterFailedExtract {
            get {
                return ((global::MusicCollection.Fundation.CompleteFileBehaviour)(this["RarZipFileAfterFailedExtract"]));
            }
            set {
                this["RarZipFileAfterFailedExtract"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DoNothing")]
        public global::MusicCollection.Fundation.PartialFileBehaviour SourceFileUsedForConvertion {
            get {
                return ((global::MusicCollection.Fundation.PartialFileBehaviour)(this["SourceFileUsedForConvertion"]));
            }
            set {
                this["SourceFileUsedForConvertion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AskEndUser")]
        public global::MusicCollection.Fundation.BasicBehaviour DeleteRemovedFile {
            get {
                return ((global::MusicCollection.Fundation.BasicBehaviour)(this["DeleteRemovedFile"]));
            }
            set {
                this["DeleteRemovedFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CopyInMananagedFolder")]
        public global::MusicCollection.Fundation.ConvertFileBehaviour RarExctractManagement {
            get {
                return ((global::MusicCollection.Fundation.ConvertFileBehaviour)(this["RarExctractManagement"]));
            }
            set {
                this["RarExctractManagement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection RarPasswords {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["RarPasswords"]));
            }
            set {
                this["RarPasswords"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AddUseRarPasswordToList {
            get {
                return ((bool)(this["AddUseRarPasswordToList"]));
            }
            set {
                this["AddUseRarPasswordToList"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("desmaisons_david@hotmail.com")]
        public string BassUser {
            get {
                return ((string)(this["BassUser"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastPathImportFolder {
            get {
                return ((string)(this["LastPathImportFolder"]));
            }
            set {
                this["LastPathImportFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastPathImportRar {
            get {
                return ((string)(this["LastPathImportRar"]));
            }
            set {
                this["LastPathImportRar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double ImageSizeMoLimit {
            get {
                return ((double)(this["ImageSizeMoLimit"]));
            }
            set {
                this["ImageSizeMoLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public uint ImageNumberLimit {
            get {
                return ((uint)(this["ImageNumberLimit"]));
            }
            set {
                this["ImageNumberLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ImageNumber {
            get {
                return ((bool)(this["ImageNumber"]));
            }
            set {
                this["ImageNumber"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ImageSizeLimit {
            get {
                return ((bool)(this["ImageSizeLimit"]));
            }
            set {
                this["ImageSizeLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastPathExportFile {
            get {
                return ((string)(this["LastPathExportFile"]));
            }
            set {
                this["LastPathExportFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AlbumSorterPolarity {
            get {
                return ((bool)(this["AlbumSorterPolarity"]));
            }
            set {
                this["AlbumSorterPolarity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ImportDate")]
        public global::MusicCollection.Fundation.AlbumFieldType AlbumSortering {
            get {
                return ((global::MusicCollection.Fundation.AlbumFieldType)(this["AlbumSortering"]));
            }
            set {
                this["AlbumSortering"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public ulong SessionOpenCount {
            get {
                return ((ulong)(this["SessionOpenCount"]));
            }
            set {
                this["SessionOpenCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SplashScreenPath {
            get {
                return ((string)(this["SplashScreenPath"]));
            }
            set {
                this["SplashScreenPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Directory")]
        public global::MusicCollection.Fundation.MusicExportType LastExportType {
            get {
                return ((global::MusicCollection.Fundation.MusicExportType)(this["LastExportType"]));
            }
            set {
                this["LastExportType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Compressed")]
        public global::MusicCollection.Fundation.MusicImportType LastImportType {
            get {
                return ((global::MusicCollection.Fundation.MusicImportType)(this["LastImportType"]));
            }
            set {
                this["LastImportType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SynchronizeBrokeniTunes {
            get {
                return ((bool)(this["SynchronizeBrokeniTunes"]));
            }
            set {
                this["SynchronizeBrokeniTunes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ExportOutputPath {
            get {
                return ((string)(this["ExportOutputPath"]));
            }
            set {
                this["ExportOutputPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastPathImportCusto {
            get {
                return ((string)(this["LastPathImportCusto"]));
            }
            set {
                this["LastPathImportCusto"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SQLite")]
        public global::MusicCollection.Implementation.Session.SessionBuilder.DBtype Persistence {
            get {
                return ((global::MusicCollection.Implementation.Session.SessionBuilder.DBtype)(this["Persistence"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>www.gnudb.org:80</string>
  <string>gnudb.gnudb.org:80</string>
  <string>freedb.org:80</string>
  <string>freedb.freedb.org:80</string>
  <string>at.freedb.org:80</string>
  <string>au.freedb.org:80</string>
  <string>ca.freedb.org:80</string>
  <string>es.freedb.org:80</string>
  <string>fi.freedb.org:80</string>
  <string>lu.freedb.org:80</string>
  <string>ru.freedb.org:80</string>
  <string>uk.freedb.org:80</string>
  <string>us.freedb.org:80</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection FreedbServers {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FreedbServers"]));
            }
            set {
                this["FreedbServers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("us.freedb.org:80")]
        public string FreedbServer {
            get {
                return ((string)(this["FreedbServer"]));
            }
            set {
                this["FreedbServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Classic")]
        public global::MusicCollection.Fundation.AlbumPresenter PresenterMode {
            get {
                return ((global::MusicCollection.Fundation.AlbumPresenter)(this["PresenterMode"]));
            }
            set {
                this["PresenterMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfPersistentColumn xmlns:xsi=\"http" +
            "://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSche" +
            "ma\">\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <DisplayIndex" +
            ">0</DisplayIndex>\r\n    <Width>110</Width>\r\n    <Index>0</Index>\r\n  </PersistentC" +
            "olumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <DisplayInd" +
            "ex>1</DisplayIndex>\r\n    <Width>305</Width>\r\n    <Index>1</Index>\r\n  </Persisten" +
            "tColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <DisplayI" +
            "ndex>2</DisplayIndex>\r\n    <Width>55</Width>\r\n    <Index>2</Index>\r\n  </Persiste" +
            "ntColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <Display" +
            "Index>3</DisplayIndex>\r\n    <Width>205</Width>\r\n    <Index>3</Index>\r\n  </Persis" +
            "tentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <Displ" +
            "ayIndex>4</DisplayIndex>\r\n    <Width>20</Width>\r\n    <Index>4</Index>\r\n  </Persi" +
            "stentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <Disp" +
            "layIndex>5</DisplayIndex>\r\n    <Width>20</Width>\r\n    <Index>5</Index>\r\n  </Pers" +
            "istentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <Dis" +
            "playIndex>6</DisplayIndex>\r\n    <Width>20</Width>\r\n    <Index>6</Index>\r\n  </Per" +
            "sistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <Di" +
            "splayIndex>7</DisplayIndex>\r\n    <Width>20</Width>\r\n    <Index>7</Index>\r\n  </Pe" +
            "rsistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    <D" +
            "isplayIndex>8</DisplayIndex>\r\n    <Width>105</Width>\r\n    <Index>8</Index>\r\n  </" +
            "PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n    " +
            "<DisplayIndex>9</DisplayIndex>\r\n    <Width>105</Width>\r\n    <Index>9</Index>\r\n  " +
            "</PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>\r\n  " +
            "  <DisplayIndex>10</DisplayIndex>\r\n    <Width>105</Width>\r\n    <Index>10</Index>" +
            "\r\n  </PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibility>" +
            "\r\n    <DisplayIndex>11</DisplayIndex>\r\n    <Width>35</Width>\r\n    <Index>11</Ind" +
            "ex>\r\n  </PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visibili" +
            "ty>\r\n    <DisplayIndex>12</DisplayIndex>\r\n    <Width>35</Width>\r\n    <Index>12</" +
            "Index>\r\n  </PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Visib" +
            "ility>\r\n    <DisplayIndex>13</DisplayIndex>\r\n    <Width>20</Width>\r\n    <Index>1" +
            "3</Index>\r\n  </PersistentColumn>\r\n  <PersistentColumn>\r\n    <Visibility>true</Vi" +
            "sibility>\r\n    <DisplayIndex>14</DisplayIndex>\r\n    <Width>810</Width>\r\n    <Ind" +
            "ex>14</Index>\r\n  </PersistentColumn>\r\n</ArrayOfPersistentColumn>")]
        public global::MusicCollection.SettingsManagement.PersistentColumns TrackGrid {
            get {
                return ((global::MusicCollection.SettingsManagement.PersistentColumns)(this["TrackGrid"]));
            }
            set {
                this["TrackGrid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string MoveOutputPath {
            get {
                return ((string)(this["MoveOutputPath"]));
            }
            set {
                this["MoveOutputPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int DiscogsTimeOut {
            get {
                return ((int)(this["DiscogsTimeOut"]));
            }
            set {
                this["DiscogsTimeOut"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DiscogsActivated {
            get {
                return ((bool)(this["DiscogsActivated"]));
            }
            set {
                this["DiscogsActivated"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AmazonActivated {
            get {
                return ((bool)(this["AmazonActivated"]));
            }
            set {
                this["AmazonActivated"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int DisplaySizer {
            get {
                return ((int)(this["DisplaySizer"]));
            }
            set {
                this["DisplaySizer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OpenCDDoorOnEndImport {
            get {
                return ((bool)(this["OpenCDDoorOnEndImport"]));
            }
            set {
                this["OpenCDDoorOnEndImport"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SplashScreenPath1 {
            get {
                return ((string)(this["SplashScreenPath1"]));
            }
            set {
                this["SplashScreenPath1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DiscogsToken {
            get {
                return ((string)(this["DiscogsToken"]));
            }
            set {
                this["DiscogsToken"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DiscogsTokenSecret {
            get {
                return ((string)(this["DiscogsTokenSecret"]));
            }
            set {
                this["DiscogsTokenSecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DestinationDirForComputedFiles {
            get {
                return ((string)(this["DestinationDirForComputedFiles"]));
            }
            set {
                this["DestinationDirForComputedFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DestinationDirForFailedFiles {
            get {
                return ((string)(this["DestinationDirForFailedFiles"]));
            }
            set {
                this["DestinationDirForFailedFiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string GraceNoteDeviceID {
            get {
                return ((string)(this["GraceNoteDeviceID"]));
            }
            set {
                this["GraceNoteDeviceID"] = value;
            }
        }
    }
}
