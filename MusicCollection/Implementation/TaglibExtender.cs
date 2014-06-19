using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TagLib;
using TagLib.Id3v2;

using MusicCollection.Fundation;
using MusicCollection.ToolBox;

namespace MusicCollection.Implementation
{
    static class TaglibExtender
    {

        static private Tuple<long, long> RawMusicPosition(this TagLib.File TLF)
        {
            Tuple<long, long> res = null;

            if (TLF == null)
                return null;

            res = new Tuple<long, long>(TLF.InvariantStartPosition, TLF.InvariantEndPosition);

            if ((res.Item1 == -1) || (res.Item2 == -1))
            {
                return null;
            }

            return res;
        }

        static internal Stream RawMusicStream(this TagLib.File TLF)
        {
            Tuple<long, long> pos = TLF.RawMusicPosition();
            

            if (pos == null)
                return new FileStream(TLF.Name, FileMode.Open, FileAccess.Read);

            return new ReadableDuplicateStream(TLF.Name, pos.Item1, pos.Item2);
        }


        //static internal ISRC ISRC(this TagLib.File file)
        //{
        //    ISRC res = null;

        //    if (file == null)
        //        return null;

        //    TagLib.Id3v2.Tag tagv2 = file.GetTag(TagLib.TagTypes.Id3v2) as TagLib.Id3v2.Tag;

        //    if (tagv2 != null)
        //    {
        //        var irschelper = tagv2.GetFrames<TextInformationFrame>("TSRC").FirstOrDefault();
        //        if (irschelper != null)
        //        {
        //            var text = irschelper.Text;
        //            res = ((text != null) && (text.Length > 0)) ? MusicCollection.Fundation.ISRC.Fromstring(text[0]) : null;
        //        }
        //    }

        //    return res;
        //}


        static internal string MD5(this TagLib.File TLF)
        {
            if (TLF==null)
                return SHA1KeyComputer.Dummy;

            Tuple<long, long> res = TLF.RawMusicPosition();
            if (res == null)
            {
                using (SHA1KeyComputer RDS = SHA1KeyComputer.FromPath(TLF.Name, 0, -1))
                {
                    return RDS.ComputeKey();
                }
            }

            using (SHA1KeyComputer RDS = SHA1KeyComputer.FromPath(TLF.Name, res.Item1, res.Item2))
            {
                return RDS.ComputeKey();
            }
        }
    }
}
