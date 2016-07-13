namespace Eneo.WindowsPhone.DataModel
{
    public class LoginExternalModel
    {
        public string UserName { get; set; }

        public string Provider { get; set; }

        public string UserIDFromProvider { get; set; }

        public LoginExternalModel(string userName, string provider, string userIDFromProvider)
        {
            UserName = userName;
            Provider = provider;
            UserIDFromProvider = userIDFromProvider;
        }
    }
}