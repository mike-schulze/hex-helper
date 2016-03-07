using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HexHelper.Hex.Interface
{
    public interface IRepository
    {
        void UpdateItemInfo( IDictionary<Guid, Info> aItemInfo );
        void UpdatePrices( IDictionary<Guid, AuctionHouseInfo> aAuctionHouseData );
        void UpdateInventory( IDictionary<Guid, CollectionInfo> aCollectionData );
        void UpdateCopiesOwned( Guid aId, int aDelta );
        IEnumerable<ItemViewModel> AllCards();
        Task Initialize();
        Task Persist();
    }
}
