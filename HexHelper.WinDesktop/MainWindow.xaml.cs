using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
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
            // need to register ourselves as default MetroWindow for dialogs
            SimpleIoc.Default.Register<MetroWindow>( () => this );

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
