using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MusicCollection.Infra;
using MusicCollection.Implementation;

namespace MusicCollection.Fundation
{
    public class OptionArtistChooser : IDisposable
    {
        private bool _Changed = false;
        private IInternalMusicSession _Session;
        internal OptionArtistChooser(IEnumerable<IList<IArtist>> iOptions,IInternalMusicSession iSession)
        {
            _Session = iSession;
            var l = iOptions.ToList();
            if (l.Count == 1)
            {
                Options = new ObservableCollection<IArtist>();
                Values = new ObservableCollection<IArtist>(l[0]);
                Values.CollectionChanged += Values_CollectionChanged; 
                return;
            }


            var First = l[0];
            bool allthesame = l.Skip(1).All(la => la.SequenceEqual(First));
            if (allthesame)
            {
                Options = new ObservableCollection<IArtist>();
                Values = new ObservableCollection<IArtist>(l[0]);
                Values.CollectionChanged += Values_CollectionChanged; 
                return;
            }

            var options = l.SelectMany(al => al).Distinct();
            Options = new ObservableCollection<IArtist>(options);
            Values = new ObservableCollection<IArtist>();

            Values.CollectionChanged += Values_CollectionChanged; 
        }

        private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _Changed = true;
        }

        public bool Changed { get { return _Changed; } }

        public string StringValue
        {
            get { return Artist.AuthorName(Values); }
            set { Values.Clear(); Values.AddCollection(Artist.GetArtistFromName(value,_Session)); }
        }

        public void Dispose()
        {
            Values.CollectionChanged -= Values_CollectionChanged;
        }

        public bool HasOptions { get { return Options.Count > 0; } }

        public ObservableCollection<IArtist> Options { get; private set; }

        public ObservableCollection<IArtist> Values { get; private set; }
    
    }
}
