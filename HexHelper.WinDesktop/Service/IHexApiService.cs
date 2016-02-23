using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.HexApi;

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
