using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using HexHelper.JsonApi.HexApi;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel( IHexApiService aHexService)
        {
            mHexApi = aHexService;

            Messages = new ObservableCollection<IMessage>();

            mHexApi.MessageReceived += HandleMessageReceived;
        }

        private void HandleMessageReceived( object sender, IMessage e )
        {
            DispatcherHelper.CheckBeginInvokeOnUI( () => Messages.Add( e ) );
        }

        public ObservableCollection<IMessage> Messages
        {
            get
            {
                return mMessages;
            }
            set
            {
                Set( nameof( Messages ), ref mMessages, value );
            }
        }
        private ObservableCollection<IMessage> mMessages;

        private readonly IHexApiService mHexApi;
    }
}
