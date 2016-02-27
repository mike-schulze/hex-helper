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
        void UpdateInventory( IEnumerable<ObjectCount> aCollection, IEnumerable<ObjectCount> aAdded, IEnumerable<ObjectCount> aRemoved );
        IEnumerable<Card> AllCards();
        Task Initialize();
        Task Persist();
    }
}
