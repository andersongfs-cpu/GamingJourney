using System.ComponentModel.DataAnnotations;

namespace GamingJourney.DTOs
{
	public class UsuarioExibicaoDto
	{
		public string Nome { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;

		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime DtNasc { get; set; }
	}
}
