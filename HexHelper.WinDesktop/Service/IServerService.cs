using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexHelper.WinDesktop.Service
{
    public interface IServerService
    {
        void Start( int port = 1408 );

        void Stop();

        event EventHandler<string> DataPosted;

        event EventHandler<string> ErrorOccurred;
    }
}
