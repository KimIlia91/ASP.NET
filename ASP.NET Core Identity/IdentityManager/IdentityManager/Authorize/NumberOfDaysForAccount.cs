using IdentityManager.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityManager.Authorize
{
    public class NumberOfDaysForAccount : INumberOfDaysForAccount
    {
        private readonly ApplicationDbContext _context;

        public NumberOfDaysForAccount(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public int Get(string accountId)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == accountId);
            if (user is not null && user.DateCreated != DateTime.MinValue)
            {
                return (DateTime.Today - user.DateCreated).Days;
            }
            return 0;
        }
    }
}
