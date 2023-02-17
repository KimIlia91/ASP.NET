namespace IdentityManager.Models.ViewModels
{
    public class TwoFactorAuthenticationViewModel
    {
        //Used to login
        public string? Code { get; set; }

        //Used to register / signin 
        public string? Token { get; set; }

        public string QRCodeURL { get; set; }
    }
}
