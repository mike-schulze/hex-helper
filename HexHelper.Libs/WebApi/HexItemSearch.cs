using System;
using System.Collections.Generic;
using HexHelper.Libs.Model;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.WebApi
{
    public static class HexItemSearch
    {
        public static IDictionary<Guid, Info> ParseJson( string aJson )
        {
            var theList = new Dictionary<Guid, Info>();

            try
            {
                var theJson = JArray.Parse( aJson );

                foreach( var theItem in theJson )
                {
                    string theName = ( string ) theItem["name"];
                    if( String.IsNullOrWhiteSpace( theName ) )
                    {
                        continue;
                    }

                    Guid theGuid;
                    if( !Guid.TryParse( ( string ) theItem["uuid"], out theGuid ) )
                    {
                        continue;
                    }

                    EItemType theType = EItemType.Unknown;                    
                    var theTypes = ( JArray ) theItem["type"];
                    if( theTypes != null && theTypes.Count != 0  )
                    {
                        var theFirstType = ( string ) theTypes[0];
                        switch( theFirstType )
                        {
                            case "Basic Action":
                            case "Troop":
                            case "Artifact":
                            case "Constant":
                            case "Quick Action":
                            case "Quick":
                            case "Resource":
                                theType = EItemType.Card;
                                break;
                            case "Champion":
                                theType = EItemType.Champion;
                                break;
                            case "Equipment":
                                theType = EItemType.Equipment;
                                break;
                            case "Gem":
                                theType = EItemType.Gem;
                                break;
                            case "Pack":
                                theType = EItemType.Pack;
                                break;
                            case "Mod":
                                theType = EItemType.Mod;
                                break;
                            case "Bane":
                                theType = EItemType.Bane;
                                break;
                            default:
                                break;
                        }
                    }

                    ERarity theRarity = ERarity.Unknown;
                    string theRarityString = ( string ) theItem["rarity"];
                    switch( theRarityString )
                    {
                        case "Promo":
                            theRarity = ERarity.Promo;
                            break;
                        case "Non-Collectible":
                            theRarity = ERarity.NonCollectible;
                            break;
                        case "Common":
                            theRarity = ERarity.Common;
                            break;
                        case "Uncommon":
                            theRarity = ERarity.Uncommon;
                            break;
                        case "Rare":
                            theRarity = ERarity.Rare;
                            break;
                        case "Legendary":
                            theRarity = ERarity.Legendary;
                            break;
                        case "Epic":
                            theRarity = ERarity.Epic;
                            break;
                        default:
                            theRarity = ERarity.Unknown;
                            break;
                    }

                    theList.Add( theGuid, new Info()
                    {
                        Name = theName,
                        Type = theType,
                        Rarity = theRarity
                    } );

                }
            }
            catch{}

            return theList;
        }
    }
}
