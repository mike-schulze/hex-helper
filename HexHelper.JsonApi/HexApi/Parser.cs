using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public static class Parser
    {
        public static IMessage ParseMessage( string aMessageString, bool? aLogToFile )
        {
            MessageType theType = MessageType.Unknown;
            try
            {
                var theJson = JObject.Parse( aMessageString );
                theType = TypeFromString( ( string ) theJson["Message"] );

                switch( theType )
                {
                    case MessageType.Collection:
                        if( aLogToFile.HasValue )
                        {
                            return new CollectionMessage( theJson, aLogToFile.Value );
                        }
                        else
                        {
                            return new CollectionMessage( theJson );
                        }
                        
                    default:
                        break;
                }
            }
            catch
            {

            }

            if( aLogToFile.HasValue )
            {
                return new GenericMessage( theType, aLogToFile.Value );
            }

            return new GenericMessage( theType );
        }

        private static MessageType TypeFromString( string aMessageTypeString )
        {
            MessageType theType;
            if( Enum.TryParse( aMessageTypeString, out theType ) )
            {
                return theType;
            }

            Debug.WriteLine( String.Format( "Unknown message type: {0}", aMessageTypeString ) );
            return MessageType.Unknown;
        }
    }
}
