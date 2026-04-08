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

		// Lista gêneros por nome
		public async Task<List<GeneroExibicaoDto>> ExibirTodosAsync(string? nome)
		{
			var query = _context.Generos.AsQueryable();
			if (!string.IsNullOrWhiteSpace(nome))
			{
				query = query.Where(q => q.Nome.Contains(nome));
			}

			var generos = await query.ToListAsync();
			return _mapper.Map<List<GeneroExibicaoDto>>(generos);
		}

		// Lista gêneros por Id
		public async Task<GeneroExibicaoDto?> ExibirTodosIdAsync(int id)
		{
			var genero = await _context.Generos.FindAsync(id);
			if (genero == null) return null;

			return _mapper.Map<GeneroExibicaoDto>(genero);
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

		// Edita/Put gênero por Id
		public async Task<GeneroExibicaoDto?> AtualizarAsync(int id, GeneroAtualizarDto dto)
		{
			var genero = await _context.Generos.FindAsync(id);
			
			if (genero == null) return null;

			if(!string.IsNullOrWhiteSpace(dto.Nome))
			{
				var existe = await _context.Generos.AnyAsync(g => g.Nome == dto.Nome && g.Id != id);
				if (existe)
				{
					throw new Exception("Gênero já cadastrado.");
				}
				genero.Nome = dto.Nome;
			}

			await _context.SaveChangesAsync();
			return _mapper.Map<GeneroExibicaoDto>(genero);
		}
		
		// Remove um gênero por Id
		public async Task<bool?> DeletarGeneroAsync(int id)
		{
			var genero = await _context.Generos.FindAsync(id);
			if (genero == null) return null;

			_context.Generos.Remove(genero);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
