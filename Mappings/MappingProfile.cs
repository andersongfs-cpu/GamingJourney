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

			// Transforma o que veio do BD em um pacote DTO
			// De: Modelo de BD -> Para: Resposta visual
			CreateMap<Usuario, UsuarioResponseDto>();

			CreateMap<Usuario, UsuarioExibicaoDto>();
			
			CreateMap<Genero, GeneroResponseDto>();
		}
	}
}
