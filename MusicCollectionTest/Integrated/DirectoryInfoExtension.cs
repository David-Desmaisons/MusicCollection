using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace MusicCollectionTest.Integrated
{
    public static class DirectoryInfoExtension
    {
        public static bool RemoveReadOnly(this DirectoryInfo di)
        {
            bool res = true;
            try
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        fi.Attributes = FileAttributes.Normal;
                        fi.IsReadOnly = false;
                    }
                    catch
                    {
                        Console.WriteLine("Cas Pourri during remove read-only");
                        return false;
                    }
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    if (!RemoveReadOnly(dir))
                        res = false;
                }
            }
            catch
            {
                Console.WriteLine("Cas Pourri during remove read-only");
                return false;
            }

            return res;
        }
    }
}
