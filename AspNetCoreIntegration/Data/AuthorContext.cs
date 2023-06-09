using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.Data {
    public class AuthorContext : DbContext {
        public AuthorContext(DbContextOptions<AuthorContext> options)
            : base(options) { }

        public DbSet<Author> Authors { get; set; }
    }
}
