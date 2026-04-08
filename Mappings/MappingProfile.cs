using AutoMapper;
using GamingJourney.Models;
using GamingJourney.DTOs;

namespace GamingJourney.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Prepara o DTO em um pacote para mandar para o BD
			// De: Registro -> Para: Modelo de BD
			CreateMap<UsuarioRegistroDto, Usuario>();

			CreateMap<GeneroRegistroDto, Genero>();

			CreateMap<JogoRegistroDto, Jogo>();

			CreateMap<PlataformaRegistroDto, Plataforma>();

			// Transforma o que veio do BD em um pacote DTO
			// De: Modelo de BD -> Para: Resposta visual
			CreateMap<Usuario, UsuarioResponseDto>();

			CreateMap<Usuario, UsuarioExibicaoDto>();

			CreateMap<Genero, GeneroResponseDto>();

			CreateMap<Genero, GeneroExibicaoDto>();

			CreateMap<Jogo, JogoResponseDto>()
				.ForMember(dest => dest.Generos, opt => opt.MapFrom(src =>
					(src.Generos ?? new List<Genero>()).Select(g => g.Nome).ToList()))
				.ForMember(dest => dest.Plataformas, opt => opt.MapFrom(src =>
					(src.Plataformas ?? new List<Plataforma>()).Select(p => p.Nome).ToList()));

			CreateMap<Jogo, JogoExibicaoDto>()
				.ForMember(dest => dest.Generos, opt => opt.MapFrom(src =>
					(src.Generos ?? new List<Genero>()).Select(g => g.Nome).ToList()))
				.ForMember(dest => dest.Plataformas, opt => opt.MapFrom(src =>
					(src.Plataformas ?? new List<Plataforma>()).Select(p => p.Nome).ToList()));
		}
	}
}
