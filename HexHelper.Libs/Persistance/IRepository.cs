using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Libs.Model;

namespace HexHelper.Libs.Persistance
{
    public interface IRepository
    {
        Info GetItem( string aGuid );

        void UpdateItemInfo( IDictionary<Guid, Info> aItemInfo );
        void UpdatePrices( IDictionary<Guid, AuctionHouseInfo> aAuctionHouseData );

        void UpdateUserCardCollection( string aUserName, IDictionary<Guid, CollectionInfo> aCollectionData );
        void UpdateUserInventory( string aUserName, IDictionary<Guid, CollectionInfo> aCollectionData );

        void UpdateCardQuantities( string aUserName, Guid aId, int aDelta );
        void UpdateInventoryQuantities( string aUserName, Guid aId, int aDelta );

        void AddOrUpdateUser( User aUser );
        User UserFromName( string aUserName );
        IEnumerable<User> AllUsers();

        IEnumerable<ItemViewModel> AllCards( string aUserName );
        IEnumerable<ItemViewModel> AllInventory( string aUserName );

        Task Initialize();
        Task Persist();

        event EventHandler ItemsChanged;
    }
}
