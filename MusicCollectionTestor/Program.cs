using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollectionTestor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string path = @"C:\Users\David\Documents\Visual Studio 2010\Projects\MusicCollection\MusicCollectionTest\bin\Debug\MusicCollectionTest.dll";

            //string argg = @"/run=MusicCollectionTest.Integrated.Phone8Exporter";

            string[] realsargs = (new List<string>(path.SingleItemCollection()).AddCollection(args)).ToArray();
            //List<string> ll = new List<string>(path.SingleItemCollection());
            //ll.Add(argg);
            //string[] realsargs = ll.ToArray();

            //AppDomain.CurrentDomain.ExecuteAssembly(
            // @"C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe",
            // realsargs);
             AppDomain.CurrentDomain.ExecuteAssembly(
             @"C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe",
             realsargs);
            
        }
    }
}
