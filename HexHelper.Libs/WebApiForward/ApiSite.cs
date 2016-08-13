using System.Collections.Generic;
using HexHelper.Libs.HexApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HexHelper.Libs.WebApiForward
{

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum EApiSites
    {
        HexTcgBrowser,
        HexMeta,
        ShareYourDraft
    }

    public sealed class ApiSite
    {
        public ApiSite( bool aRequiresKey, IList<EMessageType> aSupportedMessages, EApiSites aType, string aUri)
        {
            RequiresKey = aRequiresKey;
            SupportedMessages = aSupportedMessages;
            Type = aType;
            Uri = aUri;
            Name = Type.ToString();
        }

        public bool RequiresKey { get; private set; }

        public IList<EMessageType> SupportedMessages { get; private set; }

        public EApiSites Type { get; private set; }

        public string Uri { get; private set; }

        public string Name {  get; private set; }
    }
}
