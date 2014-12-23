using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;
using MusicCollection.Fundation;

namespace MusicCollectionWPF.ViewModel
{

    public abstract class GenericeNagigator<T> : NotifyCompleteListenerObject, ISharpFilterTypeIndependant where T : class
    {
        protected GenericeNagigator()
        {
            Item = null;
        }

        protected abstract string ItemName(T iItem);

        public abstract FilterType Type
        {
            get;
        }

        private T _Item;
        public T Item
        {
            get { return _Item; }
            set { this.Set(ref _Item, value); }
        }

        public bool IsFiltering
        {
            get { return Get<GenericeNagigator<T>, bool>(() => (t) => (t.Item != null)); }
        }

        public string FilterName
        {
            get { return Get<GenericeNagigator<T>, string>(() => (t) => (t.Item == null) ? string.Empty : t.ItemName(t.Item)); }
        }

        public void Reset()
        {
            Item = null;
        }

    }

    public class GenreNagigator : GenericeNagigator<IGenre>
    {
        protected override string ItemName(IGenre iItem)
        {
            return iItem.FullName;
        }

        public override FilterType Type
        {
            get { return FilterType.Genre; }
        }
    }

    public class ArtistNagigator : GenericeNagigator<IArtist>
    {
        protected override string ItemName(IArtist iItem)
        {
            return iItem.Name;
        }

        public override FilterType Type
        {
            get { return FilterType.Artist; }
        }
    }
}
