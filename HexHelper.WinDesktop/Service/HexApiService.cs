using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.HexApi;
using HexHelper.JsonApi;

namespace HexHelper.WinDesktop.Service
{
    public sealed class HexApiService : IHexApiService
    {
        public Task<IEnumerable<Card>> DownloadCardList()
        {
            return AuctionHouseData.RetrievePriceList();
        }

        public Message ParseMessageString( string aMessageString )
        {
            var theMessage = Message.ParseMessage( aMessageString );
            StoreMessage( theMessage, aMessageString );

            return theMessage;
        }

        private void StoreMessage( Message aMessage, string aMessageString )
        {
            var thePath = Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
                "HexHelper",
                "Messages" );
            Directory.CreateDirectory( thePath );

            string theFileName = String.Format( "{0}-{1}.json", DateTime.Now.ToFileTimeUtc(), aMessage.Type.ToString() );

            using( TextWriter theWriter = new StreamWriter( Path.Combine( thePath, theFileName ) ) )
            {
                theWriter.WriteLine( aMessageString );
            }
        }
    }
}
