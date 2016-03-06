using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HexHelper.Hex;

namespace HexHelper.WinDesktop.View
{
    /// <summary>
    /// Interaction logic for CardListControl.xaml
    /// </summary>
    public partial class CardListControl : UserControl
    {
        public CardListControl()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------------------------
        public IList<Item> Cards
        {
            get
            {
                return ( IList<Item> ) GetValue( CardsProperty );
            }
            set
            {
                SetValue( CardsProperty, value );
            }
        }
        //--------------------------------------------------------------------------
        public static readonly DependencyProperty CardsProperty = DependencyProperty.Register(
            "Cards",
            typeof( IList<Item> ),
            typeof( CardListControl ),
            new PropertyMetadata( OnCardsChanged ) );

        //--------------------------------------------------------------------------
        private static void
        OnCardsChanged
            (
            DependencyObject aObject,
            DependencyPropertyChangedEventArgs aArgs
            )
        {
            var theControl = aObject as CardListControl;
            if( theControl != null )
            {
                theControl.OnCardsChanged( aArgs );
            }
        }

        //--------------------------------------------------------------------------
        private void
        OnCardsChanged
            (
            DependencyPropertyChangedEventArgs aArgs
            )
        {
            mList.ItemsSource = (IList) aArgs.NewValue;
        }

        private void HandlePlatClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Card.PricePlatinum ) );
        }

        private void HandleGoldClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Card.PriceGold ) );
        }

        private void HandlePlatSalesClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Card.SalesPlatinum ) );
        }

        private void HandleGoldSalesClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Card.SalesGold ) );
        }

        private void HandleNameClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Info.Name ) );
        }

        private void HandleOwnedClick( object sender, RoutedEventArgs e )
        {
            //SortList( nameof( Card.CopiesOwned ) );
        }

        private void SortList( string aPropertyName )
        {
            if( mList.ItemsSource == null )
            {
                return;
            }

            var theView = ( ListCollectionView ) CollectionViewSource.GetDefaultView( mList.ItemsSource );
            theView.CustomSort = new PropertySorter<Info>( aPropertyName, mIsAscending );
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
