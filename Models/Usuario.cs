namespace GamingJourney.Models
{
	public enum CargoUsuario
	{
		Comum = 0,
		GM = 1,
		Admin = 2
	}

	public class Usuario
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string SenhaHash { get; set; } = string.Empty;
		public DateTime DtNasc { get; set; }
		public DateTime DtCadastro { get; set; } = DateTime.UtcNow;
		public CargoUsuario Cargo { get; set; } = CargoUsuario.Comum;
		public bool EmailConfirmado { get; set; } = false;
		public string? TokenConfirmacao { get; set; }
	}
}
