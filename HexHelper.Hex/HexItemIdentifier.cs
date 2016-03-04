using System;

namespace HexHelper.Hex
{
    public enum HexItemType
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

    public class HexItemIdentifier
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public HexItemType Type { get; set; }
    }
}
