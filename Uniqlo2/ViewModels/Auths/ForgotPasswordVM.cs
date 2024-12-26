using System.ComponentModel.DataAnnotations;

namespace Uniqlo2.ViewModels.Auths
{
    public class ForgotPasswordVM
    {

        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; }
    }
}
