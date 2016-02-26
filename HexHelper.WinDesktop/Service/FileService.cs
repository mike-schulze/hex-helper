using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex.Interface;

namespace HexHelper.WinDesktop.Service
{
    public class FileService : IFileService
    {
        public async Task<string> LoadFile( string aPath )
        {
            if( !File.Exists( aPath ) )
            {
                return null;
            }

            using( TextReader theReader = new StreamReader( aPath ) )
            {
                return await theReader.ReadToEndAsync();
            }
        }

        public async Task<string> LoadFile( string aRelativeDirectory, string aFileName )
        {
            var thePath = ConstructPath( aRelativeDirectory );
            if( !Directory.Exists( thePath ) )
            {
                return null;
            }
           
            var theFileName = Path.Combine( thePath, aFileName );
            return await LoadFile( theFileName );
        }

        public Task SaveFile( string aRelativeDirectory, string aFileName, string aContent )
        {
            var thePath = ConstructPath( aRelativeDirectory );
            Directory.CreateDirectory( thePath );

            using( TextWriter theWriter = new StreamWriter( Path.Combine( thePath, aFileName ) ) )
            {
                // This is a hack. When using WriteLineAsync at app shut-down, file gets truncated.
                theWriter.WriteLine( aContent );
                return Task.FromResult( 0 );
            }
        }

        private string ConstructPath( string aRelativeDirectory )
        {
            return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
                                 "HexHelper",
                                 aRelativeDirectory );
        }
    }
}
