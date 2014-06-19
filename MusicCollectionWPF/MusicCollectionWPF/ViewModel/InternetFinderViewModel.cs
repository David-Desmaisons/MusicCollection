using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class InternetFinderViewModel : ViewModelBase
    {

        public InternetFinderViewModel(IModifiableAlbum IAl)
        {
            _Uri = new Uri (@"http://www.google.com/images?q="+IAl.CreateSearchGoogleSearchString(), 
                    UriKind.RelativeOrAbsolute);;
        }

        private Uri _Uri;
        public string Url
        {
            get { return _Uri.AbsoluteUri; }
        }
    }
}
