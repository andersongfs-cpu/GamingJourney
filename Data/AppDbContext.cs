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
	}
}
