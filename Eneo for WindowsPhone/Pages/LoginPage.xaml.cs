using Eneo.WindowsPhone.Authentication;
using Eneo.WindowsPhone.Common;
using Eneo.WindowsPhone.DataModel;
using Eneo.WindowsPhone.Helpers;
using Facebook;
using Microsoft.Advertising.Mobile.Common;
using Microsoft.Advertising.Mobile.UI;
using NdefLibrary.Ndef;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Networking.Proximity;
using Windows.System.Threading;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace Eneo.WindowsPhone.Pages
{
    public sealed partial class LoginPage : Page, IWebAuthenticationBrokerContinuable
    {
        private const string FbProviderName = "Facebook";
        private const string GoogleProviderName = "Google";
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel;
        private readonly ResourceLoader resourceLoader;
        private EneoWebApi webApi;
        private FaceBookHelper _fbHelper;
        private FacebookClient _fbclient;

        //NFC//
        private ProximityDevice _proximityDevice;

        private long _publishingMessageId;
        private long _subscriptionIdNdef;
        private string args;
        //NFC//

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public LoginPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            webApi = new EneoWebApi();
            resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            defaultViewModel = new ObservableDictionary();
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //string args = string.Empty;
            //args = e.NavigationParameter.ToString();
            //hRegister.Content = args;
            //if (NavigationContext.QueryString.TryGetValue("ms_nfp_launchargs", out args))
            //{
            //    MessageBox.Show("Congratulation\nYou launch application with a NFC tag.\nParamaters : " + args);
            //    NavigationContext.QueryString.Remove("ms_nfp_launchargs");
            //}

            //args = e.Content.ToString();
            //this.navigationHelper.OnNavigatedTo(e);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private static void ShowProgressindicator(string text)
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = text;
            statusbar.ProgressIndicator.ShowAsync();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnFacebook.IsEnabled = false;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                await Login();
            }
            else
            {
                new MessageDialog("Włącz internet.");
            }
        }

        private async Task<string> Login()
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Logowanie";
            statusbar.ProgressIndicator.ShowAsync();
            TokenResponse loginResult = await webApi.Login(tbLogin.Text, tbPassword.Password);
            await statusbar.ProgressIndicator.HideAsync();

            if (loginResult.Error == null)
            {
                StorageHelper.SaveTokenToLocalStorage(loginResult);
                Frame.Navigate(typeof(MapPage), loginResult);
            }
            else
            {
                await new MessageDialog(loginResult.Error_description).ShowAsync();
                tbLogin.Text = "";
                tbPassword.Password = "";
            }
            return "";
        }

        private async void hRegister_Click(object sender, RoutedEventArgs e)
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Rejestrowanie...";
            statusbar.ProgressIndicator.ShowAsync();
            try
            {
                string register = await webApi.Register(new UserLogin
                    {
                        UserName = tbLogin.Text,
                        Password = tbPassword.Password,
                        ConfirmPassword = tbPassword.Password
                    });
                await statusbar.ProgressIndicator.HideAsync();
                await new MessageDialog("Udało się zarejestrować!\n\nTeraz możesz się zalogować").ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private void btnFacebook_Click(object sender, RoutedEventArgs e)
        {
            _fbHelper = new FaceBookHelper();
            _fbHelper.LoginAndContinue();
        }

        public async void ContinueWithWebAuthenticationBroker(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var statusbar = StatusBar.GetForCurrentView();
            statusbar.ProgressIndicator.Text = "Logowanie...";
            statusbar.ProgressIndicator.ShowAsync();

            btnLogin.IsEnabled = false;
            btnFacebook.IsEnabled = false;
            hRegister.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbPassword.IsEnabled = false;

            _fbHelper.ContinueAuthentication(args);
            if (_fbHelper.AccessToken != null)
            {
                _fbclient = new FacebookClient(_fbHelper.AccessToken);
                //Fetch facebook UserProfile:
                dynamic result = await _fbclient.GetTaskAsync("me");
                string id = result.id;
                string FBName = result.name;

                TokenResponse loginResult = await webApi.LoginExternal(FBName, FBName, id);
                if (loginResult.Error == null)
                {
                    Frame.Navigate(typeof(MapPage), loginResult);
                    StorageHelper.SaveTokenToLocalStorage(loginResult);
                    GetUserPhoto(loginResult.Access_Token, id);

                    AddDMPFacebook(result, loginResult);

                    //task.Start();

                    await statusbar.ProgressIndicator.HideAsync();
                }
            }
        }

        private async Task GetUserPhoto(string accessToken, string facebookUserID)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            dynamic user = await _fbclient.GetTaskAsync(facebookUserID + "/picture?redirect=false");
            string photoUrl = string.Empty;
            try
            {
                photoUrl = user.data.url;
            }
            catch
            {

            }
            if (!string.IsNullOrEmpty(photoUrl))
                webApi.AvatarFromUrl(accessToken, photoUrl);
        }

        private async Task AddDMPFacebook(dynamic result, TokenResponse loginResult)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            Debug.WriteLine("Started {0}", DateTime.Now);
            var dmpFacebook = new DMPFacebook()
            {
                FacebookID = result.id.ToString(),
                Email = result.email.ToString(),
                FirstName = result.first_name.ToString(),
                Gender = result.gender.ToString(),
                LastName = result.last_name.ToString(),
                Language = result.locale.ToString(),
                Locale = result.location.ToString(),
                Timezone = result.timezone.ToString(),
                Verified = result.verified.ToString(),
            };

            dynamic likes = await _fbclient.GetTaskAsync("me/likes?limit=100");
            dynamic values = likes.data;
            ICollection<DMPFacebookLikes> facebookLikes = new List<DMPFacebookLikes>();
            foreach (var x in values)
            {
                facebookLikes.Add(new DMPFacebookLikes() { CategoryName = x.category, LikeName = x.name, LikeID = x.id, AddedDate = DateTime.Parse(x.created_time) });
                string category = x.category;
                string name = x.name;
                string date = x.created_time;
                DateTime date2 = DateTime.Parse(x.created_time);
                //Debug.WriteLine(string.Format("{0} - {1} - {2}", category, name, date));
            }
            dmpFacebook.EneoUserID = loginResult.EneoUserID;
            dmpFacebook.DMPFacebookLikes = facebookLikes;
            Debug.WriteLine("GotLikes{0}", DateTime.Now);
            var resp = await webApi.AddDMPFacebook(loginResult.Access_Token, dmpFacebook);

            Debug.WriteLine("Response {0} {1}", resp, DateTime.Now);
            btnLogin.IsEnabled = true;
            btnFacebook.IsEnabled = true;
            hRegister.IsEnabled = true;
            tbLogin.IsEnabled = true;
            tbPassword.IsEnabled = true;
        }

        public async void AdControl_AdRefreshed(object sender, RoutedEventArgs args)
        {
            AdControl ad = (AdControl)sender;
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
         {
             ad.Visibility = Visibility.Visible;
             System.Diagnostics.Debug.WriteLine(
               "ad control '" + ad.Name + "' got ad, visibility = " + ad.Visibility);
         });
        }

        public async void AdControl_ErrorOccurred(object sender, AdErrorEventArgs args)
        {
            try
            {
                AdControl ad = (AdControl)sender;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ad.Visibility = Visibility.Collapsed;
                    System.Diagnostics.Debug.WriteLine(
                      "error in ad control '" + ad.Name + "': " + args.Error.Message);
                    System.Diagnostics.Debug.WriteLine("ad control '" + ad.Name + "' visibility = " + ad.Visibility);
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("oh no! " + e.Message);
            }
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //string args = string.Empty;
            //args = e.Parameter.ToString();
            //hRegister.Content = args;
            //if (NavigationContext.QueryString.TryGetValue("ms_nfp_launchargs", out args))
            //{
            //    MessageBox.Show("Congratulation\nYou launch application with a NFC tag.\nParamaters : " + args);
            //    NavigationContext.QueryString.Remove("ms_nfp_launchargs");
            //}

            //args = e.Content.ToString();
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion NavigationHelper registration

        private void PublishRecord(NdefRecord record, bool writeToTag)
        {
            if (_proximityDevice == null) return;
            // Wrap the NDEF record into an NDEF message
            var message = new NdefMessage { record };
            // Convert the NDEF message to a byte array
            var msgArray = message.ToByteArray();
            // Publish the NDEF message to a tag or to another device, depending on the writeToTag parameter
            // Save the publication ID so that we can cancel publication later
            _publishingMessageId = _proximityDevice.PublishBinaryMessage((writeToTag ? "NDEF:WriteTag" : "NDEF"), msgArray.AsBuffer());
        }

    }
}