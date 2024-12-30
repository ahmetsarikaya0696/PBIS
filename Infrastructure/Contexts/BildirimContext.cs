using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    public class BildirimContext : DbContext
    {
        public BildirimContext()
        {
        }

        public BildirimContext(DbContextOptions<BildirimContext> options)
            : base(options)
        {

        }
    }
}
