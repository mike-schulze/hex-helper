using System.Collections.Generic;
using GalaSoft.MvvmLight;
using HexHelper.Libs.Model;
using HexHelper.Service;

namespace HexHelper.ViewModel
{
    public sealed class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel( IHexApiService aHexApi )
        {
            mHexApi = aHexApi;

            CurrentUser = mHexApi.GetCurrentUser();
            AllUsers = mHexApi.GetUsers();
            mHexApi.UserChanged += HandleUserChanged;
        }

        private void HandleUserChanged( object sender, User e )
        {
            CurrentUser = e;
            AllUsers = mHexApi.GetUsers();
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
                    mHexApi.SetCurrentUser( value.UserName );
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


        private readonly IHexApiService mHexApi;
    }
}
