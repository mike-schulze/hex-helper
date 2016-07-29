using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HexHelper.Hex.Interface
{
    public interface IRepository
    {
        Info GetItem( string aGuid );
        void UpdateItemInfo( IDictionary<Guid, Info> aItemInfo );
        void UpdatePrices( IDictionary<Guid, AuctionHouseInfo> aAuctionHouseData );
        void UpdateInventory( string aUserName, IDictionary<Guid, CollectionInfo> aCollectionData );
        void UpdateCopiesOwned( string aUserName, Guid aId, int aDelta );
        void AddOrUpdateUser( User aUser );
        User UserFromName( string aUserName );
        IEnumerable<User> AllUsers();
        IEnumerable<ItemViewModel> AllCards( string aUserName );
        Task Initialize();
        Task Persist();

        event EventHandler ItemsChanged;
    }
}
