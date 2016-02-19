using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IServerService aServer )
        {
            mServer = aServer;
            mServer.DataPosted += HandleDataPosted;
            mServer.ErrorOccurred += HandleErrorOccurred;

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

        private void HandleDataPosted( object sender, string e )
        {
            Status += e;
        }

        public override void Cleanup()
        {
            mServer.Stop();

            base.Cleanup();
        }

        public string Status { 
            get {
                return _status;
            }

            set
            {
                Set( nameof( Status ), ref _status, value );
            }
        }
        private string _status;

        public RelayCommand StartCommand { get; private set; }

        private IServerService mServer;
    }
}