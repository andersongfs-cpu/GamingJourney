namespace GamingJourney.Models
{
	public enum StatusJogo
	{
		toPlay = 0,
		playing = 1,
		completed = 2,
		onHold = 3,
		dropped = 4
	}
	
	public class UsuarioJogo
	{
		public int UsuarioId { get; set; }
		public Usuario Usuario { get; set; } = null!;
		public int JogoId{ get; set; }
		public Jogo Jogo { get; set; } = null!;
		public decimal? Nota{ get; set; }
		public StatusJogo Status { get; set; } = StatusJogo.toPlay;
	}
}
