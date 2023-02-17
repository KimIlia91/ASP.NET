namespace IdentityManager.Models.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Claims = new List<UserCalim>();
        }

        public string UserId { get; set; }

        public List<UserCalim> Claims { get; set; }
    }

    public class UserCalim
    {
        public string ClaimType { get; set; }

        public bool IsSelected { get; set; }
    }
}
