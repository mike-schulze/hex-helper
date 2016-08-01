using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HexHelper.Hex;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IHexApiService aHexApi, IDialogService aDialogs )
        {
            mHexApi = aHexApi;
        }

        public async Task Initialize()
        {
            mHexApi.StatusChanged += HandleStatusChanged;

            await mHexApi.Initialize();

            CurrentUser = mHexApi.GetCurrentUser();
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );
            Inventory = new ObservableCollection<ItemViewModel>( mHexApi.GetInventory() );

            mHexApi.CollectionChanged += HandleCollectionChanged;
            mHexApi.UserChanged += HandleUserChanged;
        }
        private void HandleCollectionChanged( object sender, EventArgs e )
        {
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );
            Inventory = new ObservableCollection<ItemViewModel>( mHexApi.GetInventory() );
        }

        private void HandleUserChanged( object sender, User e )
        {
            CurrentUser = e;
        }
        private void HandleStatusChanged( object sender, string e )
        {
            Status = String.Format( "{0} - {1}", DateTime.Now.ToShortTimeString() , e);
        }

        public async Task Shutdown()
        {
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

        public ObservableCollection<ItemViewModel> Inventory
        {
            get
            {
                return mInventory;
            }
            set
            {
                Set( nameof( Inventory ), ref mInventory, value );
            }
        }
        private ObservableCollection<ItemViewModel> mInventory;


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

        private readonly IHexApiService mHexApi;
    }
}