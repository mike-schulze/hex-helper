using System;

namespace HexHelper.Hex
{
    public struct CardCount
    {
        public Guid Id;
        public int Count;
    }

    public class Card
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PricePlatinum { get; set; }
        public int PriceGold { get; set; }
        public int SalesPlatinum { get; set; }
        public int SalesGold { get; set; }
        public int CopiesOwned { get; set; }
        public bool IsAlternateArt { get; set; }
    }
}
