namespace GamingJourney.Models
{
	public class Genero
	{	
		public int Id{ get; set; }
		public string Nome { get; set; } = string.Empty;

		public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
	}
}
