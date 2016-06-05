using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.JsonApi.HexApi;

namespace HexHelper.JsonApi.WebApiForward
{
    public enum ApiSiteType
    {
        Unknown,
        HexTcgBrowser,
        HexMeta,
        ShareYourDraft
    }

    public interface IApiSite
    {
        ApiSiteType Type { get; }        
        string Uri { get; }
        bool RequiresKey { get; }        
        IList<MessageType> SupportedMessages { get; }
    };

    public sealed class ApiSite : IApiSite
    {
        public ApiSite( bool aRequiresKey, IList<MessageType> aSupportedMessages, ApiSiteType aType, string aUri)
        {
            RequiresKey = aRequiresKey;
            SupportedMessages = aSupportedMessages;
            Type = aType;
            Uri = aUri;
        }

        public bool RequiresKey { get; private set; }

        public IList<MessageType> SupportedMessages { get; private set; }

        public ApiSiteType Type { get; private set; }

        public string Uri { get; private set; }
    }

    public static class Forwarder
    {
        public static Dictionary<ApiSiteType, IApiSite> AllHexSites()
        {
            if( mAllHexSites == null )
            {
                mAllHexSites = new Dictionary<ApiSiteType, IApiSite>();

                mAllHexSites.Add( ApiSiteType.HexTcgBrowser, new ApiSite(
                    aRequiresKey: true,
                    aSupportedMessages: new List<MessageType>() { MessageType.Collection, MessageType.DraftCardPicked, MessageType.DraftPack, MessageType.Inventory },
                    aType: ApiSiteType.HexTcgBrowser,
                    aUri: "http://hex.tcgbrowser.com:8080/sync?{0}" ) );

                mAllHexSites.Add( ApiSiteType.HexMeta, new ApiSite(
                    aRequiresKey: false,
                    aSupportedMessages: new List<MessageType>() { MessageType.Tournament },
                    aType: ApiSiteType.HexMeta,
                    aUri: "http://hexmeta.com:4000/sync" ) );

                mAllHexSites.Add( ApiSiteType.ShareYourDraft, new ApiSite(
                    aRequiresKey: true,
                    aSupportedMessages: new List<MessageType>() { MessageType.Login, MessageType.DraftPack, MessageType.DraftCardPicked },
                    aType: ApiSiteType.ShareYourDraft,
                    aUri: "http://shareyourdraft.com/api/{0}" ) );
            }
            return mAllHexSites;
        }
        private static Dictionary<ApiSiteType, IApiSite> mAllHexSites;

        /// <summary>
        /// Forwards a message to a given site.
        /// </summary>
        /// <param name="aSite"></param>
        /// <param name="aMessage"></param>
        /// <param name="aMessageString"></param>
        /// <param name="aUser"></param>
        /// <returns></returns>
        public static async Task ForwardMessage( IApiSite aSite, string aMessageString, User aUser )
        {
            if( aSite == null || aUser == null || aMessageString == null )
            {
                return;
            }

            var theEnumString = aSite.Type.ToString();
            string theKey;
            if( !aUser.ApiSites.TryGetValue( theEnumString, out theKey ) )
            {
                return;
            }

            var theUri = aSite.Uri;
            if( aSite.RequiresKey )
            {
                if( String.IsNullOrWhiteSpace( theKey ) )
                {
                    return;
                }

                theUri = String.Format( theUri, theKey );
            }

            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    await theHttpClient.PostAsync( theUri, new StringContent( aMessageString, Encoding.UTF8, "application/json" ) );
                }
            }
            catch
            {
            }
        }
    }
}
