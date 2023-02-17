using System.Security.Claims;

namespace IdentityManager.Data
{
    public static class ClaimStore
    {
        //Claim права на действия для пользователей
        public static List<Claim> claimList = new()
        {
            new Claim("Create", "Create"),
            new Claim("Edit", "Edit"),
            new Claim("Delete", "Delete")
        };
    }
}
