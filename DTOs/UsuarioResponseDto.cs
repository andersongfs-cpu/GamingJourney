using System.ComponentModel.DataAnnotations;

namespace GamingJourney.DTOs
{
	public class UsuarioResponseDto
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public DateTime DtNasc { get; set; }
		public DateTime DtCadastro { get; set; }
		public bool EmailConfirmado { get; set; }
	}
}
