using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IHexApiService aHexApi, IDialogService aDialogs, IFileService aFile )
        {
            mHexApi = aHexApi;

            mDialogs = aDialogs;

            mFile = aFile;

            PickMessageCommand = new RelayCommand( PickMessage );
        }

        public async Task Initialize()
        {
            mHexApi.StatusChanged += HandleStatusChanged;

            await mHexApi.Initialize();

            CurrentUser = mHexApi.GetCurrentUser();
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );

            mHexApi.CollectionChanged += HandleCollectionChanged;
            mHexApi.UserChanged += HandleUserChanged;
        }
        private void HandleCollectionChanged( object sender, EventArgs e )
        {
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );
        }

        private void HandleUserChanged( object sender, User e )
        {
            CurrentUser = e;
        }
        private void HandleStatusChanged( object sender, string e )
        {
            Status = e;
        }

        public async Task Shutdown()
        {
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
            await mHexApi.HandleMessage( MessageText, aLogToFile: false );
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

        public ObservableCollection<ItemViewModel> Cards
        {
            get {
                return mCards;
            }
            set
            {
                Set( nameof( Cards ), ref mCards, value );
            }
        }
        private ObservableCollection<ItemViewModel> mCards;

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

        public User CurrentUser
        {
            get
            {
                return mCurrentUser;
            }

            set
            {
                Set( nameof( CurrentUser ), ref mCurrentUser, value );
            }
        }
        private User mCurrentUser;

        public string Title
        {
            get
            {
                return String.Format( "Hex Helper v{0}", Assembly.GetEntryAssembly().GetName().Version );
            }
        }

        public RelayCommand PickMessageCommand { get; private set; }

        private readonly IHexApiService mHexApi;
        private readonly IDialogService mDialogs;
        private readonly IFileService mFile;
    }
}