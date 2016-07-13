using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class RankingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private TokenResponse _tokenResponse;
        private EneoWebApi webApi;
        private List<RankItem> rankList = new List<RankItem>();

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

        public RankingPage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            webApi = new EneoWebApi();
        }


        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void CreateRanking()
        {
            rankStackPanel.Children.Clear();
            var statusbar = StatusBar.GetForCurrentView();
            waitIndicator.Visibility = Visibility.Visible;
            waitIndicator.IsActive = true;
            try
            {
                rankList = await GetRank();
                List<RankItem> sortedRankList = rankList.OrderByDescending(o => o.collectedItemsCount).ToList();

                int position = 1;
                foreach (var item in sortedRankList)
                {
                    AddRankItem(position, item.userName, item.collectedItemsCount.ToString(), item.userID);
                    position++;
                }
                waitIndicator.Visibility = Visibility.Collapsed;
                waitIndicator.IsActive = false;
                rankStackPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void AddRankItem(int position, string userName, string collectedItems, string userID)
        {
            Grid grid = new Grid();
            Grid.SetColumn(grid, 1);
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { });

            TextBlock tbPosition = new TextBlock();
            tbPosition.Text = position.ToString();
            tbPosition.FontSize = 30;
            tbPosition.HorizontalAlignment = HorizontalAlignment.Left;
            tbPosition.VerticalAlignment = VerticalAlignment.Center;

            TextBlock tbUserName = new TextBlock();
            tbUserName.Text = userName;
            tbUserName.FontSize = 20;
            tbUserName.HorizontalAlignment = HorizontalAlignment.Center;
            tbUserName.VerticalAlignment = VerticalAlignment.Center;

            TextBlock tbCollectedItems = new TextBlock();
            tbCollectedItems.Text = collectedItems;
            tbCollectedItems.FontSize = 30;
            tbCollectedItems.HorizontalAlignment = HorizontalAlignment.Right;
            tbCollectedItems.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetColumn(tbPosition, 0);
            //Grid.SetRow(tbPosition, 0);
            grid.Children.Add(tbPosition);
            Grid.SetColumn(tbUserName, 1);
            //Grid.SetRow(tbPosition, 0);
            grid.Children.Add(tbUserName);
            Grid.SetColumn(tbCollectedItems, 2);
            //Grid.SetRow(tbPosition, 0);
            grid.Children.Add(tbCollectedItems);

            grid.Tapped += new TappedEventHandler((sender, e) => UserTapped(sender, e, userID));

            rankStackPanel.Children.Add(grid);
        }

        private async void UserTapped(object sender, TappedRoutedEventArgs e, string userID)
        {
            Frame.Navigate(typeof(ProfilePage), new Tuple<TokenResponse,string>(_tokenResponse, userID));
        }

        private async Task<List<RankItem>> GetRank()
        {
            EneoWebApi webApi = new EneoWebApi();
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Pobieranie danych...";
            await statusbar.ProgressIndicator.ShowAsync();
            List<RankItem> rank = await webApi.GetUsersRanking(_tokenResponse.Access_Token);
            await statusbar.ProgressIndicator.HideAsync();
            return rank;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _tokenResponse = e.NavigationParameter as TokenResponse;
            CreateRanking();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        #region Menu
        private void btnRankingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RankingPage), _tokenResponse);
        }

        private void btnMapPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapPage), _tokenResponse);
        }

        private void btnUserProfile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), _tokenResponse);
        }

        private void btnNearestItemsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestItemsPage), _tokenResponse);
        }
        private void btnNearestUsersPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestUsersPage), _tokenResponse);
        }

        #endregion
    }
}