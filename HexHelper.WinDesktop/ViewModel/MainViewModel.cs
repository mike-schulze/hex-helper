using System;
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

            StartServer();
        }

        private void StartServer()
        {
            mServer.Start();
        }

        private void HandleErrorOccurred( object sender, string e )
        {
            
        }

        private void HandleDataPosted( object sender, string aMessageString )
        {
            var theMessage = mHexApi.ParseMessageString( aMessageString );
            if( theMessage.Type == Message.MessageType.Unknown )
            {
                Status = string.Format( "{0} - Unknown message received", DateTime.Now.ToShortTimeString() );
            }
            else
            {
                Status = string.Format( "{0} - {1} message received", DateTime.Now.ToShortTimeString(), theMessage.Type );
            }
        }

        public override void Cleanup()
        {
            mServer.Stop();

            base.Cleanup();
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