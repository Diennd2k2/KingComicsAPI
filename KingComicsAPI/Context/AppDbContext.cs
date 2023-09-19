using KingComicsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KingComicsAPI.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Comic_Genre> ComicGenres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<FollowComic> FollowComics { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("admin");
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Comic>().ToTable("comic").HasIndex(c=>c.Title).IsUnique();
            modelBuilder.Entity<Genre>().ToTable("genre");
            modelBuilder.Entity<Comic_Genre>().ToTable("comic_genre").HasKey(e => new {e.Comic_id,e.Genre_id});
            modelBuilder.Entity<Comic_Genre>().HasOne(c => c.Comic).WithMany(c => c.ComicGenres).HasForeignKey(c => c.Comic_id);
            modelBuilder.Entity<Comic_Genre>().HasOne(c => c.Genre).WithMany(c => c.ComicGenres).HasForeignKey(c => c.Genre_id);

            modelBuilder.Entity<Chapter>().ToTable("chapter");
            modelBuilder.Entity<Image>().ToTable("image");
            modelBuilder.Entity<Chapter>().HasMany(c => c.Images).WithOne(c => c.Chapter).HasForeignKey(c => c.Chapter_id);
            modelBuilder.Entity<Chapter>().HasOne(c => c.Comic).WithMany(c => c.Chapters).HasForeignKey(cg => cg.Comic_id);

            modelBuilder.Entity<FollowComic>().ToTable("follow_comic").HasKey(e => new { e.User_id, e.Comic_id });
            modelBuilder.Entity<FollowComic>().HasOne(c => c.User).WithMany(c => c.FollowComics).HasForeignKey(c => c.User_id);
            modelBuilder.Entity<FollowComic>().HasOne(c => c.Comic).WithMany(c => c.FollowComics).HasForeignKey(c => c.Comic_id);


        }
    }
}
