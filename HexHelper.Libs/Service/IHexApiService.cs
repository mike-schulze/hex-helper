using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.Model;

namespace HexHelper.Libs.Service
{
    public interface IHexApiService
    {
        Task Initialize();

        /// <summary>
        /// Processes a message. Does not Store/Forward.
        /// </summary>
        /// <param name="aMessageString"></param>
        /// <returns></returns>
        IMessage HandleMessageFromFile( string aMessageString, DateTime aDateTime );

        event EventHandler<IMessage> MessageReceived;


        IEnumerable<ItemViewModel> GetCards();
        IEnumerable<ItemViewModel> GetInventory();

        event EventHandler CollectionChanged;


        IEnumerable<User> GetUsers();
        User GetCurrentUser();
        void SetCurrentUser( string aUserName );
        void UpdateUser( User aUser );

        event EventHandler<User> UserChanged;

        
        event EventHandler<string> StatusChanged;

        event EventHandler InitializationCompleted;

        Task Shutdown();
    }
}
