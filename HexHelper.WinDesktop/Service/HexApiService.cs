using System;
using System.Collections.Generic;
using System.Linq;
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
        public HexApiService( IFileService aFileService, IRepository aRepo )
        {
            mFileService = aFileService;
            mRepo = aRepo;
        }

        public async Task Initialize()
        {
            await mRepo.Initialize();
        }

        public async Task Shutdown()
        {
            await mRepo.Persist();
        }

        public async Task UpdatePrices()
        {
            var theFile = new CachedRemoteFile( "http://doc-x.net/hex/all_prices_json.txt", mFileService );
            if( await theFile.DownloadFile() )
            {
                mRepo.UpdatePrices( AuctionHouseData.ParseJson( theFile.Content ) );
                await mRepo.Persist();
            }

        }

        public async Task UpdateItems()
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

        public async Task<IMessage> ParseMessageString( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = Parser.ParseMessage( aMessageString, aLogToFile );
            await StoreMessage( theMessage, aMessageString );

            var theCollectionMessage = theMessage as CollectionMessage;
            if( theCollectionMessage != null )
            {
                mRepo.UpdateInventory( theCollectionMessage.Complete );
                if( theCollectionMessage.CardsAdded != null )
                {
                    foreach( var theItem in theCollectionMessage.CardsAdded )
                    {
                        mRepo.UpdateCopiesOwned( theItem.Key, theItem.Value.CopiesOwned );
                    }
                }
                if( theCollectionMessage.CardsRemoved != null )
                {
                    foreach( var theItem in theCollectionMessage.CardsRemoved )
                    {
                        mRepo.UpdateCopiesOwned( theItem.Key, theItem.Value.CopiesOwned * -1 );
                    }
                }
                OnCollectionChanged();
            }

            return theMessage;
        }

        private void OnCollectionChanged()
        {
            if( CollectionChanged != null )
            {
                CollectionChanged( this, new EventArgs() );
            }
        }
        public event EventHandler CollectionChanged;

        private async Task StoreMessage( IMessage aMessage, string aMessageString )
        {
            if( !aMessage.LogToFile )
            {
                return;
            }

            string theFileName = String.Format( "{0}.json", DateTime.Now.ToFileTimeUtc() );
            await mFileService.SaveFile( "Message\\" + aMessage.Type.ToString(), theFileName, aMessageString );
        }

        public IEnumerable<Item> GetCards()
        {
            return mRepo.AllCards();
        }

        private readonly IRepository mRepo;
        private readonly IFileService mFileService;
    }
}
