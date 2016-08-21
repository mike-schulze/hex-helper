using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using HexHelper.Libs.Model;
using HexHelper.Libs.Service;
using HexHelper.Libs.WebApiForward;

namespace HexHelper.ViewModel.Settings
{
    public sealed class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel( IHexApiService aHexApi )
        {
            mHexApi = aHexApi;

            mHexApi.InitializationCompleted += HandleInitializationCompleted;
        }

        private void HandleInitializationCompleted( object sender, EventArgs e )
        {
            CurrentUser = mHexApi.GetCurrentUser();
            AllUsers = mHexApi.GetUsers();
            mHexApi.UserChanged += HandleUserChanged;
        }

        private void HandleUserChanged( object sender, User e )
        {
            CurrentUser = e;
            AllUsers = mHexApi.GetUsers();
        }

        private void UpdateForward()
        {
            if( CurrentUser != null && CurrentSite != null )    
            {
                ApiForward = new UserApiForwardViewModel( mHexApi, CurrentUser, CurrentSite );
            }        
        }

        public User CurrentUser
        {
            get
            {
                return mCurrentUser;
            }
            set
            {
                if( Set( nameof( CurrentUser ), ref mCurrentUser, value ) )
                {
                    if( value == null )
                    {
                        Sites = null;
                    }
                    else
                    {
                        Sites = Forwarder.AllHexSites().Values;
                        mHexApi.SetCurrentUser( value.UserName );
                        UpdateForward();
                    }
                }
            }
        }
        private User mCurrentUser;

        public IEnumerable<User> AllUsers
        {
            get
            {
                return mAllUsers;
            }
            set
            {
                Set( nameof( AllUsers ), ref mAllUsers, value );
            }
        }
        private IEnumerable<User> mAllUsers = null;

        public ApiSite CurrentSite
        {
            get
            {
                return mCurrentSite;
            }
            set
            {
                if( Set( nameof( CurrentSite ), ref mCurrentSite, value ) )
                {
                    UpdateForward();
                }
            }
        }
        private ApiSite mCurrentSite = null;

        public IEnumerable<ApiSite> Sites
        {
            get
            {
                return mSites;
            }
            set
            {
                Set( nameof( Sites ), ref mSites, value );
            }
        }
        private IEnumerable<ApiSite> mSites = null;


        public UserApiForwardViewModel ApiForward
        {
            get
            {
                return mApiForward;
            }
            set
            {
                Set( nameof( ApiForward ), ref mApiForward, value );
            }
        }
        private UserApiForwardViewModel mApiForward = null;


        private readonly IHexApiService mHexApi;
    }
}
