using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class NearestItemsPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel;
        private TokenResponse _tokenResponse;
        private EneoWebApi _webApi;
        private Geolocator _geolocator;
        private Geoposition _position;
        private MapIcon myLocationIcon;
        private MapControl mMapControl;
        private List<PlacedItemModel> mapItems;

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

        public NearestItemsPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            _webApi = new EneoWebApi();
            defaultViewModel = new ObservableDictionary();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            // _geolocator = new Geolocator();
        }

        private async Task<List<PlacedItemModel>> GetAllPoints()
        {
            EneoWebApi webApi = new EneoWebApi();
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Pobieranie punktów...";
            statusbar.ProgressIndicator.ShowAsync();
            List<PlacedItemModel> locations = await webApi.GetItemsLocation(_tokenResponse.Access_Token);
            await statusbar.ProgressIndicator.HideAsync();
            return locations;
        }

        private static double CalculateDistance(double myLatitude, double myLongitude, double itemLatitude, double itemLongitude)
        {
            var myCoord = new GeoCoordinate(myLatitude, myLongitude);
            var itemCoord = new GeoCoordinate(itemLatitude, itemLongitude);

            var distance = Math.Round(myCoord.GetDistanceTo(itemCoord));

            return distance;
        }

        private async Task ShowNearestPoints()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Szukanie punktów...";
            statusbar.ProgressIndicator.ShowAsync();

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 3;

            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),
                timeout: TimeSpan.FromSeconds(10));

            double myLat = geoposition.Coordinate.Latitude;
            double myLon = geoposition.Coordinate.Longitude;

            listOfDistances.Items.Clear();
            mapItems = await GetAllPoints();
            List<DistanceItem> distances = new List<DistanceItem>();

            foreach (var item in mapItems)
            {
                double itemLat = Convert.ToDouble(item.Latitude);
                double itemLon = Convert.ToDouble(item.Longitude);

                DistanceItem dist = new DistanceItem
                {
                    Distance = CalculateDistance(myLat, myLon, itemLat, itemLon),
                    Name = item.Name,
                    PlacedItemID = item.PlacedItemID
                };
                distances.Add(dist);
            }

            List<DistanceItem> sortedDistances = distances.OrderBy(o => o.Distance).ToList();

            foreach (var dist in sortedDistances)
            {
                if (dist.Distance < 10000)
                {
                    TextBlock distanceBlock = new TextBlock
                    {
                        Text = dist.Name + "      " + dist.Distance + " m",
                        FontSize = 24,

                    };
                    distanceBlock.Tapped += new TappedEventHandler((sender, e) => PlaceTapped(sender, e, dist.PlacedItemID));
                    listOfDistances.Items.Add(distanceBlock);
                }
            }
        }

        private async void PlaceTapped(object sender, TappedRoutedEventArgs e, int placedItemID)
        {
            var placedItem = mapItems.Single(s => s.PlacedItemID == placedItemID);
            Frame.Navigate(typeof(ItemPage), new Tuple<PlacedItemModel, TokenResponse>(placedItem, _tokenResponse));
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _tokenResponse = e.NavigationParameter as TokenResponse;
            ShowNearestPoints();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }


        #region  Menu
        private void btnRankingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RankingPage), _tokenResponse);
        }

        private void btnMapPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapPage), _tokenResponse);
        }

        private void btnNearestUsersPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestUsersPage), _tokenResponse);
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

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}