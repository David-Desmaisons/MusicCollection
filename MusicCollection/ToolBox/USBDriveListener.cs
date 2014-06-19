/*
* Adapted from by SharpDevelop.
* User: Jeff
* Date: 23/11/2007
* Time: 11:11 AM
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;

namespace MusicCollection.ToolBox
{
    internal class USBDriverEventArgs : EventArgs
    {
        internal bool Removed
        {
            get;
            private set;
        }

        internal string[] AssociatedDrives
        {
            get;
            private set;
        }

        static internal USBDriverEventArgs GetRemovedEvent()
        {
            return new USBDriverEventArgs();
        }

        static internal USBDriverEventArgs GetAddedEvent(string[] drives)
        {
            return new USBDriverEventArgs(drives);
        }

        private USBDriverEventArgs()
        {
            Removed = true;
            AssociatedDrives = null;
        }

        private USBDriverEventArgs(string[] drives)
        {
            Removed = false;
            AssociatedDrives = drives;
        }


    }


    internal class USBDriveListener : IDisposable
    {
        private ManagementEventWatcher _Watcher;

        internal event EventHandler<USBDriverEventArgs> _DriversChanged;

        internal USBDriveListener()
        {
            //ManagementEventWatcher w = null;
            WqlEventQuery q;

            ManagementOperationObserver observer = new ManagementOperationObserver();

            // Bind to local machine
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true; //sets required privilege
            try
            {

                q = new WqlEventQuery();
                q.EventClassName = "__InstanceOperationEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = @"TargetInstance ISA 'Win32_DiskDrive' ";

                _Watcher = new ManagementEventWatcher(q);

                _Watcher.EventArrived += UsbEventArrived;
                _Watcher.Start();

            }

            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                _Watcher.Stop();

            }
        }

        private void UsbEventArrived(object sender, EventArrivedEventArgs e)
        {
            EventHandler<USBDriverEventArgs> DriversChanged = _DriversChanged;

            if (DriversChanged == null)
                return;

            ManagementBaseObject mbo = null;

            mbo = (ManagementBaseObject)e.NewEvent;
            if (mbo.ClassPath.ClassName != "__InstanceCreationEvent")
            {
                DriversChanged(this, USBDriverEventArgs.GetRemovedEvent());
                return;
            }

            ManagementBaseObject P = (ManagementBaseObject)mbo.GetPropertyValue("TargetInstance");


            List<string> res = new List<string>();

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + P["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
            {
                foreach (ManagementObject partition in mos.Get())
                {
                    //Trace.WriteLine("Partition=" + partition["Name"]);

                    using (ManagementObjectSearcher submos = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition"))
                    {
                        // associate partitions with logical disks (drive letter volumes)
                        foreach (ManagementObject disk in submos.Get())
                        {
                            res.Add((string)disk["Name"]);
                            disk.Dispose();
                        }

                        partition.Dispose();
                    }
                }
            }

            DriversChanged(this, USBDriverEventArgs.GetAddedEvent(res.ToArray()));
        }

        public void Dispose()
        {
            _Watcher.EventArrived -= UsbEventArrived;
            _Watcher.Stop();
            _Watcher.Dispose();
        }

    }
}


