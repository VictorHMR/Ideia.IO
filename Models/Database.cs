using Microsoft.EntityFrameworkCore;

namespace Ideia.IO.Models
{
    public class Database: DbContext
    {
        protected const string ConnectionString = @"Data Source=Database\\sqlite.db";
        public Database()
        {

        }

        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuario");

            modelBuilder.Entity<Usuario>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Usuario>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .Property(x => x.NomeCompleto)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
               .Property(x => x.Email)
               .HasMaxLength(50)
               .IsRequired();

            modelBuilder.Entity<Usuario>()
               .Property(x => x.Senha)
               .HasMaxLength(15)
               .IsRequired();

            modelBuilder.Entity<Usuario>()
               .Property(p => p.ImgPerfil)
                .HasColumnType("BLOB");
        }
    }
}
