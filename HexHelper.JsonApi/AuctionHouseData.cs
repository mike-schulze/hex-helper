using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi
{
    public static class AuctionHouseData
    {
        public static async Task<IEnumerable<Card>> RetrievePriceList()
        {
            var theList = new List<Card>();

            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    var theJsonString = await theHttpClient.GetStringAsync( scPriceListUrl );
                    var theJson = JObject.Parse( theJsonString );

                    foreach( var theCard in theJson["cards"] )
                    {
                        string theName = (string) theCard["name"];
                        if( String.IsNullOrWhiteSpace( theName ) )
                        {
                            continue;
                        }

                        Guid theGuid;
                        if( !Guid.TryParse( (string ) theCard["uuid"], out theGuid ) )
                        {
                            continue;
                        }

                        theList.Add( new Card() {
                            Name = theName,
                            Id = theGuid,
                            PricePlatinum = ( int) theCard["PLATINUM"]["avg"],
                            SalesPlatinum = ( int ) theCard["PLATINUM"]["sample_size"],
                            PriceGold = ( int ) theCard["GOLD"]["avg"],
                            SalesGold = ( int ) theCard["GOLD"]["sample_size"]
                        } );

                    }
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
