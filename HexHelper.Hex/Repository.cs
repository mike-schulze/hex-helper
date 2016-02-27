using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex.Interface;
using Newtonsoft.Json;

namespace HexHelper.Hex
{
    public class Repository : IRepository
    {
        public Repository( IFileService aFileService )
        {
            mFileService = aFileService;
            mDb = new Dictionary<Guid, Card>();
        }

        public IEnumerable<Card> AllCards()
        {
            return mDb.Values;
        }

        public async Task Initialize()
        {
            string theFileContent = await mFileService.LoadFile( "Database", "db.json" );
            if( theFileContent == null )
            {
                return;
            }

            var theDatabase = JsonConvert.DeserializeObject<Dictionary<Guid, Card>>( theFileContent );
            if( theDatabase != null )
            {
                mDb = theDatabase;
            }
        }

        public async Task Persist()
        {
            await mFileService.SaveFile( "Database", "db.json", JsonConvert.SerializeObject( mDb ) );
        }

        public void UpdateInventory( IEnumerable<ObjectCount> aCollection, IEnumerable<ObjectCount> aAdded, IEnumerable<ObjectCount> aRemoved )
        {
            foreach( var theCount in aCollection )
            {
                if( !mDb.ContainsKey( theCount.Id ) )
                {
                    Debug.WriteLine( String.Format( "Could not update card count for ID# {0}.", theCount.Id ) );
                    continue;
                }

                Card theUpdatedCard = mDb[theCount.Id];
                theUpdatedCard.CopiesOwned = theCount.Count;
                mDb[theCount.Id] = theUpdatedCard;
            }

            foreach( var theCount in aAdded )
            {
                if( !mDb.ContainsKey( theCount.Id ) )
                {
                    Debug.WriteLine( String.Format( "Could not update card count for ID# {0}.", theCount.Id ) );
                    continue;
                }

                Card theUpdatedCard = mDb[theCount.Id];
                theUpdatedCard.CopiesOwned = theUpdatedCard.CopiesOwned + theCount.Count;
                mDb[theCount.Id] = theUpdatedCard;
            }

            foreach( var theCount in aRemoved )
            {
                if( !mDb.ContainsKey( theCount.Id ) )
                {
                    Debug.WriteLine( String.Format( "Could not update card count for ID# {0}.", theCount.Id ) );
                    continue;
                }

                Card theUpdatedCard = mDb[theCount.Id];
                theUpdatedCard.CopiesOwned = theUpdatedCard.CopiesOwned - theCount.Count;
                mDb[theCount.Id] = theUpdatedCard;
            }
        }

        public void UpdatePrices( IEnumerable<Card> aCards )
        {
            foreach( var theCard in aCards )
            {
                if( !mDb.ContainsKey( theCard.Id ) )
                {
                    mDb.Add( theCard.Id, theCard );
                    continue;
                }

                Card theUpdatedCard = mDb[theCard.Id];
                theUpdatedCard.PriceGold = theCard.PriceGold;
                theUpdatedCard.PricePlatinum = theCard.PricePlatinum;
                theUpdatedCard.SalesGold = theCard.SalesGold;
                theUpdatedCard.SalesPlatinum = theCard.SalesPlatinum;
                mDb[theCard.Id] = theUpdatedCard;
            }
        }

        private readonly IFileService mFileService;
        private Dictionary<Guid, Card> mDb;
    }
}
