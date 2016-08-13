using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.Model;
using static HexHelper.Libs.Model.User;

namespace HexHelper.Libs.WebApiForward
{
    public static class Forwarder
    {
        public static Dictionary<EApiSites, ApiSite> AllHexSites()
        {
            if( mAllHexSites == null )
            {
                mAllHexSites = new Dictionary<EApiSites, ApiSite>();

                mAllHexSites.Add( EApiSites.HexTcgBrowser, new ApiSite(
                    aRequiresKey: true,
                    aSupportedMessages: new List<EMessageType>() { EMessageType.Collection, EMessageType.DraftCardPicked, EMessageType.DraftPack, EMessageType.Inventory, EMessageType.SaveDeck },
                    aType: EApiSites.HexTcgBrowser,
                    aUri: "http://hex.tcgbrowser.com:8080/sync?{0}" ) );

                mAllHexSites.Add( EApiSites.HexMeta, new ApiSite(
                    aRequiresKey: false,
                    aSupportedMessages: new List<EMessageType>() { EMessageType.Tournament },
                    aType: EApiSites.HexMeta,
                    aUri: "http://hexmeta.com:4000/sync" ) );

                mAllHexSites.Add( EApiSites.ShareYourDraft, new ApiSite(
                    aRequiresKey: true,
                    aSupportedMessages: new List<EMessageType>() { EMessageType.Login, EMessageType.DraftPack, EMessageType.DraftCardPicked, EMessageType.Collection, EMessageType.Tournament, EMessageType.SaveDeck },
                    aType: EApiSites.ShareYourDraft,
                    aUri: "http://shareyourdraft.com/api/draft/{0}" ) );
            }
            return mAllHexSites;
        }
        private static Dictionary<EApiSites, ApiSite> mAllHexSites;

        /// <summary>
        /// Forwards message to all supported sites.
        /// </summary>
        /// <param name="aMessage"></param>
        /// <param name="aMessageString"></param>
        /// <param name="aUser"></param>
        /// <returns></returns>
        public static async Task ForwardMessage( IMessage aMessage, string aMessageString, User aUser )
        {
            if( aMessage == null || aUser == null || aMessageString == null )
            {
                return;
            }

            foreach( var theSite in AllHexSites() )
            {
                if( theSite.Value.SupportedMessages.Contains( aMessage.Type ) )
                {
                    UserApiForward theApiForward = null;
                    foreach( var theForward in aUser.Forwards )
                    {
                        if( theForward.Type == theSite.Value.Type )
                        {
                            theApiForward = theForward;
                        }
                    }

                    if( theApiForward != null && theApiForward.Messages.Contains( aMessage.Type) )
                    {
                        await ForwardToSite( theSite.Value, theApiForward, aMessageString );
                    }
                }
            }
        }

        /// <summary>
        /// Forwards to specific site
        /// </summary>
        /// <param name="aSite"></param>
        /// <param name="aUserForward"></param>
        /// <param name="aMessageString"></param>
        /// <returns></returns>
        private static async Task<bool> ForwardToSite( ApiSite aSite, UserApiForward aUserForward, string aMessageString )
        {
            var theUri = aSite.Uri;
            if( aSite.RequiresKey )
            {
                if( String.IsNullOrWhiteSpace( aUserForward.ApiKey ) )
                {
                    return false;
                }

                theUri = String.Format( theUri, aUserForward.ApiKey );
            }

            return await PostMessage( theUri, aMessageString );
        }

        /// <summary>
        /// Posts message to given Uri
        /// </summary>
        /// <param name="aUri"></param>
        /// <param name="aMessageString"></param>
        /// <returns></returns>
        private static async Task<bool> PostMessage( string aUri, string aMessageString )
        {
            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    await theHttpClient.PostAsync( aUri, new StringContent( aMessageString, Encoding.UTF8, "application/json" ) );
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
