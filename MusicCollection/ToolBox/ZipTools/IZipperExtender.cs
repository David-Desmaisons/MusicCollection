using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  MusicCollection.ToolBox.ZipTools;

namespace MusicCollection.ToolBox.ZipTools
{
    public static class IZipperExtender
    {
      
        public static Task<bool> SerializeSafeAsync(this IZipper _Zipper,IDictionary<string,string> idic,string iFilename, string ipassword)
        {
            return _Zipper.ZippAsync(idic.SelectMany(kvp=>new string[]{kvp.Key,kvp.Value}),iFilename,ipassword);
        }

        public static bool SerializeSafe(this IZipper _Zipper, IDictionary<string, string> idic, string iFilename, string ipassword)
        {
            return _Zipper.Zipp(idic.SelectMany(kvp => new string[] { kvp.Key, kvp.Value }), iFilename, ipassword);
        }

        public static IDictionary<string, string> UnSerializeSafe(this IZipper _Zipper, string iFilename, string ipassword)
        {
            var list = _Zipper.UnZipp(iFilename, ipassword).ToList();
            int count = list.Count;
            if (count % 2 != 0)
                return null;

            IDictionary<string, string> res = new Dictionary<string,string>();
            int halfcount = count / 2;
            for (int i = 0; i < halfcount; i++)
            {
                res.Add(list[i * 2], list[i * 2 + 1]);
            }
            return res;
        }
    }
}
