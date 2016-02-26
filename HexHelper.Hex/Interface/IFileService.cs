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
    }
}
