using System;

namespace HexHelper.Hex
{
    /// <summary>
    /// Type of Hex item.
    /// </summary>
    public enum ItemType
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
    public enum CardType
    {
        DoesNotApply,
        Troop,
        Constant,
        Resource,
        Action
    }

    /// <summary>
    /// Properties of an item that generally do never change.
    /// </summary>
    public class Info
    {
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public CardType CardType { get; set; }

        public bool IsQuick { get; set; }
        public bool IsArtifact { get; set; }
    }

    /// <summary>
    /// Info about how many of an item are in the user's collection.
    /// </summary>
    public class CollectionInfo
    {
        public int CopiesOwned { get; set; }
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
    /// </summary>
    public class Item
    {
        public Item( Guid aId, Info aInfo, CollectionInfo aCollection, AuctionHouseInfo aAuctionHouse )
        {
            Id = aId;
            Info = aInfo;
            Collection = aCollection;
            AuctionHouse = aAuctionHouse;
        }

        public Guid Id { get; private set; }
        public Info Info { get; private set; }
        public CollectionInfo Collection { get; private set; }
        public AuctionHouseInfo AuctionHouse { get; private set; }
    }
}
