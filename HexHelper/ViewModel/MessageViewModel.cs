using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.Service;
using HexHelper.Service;

namespace HexHelper.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel( IHexApiService aHexService, IFileService aFileService, IDialogService aDialogs )
        {
            mHexApi = aHexService;
            mFile = aFileService;
            mDialogs = aDialogs;

            Messages = new ObservableCollection<IMessage>();

            mHexApi.MessageReceived += HandleMessageReceived;

            ShowMessageCommand = new RelayCommand<IMessage>( ShowMessage );
            PickMessageCommand = new RelayCommand( PickMessage );
        }

        private void HandleMessageReceived( object sender, IMessage e )
        {
            DispatcherHelper.CheckBeginInvokeOnUI( () => Messages.Add( e ) );
        }

        private void ShowMessage( IMessage aMessage )
        {
            if( aMessage.SourceFile != null )
            {
                mFile.OpenByOS( aMessage.SourceFile );
            }
        }

        private async void PickMessage()
        {
            var theFilePath = mDialogs.ShowFileOpenDialog( "Pick a Hex API json message", "*.json" );
            if( String.IsNullOrWhiteSpace( theFilePath ) )
            {
                return;
            }

            var theMessageText = await mFile.LoadFile( theFilePath );
            var theMessage = mHexApi.HandleMessageFromFile( theMessageText, mFile.LastWriteTime( theFilePath ) );
            Messages.Add( theMessage );
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
        public RelayCommand PickMessageCommand { get; private set; }

        private readonly IHexApiService mHexApi;
        private readonly IFileService mFile;
        private readonly IDialogService mDialogs;
    }
}
