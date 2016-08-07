using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HexHelper.Hex;

namespace HexHelper.View
{
    /// <summary>
    /// Interaction logic for HexItemsListControl.xaml
    /// </summary>
    public partial class HexItemsListControl : UserControl
    {
        public HexItemsListControl()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------------------------
        public IList<ItemViewModel> Items
        {
            get
            {
                return ( IList<ItemViewModel> ) GetValue( ItemsProperty );
            }
            set
            {
                SetValue( ItemsProperty, value );
            }
        }
        //--------------------------------------------------------------------------
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof( IList<ItemViewModel> ),
            typeof( HexItemsListControl ),
            new PropertyMetadata( OnItemsChanged ) );

        //--------------------------------------------------------------------------
        private static void
        OnItemsChanged
            (
            DependencyObject aObject,
            DependencyPropertyChangedEventArgs aArgs
            )
        {
            var theControl = aObject as HexItemsListControl;
            if( theControl != null )
            {
                theControl.OnItemsChanged( aArgs );
            }
        }

        //--------------------------------------------------------------------------
        private void
        OnItemsChanged
            (
            DependencyPropertyChangedEventArgs aArgs
            )
        {
            mList.ItemsSource = (IList) aArgs.NewValue;
            SetFilter();
        }

        private void HandlePlatClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.PricePlatinum ) );
        }

        private void HandleGoldClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.PriceGold ) );
        }

        private void HandlePlatSalesClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.SalesPlatinum ) );
        }

        private void HandleGoldSalesClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.SalesGold ) );
        }

        private void HandleNameClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.Name ) );
        }

        private void HandleOwnedClick( object sender, RoutedEventArgs e )
        {
            SortList( nameof( ItemViewModel.QuantityOwned ) );
        }

        private void HandleSearchKeyUp( object sender, KeyEventArgs e )
        {
            SetFilter();
        }

        private void SetFilter()
        {
            if( mList.ItemsSource == null )
            {
                return;
            }

            var theView = ( ListCollectionView ) CollectionViewSource.GetDefaultView( mList.ItemsSource );
            theView.Filter = NameMatches;
        }

        private bool NameMatches( object aObject )
        {
            ItemViewModel theItem = ( ItemViewModel ) aObject;
            if( theItem.Name.IndexOf( mSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase ) >= 0 )
            {
                return true;
            }

            return false;
        }

        private void SortList( string aPropertyName )
        {
            if( mList.ItemsSource == null )
            {
                return;
            }

            var theView = ( ListCollectionView ) CollectionViewSource.GetDefaultView( mList.ItemsSource );
            theView.CustomSort = new PropertySorter<ItemViewModel>( aPropertyName, mIsAscending );
            mIsAscending = !mIsAscending;
            mList.Items.Refresh();
        }

        private class PropertySorter<T> : IComparer
        {
            public PropertySorter( string aPropertyName, bool aIsAscending )
            {
                mIsAscending = aIsAscending;
                mPropertyName = aPropertyName;
        }
            private readonly bool mIsAscending;
            private readonly string mPropertyName;

            private IComparable ValueForPropertyName( T aObject )
            {
                return (IComparable) typeof( T ).GetProperty( mPropertyName ).GetValue( aObject, null );
            }

            public int Compare( object x, object y )
            {
                T theFirst = ( T ) x;
                T theSecond = ( T ) y;

                int theCompareResult = ValueForPropertyName( theFirst ).CompareTo( ValueForPropertyName( theSecond ) );
                if( mIsAscending )
                {
                    theCompareResult = theCompareResult * -1;
                }

                return theCompareResult;
            }
        };

        private bool mIsAscending = true;

    }
}
