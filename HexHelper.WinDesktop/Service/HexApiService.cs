using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.JsonApi.Prices;
using HexHelper.JsonApi.Utils;

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

        public async Task<IMessage> ParseMessageString( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = Parser.ParseMessage( aMessageString, aLogToFile );
            await StoreMessage( theMessage, aMessageString );

            var theCollectionMessage = theMessage as CollectionMessage;
            if( theCollectionMessage != null )
            {
                mRepo.UpdateInventory( theCollectionMessage.Complete, theCollectionMessage.CardsAdded, theCollectionMessage.CardsRemoved );
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

        public IEnumerable<Card> GetCards()
        {
            return mRepo.AllCards();
        }

        private readonly IRepository mRepo;
        private readonly IFileService mFileService;
    }
}
