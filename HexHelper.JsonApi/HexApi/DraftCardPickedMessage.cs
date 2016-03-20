using System;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public class DraftCardPickedMessage : IMessage
    {
        public DraftCardPickedMessage( JObject aJson )
        {
            Parse( aJson );
        }

        public DateTime Date { get; private set; } = DateTime.Now;

        public string Summary { get; private set; }

        private void Parse( JObject aJson )
        {
        }

        public bool LogToFile { get; private set; } = true;
        public MessageType Type { get; private set; } = MessageType.DraftCardPicked;
    }
}
