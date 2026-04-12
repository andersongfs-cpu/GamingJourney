using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Data
{
	// DbContext é um tradutor entre as classes C# e as tabelas do SQL
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		// Tabelas criadas com base nas Classes/Models:	
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Genero> Generos { get; set; }
		public DbSet<Jogo> Jogos{ get; set; }
		public DbSet<Plataforma> Plataformas { get; set; }		
		public DbSet<UsuarioJogo> UsuariosJogos { get; set; }
		

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UsuarioJogo>()
				.Property(l => l.Nota)
				.HasColumnType("decimal(3,1)");

			modelBuilder.Entity<UsuarioJogo>()
				.HasKey(uj => new { uj.UsuarioId, uj.JogoId });

			base.OnModelCreating(modelBuilder);
		}
	}
}
