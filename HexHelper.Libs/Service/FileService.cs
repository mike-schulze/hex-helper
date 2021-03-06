﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HexHelper.Libs.Service
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
            var thePath = ConstructDirectoryPath( aRelativeDirectory );
            if( !Directory.Exists( thePath ) )
            {
                return null;
            }
           
            var theFileName = Path.Combine( thePath, aFileName );
            return await LoadFile( theFileName );
        }

        public Task SaveFile( string aRelativeDirectory, string aFileName, string aContent )
        {
            var thePath = ConstructDirectoryPath( aRelativeDirectory );
            Directory.CreateDirectory( thePath );

            using( TextWriter theWriter = new StreamWriter( Path.Combine( thePath, aFileName ) ) )
            {
                // This is a hack. When using WriteLineAsync at app shut-down, file gets truncated.
                theWriter.WriteLine( aContent );
                return Task.FromResult( 0 );
            }
        }

        public bool Exists( string aRelativeDirectory, string aFileName )
        {
            return File.Exists( Path.Combine( ConstructDirectoryPath( aRelativeDirectory ), aFileName ) );
        }

        public DateTime LastWriteTime( string aPath )
        {
            DateTime theReturn = DateTime.MinValue;
            try
            {
                var theFileInfo = new System.IO.FileInfo( aPath );
                theReturn = theFileInfo.LastWriteTime;
            }
            catch
            {
                // Fall back to default
            }

            return theReturn;
        }

        public DateTime LastWriteTime( string aRelativeDirectory, string aFileName )
        {
            return LastWriteTime( ConstructFilePath( aRelativeDirectory, aFileName ) );
        }

        public DateTime LastWriteTime( HexHelper.Libs.Service.FileInfo aFileInfo )
        {
            return LastWriteTime( aFileInfo.RelativeFolder, aFileInfo.FileName );
        }

        public void OpenByOS( HexHelper.Libs.Service.FileInfo aFileInfo )
        {
            Process.Start( ConstructFilePath( aFileInfo ) );
        }

        private string ConstructDirectoryPath( string aRelativeDirectory )
        {
            return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ),
                                 "HexHelper",
                                 aRelativeDirectory );
        }

        private string ConstructFilePath( string aRelativeDirectory, string aFileName )
        {
            return Path.Combine( ConstructDirectoryPath( aRelativeDirectory ), aFileName );
        }


        private string ConstructFilePath( HexHelper.Libs.Service.FileInfo aFileInfo )
        {
            return ConstructFilePath( aFileInfo.RelativeFolder, aFileInfo.FileName );
        }
    }
}
