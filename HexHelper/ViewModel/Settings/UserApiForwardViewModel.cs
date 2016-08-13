using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.Model;
using HexHelper.Libs.WebApiForward;
using HexHelper.Service;
using static HexHelper.Libs.Model.User;

namespace HexHelper.ViewModel.Settings
{
    public sealed class UserApiForwardViewModel : ViewModelBase
    {
        public UserApiForwardViewModel( IHexApiService aHexApi, User aUser, ApiSite aApiSite )
        {
            mHexApi = aHexApi;
            mUser = aUser;
            mApiSite = aApiSite;

            mMessagesToForward = new ObservableCollection<EMessageType>( aApiSite.SupportedMessages );
            mMessagesToIgnore = new ObservableCollection<EMessageType>();

            foreach( var theUserApiForward in aUser.Forwards )
            {
                if( theUserApiForward.Type == aApiSite.Type )
                {
                    mForwardToThis = theUserApiForward.ForwardToThis;
                    mApiKey = theUserApiForward.ApiKey;

                    foreach( var theMessageType in aApiSite.SupportedMessages )
                    {
                        if( !theUserApiForward.Messages.Contains( theMessageType ) )
                        {
                            mMessagesToIgnore.Add( theMessageType );
                            mMessagesToForward.Remove( theMessageType );
                        }
                    }
                }
            }

            SendToRightCommand = new RelayCommand<EMessageType>( SendToRight );
            SendToLeftCommand = new RelayCommand<EMessageType>( SendToLeft );
            SendAllToRightCommand = new RelayCommand( SendAllToRight, () => MessagesToForward.Any() );
            SendAllToLeftCommand = new RelayCommand( SendAllToLeft, () => MessagesToIgnore.Any() );
        }

        private void UpdateUser()
        {
            UserApiForward theNewForward = new UserApiForward()
            {
                Type = mApiSite.Type,
                ApiKey = this.ApiKey,
                ForwardToThis = this.ForwardToThis,
                Messages = MessagesToForward.ToList()
            };

            UserApiForward theOldForward = null;
            foreach( var theUserApiForward in mUser.Forwards )
            {
                if( theUserApiForward.Type == theNewForward.Type )
                {
                    theOldForward = theUserApiForward;
                }
            }

            if( theOldForward != null )
            {
                mUser.Forwards.Remove( theOldForward );
            }

            mUser.Forwards.Add( theNewForward );
            mHexApi.UpdateUser( mUser );

            SendAllToLeftCommand.RaiseCanExecuteChanged();
            SendAllToRightCommand.RaiseCanExecuteChanged();
        }

        private void
        SendToRight
            (
            EMessageType aType
            )
        {
            if( aType == EMessageType.Unknown )
            {
                return;
            }

            MessagesToForward.Remove( aType );
            MessagesToIgnore.Add( aType );

            UpdateUser();
        }

        private void
        SendToLeft
            (
            EMessageType aType
            )
        {
            if( aType == EMessageType.Unknown )
            {
                return;
            }

            MessagesToForward.Add( aType );
            MessagesToIgnore.Remove( aType );

            UpdateUser();
        }

        private void
        SendAllToRight()
        {
            foreach( var theType in MessagesToForward )
            {
                MessagesToIgnore.Add( theType );
            }
            MessagesToForward.Clear();

            UpdateUser();
        }

        private void
        SendAllToLeft()
        {
            foreach( var theType in MessagesToIgnore )
            {
                MessagesToForward.Add( theType );
            }
            MessagesToIgnore.Clear();

            UpdateUser();
        }

        public bool ForwardToThis
        {
            get
            {
                return mForwardToThis;
            }
            set
            {
                if( Set( nameof( ForwardToThis ), ref mForwardToThis, value ) )
                {
                    UpdateUser();
                }
            }
        }
        private bool mForwardToThis = false;

        public string ForwardText
        {
            get
            {
                return String.Format( "Forward messages to {0}", mApiSite.Name );
            }

        }

        public bool ApiKeyRequired
        {
            get
            {
                return mApiSite.RequiresKey;
            }
        }

        public ObservableCollection<EMessageType> MessagesToForward
        {
            get
            {
                return mMessagesToForward;
            }
        }
        private readonly ObservableCollection<EMessageType> mMessagesToForward = null;

        public ObservableCollection<EMessageType> MessagesToIgnore
        {
            get
            {
                return mMessagesToIgnore;
            }
        }
        private readonly ObservableCollection<EMessageType> mMessagesToIgnore = null;


        public string ApiKey
        {
            get
            {
                return mApiKey;
            }
            set
            {
                if( Set( nameof( ApiKey ), ref mApiKey, value ) )
                {
                    UpdateUser();
                }
            }
        }
        private string mApiKey = null;

        public RelayCommand<EMessageType> SendToRightCommand { get; private set; }

        public RelayCommand<EMessageType> SendToLeftCommand { get; private set; }

        public RelayCommand SendAllToRightCommand { get; private set; }

        public RelayCommand SendAllToLeftCommand { get; private set; }

        private readonly IHexApiService mHexApi;
        private readonly User mUser;
        private readonly ApiSite mApiSite;
    }
}
