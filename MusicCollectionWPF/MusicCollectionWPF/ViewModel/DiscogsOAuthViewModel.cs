using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicCollection.Fundation;
using MusicCollectionWPF.ViewModelHelper;

namespace MusicCollectionWPF.ViewModel
{
    public class DiscogsOAuthViewModel : ViewModelBase
    {
        private IDiscogsAuthentificationProvider _IWebServicesSettings;
        private string _Url;

        public DiscogsOAuthViewModel(IDiscogsAuthentificationProvider iWebServicesSettings)
        {
            _CanFinalizePin = false;
            _IWebServicesSettings = iWebServicesSettings;
            _Url = _IWebServicesSettings.ComputeLinkForDiscogsOAuthAuthorization();

            FinalizePin = RelayCommand.Instanciate(Activate, ()=>CanFinalizePin);
        }

        public ICommand FinalizePin { get; private set; }

        public string Url { get { return _Url; }}

        private string _Pin;
        public string Pin
        {
            get { return _Pin; }
            set 
            {
                if (string.IsNullOrEmpty(value))
                    return;

                this.Set(ref _Pin, value); 
                CanFinalizePin = true;
            }
        }

        private bool _CanFinalizePin;
        public bool CanFinalizePin
        {
            get { return _CanFinalizePin; }
            set { this.Set(ref _CanFinalizePin, value); }
        }

        private void Activate()
        {
            if (!CanFinalizePin)
                return;

            _IWebServicesSettings.AuthorizeDiscogsPin(_Pin);
            CanFinalizePin = false;
            Window.Close();
        }

        //private void DoClose()
        //{
        //    Window.Close();
        //}

        public override void Dispose()
        {
            Activate();        
            base.Dispose();
        }
    }
}
