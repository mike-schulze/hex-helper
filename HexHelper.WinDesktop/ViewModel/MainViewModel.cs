using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IServerService aServer, IHexApiService aHexApi, IDialogService aDialogs, IFileService aFile )
        {
            mServer = aServer;
            mServer.DataPosted += HandleDataPosted;
            mServer.ErrorOccurred += HandleErrorOccurred;

            mHexApi = aHexApi;

            mDialogs = aDialogs;

            mFile = aFile;

            StartCommand = new RelayCommand( StartServer );
            PickMessageCommand = new RelayCommand( PickMessage );
        }

        public async Task Initialize()
        {
            StartServer();

            Status = "Initializing database...";
            await mHexApi.Initialize();

            Status = "Updating items...";
            await mHexApi.UpdateItems();

            Status = "Updating prices...";
            await mHexApi.UpdatePrices();

            Cards = new ObservableCollection<Item>( mHexApi.GetCards() );
            Status = "Collection loaded.";

            mHexApi.CollectionChanged += HandleCollectionChanged;
        }

        private void HandleCollectionChanged( object sender, EventArgs e )
        {
            Cards = new ObservableCollection<Item>( mHexApi.GetCards() );
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
            await HandleMessage( aMessageString );
        }

        private async Task HandleMessage( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = await mHexApi.ParseMessageString( aMessageString, aLogToFile );
            if( theMessage.Type == MessageType.Unknown )
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

        private async void PickMessage()
        {
            var theFilePath = mDialogs.ShowFileOpenDialog( "Pick a Hex API json message", "*.json" );
            if( String.IsNullOrWhiteSpace( theFilePath ) )
            {
                return;
            }

            MessageText = await mFile.LoadFile( theFilePath );
            await HandleMessage( MessageText, aLogToFile: false );
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

        public ObservableCollection<Item> Cards
        {
            get {
                return mCards;
            }
            set
            {
                Set( nameof( Cards ), ref mCards, value );
            }
        }
        private ObservableCollection<Item> mCards;

        public string MessageText
        {
            get
            {
                return mMessageText;
            }
            set
            {
                Set( nameof( MessageText ), ref mMessageText, value );
            }
        }
        private string mMessageText;


        public RelayCommand StartCommand { get; private set; }
        public RelayCommand PickMessageCommand { get; private set; }

        private readonly IServerService mServer;
        private readonly IHexApiService mHexApi;
        private readonly IDialogService mDialogs;
        private readonly IFileService mFile;
    }
}