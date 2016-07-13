using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class ItemPage : Page
    {
        private NavigationHelper _navigationHelper;
        private ObservableDictionary defaultViewModel;
        private PlacedItemModel _placedItem;
        private TokenResponse _tokenResponse;
        private EneoWebApi webApi;
        private Image _mapPoint;

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public ItemPage()
        {
            this.InitializeComponent();
            defaultViewModel = new ObservableDictionary();
            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this._navigationHelper.SaveState += this.NavigationHelper_SaveState;
            webApi = new EneoWebApi();

        }

        private async Task<List<CommentModel>> GetComments()
        {
            waitIndicator.Visibility = Windows.UI.Xaml.Visibility.Visible;
            waitIndicator.IsActive = true;
            List<CommentModel> commentsList = await webApi.GetComments(_tokenResponse.Access_Token, _placedItem.PlacedItemID);
            return commentsList;
        }

        private async void CreateCommentListView()
        {
            List<CommentModel> commentsList = await GetComments();

            commentsListView.Items.Clear();
            foreach (var item in commentsList)
            {
                AddCommentToListView(item.userName, item.content, item.userID);
            }

            waitIndicator.IsActive = false;
            waitIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            commentsListView.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void AddCommentToListView(string userName, string commentText, string userID)
        {
            StackPanel sp = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 19)
            };

            TextBlock tbUserName = new TextBlock
            {
                Text = userName,
                FontSize = 20,
                FontWeight = FontWeights.ExtraBlack,
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock tbCommentText = new TextBlock
            {
                Text = commentText,
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(19, 0, 0, 0)
            };
            sp.Children.Add(tbUserName);
            sp.Children.Add(tbCommentText);
            sp.Tapped += new TappedEventHandler((sender, e) => UserTapped(sender, e, userID));
            commentsListView.Items.Add(sp);
        }

        private async void UserTapped(object sender, TappedRoutedEventArgs e, string userID)
        {
            Frame.Navigate(typeof(ProfilePage), new Tuple<TokenResponse, string>(_tokenResponse, userID));
        }

        private void LoadItemImage()
        {
            imageWaitIndicator.IsActive = true;
            if (!string.IsNullOrEmpty(_placedItem.ImageUrl))
            {
                imageWaitIndicator.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Uri myUri = new Uri(_placedItem.ImageUrl, UriKind.Absolute);
                BitmapImage bmi = new BitmapImage();
                bmi.CreateOptions = BitmapCreateOptions.None;
                bmi.UriSource = myUri;
                mapItemImage.Source = bmi;
                return;
            }
            imageWaitIndicator.IsActive = false;

        }

        private void itemImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            imageWaitIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            imageWaitIndicator.IsActive = false;
            mapItemImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Tuple<PlacedItemModel, TokenResponse, Image> args = e.NavigationParameter as Tuple<PlacedItemModel, TokenResponse, Image>;
            if (args != null)
            {
                _tokenResponse = args.Item2;
                _placedItem = args.Item1;
                _mapPoint = args.Item3;
            }
            else
            {
                Tuple<PlacedItemModel, TokenResponse> lessArgs = e.NavigationParameter as Tuple<PlacedItemModel, TokenResponse>;
                _tokenResponse = lessArgs.Item2;
                _placedItem = lessArgs.Item1;
            }

            tbItemid.Text = _placedItem.PlacedItemID.ToString();
            tbName.Text = _placedItem.Name;
            tbLongitude.Text = _placedItem.Longitude;
            tbLatitude.Text = _placedItem.Latitude;
            tbDescription.Text = _placedItem.Description;
            LoadItemImage();
            CreateCommentListView();

            if (_placedItem.Checked)
            {
                appBarBtnCheckIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                appBarBtnCheckIn.IsEnabled = false;
            }
        }

        private void btnMapPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapPage), _tokenResponse);
        }

        private void btnUserProfile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage), _tokenResponse);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
            _placedItem.Unvisited = true;
        }

        #endregion NavigationHelper registration

        private void appBarBtnCommentAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCommentPage), new Tuple<PlacedItemModel, TokenResponse>(_placedItem, _tokenResponse));
        }

        private void btnRankingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RankingPage), _tokenResponse);
        }

        private void btnNearestUsersPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestUsersPage), _tokenResponse);
        }

        private async void appBarBtnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Checkinowanie...";
            statusbar.ProgressIndicator.ShowAsync();
            string content = await webApi.Checkin(_tokenResponse.Access_Token, _placedItem.PlacedItemID.ToString());
            string imgUri = "ms-appx:///Assets/point-visited.png";
            if (_mapPoint != null)
            {
                _mapPoint.Source = new BitmapImage(new Uri(imgUri));
            }
            _placedItem.Checked = true;
            appBarBtnCheckIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            new MessageDialog("Zameldowano pomyślnie").ShowAsync();
            statusbar.ProgressIndicator.HideAsync();
        }

        private void btnNearestItemsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestItemsPage), _tokenResponse);
        }
    }
}