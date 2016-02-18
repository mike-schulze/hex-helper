using GalaSoft.MvvmLight;

namespace HexHelper.WinDesktop.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if( IsInDesignMode )
            {
                WelcomeMessage = "Hello Designer.";
            }
            else
            {
                WelcomeMessage = "Hello World.";
            }
        }

        public string WelcomeMessage { get; private set; }
    }
}