using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Eneo.WindowsPhone.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NearestUsersPage : Page
    {
        private EneoWebApi _webApi;
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel;
        private Geoposition _position;
        private Geolocator _geolocator;
        private TokenResponse _tokenResponse;
        public ObservableCollection<NearestUser> NearestUsers;


        public NearestUsersPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            NearestUsers = new ObservableCollection<NearestUser>();
            _webApi = new EneoWebApi();
            defaultViewModel = new ObservableDictionary();
            _geolocator = new Geolocator();
            _geolocator.MovementThreshold = 5;
            _geolocator.DesiredAccuracyInMeters = 1;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            usersListView.DataContext = NearestUsers;
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _tokenResponse = e.NavigationParameter as TokenResponse;
            _position = await _geolocator.GetGeopositionAsync(TimeSpan.FromDays(30), TimeSpan.FromSeconds(1));
            ShowNearestUsers();
        }

        private async Task ShowNearestUsers()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Wyszukiwanie użytkowników w pobliżu...";
            statusbar.ProgressIndicator.ShowAsync();

            double myLatitude = _position.Coordinate.Latitude;
            double myLongitude = _position.Coordinate.Longitude;

            List<NearestUser> users = await _webApi.GetNearestUsers(_tokenResponse.Access_Token, myLatitude, myLongitude);
            foreach (var user in users)
            {
                NearestUsers.Add(user);
            }

            //foreach (var user in nearestUsers)
            //{
            //    TextBlock userBlock = new TextBlock
            //    {
            //        Text = user.userName + "       " + Math.Round(user.distance, 1) + " km",
            //        FontSize = 24
            //    };
            //    userBlock.Tapped +=  new TappedEventHandler((sender, e) => UserTapped(sender, e, user.userID));
            //    usersListView.Items.Add(userBlock);
            //}
            statusbar.ProgressIndicator.HideAsync();

        }

        private async void UserTapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            if (element.DataContext != null && element.DataContext is NearestUser)
            {
                NearestUser user = (NearestUser)element.DataContext;
                // rest of the code
                //string userName = (string)(sender as ListViewItem).Tag;
                //string userID = NearestUsers.Single(u => u.userName == userName).userID;
                Frame.Navigate(typeof(ProfilePage), new Tuple<TokenResponse, string>(_tokenResponse, user.userID));
            }
     
        }

        #region  Menu
        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

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

        #endregion

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            NearestUsers.Clear();
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

    }
}
