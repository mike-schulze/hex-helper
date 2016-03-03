using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
