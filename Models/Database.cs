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
        public DbSet<Projeto> Projeto { get; set; }
        public DbSet<ImagemProjeto> ImagemProjeto { get; set; }

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



            modelBuilder.Entity<Projeto>()
                .ToTable("Projeto");

            modelBuilder.Entity<Projeto>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Projeto>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<Projeto>()
                .Property(x => x.Titulo)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Projeto>()
                .Property(x => x.Descricao)
                .IsRequired();

            modelBuilder.Entity<Projeto>()
                .Property(x => x.Meta)
                .IsRequired();

            modelBuilder.Entity<Projeto>()
                .Property(x => x.DtCriacao)
                .IsRequired();

            modelBuilder.Entity<Projeto>()
                .Property(x => x.IdUsuAutor)
                .IsRequired();


            modelBuilder.Entity<ImagemProjeto>()
                .ToTable("ImagemProjeto");

            modelBuilder.Entity<ImagemProjeto>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ImagemProjeto>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder.Entity<ImagemProjeto>()
               .Property(p => p.Imagem)
               .HasColumnType("BLOB");

            modelBuilder.Entity<ImagemProjeto>()
               .Property(x => x.NrOrdem)
               .IsRequired();

            modelBuilder.Entity<ImagemProjeto>()
               .Property(x => x.IdProjeto)
               .IsRequired();

        }
    }
}
