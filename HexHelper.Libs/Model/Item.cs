using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HexHelper.Libs.Model
{
    /// <summary>
    /// Type of Hex item.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EItemType
    {
        Card,
        Champion,
        Equipment,
        Gem,
        Pack,
        Mod,
        Bane,
        Unknown
    }

    /// <summary>
    /// Card item.
    /// </summary>
    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum ECardType
    {
        DoesNotApply,
        Troop,
        Constant,
        Resource,
        Action
    }

    /// <summary>
    /// Rarities
    /// </summary>
    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum ERarity
    {
        Unknown,
        NonCollectible,
        Common,
        Uncommon,
        Rare,
        Legendary,
        Epic,
        Promo
    }

    /// <summary>
    /// Divsions
    /// </summary>
    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum EDivision
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Platinum = 3,
        Cosmic = 4
    }

    /// <summary>
    /// Formats
    /// </summary>
    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum EFormat
    {
        Constructed,
        Limited
    }

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum ERace
    {
        Human = 1,
        Elf = 2,
        Coyotle = 3,
        Orc = 4,
        Dwarf = 5,
        Shinhare = 6,
        Vennen = 7,
        Necrotic = 8
    }

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum EClass
    {
        Mage = 1,
        Warrior = 2,
        Cleric = 3
    }

    /// <summary>
    /// Properties of an item that generally never change.
    /// </summary>
    public class Info
    {
        public string Name { get; set; }
        public EItemType Type { get; set; }
        public ECardType CardType { get; set; }
        public ERarity Rarity { get; set; }

        public bool IsQuick { get; set; }
        public bool IsArtifact { get; set; }
    }

    /// <summary>
    /// Info about how many of an item are in the user's collection.
    /// </summary>
    public class CollectionInfo
    {
        public int QuantityOwned { get; set; }
    }

    /// <summary>
    /// Auction House data about an item.
    /// </summary>
    public class AuctionHouseInfo
    {
        public int PricePlatinum { get; set; }
        public int PriceGold { get; set; }
        public int SalesPlatinum { get; set; }
        public int SalesGold { get; set; }
    }

    /// <summary>
    /// All information combined for a Hex item.
    /// A lot repetition, but makes sorting code easier.
    /// </summary>
    public class ItemViewModel
    {
        public ItemViewModel( Guid aId, Info aInfo, CollectionInfo aCollection, AuctionHouseInfo aAuctionHouse )
        {
            Id = aId;

            if( aInfo != null )
            {
                Name = aInfo.Name;
                Type = aInfo.Type;
                CardType = aInfo.CardType;
                Rarity = aInfo.Rarity;
                IsQuick = aInfo.IsQuick;
                IsArtifact = aInfo.IsArtifact;
            }

            if( aCollection != null )
            {
                QuantityOwned = aCollection.QuantityOwned;
            }
            
            if( aAuctionHouse != null )
            {
                PricePlatinum = aAuctionHouse.PricePlatinum;
                PriceGold = aAuctionHouse.PriceGold;
                SalesPlatinum = aAuctionHouse.SalesPlatinum;
                SalesGold = aAuctionHouse.SalesGold;
            }
        }

        // Id
        public Guid Id { get; private set; }

        // Info
        public string Name { get; set; }
        public EItemType Type { get; set; }
        public ECardType CardType { get; set; }
        public ERarity Rarity { get; set; }
        public bool IsQuick { get; set; }
        public bool IsArtifact { get; set; }

        // Collection
        public int QuantityOwned { get; set; }

        // AuctionHouse
        public int PricePlatinum { get; set; }
        public int PriceGold { get; set; }
        public int SalesPlatinum { get; set; }
        public int SalesGold { get; set; }
    }
}
