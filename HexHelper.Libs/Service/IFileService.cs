﻿using System;
using System.Threading.Tasks;

namespace HexHelper.Libs.Service
{
    public interface IFileService
    {
        Task SaveFile( string aRelativeDirectory, string aFileName, string aContent );
        Task<string> LoadFile( string aRelativeDirectory, string aFileName );
        Task<string> LoadFile( string aPath );

        DateTime LastWriteTime( string aRelativeDirectory, string aFileName );
        DateTime LastWriteTime( FileInfo aFileInfo );
        DateTime LastWriteTime( string aPath );

        bool Exists( string aRelativeDirectory, string aFileName );
        void OpenByOS( FileInfo aFileInfo );
    }

    public class FileInfo
    {
        public string RelativeFolder { get; set; }
        public string FileName { get; set; }
    }
}
