using Microsoft.EntityFrameworkCore;
namespace FYP_Backend.Context


{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


    }
}
