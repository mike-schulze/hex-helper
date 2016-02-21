using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using HexHelper.HexApi;
using HexHelper.JsonApi;

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

        public Task Shutdown()
        {
            // bugged
            // await mRepo.Persist();
            return Task.FromResult( 0 );
        }

        public async Task UpdatePrices()
        {
            var theCards = await AuctionHouseData.RetrievePriceList();
            mRepo.UpdatePrices( theCards );
            await mRepo.Persist();
        }

        public async Task<Message> ParseMessageString( string aMessageString )
        {
            var theMessage = Message.ParseMessage( aMessageString );
            await StoreMessage( theMessage, aMessageString );
            return theMessage;
        }

        private async Task StoreMessage( Message aMessage, string aMessageString )
        {
            string theFileName = String.Format( "{0}.json", DateTime.Now.ToFileTimeUtc() );
            await mFileService.SaveFile( "Message\\" + aMessage.Type.ToString(), theFileName, aMessageString );
        }

        private readonly IRepository mRepo;
        private readonly IFileService mFileService;
    }
}
