using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.JsonApi.Utils;
using HexHelper.JsonApi.WebApi;
using HexHelper.JsonApi.WebApiForward;

namespace HexHelper.WinDesktop.Service
{
    public sealed class HexApiService : IHexApiService
    {
        public HexApiService( IFileService aFileService, IRepository aRepo, IServerService aServer )
        {
            mServer = aServer;
            mServer.DataPosted += HandleDataPosted;
            mServer.ErrorOccurred += HandleErrorOccurred;

            mFileService = aFileService;
            mRepo = aRepo;
        }

        public async Task Initialize()
        {
            mServer.Start();

            OnStatusChanged( "Initializing database..." );
            await mRepo.Initialize();

            string theLastUser = ( string ) Properties.Settings.Default.LastUser;
            if( theLastUser != null )
            {
                mCurrentUser = mRepo.UserFromName( theLastUser );
            }

            OnStatusChanged( "Updating items..." );
            await UpdateItems();

            OnStatusChanged( "Updating prices..." );
            await UpdatePrices();

            mRepo.ItemsChanged += HandleItemsChanged;
        }

        public IMessage HandleMessageFromFile( string aMessageString, DateTime aDateTime )
        {
            var theMessage = HandleMessage( aMessageString );
            theMessage.Date = aDateTime;
            return theMessage;
        }

        private void HandleItemsChanged( object sender, EventArgs e )
        {
            OnCollectionChanged();
        }

        public async Task Shutdown()
        {
            mServer.Stop();
            await mRepo.Persist();
        }

        private void OnCollectionChanged()
        {
            CollectionChanged?.Invoke( this, new EventArgs() );
        }
        public event EventHandler CollectionChanged;

        private void OnMessageReceived( IMessage aMessage )
        {
            MessageReceived?.Invoke( this, aMessage );
        }
        public event EventHandler<IMessage> MessageReceived;

        private void OnStatusChanged( string aStatusText )
        {
            StatusChanged?.Invoke( this, aStatusText );
        }
        public event EventHandler<string> StatusChanged;

        private void OnUserChanged( User aUser )
        {
            UserChanged?.Invoke( this, aUser );
        }
        public event EventHandler<User> UserChanged;

        public IEnumerable<ItemViewModel> GetCards()
        {
            OnStatusChanged( "Collection loaded." );
            return mRepo.AllCards( mCurrentUser?.UserName );
        }

        public IEnumerable<ItemViewModel> GetInventory()
        {
            OnStatusChanged( "Inventory loaded." );
            return mRepo.AllInventory( mCurrentUser?.UserName );
        }

        public IEnumerable<User> GetUsers()
        {
            return mRepo.AllUsers();
        }

        public User GetCurrentUser()
        {
            return mCurrentUser;
        }

        public void SetCurrentUser( User aUser )
        {
            if( !mRepo.AllUsers().Contains( aUser ) )
            {
                mRepo.AddOrUpdateUser( aUser );
            }

            mCurrentUser = aUser;
            OnUserChanged( aUser );
            Properties.Settings.Default.LastUser = aUser.UserName;
            Properties.Settings.Default.Save();
        }

        private async Task UpdatePrices()
        {
            var theFile = new CachedRemoteFile( "http://doc-x.net/hex/all_prices_json.txt", mFileService );
            if( await theFile.DownloadFile() )
            {
                mRepo.UpdatePrices( AuctionHouseData.ParseJson( theFile.Content ) );
                await mRepo.Persist();
            }
        }

        private async Task UpdateItems()
        {
            var theItemListFile = new CachedRemoteFile( "http://hexdbapi.hexsales.net/v1/objects/search", mFileService );
            theItemListFile.PostMessage = "{}";
            theItemListFile.CacheFileName = "itemlist.json";
            if( await theItemListFile.DownloadFile() )
            {
                var theItems = HexItemSearch.ParseJson( theItemListFile.Content );
                mRepo.UpdateItemInfo( theItems );
                await mRepo.Persist();
            }
        }

        private IMessage HandleMessage( string aMessageString )
        {
            var theMessage = Parser.ParseMessage( aMessageString, mRepo );
            if( theMessage == null )
            {
                OnStatusChanged( "Message could not be parsed successfully." );
                return null;
            }

            if( !String.IsNullOrEmpty( theMessage.User ) &&
                ( mCurrentUser == null || theMessage.User != mCurrentUser.UserName ) )
            {
                var theUser = mRepo.UserFromName( theMessage.User );
                if( theUser == null )
                {
                    theUser = new User( theMessage.User );
                }

                SetCurrentUser( theUser );
                OnCollectionChanged();
            }

            if( theMessage.Type == MessageType.Unknown )
            {
                OnStatusChanged( "Unknown message received." );
            }
            else
            {
                OnStatusChanged( theMessage.Summary );
            }

            return theMessage;
        }

        private async Task ForwardMessage( IMessage aMessage, string aMessageString )
        {
            if( aMessage == null || String.IsNullOrWhiteSpace( aMessage.User ) || String.IsNullOrWhiteSpace( aMessageString ) )
            {
                return;
            }

            var theUser = mRepo.UserFromName( aMessage.User );
            if( theUser == null )
            {
                return;
            }

            var theHexSites = Forwarder.AllHexSites();
            foreach( var theSite in theHexSites )
            {
                if( theSite.Value.SupportedMessages.Contains( aMessage.Type ) )
                {
                    await Forwarder.ForwardMessage( theSite.Value, aMessageString, theUser );
                }
            }
        }

        private async Task<FileInfo> StoreMessage( IMessage aMessage, string aMessageString )
        {
            string theFileName = String.Format( "{0}.json", DateTime.Now.ToFileTimeUtc() );
            var theFileInfo = new FileInfo() { RelativeFolder = "Message\\" + aMessage.Type.ToString(), FileName = theFileName };
            await mFileService.SaveFile( theFileInfo.RelativeFolder, theFileName, aMessageString );
            return theFileInfo;
        }

        private void HandleErrorOccurred( object sender, string e )
        {

        }

        private async void HandleDataPosted( object sender, string aMessageString )
        {
            var theMessage = HandleMessage( aMessageString );
            theMessage.SourceFile = await StoreMessage( theMessage, aMessageString );
            OnMessageReceived( theMessage );
            await ForwardMessage( theMessage, aMessageString );
        }

        private readonly IServerService mServer;
        private readonly IRepository mRepo;
        private readonly IFileService mFileService;

        private User mCurrentUser;
    }
}
