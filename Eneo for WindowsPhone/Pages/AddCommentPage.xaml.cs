using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class AddCommentPage : Page
    {
        private PlacedItemModel _placedItem;
        private TokenResponse _tokenResponse;
        private EneoWebApi webApi;
        private NavigationHelper _navigationHelper;

        public async void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteTokenFromLocalStorage();
            Frame.Navigate(typeof(LoginPage), "");
        }

        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        public AddCommentPage()
        {
            this.InitializeComponent();
            this._navigationHelper = new NavigationHelper(this);
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            webApi = new EneoWebApi();
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Tuple<PlacedItemModel, TokenResponse> args = (Tuple<PlacedItemModel, TokenResponse>)e.NavigationParameter;
            _tokenResponse = args.Item2;
            _placedItem = args.Item1;
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        private async void addCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (tbComment.Text.Length > 0)
            {
                var statusbar = StatusBar.GetForCurrentView();
                statusbar.ProgressIndicator.Text = "Dodawanie komentarza...";
                statusbar.ProgressIndicator.ShowAsync();
                try
                {
                    string message = await webApi.AddComment(_tokenResponse.Access_Token, new AddCommentModel
                    {
                        Content = tbComment.Text,
                        PlacedItemID = _placedItem.PlacedItemID
                    });
                    await statusbar.ProgressIndicator.HideAsync();
                    if (message.Contains("Comment already added"))
                    {
                        new MessageDialog("Już dodałeś komentarz dla tego punktu.\nKomentarz nie został dodany!").ShowAsync();
                        ReturnToItemPage();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    new MessageDialog(ex.Message).ShowAsync();
                }
                ReturnToItemPage();
            }
        }

        private async void ReturnToItemPage()
        {
            tbComment.Text = string.Empty;
            Frame.GoBack();
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
    }
}