using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexHelper.Hex.Interface
{
    public interface IRepository
    {
        void UpdatePrices( IEnumerable<Card> aCards );
        void UpdateInventory( IEnumerable<CardCount> aCardCount );
        IEnumerable<Card> AllCards();
        Task Initialize();
        Task Persist();
    }
}
