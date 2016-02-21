using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.HexApi;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IServerService aServer, IHexApiService aHexApi )
        {
            mServer = aServer;
            mServer.DataPosted += HandleDataPosted;
            mServer.ErrorOccurred += HandleErrorOccurred;

            mHexApi = aHexApi;

            StartCommand = new RelayCommand( StartServer );
        }

        public async Task Initialize()
        {
            StartServer();

            await mHexApi.Initialize();
            await mHexApi.UpdatePrices();
        }

        private void StartServer()
        {
            mServer.Start();
        }

        private void HandleErrorOccurred( object sender, string e )
        {
            
        }

        private async void HandleDataPosted( object sender, string aMessageString )
        {
            var theMessage = await mHexApi.ParseMessageString( aMessageString );
            if( theMessage.Type == Message.MessageType.Unknown )
            {
                Status = string.Format( "{0} - Unknown message received", DateTime.Now.ToShortTimeString() );
            }
            else
            {
                Status = string.Format( "{0} - {1} message received", DateTime.Now.ToShortTimeString(), theMessage.Type );
            }
        }

        public async Task Shutdown()
        {
            mServer.Stop();
            await mHexApi.Shutdown();
        }

        public string Status { 
            get {
                return mStatus;
            }

            set
            {
                Set( nameof( Status ), ref mStatus, value );
            }
        }
        private string mStatus;

        public RelayCommand StartCommand { get; private set; }

        private IServerService mServer;
        private IHexApiService mHexApi;
    }
}