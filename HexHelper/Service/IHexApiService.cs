using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.Model;

namespace HexHelper.Service
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
        Task Shutdown();

        IEnumerable<ItemViewModel> GetCards();
        IEnumerable<ItemViewModel> GetInventory();
        IEnumerable<User> GetUsers();
        User GetCurrentUser();
        void SetCurrentUser( string aUserName );

        event EventHandler CollectionChanged;
        event EventHandler<IMessage> MessageReceived;
        event EventHandler<string> StatusChanged;
        event EventHandler<User> UserChanged;
    }
}
