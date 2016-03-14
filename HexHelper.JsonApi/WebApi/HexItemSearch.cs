﻿using System;
using System.Collections.Generic;
using HexHelper.Hex;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.WebApi
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

                    ItemType theType = ItemType.Unknown;                    
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
                                theType = ItemType.Card;
                                break;
                            case "Champion":
                                theType = ItemType.Champion;
                                break;
                            case "Equipment":
                                theType = ItemType.Equipment;
                                break;
                            case "Gem":
                                theType = ItemType.Gem;
                                break;
                            case "Pack":
                                theType = ItemType.Pack;
                                break;
                            case "Mod":
                                theType = ItemType.Mod;
                                break;
                            case "Bane":
                                theType = ItemType.Bane;
                                break;
                            default:
                                break;
                        }
                    }

                    RarityType theRarity = RarityType.Unknown;
                    string theRarityString = ( string ) theItem["rarity"];
                    switch( theRarityString )
                    {
                        case "Promo":
                            theRarity = RarityType.Promo;
                            break;
                        case "Non-Collectible":
                            theRarity = RarityType.NonCollectible;
                            break;
                        case "Common":
                            theRarity = RarityType.Common;
                            break;
                        case "Uncommon":
                            theRarity = RarityType.Uncommon;
                            break;
                        case "Rare":
                            theRarity = RarityType.Rare;
                            break;
                        case "Legendary":
                            theRarity = RarityType.Legendary;
                            break;
                        case "Epic":
                            theRarity = RarityType.Epic;
                            break;
                        default:
                            theRarity = RarityType.Unknown;
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
