using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public class CollectionMessage : IMessage
    {
        public CollectionMessage( JObject aJson, bool aLogToFile = true )
        {
            LogToFile = aLogToFile;
        }

        public bool LogToFile { get; private set; }

        public MessageType Type { get; private set; } = MessageType.Collection;
    }
}
