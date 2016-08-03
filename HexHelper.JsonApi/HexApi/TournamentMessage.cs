using System;
using System.Collections.Generic;
using System.Linq;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class TournamentMessage : MessageBase
    {
        public enum EFormats
        {
            Constructed = 0,
            Sealed = 9,
            Draft = 10,
            Unknown = 99
        }

        public enum EStyles
        {
            SingleElimination = 0,
            Swiss = 1,
            Evolving = 4,
            Ladder = 8,
            Unknown = 99
        }

        public class Game
        {
            public Game( string aPlayerOne, string aPlayerTwo, string aWinnerOne, string aWinnerTwo, string aWinnerThree )
            {
                PlayerOne = aPlayerOne;
                PlayerTwo = aPlayerTwo;
                WinnerOne = aWinnerOne;
                WinnerTwo = aWinnerTwo;
                WinnerThree = aWinnerThree;
            }

            public string PlayerOne { get; private set; }
            public string PlayerTwo { get; private set; }
            public string WinnerOne { get; private set; }
            public string WinnerTwo { get; private set; }
            public string WinnerThree { get; private set; }
        }

        public class Standing
        {
            public Standing( string aPlayer, int aGameWins, int aGameLosses, int aMatchWins )
            {
                Player = aPlayer;
                GameWins = aGameWins;
                GameLosses = aGameLosses;
                MatchWins = aMatchWins;
            }

            public string Player { get; private set; }
            public int GameWins { get; private set; }
            public int GameLosses { get; private set; }
            public int MatchWins { get; private set; }
        }

        public TournamentMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.Tournament, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var theData = aJson["TournamentData"];

            Id = ( string ) theData["ID"];

            NextEventTime = DateTime.Parse( ( string ) theData["NextEventTime"] );

            int theStyle = ( int ) theData["Style"];
            if( Enum.IsDefined( typeof( EStyles ), theStyle ) )
            {
                Style = ( EStyles ) theStyle;
            }
            else
            {
                Style = EStyles.Unknown;
            }

            int theFormat = ( int ) theData["Format"];
            if( Enum.IsDefined( typeof( EFormats ), theFormat ) )
            {
                Format = ( EFormats ) theFormat;
            }
            else
            {
                Format = EFormats.Unknown;
            }

            var theGames = theData["Games"];
            var theGamesList = new List<Game>();
            foreach( var theGame in theGames )
            {
                var thePlayerOne = ( string ) theGame["PlayerOne"];
                var thePlayerTwo = ( string ) theGame["PlayerTwo"];
                var theGameOne = ( string ) theGame["GameOneWinner"];
                var theGameTwo = ( string ) theGame["GameTwoWinner"];
                var theGameThree = ( string ) theGame["GameThreeWinner"];

                theGamesList.Add( new Game( thePlayerOne, thePlayerTwo, theGameOne, theGameTwo, theGameThree ) );
            }
            Games = theGamesList;

            var theStandings = theData["Players"];
            var theStandingsList = new List<Standing>();
            foreach( var theStanding in theStandings )
            {
                var thePlayer = ( string ) theStanding["Name"];
                var theGameWins = ( int ) theStanding["Wins"];
                var theGameLosses = ( int ) theStanding["Losses"];
                var theMatchWins = ( int ) theStanding["Points"];

                theStandingsList.Add( new Standing( thePlayer, theGameWins, theGameLosses, theMatchWins ) );

                if( thePlayer == User )
                {
                    UserStanding = theStandingsList.Last();
                }
            }
        }

        protected override void CreateSummary()
        {
            Summary = String.Format( "Tournament: {0} {1}", Format.ToString(), Style.ToString() );

            if( UserStanding != null )
            {
                Summary += String.Format( " ({0} Wins)", UserStanding.MatchWins );
            }
        }

        public string Id { get; private set; }
        public DateTime NextEventTime { get; private set; }
        public EFormats Format { get; private set; }
        public EStyles Style { get; private set; }
        public IEnumerable<Game> Games{ get; private set; }
        public IEnumerable<Standing> Standings { get; private set; }
        public Standing UserStanding { get; private set; }
    }
}
