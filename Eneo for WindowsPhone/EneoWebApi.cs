using Eneo.WindowsPhone.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Eneo.WindowsPhone
{
    public class EneoWebApi : INotifyPropertyChanged
    {
        private const string apiUrl = "http://eneo.azurewebsites.net/api/";
        private const string tokenUrl = "http://eneo.azurewebsites.net/api/token";
        //private const string apiUrl = "http://localhost:3193/";
        //private const string tokenUrl = "http://localhost:3193/token";
        private const string clientId = "WP";
        private readonly HttpClient client;

        private TokenResponse _token;
        private ResourceLoader _resources;

        public event PropertyChangedEventHandler PropertyChanged;

        public TokenResponse Token
        {
            get { return _token; }
            set { _token = value; }
        }

        public EneoWebApi()
        {
            int x = 5;
            _resources = ResourceLoader.GetForViewIndependentUse();
            this._token = new TokenResponse();
            client = new HttpClient();
        }

        public async Task<TokenResponse> Login(string login, string password)
        {
            Uri tokenUri = new Uri(tokenUrl);
            var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
            request.Headers["Accept"] = "application/json";
            string requestContent = string.Format("grant_type=password&username={0}&password={1}&client_id={2}", login, password, clientId);
            request.Content = new HttpStringContent(requestContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return HandleUnsuccessfulLogin(response, content);
            }
            Token = (TokenResponse)JsonConvert.DeserializeObject(content, typeof(TokenResponse));
            return Token;
        }

        private TokenResponse HandleUnsuccessfulLogin(HttpResponseMessage response, string content)
        {
            try
            {
                Token = (TokenResponse)JsonConvert.DeserializeObject(content, typeof(TokenResponse));
                new MessageDialog(string.Format("{0}: {1}, {2}: {3}",
                    _resources.GetString("LoginError"),
                    Token.Error,
                    _resources.GetString("LoginErrorDescription"),
                    Token.Error_description));
                return Token;
            }
            catch (Exception)
            {
                new MessageDialog(response.ReasonPhrase).ShowAsync();
                return null;
            }
        }

        //public async Task<string> Register(string login, string password, string confirmPassword)
        //{
        //    Uri tokenUri = new Uri(tokenUrl);
        //    var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
        //    request.Headers["Accept"] = "application/json";
        //    string requestContent = string.Format("grant_type=password&username={0}&password={1}&client_id={2}", login, password, clientId);
        //    request.Content = new HttpStringContent(requestContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

        //    var response = await client.SendRequestAsync(request);
        //    string content = await response.Content.ReadAsStringAsync();
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return HandleUnsuccessfulLogin(response, content);
        //    }
        //    Token = (TokenResponse)JsonConvert.DeserializeObject(content, typeof(TokenResponse));
        //    return Token;


        //    //Uri tokenUri = new Uri(tokenUrl);
        //    //var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
        //    //request.Headers["Accept"] = "application/json";
        //    //var requestContent = string.Format("{ \"UserName\": {0}, \"password\": {1}, \"confirmPassword\": {2} }", login, password, confirmPassword);
        //    //request.Content = new HttpStringContent(requestContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

        //    //var response = await client.SendRequestAsync(request);
        //    //string content = await response.Content.ReadAsStringAsync();
        //    //Debug.WriteLine(content);
        //    //return "";
        //}

        public async Task<TokenResponse> LoginExternal(string userName, string provider, string userIDFromProvider)
        {
            Uri loginUri = new Uri(apiUrl + "/Account/LoginExternal");
            var request = new HttpRequestMessage(HttpMethod.Post, loginUri);
            LoginExternalModel loginModel = new LoginExternalModel(userName, provider, userIDFromProvider);
            string requestContent = JsonConvert.SerializeObject(loginModel);

            request.Content = new HttpStringContent(requestContent, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            request.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return HandleUnsuccessfulLogin(response, content);
            }
            Token = (TokenResponse)JsonConvert.DeserializeObject(content, typeof(TokenResponse));
            return Token;
        }

        public async Task<List<PlacedItemModel>> GetItemsLocation(string token)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, "/Map/GetItems", HttpMethod.Get);
            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            List<PlacedItemModel> mapItems;
            try
            {
                mapItems = JsonConvert.DeserializeObject<List<PlacedItemModel>>(content);
                return mapItems;
            }
            catch (Exception ex)
            {
                new MessageDialog(ex.Message);
                return null;
            }
        }

        public async Task<Profile> GetUserProfileData(string token)
        {
            var request = PrepareBasicRequest(token, "User/GetProfile/", HttpMethod.Get);
            var response = await client.SendRequestAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            try
            {
                Profile profile = JsonConvert.DeserializeObject<Profile>(content);
                return profile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu profilu");
                return null;
            }
        }

        public async Task<Profile> GetUserProfileData(string token, string userID)
        {
            var request = PrepareBasicRequest(token, "User/GetProfile?userID=" + userID, HttpMethod.Get);
            var response = await client.SendRequestAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            try
            {
                Profile profile = JsonConvert.DeserializeObject<Profile>(content);
                return profile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu profilu");
                return null;
            }
        }

        public async Task<string> AddComment(string token, AddCommentModel model)
        {
            var request = PrepareBasicRequest(token, "/Map/AddComment", HttpMethod.Post);
            AttachRequestContent(model, request);

            var response = await client.SendRequestAsync(request);
            string properResponse = await response.Content.ReadAsStringAsync();
            return properResponse;
        }

        public async Task<string> Register(UserLogin model)
        {
            Uri apiUri = new Uri(apiUrl + "Account/Register");
            var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
            AttachRequestContent(model, request);

            var response = await client.SendRequestAsync(request);
            string properResponse = await response.Content.ReadAsStringAsync();
            return properResponse;
        }

        public async Task<string> Checkin(string token, string placedItemID)
        {
            var request = PrepareBasicRequest(token, "/Map/Checkin", HttpMethod.Post);
            AttachRequestContent(placedItemID, request);

            var response = await client.SendRequestAsync(request);
            string properResponse = await response.Content.ReadAsStringAsync();
            return properResponse;
        }

        public async Task<string> UpdatePosition(string token, LastPosition position)
        {
            var request = PrepareBasicRequest(token, "/User/UpdatePosition", HttpMethod.Post);
            AttachRequestContent(position, request);
            var response = await client.SendRequestAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddDMPPosition(string token, DMPPosition model)
        {
            var request = PrepareBasicRequest(token, "/DMP/AddPosition", HttpMethod.Post);
            AttachRequestContent(model, request);
            var response = await client.SendRequestAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddDMPDevice(string token, DMPDevice model)
        {
            var request = PrepareBasicRequest(token, "/DMP/AddDevice", HttpMethod.Post);
            AttachRequestContent(model, request);
            var response = await client.SendRequestAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddDMPFacebook(string token, DMPFacebook model)
        {
            var request = PrepareBasicRequest(token, "/DMP/AddFacebook", HttpMethod.Post);
            AttachRequestContent(model, request);
            var response = await client.SendRequestAsync(request);
            return await response.Content.ReadAsStringAsync();
        }



        public async Task<List<RankItem>> GetUsersRanking(string token)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, "/User/GetRanking/", HttpMethod.Get);
            var response = await client.SendRequestAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            try
            {
                List<RankItem> rankItemsList = JsonConvert.DeserializeObject<List<RankItem>>(content);
                return rankItemsList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu profilu");
                return null;
            }
        }

        public async Task<List<CommentModel>> GetComments(string token, int id)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, "Map/GetComments/" + id, HttpMethod.Get);
            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            try
            {
                List<CommentModel> commentsList = JsonConvert.DeserializeObject<List<CommentModel>>(content);
                return commentsList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu komentarzy");
                return null;
            }
        }

        public async Task<List<CollectedItemModel>> GetUserCollectedItems(string token, string userID)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, "Map/GetUserCollectedItems?_userID=" + userID, HttpMethod.Post);
            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            try
            {
                List<CollectedItemModel> collectedItems = JsonConvert.DeserializeObject<List<CollectedItemModel>>(content);
                return collectedItems;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }

        }

        public async Task<List<NearestUser>> GetNearestUsers(string token, double latitude, double longitude)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, string.Format("User/Nearest?latitude={0}&longitude={1}", latitude, longitude), HttpMethod.Get);
            var response = await client.SendRequestAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            try
            {
                List<NearestUser> nearestUsers = JsonConvert.DeserializeObject<List<NearestUser>>(content);
                return nearestUsers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu najbliższych użytkownikó");
                return null;
            }
        }

        public async Task<string> AvatarFromUrl(string token, string avatarUrl)
        {
            HttpRequestMessage request = PrepareBasicRequest(token, "User/AvatarFromUrl?url=" + avatarUrl, HttpMethod.Get);
            // AttachRequestContent(new AvatarUrlModel { Url = avatarUrl }, request);
            try
            {
                var response = await client.SendRequestAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MessageDialog("Problem przy pobieraniu najbliższych użytkowników");
                return null;
            }

        }

        /*
        public async Task<List<IJsonValue>> GetUsersToRanking(string token)
        {
        }
        */

        #region Helpers
        private static void AttachRequestContent<T>(T model, HttpRequestMessage request)
        {
            string requestContent = JsonConvert.SerializeObject(model);
            request.Content = new HttpStringContent(requestContent);
            request.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
        }

        private static HttpRequestMessage PrepareBasicRequest(string token, string rightUrlPart, HttpMethod method)
        {
            Uri apiUri = new Uri(apiUrl + rightUrlPart);
            var request = new HttpRequestMessage(method, apiUri);
            request.Headers["Authorization"] = "Bearer " + token;
            request.Headers.IfModifiedSince = new DateTimeOffset(DateTime.Now);
            return request;
        }

        #endregion Helpers
    }
}