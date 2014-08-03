using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using SevenZip;

using MusicCollection.Implementation;
using MusicCollection.Fundation;
//using MusicCollection.Properties;
using MusicCollection.ToolBox;
using MusicCollection.FileImporter;
using MusicCollection.Infra;
using MusicCollection.Implementation.Session;

namespace MusicCollection.SettingsManagement
{
    internal class RarManagerImpl : FileChangeManager, IRarManager
    {

        private CompleteFileBehaviour _RarZipFileAfterSuccessfullExtract;
        private CompleteFileBehaviour _RarZipFileAfterFailedExtract;
        private ConvertFileBehaviour _RarExctractManagement;

        private bool _Init = false;
        private bool _AddRar = false;

        private Func<string, bool> _OnOK;
        private Func<string, bool> _OnKO;

        private IUnrarUserSettings _IUnrarUserSettings;

        internal RarManagerImpl(IImportContext Ms, IUnrarUserSettings unrarsettings)
            : base(Ms)
        {
            _IUnrarUserSettings = unrarsettings;
            _RarZipFileAfterSuccessfullExtract = _IUnrarUserSettings.RarZipFileAfterSuccessfullExtract;
            _RarZipFileAfterFailedExtract = _IUnrarUserSettings.RarZipFileAfterFailedExtract;
            _RarExctractManagement = _IUnrarUserSettings.RarExctractManagement;
            _AddRar = _IUnrarUserSettings.AddUseRarPasswordToList;
            //InitPassword();
        }

        //internal RarManagerImpl(IImportContext Ms, CompleteFileBehaviour iRarZipFileAfterSuccessfullExtract, CompleteFileBehaviour iRarZipFileAfterFailedExtract,
        //    ConvertFileBehaviour iRarExctractManagement, bool iAddRar, IUnrarUserSettings unrarsettings )
        //    : base(Ms)
        //{
        //    _IUnrarUserSettings = unrarsettings;
        //    _RarZipFileAfterSuccessfullExtract = iRarZipFileAfterSuccessfullExtract;
        //    _RarZipFileAfterFailedExtract = iRarZipFileAfterFailedExtract;
        //    _RarExctractManagement = iRarExctractManagement;
        //    _AddRar = iAddRar;
        //    //InitPassword();
        //}

        //private void InitPassword()
        //{
        //    _Passwords = (Settings.Default.RarPasswords == null) ? new string[] { } : (from s in Settings.Default.RarPasswords.Cast<string>() where !String.IsNullOrEmpty(s) select s).ToArray();
        //}


        private void Init()
        {
            if (_Init)
                return;

            switch (_RarZipFileAfterSuccessfullExtract)
            {
                case CompleteFileBehaviour.DoNothing:
                    _OnOK = (fn) => true;
                    break;

                //case CompleteFileBehaviour.Copy:
                //    if (_DestinationDirForComputedFiles == null)
                //        throw new Exception("Algo Error");
                //    _OnOK = (fn) => Copy(fn, _DestinationDirForComputedFiles);
                //    break;

                case CompleteFileBehaviour.Delete:
                    _OnOK = (fn) => Delete(fn);
                    break;

            }

            switch (_RarZipFileAfterFailedExtract)
            {
                case CompleteFileBehaviour.DoNothing:
                    _OnKO = (fn) => true;
                    break;

                //case CompleteFileBehaviour.Copy:
                //    string output = _DestinationDirForFailedFiles ?? _DestinationDirForComputedFiles;
                //    if (output == null)
                //        throw new Exception("Algo Error");
                //    _OnKO = (fn) => Copy(fn, output);
                //    break;

                case CompleteFileBehaviour.Delete:
                    _OnKO = (fn) => Delete(fn);
                    break;

            }


            _Init = true;
        }

        //private string[] _Passwords;
        //private IEnumerable<string> PasswordLists
        //{
        //    get
        //    {
        //        return _Passwords;
        //    }
        //}

        //private void AddPassword(string psswd)
        //{
        //    if (string.IsNullOrEmpty(psswd))
        //        return;

        //    if (Settings.Default.RarPasswords == null)
        //    {
        //        var res = new System.Collections.Specialized.StringCollection();
        //        res.Add(psswd);
        //        Settings.Default.RarPasswords = res;
        //        //Settings.Default.Save();
        //        return;
        //    }
        //    else
        //    {
        //        if (!Settings.Default.RarPasswords.Contains(psswd))
        //        {
        //            Settings.Default.RarPasswords.Add(psswd);
        //            //Settings.Default.Save();
        //        }
        //    }
        //}

        private bool OnErrorUserExit(IEventListener iel,CorruptedRarOrMissingPasswordArgs cr)
        {
            iel.Report(cr);
            if ((cr.accept) && (cr.SavePassword))
               _IUnrarUserSettings. AddPassword(cr.Password);

            if (_AddRar!=cr.SavePassword)
            {
                //Settings.Default.AddUseRarPasswordToList = cr.SavePassword;
                _IUnrarUserSettings.AddUseRarPasswordToList = cr.SavePassword;
                _AddRar = cr.SavePassword;
                return true;
            }

            return false;
        }

        private SevenZipExtractor InstanciateZipExctractorWithPassword(string FileName, IEventListener iel)
        {
            SevenZipExtractor Sex = new SevenZipExtractor(FileName);

            bool valid = Sex.Check();

            if (valid == false)
            {
                Sex.Dispose();

                //foreach (string psw in PasswordLists)
                foreach (string psw in _IUnrarUserSettings.EnumerableRarPasswords)
                {                     
                      Sex = new SevenZipExtractor(FileName, psw);
                      if (Sex.Check())
                      {
                          Trace.WriteLine(string.Format("Find Password in list: {0}", psw));
                          return Sex;
                      }
                      else
                        Sex.Dispose();
                }

                string same = Path.GetFileName(FileName);
                CorruptedRarOrMissingPasswordArgs cr = new CorruptedRarOrMissingPasswordArgs(same, _AddRar);

              // bool res=
                OnErrorUserExit(iel, cr);

 
                while ((!valid) && (cr.accept == true))
                {                      

                    Sex = new SevenZipExtractor(FileName, cr.Password);

                    valid = Sex.Check();
                    if (valid == false)
                    {
                        Sex.Dispose();
                        cr = new CorruptedRarOrMissingPasswordArgs(same, cr.SavePassword);

                        //bool res2 = 
                        OnErrorUserExit(iel, cr);
                        //res = res || res2;
                    } 
                }

                if (valid == false)
                {
                    return null;
                }
               
            }

            return Sex;
        }



        public void OnUnrar(string FileName, bool Successfull)
        {
            Init();

            Func<string, bool> Action = Successfull ? _OnOK : _OnKO;
            bool res = Action(FileName);

            if (res == false)
            {
                if (Successfull)
                    _OnOK = (fn) => true;
                else
                    _OnKO = (fn) => true; ;
            }
        }

        public void OnUnrar(IEnumerable<string> FileNames, bool Successfull)
        {
            foreach (string FileName in FileNames)
                OnUnrar(FileName, Successfull);
        }

        public IMccDescompactor InstanciateExctractor(string FileName, IEventListener iel,IImportContext tmp)
        {
            SevenZipExtractor res = InstanciateZipExctractorWithPassword(FileName, iel);
            if (res == null)
            {
                // OnUnrar(FileName, false);
                return null;
            }

            return new MccDecompactor(res,tmp);
            //ZipabstractDescompactor.GetDescompactor(_RarExctractManagement == ConvertFileBehaviour.SameFolder, this, res);
        }


        public IRarDescompactor InstanciateExctractorWithPassword(string FileName, IEventListener iel)
        {
            SevenZipExtractor res = InstanciateZipExctractorWithPassword(FileName, iel);
            if (res == null)
            {
               // OnUnrar(FileName, false);
                return null;
            }

            return ZipabstractDescompactor.GetDescompactor(_RarExctractManagement == ConvertFileBehaviour.SameFolder,this,res);
        }

    }
}
