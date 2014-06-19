using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace MusicCollection.DataExchange.Cue
{
    [TypeConverter(typeof(TimeCueIndexerConverter))]
    public class TimeCueIndexer
    {
        public TimeCueIndexer()
        {
        }

        private uint _Minutes;
        internal uint Minutes 
        { get { return _Minutes; } set { if (value > 99) throw new ArgumentException(); _Minutes = value; } }

        private uint _Second;
        internal uint Seconds 
        { get { return _Second; } set { if (value > 59) throw new ArgumentException(); _Second = value; } }

        private uint _Frames;
        internal uint Frames 
        { get { return _Frames; } set { if (value > 74) throw new ArgumentException(); _Frames = value; } }

        internal TimeCueIndexer(uint iMinutes, uint iSeconds, uint iFrames)
        {
            Minutes = iMinutes;
            Seconds = iSeconds;
            Frames = iFrames;
        }

        internal TimeCueIndexer(uint iTotalFrames)
        { 
            Minutes = iTotalFrames / ( 60*75);
            Seconds = (iTotalFrames - Minutes*60*75) / 75;
            Frames = iTotalFrames % 75;
        }

        internal uint TotalFrames
        {
            get { return TotalSeconds * 75 + Frames; }
        }

        internal uint TotalSeconds
        {
            get { return Minutes * 60 + Seconds; }
        }

        internal TimeSpan ToTimeSpan()
        {
            return TimeSpan.FromSeconds(TotalSeconds);
        }


        static public bool operator <=(TimeCueIndexer c1, TimeCueIndexer c2)
        {
            if ((c1 == null) || (c2 == null))
                throw new ArgumentNullException();

            return c1.TotalFrames <= c2.TotalFrames;
        }

        static public bool operator >=(TimeCueIndexer c1, TimeCueIndexer c2)
        {
            if ((c1 == null) || (c2 == null))
                throw new ArgumentNullException();

            return c1.TotalFrames >= c2.TotalFrames;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}",Minutes,Seconds,Frames);
        }
    }

    internal class TimeCueIndexerConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return new TimeCueIndexer();

            string s = value as string;
            if (s == null)
                return null;

            if (s.Length == 0)
                return new TimeCueIndexer();

            string[] parts = s.Split(':');

            if (parts.Length != 3)
                throw new ArgumentException();

            return new TimeCueIndexer(uint.Parse(parts[0]), uint.Parse(parts[1]), uint.Parse(parts[2]));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ( (value != null) && (!(value is TimeCueIndexer)))
            {
                throw new ArgumentException("Invalid TimeCueIndexer", "value");
            }

            if (destinationType != typeof(string))
                return null;

            if (value == null)
                return String.Empty;

            TimeCueIndexer tci = (TimeCueIndexer)value;
            return String.Format("{0:00}:{1:00}:{2:00}",tci.Minutes,tci.Seconds,tci.Frames);
        }
    }
}
