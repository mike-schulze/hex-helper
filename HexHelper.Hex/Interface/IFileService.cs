using System;
using System.Threading.Tasks;

namespace HexHelper.Hex.Interface
{
    public interface IFileService
    {
        Task SaveFile( string aRelativeDirectory, string aFileName, string aContent );
        Task<string> LoadFile( string aRelativeDirectory, string aFileName );
        Task<string> LoadFile( string aPath );
        DateTime LastWriteTime( string aRelativeDirectory, string aFileName );
        bool Exists( string aRelativeDirectory, string aFileName );
    }

    public class FileInfo
    {
        public string RelativeFolder { get; set; }
        public string FileName { get; set; }
    }
}
