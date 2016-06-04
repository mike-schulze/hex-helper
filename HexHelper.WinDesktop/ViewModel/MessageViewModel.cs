using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using HexHelper.Hex.Interface;
using HexHelper.JsonApi.HexApi;
using HexHelper.WinDesktop.Service;

namespace HexHelper.WinDesktop.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel( IHexApiService aHexService, IFileService aFileService )
        {
            mHexApi = aHexService;
            mFileService = aFileService;

            Messages = new ObservableCollection<IMessage>();

            mHexApi.MessageReceived += HandleMessageReceived;

            ShowMessageCommand = new RelayCommand<IMessage>( ShowMessage );
        }

        private void HandleMessageReceived( object sender, IMessage e )
        {
            DispatcherHelper.CheckBeginInvokeOnUI( () => Messages.Add( e ) );
        }

        private void ShowMessage( IMessage aMessage )
        {
            mFileService.OpenByOS( aMessage.SourceFile );
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

        public RelayCommand<IMessage> ShowMessageCommand { get; private set; }

        private readonly IHexApiService mHexApi;
        private readonly IFileService mFileService;
    }
}
