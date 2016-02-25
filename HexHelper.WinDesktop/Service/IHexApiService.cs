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
        Task<Message> ParseMessageString( string aMessageString );        
        Task Shutdown();
        IEnumerable<Card> GetCards();
    }
}
