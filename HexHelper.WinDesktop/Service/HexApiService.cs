using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.JsonApi.Utils;
using HexHelper.JsonApi.WebApi;

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
        }

        public async Task HandleMessage( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = await ParseMessageString( aMessageString, aLogToFile );
            if( theMessage == null )
            {
                return;
            }

            if( !String.IsNullOrEmpty( theMessage.User ) )
            {
                if( mCurrentUser == null || theMessage.User != mCurrentUser.UserName )
                {
                    var theUser = mRepo.UserFromName( theMessage.User );
                    if( theUser == null )
                    {
                        theUser = new User( theMessage.User );
                    }

                    SetCurrentUser( theUser );
                    OnCollectionChanged();
                }
            }

            if( theMessage.Type == MessageType.Unknown )
            {
                OnStatusChanged( String.Format( "{0} - Unknown message received", DateTime.Now.ToShortTimeString() ) );
            }
            else
            {
                OnStatusChanged( String.Format( "{0} - {1} message received", DateTime.Now.ToShortTimeString(), theMessage.Type ) );
            }

            OnMessageReceived( theMessage );
        }

        public async Task Shutdown()
        {
            mServer.Stop();
            await mRepo.Persist();
        }

        private void OnCollectionChanged()
        {
            if( CollectionChanged != null )
            {
                CollectionChanged( this, new EventArgs() );
            }
        }
        public event EventHandler CollectionChanged;

        private void OnMessageReceived( IMessage aMessage )
        {
            if( MessageReceived != null )
            {
                MessageReceived( this, aMessage );
            }
        }
        public event EventHandler<IMessage> MessageReceived;

        private void OnStatusChanged( string aStatusText )
        {
            if( StatusChanged != null )
            {
                StatusChanged( this, aStatusText );
            }
        }
        public event EventHandler<string> StatusChanged;

        private void OnUserChanged( User aUser )
        {
            if( UserChanged != null )
            {
                UserChanged( this, aUser );
            }
        }
        public event EventHandler<User> UserChanged;

        private async Task StoreMessage( IMessage aMessage, string aMessageString )
        {
            if( !aMessage.LogToFile )
            {
                return;
            }

            string theFileName = String.Format( "{0}.json", DateTime.Now.ToFileTimeUtc() );
            await mFileService.SaveFile( "Message\\" + aMessage.Type.ToString(), theFileName, aMessageString );
        }

        public IEnumerable<ItemViewModel> GetCards()
        {
            OnStatusChanged( "Collection loaded." );
            return mRepo.AllCards( mCurrentUser != null ? mCurrentUser.UserName : null );
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
            var theItemListFile = new CachedRemoteFile( "http://hexdbapi2.hexsales.net/v1/objects/search", mFileService );
            theItemListFile.PostMessage = "{}";
            theItemListFile.CacheFileName = "itemlist.json";
            if( await theItemListFile.DownloadFile() )
            {
                var theItems = HexItemSearch.ParseJson( theItemListFile.Content );
                mRepo.UpdateItemInfo( theItems );
                await mRepo.Persist();
            }
        }

        private async Task<IMessage> ParseMessageString( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = Parser.ParseMessage( aMessageString, aLogToFile );
            await StoreMessage( theMessage, aMessageString );
            ForwardMessage( theMessage, aMessageString );

            var theCollectionMessage = theMessage as CollectionMessage;
            if( theCollectionMessage != null )
            {
                mRepo.UpdateInventory( theMessage.User, theCollectionMessage.Complete );
                if( theCollectionMessage.CardsAdded != null )
                {
                    foreach( var theItem in theCollectionMessage.CardsAdded )
                    {
                        mRepo.UpdateCopiesOwned( theMessage.User, theItem.Key, theItem.Value.CopiesOwned );
                    }
                }
                if( theCollectionMessage.CardsRemoved != null )
                {
                    foreach( var theItem in theCollectionMessage.CardsRemoved )
                    {
                        mRepo.UpdateCopiesOwned( theMessage.User, theItem.Key, theItem.Value.CopiesOwned * -1 );
                    }
                }
                OnCollectionChanged();
            }

            return theMessage;
        }

        private async void ForwardMessage( IMessage aMessage, string aMessageString )
        {
            if( aMessage == null || String.IsNullOrWhiteSpace( aMessage.User ) || !aMessage.SupportsHexTcgBrowser )
            {
                return;
            }

            var theUser = mRepo.UserFromName( aMessage.User );
            if( theUser == null || String.IsNullOrWhiteSpace( theUser.TcgBrowserSyncCode  ) )
            {
                return;
            }

            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    await theHttpClient.PostAsync(
                        String.Format( "http://hex.tcgbrowser.com:8080/sync?{0}", theUser.TcgBrowserSyncCode ),
                        new StringContent( aMessageString, Encoding.UTF8, "application/json" ) );
                }
            }
            catch
            {
            }
        }

        private void HandleErrorOccurred( object sender, string e )
        {

        }

        private async void HandleDataPosted( object sender, string aMessageString )
        {
            await HandleMessage( aMessageString );
        }

        private readonly IServerService mServer;
        private readonly IRepository mRepo;
        private readonly IFileService mFileService;

        private User mCurrentUser;
    }
}
