using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Services
{
	public class GeneroService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;		
		public GeneroService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;			
		}

		// Lista generos por nome
		public async Task<List<GeneroExibicaoDto>> GetTodos(string? nome)
		{
			var query = _context.Generos.AsQueryable();
			if (!string.IsNullOrWhiteSpace(nome))
			{
				query = query.Where(q => q.Nome.Contains(nome));
			}

			var generos = await query.ToListAsync();
			return _mapper.Map<List<GeneroExibicaoDto>>(generos);
		}

		// Registra um novo gênero
		public async Task<GeneroResponseDto> RegistrarAsync(GeneroRegistroDto dto)
		{
			var generoExiste = await _context.Generos.AnyAsync(g => g.Nome == dto.Nome);
			
			if(generoExiste)
			{
				throw new Exception("Gênero já cadastrado.");
			}

			var genero = _mapper.Map<Genero>(dto);

			_context.Generos.Add(genero);
			await _context.SaveChangesAsync();

			return _mapper.Map<GeneroResponseDto>(genero);
		}		
	}
}
