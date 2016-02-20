using System.Windows;

namespace HexHelper.WinDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void HandleStartup( object sender, StartupEventArgs e )
        {
            Current.DispatcherUnhandledException += HandleUnhandledException;
        }

        private void HandleUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
        {
            e.Handled = true;

            string theErrorMessage = e.Exception.Message + ( e.Exception.InnerException != null ? "\n" + e.Exception.InnerException.Message : null );
            MessageBox.Show( theErrorMessage, "An error occurred" );

            Application.Current.Shutdown();
        }
    }
}
