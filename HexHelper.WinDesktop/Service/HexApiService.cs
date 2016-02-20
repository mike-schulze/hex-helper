using System;
using System.IO;
using HexHelper.HexApi;

namespace HexHelper.WinDesktop.Service
{
    public sealed class HexApiService : IHexApiService
    {
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
