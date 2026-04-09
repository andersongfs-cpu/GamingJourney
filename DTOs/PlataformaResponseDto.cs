using GamingJourney.Models;

namespace GamingJourney.DTOs
{
	public class PlataformaResponseDto
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string? LogoUrl { get; set; }
	}
}
