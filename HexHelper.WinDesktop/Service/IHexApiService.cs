using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.JsonApi.HexApi;

namespace HexHelper.WinDesktop.Service
{
    public interface IHexApiService
    {
        Task Initialize();
        Task HandleMessage( string aMessageString, bool? aLogToFile = null );
        Task Shutdown();

        IEnumerable<ItemViewModel> GetCards();

        event EventHandler CollectionChanged;
        event EventHandler<IMessage> MessageReceived;
        event EventHandler<string> StatusChanged;
    }
}
