using System.ComponentModel.DataAnnotations;

namespace GamingJourney.DTOs
{
	public class GeneroRegistroDto
	{
		[Required(ErrorMessage = "O nome do gênero é obrigatório.")]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 50 caracteres.")]
		public string Nome { get; set; } = string.Empty;	
	}
}
