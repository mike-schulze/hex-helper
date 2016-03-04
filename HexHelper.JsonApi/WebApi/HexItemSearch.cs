using System;
using System.Collections.Generic;
using HexHelper.Hex;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.WebApi
{
    public static class HexItemSearch
    {
        public static IEnumerable<HexItemIdentifier> ParseJson( string aJson )
        {
            var theList = new List<HexItemIdentifier>();

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

                    HexItemType theType = HexItemType.Unknown;
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
                                theType = HexItemType.Card;
                                break;
                            case "Champion":
                                theType = HexItemType.Champion;
                                break;
                            case "Equipment":
                                theType = HexItemType.Equipment;
                                break;
                            case "Gem":
                                theType = HexItemType.Gem;
                                break;
                            case "Pack":
                                theType = HexItemType.Pack;
                                break;
                            case "Mod":
                                theType = HexItemType.Mod;
                                break;
                            case "Bane":
                                theType = HexItemType.Bane;
                                break;
                            default:
                                break;
                        }
                    }

                    theList.Add( new HexItemIdentifier()
                    {
                        Name = theName,
                        Id = theGuid,
                        Type = theType
                    } );

                }
            }
            catch{}

            return theList;
        }
    }
}
