using System;

namespace HexHelper.WinDesktop.Service
{
    public interface IServerService
    {
        void Start( int aPort = 1408 );

        void Stop();

        event EventHandler<string> DataPosted;

        event EventHandler<string> ErrorOccurred;
    }
}
