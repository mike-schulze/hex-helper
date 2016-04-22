using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexHelper.Hex;
using HexHelper.JsonApi.HexApi;

namespace HexHelper.WinDesktop.Service
{
    public interface IHexApiService
    {
        Task Initialize();
        Task HandleMessage( string aMessageString );
        Task Shutdown();

        IEnumerable<ItemViewModel> GetCards();
        IEnumerable<User> GetUsers();
        User GetCurrentUser();
        void SetCurrentUser( User aUserName );

        event EventHandler CollectionChanged;
        event EventHandler<IMessage> MessageReceived;
        event EventHandler<string> StatusChanged;
        event EventHandler<User> UserChanged;
    }
}
