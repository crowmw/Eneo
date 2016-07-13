using Newtonsoft.Json;

namespace Eneo.WindowsPhone.DataModel
{
    [JsonObject(Title = "ErrorTokenResponse")]
    public class TokenResponse
    {
        public string Access_Token { get; set; }

        public string Token_Type { get; set; }

        public int Expires_In { get; set; }

        public string refresh_token { get; set; }

        [JsonProperty("as:client_id")]
        public string asclient_id { get; set; }

        public string UserName { get; set; }

        [JsonProperty(".Issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string expires { get; set; }

        public string LastLoginDate { get; set; }

        public string EneoUserID { get; set; }

        public string DisplayName { get; set; }

        public string Error { get; set; }

        public string Error_description { get; set; }
    }
}