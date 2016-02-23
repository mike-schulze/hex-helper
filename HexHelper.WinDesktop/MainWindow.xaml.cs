using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using HexHelper.WinDesktop.ViewModel;
using MahApps.Metro.Controls;

namespace HexHelper.WinDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += HandleLoaded;
            this.Closed += HandleWindowClosed;
        }

        private async void HandleLoaded( object sender, RoutedEventArgs e )
        {
            var theVM = DataContext as MainViewModel;
            
            if( theVM != null )
            {
                await theVM.Initialize();
            }
        }

        private async void HandleWindowClosed( object sender, EventArgs e )
        {
            await ViewModel.ViewModelLocator.Cleanup();
        }
    }
}
