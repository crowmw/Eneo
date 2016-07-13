using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class ProfilePage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel;
        private TokenResponse _tokenResponse;
        private EneoWebApi _webApi;
        private Profile _userProfile;
        private string _userID;

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

        public ProfilePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            defaultViewModel = new ObservableDictionary();
            _webApi = new EneoWebApi();
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void ShowUserData()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Pobieranie danych...";
            statusbar.ProgressIndicator.ShowAsync();

            try
            {
                await statusbar.ProgressIndicator.HideAsync();
                waitIndicator.Visibility = Visibility.Visible;
                _userProfile = await GetUserProfile();
                LoadAvatar(_userProfile.AvatarLink);
                tbUsername.Text = string.IsNullOrEmpty(_userProfile.UserName) ? string.Empty : _userProfile.UserName;
                tbEmail.Text = string.IsNullOrEmpty(_userProfile.Email) ? string.Empty : _userProfile.Email;
                tbRegisterdate.Text = string.IsNullOrEmpty(_userProfile.RegisterDate.ToString()) ? string.Empty : _userProfile.RegisterDate.ToString();
                tbCollecteditems.Text = _userProfile.CollectedItemsCount.ToString();
            }
            catch (Exception ex)
            {
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void LoadAvatar(string avatarUrl)
        {
            avatarImage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                Uri myUri = new Uri(avatarUrl, UriKind.Absolute);
                BitmapImage bmi = new BitmapImage();
                bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bmi.UriSource = myUri;
                avatarImage.Source = bmi;
                return;
            }
            waitIndicator.IsActive = false;
            //waitIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

        }

        private void avatarImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            waitIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            waitIndicator.IsActive = false;
            avatarImage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async Task<Profile> GetUserProfile()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Pobieranie danych...";
            statusbar.ProgressIndicator.ShowAsync();
            Profile data;
            if (string.IsNullOrEmpty(_userID))
                data = await _webApi.GetUserProfileData(_tokenResponse.Access_Token);
            else
                data = await _webApi.GetUserProfileData(_tokenResponse.Access_Token, _userID);
            statusbar.ProgressIndicator.HideAsync();

            return data;
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _tokenResponse = e.NavigationParameter as TokenResponse;

            if (_tokenResponse == null)
            {
                var parameters = e.NavigationParameter as Tuple<TokenResponse, string>;
                _tokenResponse = parameters.Item1;
                _userID = parameters.Item2;
            }
            ShowUserData();
        }

        #region Menu

        private void btnMapPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapPage), _tokenResponse);
        }

        private void btnNearestUsersPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NearestUsersPage), _tokenResponse);
        }

        private void btnRankingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RankingPage), _tokenResponse);
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

        private async void tbEmail_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_userProfile.Email))
            {
                //predefine Recipient
                EmailRecipient sendTo = new EmailRecipient()
                {
                    Address = _userProfile.Email
                };

                EmailMessage mail = new EmailMessage();
                mail.Subject = "Eneo - ";
                mail.To.Add(sendTo);

                //open the share contract with Mail only:
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
        }

        private async void CollectedItemsTapped (object sender, TappedRoutedEventArgs e)
        {
            var items = await _webApi.GetUserCollectedItems(_tokenResponse.Access_Token, _userID);
            int x = 5;
        }
    }
}