using System;

namespace HexHelper.Service
{
    /// <summary>
    /// Provides a local server that is listening for API messages
    /// </summary>
    public interface IServerService
    {
        void Start( int aPort = 1408 );

        void Stop();

        event EventHandler<string> DataPosted;

        event EventHandler<string> ErrorOccurred;
    }
}
