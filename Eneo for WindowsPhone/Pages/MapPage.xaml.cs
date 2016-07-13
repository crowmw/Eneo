using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using System.Linq;

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class MapPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel;
        private TokenResponse _tokenResponse;
        private string oldAccessToken;
        private EneoWebApi _webApi;
        private Geolocator _geolocator;
        private Geoposition _position;
        private Geoposition _lastPosition;
        private Image myLocationIcon;
        private DispatcherTimer _dispatcherTimer;
        private List<Tuple<int, Image>> _mapItemsPoints;
        private MapControl mMapControl;
        private DateTime _lastDMPSend;
        private List<PlacedItemModel> _mapItems;

        public MapPage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            _webApi = new EneoWebApi();
            defaultViewModel = new ObservableDictionary();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            mMapControl = new MapControl
            {
                MapServiceToken = "UsZVMW_9kEs0m_cfNlrwJg",
                Height = Window.Current.Bounds.Height,
                ZoomLevel = 14,
                LandmarksVisible = true,
                PedestrianFeaturesVisible = true
            };
            mapStackPanel.Children.Add(mMapControl);

            _geolocator = new Geolocator();
            _geolocator.MovementThreshold = 5;
            _geolocator.DesiredAccuracyInMeters = 1;
            _lastDMPSend = DateTime.MinValue;
            _mapItemsPoints = new List<Tuple<int, Image>>();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void AddUserPositionToMap(Geoposition position)
        {
            myLocationIcon = new Image();
            string imgUri = "ms-appx:///Assets/point-user.png";
            myLocationIcon.Source = new BitmapImage(new Uri(imgUri));
            mMapControl.Children.Add(myLocationIcon);
            MapControl.SetLocation(myLocationIcon, position.Coordinate.Point);
        }

        private async void UpdateMap()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Wyszukiwanie twojej pozycji...";
            statusbar.ProgressIndicator.ShowAsync();
            try
            {
                _position = await _geolocator.GetGeopositionAsync(TimeSpan.FromDays(30), TimeSpan.FromSeconds(5));
                mMapControl.TrySetViewAsync(new Geopoint(_position.Coordinate.Point.Position));
                AddUserPositionToMap(_position);
                statusbar.ProgressIndicator.HideAsync();
                AddItemsToMap();
                _geolocator.PositionChanged += GeolocatorPositionChanged;
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e.ToString());
                new MessageDialog("Usługa lokalizacji nie została włączona!").ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void GeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync
                (CoreDispatcherPriority.Normal, () =>
                {
                    Geopoint currentPosition = new Geopoint(args.Position.Coordinate.Point.Position);
                    MapControl.SetLocation(myLocationIcon, currentPosition);
                    _position = args.Position;
                    CenterMap();
                    if (_lastDMPSend.AddSeconds(30) < DateTime.Now)
                    {
                        AddDMPPosition();
                        AddDMPDevice();
                        UpdatePosition();
                        _lastDMPSend = DateTime.Now;
                    }
                    CheckItemRange(_position);
                });
        }

        private void CheckItemRange(Geoposition position)
        {
            if (_mapItems != null)
            {
                GeoCoordinate currentPosition = new GeoCoordinate(_position.Coordinate.Latitude, position.Coordinate.Longitude);
                foreach (PlacedItemModel item in _mapItems)
                {
                    double distance = currentPosition.GetDistanceTo(
                        new GeoCoordinate(double.Parse(item.Latitude), double.Parse(item.Longitude)));

                    if (distance <= 10 && !item.Checked && !item.Unvisited)
                    {
                        NavigateToItemPage(item);
                        _geolocator.PositionChanged -= GeolocatorPositionChanged;
                        break;
                    }
                }
            }
        }

        private void NavigateToItemPage(PlacedItemModel item)
        {
            var mapPoint = _mapItemsPoints.First(f => f.Item1 == item.PlacedItemID).Item2;
            Frame.Navigate(typeof(ItemPage), new Tuple<PlacedItemModel, TokenResponse, Image>(item, _tokenResponse, mapPoint));
        }

        private async Task AddItemsToMap()
        {
            _mapItems = await GetItems();
            foreach (var item in _mapItems)
            {
                Geopoint point = new Geopoint(new BasicGeoposition()
                {
                    Latitude = Convert.ToDouble(item.Latitude),
                    Longitude = Convert.ToDouble(item.Longitude)
                });

                Image img = new Image();
                string imgUri = item.Checked ? "ms-appx:///Assets/point-visited.png" : "ms-appx:///Assets/point-unvisited.png";
                img.Source = new BitmapImage(new Uri(imgUri));
                img.Tapped += new TappedEventHandler((sender, e) => PointTapped(sender, e, item));
                mMapControl.Children.Add(img);
                _mapItemsPoints.Add(new Tuple<int, Image>(item.PlacedItemID, img));
                MapControl.SetLocation(img, point);
            }
        }

        private void PointTapped(object sender, TappedRoutedEventArgs e, PlacedItemModel item)
        {
            ;
            NavigateToItemPage(item);
        }

        private async Task<List<PlacedItemModel>> GetItems()
        {
            EneoWebApi webApi = new EneoWebApi();
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Pobieranie punktów...";
            statusbar.ProgressIndicator.ShowAsync();
            List<PlacedItemModel> locations = await webApi.GetItemsLocation(_tokenResponse.Access_Token);
            await statusbar.ProgressIndicator.HideAsync();
            return locations;
        }

        private async void CenterMap()
        {
            await mMapControl.TrySetViewAsync(_position.Coordinate.Point, null, 0, 0, MapAnimationKind.Bow);
            mMapControl.LandmarksVisible = true;
            mMapControl.PedestrianFeaturesVisible = true;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void AddDMPPosition()
        {
            if (_position != null)
            {
                if (_position != _lastPosition)
                {
                    DMPPosition position = new DMPPosition
                    {
                        Latitude = _position.Coordinate.Latitude.ToString(),
                        Longitude = _position.Coordinate.Longitude.ToString()
                    };
                    _lastPosition = _position;
                    await _webApi.AddDMPPosition(_tokenResponse.Access_Token, position);
                }
            }
            //do whatever you want to do here
        }

        private async void AddDMPDevice()
        {
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();

            DMPDevice device = new DMPDevice
            {
                OperatingSystem = deviceInfo.OperatingSystem,
                SystemFirmwareVersion = deviceInfo.SystemFirmwareVersion,
                SystemHardwareVersion = deviceInfo.SystemHardwareVersion,
                SystemManufacturer = deviceInfo.SystemManufacturer,
                SystemProductName = deviceInfo.SystemProductName
            };

            await _webApi.AddDMPDevice(_tokenResponse.Access_Token, device);
        }

        private async void UpdatePosition()
        {
            LastPosition position = new LastPosition
            {
                Latitude = _position.Coordinate.Latitude,
                Longitude = _position.Coordinate.Longitude
            };
            await _webApi.UpdatePosition(_tokenResponse.Access_Token, position);
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _tokenResponse = e.NavigationParameter as TokenResponse;

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region Menu

        private void btnUserProfile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), _tokenResponse);
        }

        private void btnRankingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RankingPage), _tokenResponse);
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

        private void appBarBtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            lock (mMapControl)
            {
                if (mMapControl.ZoomLevel < mMapControl.MaxZoomLevel)
                {
                    mMapControl.ZoomLevel++;
                }
            }
        }

        private void appBarBtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            lock (mMapControl)
            {
                if (mMapControl.ZoomLevel > mMapControl.MinZoomLevel)
                {
                    mMapControl.ZoomLevel--;
                }
            }

        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            if (_mapItems == null)
            {
                UpdateMap();
            }
            else if (oldAccessToken != _tokenResponse.Access_Token)
            {
                _mapItems = null;
                mMapControl.Children.Clear();
                UpdateMap();
            }
            else
            {
                AddUserPositionToMap(_position);
                _geolocator.PositionChanged += GeolocatorPositionChanged;
            }
            oldAccessToken = _tokenResponse.Access_Token;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            mMapControl.Children.Remove(myLocationIcon);
        }

        #endregion NavigationHelper registration

        private void TextBlock_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WriteTag), _tokenResponse);
        }

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }
    }
}