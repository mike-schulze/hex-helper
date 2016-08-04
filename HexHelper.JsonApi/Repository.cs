using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using Newtonsoft.Json;

namespace HexHelper.JsonApi
{
    public class Repository : IRepository
    {
        public Repository( IFileService aFileService )
        {
            mFileService = aFileService;

            mItemInfo = new Dictionary<Guid, Info>();
            mUserData = new Dictionary<string, User>();
            mCardCollection = new Dictionary<string, Dictionary<Guid, CollectionInfo>>();
            mInventoryCollection = new Dictionary<string, Dictionary<Guid, CollectionInfo>>();
            mAuctionHouseData = new Dictionary<Guid, AuctionHouseInfo>();
        }

        public async Task Initialize()
        {
            string theInfoFileContent = await mFileService.LoadFile( "Database", "item_info.json" );
            if( theInfoFileContent == null )
            {
                return;
            }

            var theInfo = JsonConvert.DeserializeObject<Dictionary<Guid, Info>>( theInfoFileContent );
            if( theInfo != null )
            {
                mItemInfo = theInfo;
            }

            string theUserFileContent = await mFileService.LoadFile( "Database", "users.json" );
            if( theUserFileContent != null )
            {
                var theUserData = JsonConvert.DeserializeObject<Dictionary<string, User>>( theUserFileContent );
                if( theUserData != null )
                {
                    mUserData = theUserData;
                }
            }

            foreach( var theUser in mUserData )
            {
                string theCollectionFileContent = await mFileService.LoadFile( Path.Combine( "Database", theUser.Key ), "collection_info.json" );
                if( theCollectionFileContent != null )
                {
                    var theCollectionData = JsonConvert.DeserializeObject<Dictionary<Guid, CollectionInfo>>( theCollectionFileContent );
                    if( theCollectionData != null )
                    {
                        mCardCollection[theUser.Key] = theCollectionData;
                    }
                    else
                    {
                        mCardCollection[theUser.Key] = new Dictionary<Guid, CollectionInfo>();
                    }
                }

                string theInventoryFileContent = await mFileService.LoadFile( Path.Combine( "Database", theUser.Key ), "inventory_info.json" );
                if( theInventoryFileContent != null )
                {
                    var theCollectionData = JsonConvert.DeserializeObject<Dictionary<Guid, CollectionInfo>>( theInventoryFileContent );
                    if( theCollectionData != null )
                    {
                        mInventoryCollection[theUser.Key] = theCollectionData;
                    }
                    else
                    {
                        mInventoryCollection[theUser.Key] = new Dictionary<Guid, CollectionInfo>();
                    }
                }
            }

            string theAuctionHouseFileContent = await mFileService.LoadFile( "Database", "ah_info.json" );
            if( theAuctionHouseFileContent != null )
            {
                var theAuctionHouseData = JsonConvert.DeserializeObject<Dictionary<Guid, AuctionHouseInfo>>( theAuctionHouseFileContent );
                if( theAuctionHouseData != null )
                {
                    mAuctionHouseData = theAuctionHouseData;
                }
            }
        }

        public async Task Persist()
        {
            await mFileService.SaveFile( "Database", "item_info.json", JsonConvert.SerializeObject( mItemInfo ) );
            await mFileService.SaveFile( "Database", "users.json", JsonConvert.SerializeObject( mUserData ) );
            foreach( var theUserCollection in mCardCollection )
            {
                await mFileService.SaveFile( Path.Combine( "Database", theUserCollection.Key ), "collection_info.json", JsonConvert.SerializeObject( theUserCollection.Value ) );
            }
            foreach( var theUserCollection in mInventoryCollection )
            {
                await mFileService.SaveFile( Path.Combine( "Database", theUserCollection.Key ), "inventory_info.json", JsonConvert.SerializeObject( theUserCollection.Value ) );
            }
            await mFileService.SaveFile( "Database", "ah_info.json", JsonConvert.SerializeObject( mAuctionHouseData ) );
        }

        public Info GetItem( string aGuid )
        {
            Guid theGuid;
            if( Guid.TryParse( aGuid, out theGuid ) && 
                mItemInfo.ContainsKey( theGuid ) )
            {
                return mItemInfo[theGuid];
            }

            return null;
        }

        private void OnItemsChanged()
        {
            ItemsChanged?.Invoke( this, new EventArgs() );
        }
        public event EventHandler ItemsChanged;

        public void UpdateItemInfo( IDictionary<Guid, Info> aInfo )
        {
            mItemInfo = (Dictionary<Guid, Info>)aInfo;
            OnItemsChanged();
        }

        public void UpdatePrices( IDictionary<Guid, AuctionHouseInfo> aAuctionHouseData )
        {            
            foreach( var theCard in aAuctionHouseData )
            {
                if( !mAuctionHouseData.ContainsKey( theCard.Key ) )
                {
                    mAuctionHouseData.Add( theCard.Key, theCard.Value );
                    continue;
                }

                mAuctionHouseData[theCard.Key] = theCard.Value;
            }

            OnItemsChanged();
        }

        public void UpdateUserCardCollection( string aUserName, IDictionary<Guid, CollectionInfo> aCollectionData )
        {
            if( aCollectionData == null || !aCollectionData.Any() )
            {
                return;
            }

            mCardCollection[aUserName] = ( Dictionary<Guid, CollectionInfo> ) aCollectionData;

            OnItemsChanged();
        }

        public void UpdateUserInventory( string aUserName, IDictionary<Guid, CollectionInfo> aCollectionData )
        {
            if( aCollectionData == null || !aCollectionData.Any() )
            {
                return;
            }

            mInventoryCollection[aUserName] = ( Dictionary<Guid, CollectionInfo> ) aCollectionData;

            OnItemsChanged();
        }

        public void UpdateCardQuantities( string aUserName, Guid aId, int aDelta )
        {
            if( !mCardCollection.ContainsKey( aUserName ) )
            {
                return;
            }

            if( mCardCollection[aUserName].ContainsKey( aId ) )
            {
                mCardCollection[aUserName][aId].QuantityOwned += aDelta;
                return;
            }

            mCardCollection[aUserName].Add( aId, new CollectionInfo() { QuantityOwned = Math.Max( 0, aDelta ) } );

            OnItemsChanged();
        }

        public void UpdateInventoryQuantities( string aUserName, Guid aId, int aDelta )
        {
            if( !mInventoryCollection.ContainsKey( aUserName ) )
            {
                return;
            }

            if( mInventoryCollection[aUserName].ContainsKey( aId ) )
            {
                mInventoryCollection[aUserName][aId].QuantityOwned += aDelta;
                return;
            }

            mInventoryCollection[aUserName].Add( aId, new CollectionInfo() { QuantityOwned = Math.Max( 0, aDelta ) } );

            OnItemsChanged();
        }

        public IEnumerable<ItemViewModel> AllCards( string aUserName )
        {
            return from theInfo in mItemInfo
                   where ( theInfo.Value.Type == ItemType.Card &&
                           theInfo.Value.Rarity != RarityType.Unknown &&
                           theInfo.Value.Rarity != RarityType.Promo &&
                           theInfo.Value.Rarity != RarityType.NonCollectible )
                   select CreateItemViewModel( aUserName, theInfo.Key );
        }

        public IEnumerable<ItemViewModel> AllInventory( string aUserName )
        {
            return from theInfo in mItemInfo
                   where ( theInfo.Value.Type != ItemType.Card )
                   select CreateItemViewModel( aUserName, theInfo.Key );
        }

        public void AddOrUpdateUser( User aUser )
        {
            if( mUserData.ContainsKey( aUser.UserName ) )
            {
                mUserData[aUser.UserName] = aUser;
            }
            else if( !String.IsNullOrEmpty( aUser.UserName ) )
            {
                mUserData.Add( aUser.UserName, aUser );
                mCardCollection.Add( aUser.UserName, new Dictionary<Guid, CollectionInfo>() );
                mInventoryCollection.Add( aUser.UserName, new Dictionary<Guid, CollectionInfo>() );
            }
        }

        public IEnumerable<User> AllUsers()
        {
            return mUserData.Values;
        }

        public User UserFromName( string aUserName )
        {
            if( aUserName != null && mUserData.ContainsKey( aUserName ) )
            {
                return mUserData[aUserName];
            }

            return null;
        }

        private ItemViewModel CreateItemViewModel( string aUserName, Guid aId )
        {         
            if( !mItemInfo.ContainsKey( aId ) )
            {
                return null;
            }
             
            CollectionInfo theCollection = null;
            if( aUserName != null )
            {
                if( mCardCollection.ContainsKey( aUserName ) && mCardCollection[aUserName].ContainsKey( aId ) )
                {
                    theCollection = mCardCollection[aUserName][aId];
                }

                if( mInventoryCollection.ContainsKey( aUserName ) && mInventoryCollection[aUserName].ContainsKey( aId ) )
                {
                    theCollection = mInventoryCollection[aUserName][aId];
                }
            }

            AuctionHouseInfo theAuctionHouse = null;
            if( mAuctionHouseData.ContainsKey( aId ) )
            {
                theAuctionHouse = mAuctionHouseData[aId];
            }

            return new ItemViewModel( aId, mItemInfo[aId], theCollection, theAuctionHouse );
        }

        private readonly IFileService mFileService;
        private Dictionary<Guid, Info> mItemInfo;        
        private Dictionary<string, User> mUserData;
        private Dictionary<string, Dictionary<Guid, CollectionInfo>> mCardCollection;
        private Dictionary<string, Dictionary<Guid, CollectionInfo>> mInventoryCollection;
        private Dictionary<Guid, AuctionHouseInfo> mAuctionHouseData;
    }
}
