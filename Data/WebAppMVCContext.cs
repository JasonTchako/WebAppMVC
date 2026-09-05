using Microsoft.EntityFrameworkCore;
using WebAppMVC.Models;

namespace WebAppMVC.Data
{
    public class WebAppMVCContext : DbContext
    {
        public WebAppMVCContext(DbContextOptions<WebAppMVCContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
    }
}
