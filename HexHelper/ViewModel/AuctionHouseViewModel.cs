using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using HexHelper.Libs.Model;
using HexHelper.Service;

namespace HexHelper.ViewModel
{
    public sealed class AuctionHouseViewModel : ViewModelBase
    {
        public AuctionHouseViewModel( IHexApiService aHexApi )
        {
            mHexApi = aHexApi;

            Update();

            mHexApi.CollectionChanged += HandleCollectionChanged;
        }

        private void HandleCollectionChanged( object sender, EventArgs e )
        {
            Update();
        }

        private void Update()
        {
            var theCards = mHexApi.GetCards();

            var theRecommendations = new List<ItemViewModel>();
            foreach( var theCard in theCards )
            {
                if( theCard.QuantityOwned < 5 )
                {
                    continue;
                }

                if( theCard.Rarity == ERarity.Rare || theCard.Rarity == ERarity.Legendary )
                {
                    if( theCard.SalesPlatinum > 10 && theCard.PricePlatinum > 40 )
                    {
                        theRecommendations.Add( theCard );
                    }
                }
                else if( theCard.Rarity == ERarity.Uncommon || theCard.Rarity == ERarity.Common )
                {
                    if( ( theCard.SalesGold > 10 && theCard.PriceGold > 1000  ) ||
                        ( theCard.SalesPlatinum > 10 && theCard.PricePlatinum > 11 ) )
                    {
                        theRecommendations.Add( theCard );
                    }
                }
            }

            Items = theRecommendations;
        }

        public IEnumerable<ItemViewModel> Items
        {
            get
            {
                return mItems;
            }
            set
            {
                Set( nameof( Items ), ref mItems, value );
            }
        }
        private IEnumerable<ItemViewModel> mItems = null;

        private readonly IHexApiService mHexApi;
    }
}
