using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.JsonApi.Prices;

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
            var theCachedPricesPath = mFileService.FilePath( "Prices", "cache.json" );
            if( File.Exists( theCachedPricesPath ) )
            {
                var theFileInfo = new FileInfo( theCachedPricesPath );
                var theDiff = DateTime.Now - theFileInfo.LastWriteTime;
                if( theDiff.Hours < 8 )
                {
                    var theCards = AuctionHouseData.ParseJson( await mFileService.LoadFile( "Prices", "cache.json" ) );
                    if( theCards != null )
                    {
                        mRepo.UpdatePrices( theCards );
                        await mRepo.Persist();
                        return;
                    }
                }
            }

            var theJson = await AuctionHouseData.DownloadPricelist();
            await mFileService.SaveFile( "Prices", "cache.json", theJson );
            mRepo.UpdatePrices( AuctionHouseData.ParseJson( theJson ) );
            await mRepo.Persist();
        }

        public async Task<IMessage> ParseMessageString( string aMessageString, bool? aLogToFile = null )
        {
            var theMessage = Parser.ParseMessage( aMessageString, aLogToFile );
            await StoreMessage( theMessage, aMessageString );

            var theCollectionMessage = ( CollectionMessage ) theMessage;
            if( theMessage != null )
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
