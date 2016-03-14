using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HexHelper.Hex.Interface;
using Newtonsoft.Json;

namespace HexHelper.Hex
{
    public class Repository : IRepository
    {
        public Repository( IFileService aFileService )
        {
            mFileService = aFileService;

            mItemInfo = new Dictionary<Guid, Info>();
            mCollectionData = new Dictionary<Guid, CollectionInfo>();
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

            string theCollectionFileContent = await mFileService.LoadFile( "Database", "collection_info.json" );
            if( theCollectionFileContent != null )
            {
                var theCollectionData = JsonConvert.DeserializeObject<Dictionary<Guid, CollectionInfo>>( theCollectionFileContent );
                if( theCollectionData != null )
                {
                    mCollectionData = theCollectionData;
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
            await mFileService.SaveFile( "Database", "collection_info.json", JsonConvert.SerializeObject( mCollectionData ) );
            await mFileService.SaveFile( "Database", "ah_info.json", JsonConvert.SerializeObject( mAuctionHouseData ) );
        }

        public void UpdateItemInfo( IDictionary<Guid, Info> aInfo )
        {
            mItemInfo = (Dictionary<Guid, Info>)aInfo;
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
        }

        public void UpdateInventory( IDictionary<Guid, CollectionInfo> aCollectionData )
        {
            if( aCollectionData == null || !aCollectionData.Any() )
            {
                return;
            }

            mCollectionData = ( Dictionary<Guid, CollectionInfo> ) aCollectionData;
        }

        public void UpdateCopiesOwned( Guid aId, int aDelta )
        {
            if( mCollectionData.ContainsKey( aId ) )
            {
                mCollectionData[aId].CopiesOwned += aDelta;
                return;
            }

            mCollectionData.Add( aId, new CollectionInfo() { CopiesOwned = Math.Max( 0, aDelta ) } );
        }

        IEnumerable<ItemViewModel> IRepository.AllCards()
        {
            return from theInfo in mItemInfo
                   where ( theInfo.Value.Type == ItemType.Card &&
                           theInfo.Value.Rarity != RarityType.Unknown &&
                           theInfo.Value.Rarity != RarityType.Promo && 
                           theInfo.Value.Rarity != RarityType.NonCollectible )
                   select CreateItemViewModel( theInfo.Key );
        }

        private ItemViewModel CreateItemViewModel( Guid aId )
        {         
            if( !mItemInfo.ContainsKey( aId ) )
            {
                return null;
            }
             
            CollectionInfo theCollection = null;
            if( mCollectionData.ContainsKey( aId ) )
            {
                theCollection = mCollectionData[aId];
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
        private Dictionary<Guid, CollectionInfo> mCollectionData;
        private Dictionary<Guid, AuctionHouseInfo> mAuctionHouseData;
    }
}
