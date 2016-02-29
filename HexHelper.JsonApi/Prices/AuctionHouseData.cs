using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HexHelper.Hex;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.Prices
{
    public static class AuctionHouseData
    {
        public static async Task<string> DownloadPricelist()
        {
            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    return await theHttpClient.GetStringAsync( scPriceListUrl );
                }
            }
            catch
            {
            }

            return null;
        }

        public static IEnumerable<Card> ParseJson( string aJson )
        {
            var theList = new List<Card>();

            try
            {
                var theJson = JObject.Parse( aJson );

                foreach( var theCard in theJson["cards"] )
                {
                    string theName = ( string ) theCard["name"];
                    if( String.IsNullOrWhiteSpace( theName ) )
                    {
                        continue;
                    }

                    Guid theGuid;
                    if( !Guid.TryParse( ( string ) theCard["uuid"], out theGuid ) )
                    {
                        continue;
                    }

                    theList.Add( new Card()
                    {
                        Name = theName,
                        Id = theGuid,
                        PricePlatinum = ( int ) theCard["PLATINUM"]["avg"],
                        SalesPlatinum = ( int ) theCard["PLATINUM"]["sample_size"],
                        PriceGold = ( int ) theCard["GOLD"]["avg"],
                        SalesGold = ( int ) theCard["GOLD"]["sample_size"]
                    } );

                }
            }
            catch
            {
            }

            return theList;
        }

        private const string scPriceListUrl = "http://doc-x.net/hex/all_prices_json.txt";
    }
}
