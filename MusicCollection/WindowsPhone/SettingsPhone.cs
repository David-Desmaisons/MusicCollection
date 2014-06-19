using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WPSync.Settings;
using System.IO;
using MusicCollection.ToolBox;

namespace MusicCollection.WindowsPhone
{
 
    public class SettingsPhone : ISettingsRepository
    {
        public object GetApplicationSetting(string settingName)
        {
            if ("MockDeviceCount" == settingName)
                return null;

            if ("SyncPartner" == settingName)
                return "78a80bb8-adaa-4865-abf1-390bfb12fd08";

            return null;
        }

        public string TranscodedFilesDirectoryForApplication(bool create)
        {
            return FileInternalToolBox.GetApplicationDirectoryName();
        }

        #region Not Implemented

        public object GetDeviceSetting(string deviceId, string settingName)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ApplicationSettingsChangeEventArgs> ApplicationSettingChange
        { add { } remove{} }

        public event EventHandler<DeviceSettingsChangeEventArgs> DeviceSettingChange
        { add { } remove { } }

        public void EnsureSettingsForDevice(string deviceSerialNumber)
        {
            throw new NotImplementedException();
        }

        public bool ForgetDeviceSettings(string deviceSerialNumber)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDeviceDirectories()
        {
            throw new NotImplementedException();
        }

        public string GetSettingsDirectoryForDevice(string deviceSerialNumber, bool create)
        {
            throw new NotImplementedException();
        }

        public string GetSettingsDirectoryForDevice(string deviceSerialNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsMusicSourceITunes()
        {
            throw new NotImplementedException();
        }

        public bool IsMusicSourceWindowsLibraries()
        {
            throw new NotImplementedException();
        }

        public void RemoveDeviceSettings(string deviceSerialNumber)
        {
            throw new NotImplementedException();
        }

        public void SetApplicationSetting(string settingName, object value)
        {
            throw new NotImplementedException();
        }

        public void SetDeviceSetting(string deviceId, string settingName, object value)
        {
            throw new NotImplementedException();
        }

        public string SettingsDirectoryForApplication()
        {
            throw new NotImplementedException();
        }

        public string SettingsDirectoryForApplication(bool create)
        {
            throw new NotImplementedException();
        }

        public bool SettingsFileExists(string deviceSerialNumber)
        {
            throw new NotImplementedException();
        }

        public string SqmFilesDirectoryForApplication(bool create)
        {
            throw new NotImplementedException();
        }

        public string SqmFilesDirectoryForApplication()
        {
            throw new NotImplementedException();
        }

        public string TranscodedFilesDirectoryForApplication()
        {
            throw new NotImplementedException();
        }

            #endregion
    }
}
