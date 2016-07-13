using System.ComponentModel.DataAnnotations;

namespace Eneo.Model.Models.Entities
{
    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        public string UserIDFromProvider { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Provider { get; set; }
    }

    public class ParsedExternalAccessToken
    {
        public string user_id { get; set; }

        public string app_id { get; set; }
    }
}