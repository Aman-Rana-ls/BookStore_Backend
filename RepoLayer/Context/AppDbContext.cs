using Microsoft.EntityFrameworkCore;
using RepoLayer.Entity;

namespace RepoLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
