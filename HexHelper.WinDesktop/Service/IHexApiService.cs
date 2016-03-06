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
        Task UpdatePrices();
        Task UpdateItems();
        Task<IMessage> ParseMessageString( string aMessageString, bool? aLogToFile = null );
        Task Shutdown();
        IEnumerable<Item> GetCards();
        event EventHandler CollectionChanged;
    }
}
