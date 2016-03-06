using System;
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

                    theList.Add( theGuid, new Info()
                    {
                        Name = theName,
                        Type = theType
                    } );

                }
            }
            catch{}

            return theList;
        }
    }
}
