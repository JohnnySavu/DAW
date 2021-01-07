using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DBConnectionString") {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext,
            //SocialJohnny.Migrations.Configuration>("DBConnectionString"));
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
 
}