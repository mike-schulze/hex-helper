using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.Libs.Model;
using HexHelper.Libs.Service;
using HexHelper.Service;

namespace HexHelper.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel( IHexApiService aHexApi, IDialogService aDialogs )
        {
            mHexApi = aHexApi;

            if( !IsInDesignMode )
            {
                OpenSettingsCommand = new RelayCommand( OpenSettings, () => !SettingsVisible );
                CloseSettingsCommand = new RelayCommand( CloseSettings, () => SettingsVisible );
            }
        }

        public async Task Initialize()
        {
            mHexApi.StatusChanged += HandleStatusChanged;

            await mHexApi.Initialize();
            
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );
            Inventory = new ObservableCollection<ItemViewModel>( mHexApi.GetInventory() );

            mHexApi.CollectionChanged += HandleCollectionChanged;
        }

        private void HandleCollectionChanged( object sender, EventArgs e )
        {
            Cards = new ObservableCollection<ItemViewModel>( mHexApi.GetCards() );
            Inventory = new ObservableCollection<ItemViewModel>( mHexApi.GetInventory() );
        }

        private void HandleStatusChanged( object sender, string e )
        {
            Status = String.Format( "{0} - {1}", DateTime.Now.ToShortTimeString() , e);
        }

        private void OpenSettings()
        {
            SettingsVisible = true;
        }

        private void CloseSettings()
        {
            SettingsVisible = false;
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

        public string Title
        {
            get
            {
                var theVersion = Assembly.GetEntryAssembly().GetName().Version;
                return String.Format( "Hex Helper v{0}.{1}.{2}", theVersion.Major, theVersion.MajorRevision, theVersion.Build );
            }
        }

        public bool SettingsVisible
        {
            get
            {
                return mSettingsVisible;
            }
            set
            {
                if( Set( nameof( SettingsVisible ), ref mSettingsVisible, value ) )
                {
                    OpenSettingsCommand.RaiseCanExecuteChanged();
                    CloseSettingsCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private bool mSettingsVisible = false;

        public RelayCommand OpenSettingsCommand { get; private set; }
        public RelayCommand CloseSettingsCommand { get; private set; }

        private readonly IHexApiService mHexApi;
    }
}