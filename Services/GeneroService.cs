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
		public async Task<GeneroExibicaoDto?> ExibirPorIdAsync(int id)
		{
			var genero = await _context.Generos.FindAsync(id);
			if (genero == null)
			{
				throw new KeyNotFoundException($"Gênero com ID {id} não encontrado");
			}

			return _mapper.Map<GeneroExibicaoDto>(genero);
		}

		// Registra um novo gênero
		public async Task<GeneroResponseDto> RegistrarAsync(GeneroRegistroDto dto)
		{
			// Valida se usuário colocou uma string no campo de nome
			if (string.IsNullOrWhiteSpace(dto.Nome))
			{
				throw new ArgumentException("O nome do gênero é obrigatório.");
			}

			// Verifica se o genero inserido existe no Banco de Dados
			var generoExiste = await _context.Generos.AnyAsync(g => g.Nome == dto.Nome);
			
			// Se o gênero já existe, erro 400 BAD REQUEST
			if(generoExiste)
			{
				throw new ArgumentException("Gênero já cadastrado.");
			}

			var genero = _mapper.Map<Genero>(dto);

			_context.Generos.Add(genero);
			await _context.SaveChangesAsync();

			return _mapper.Map<GeneroResponseDto>(genero);
		}

		// Edita/Put gênero por Id
		public async Task<GeneroExibicaoDto> AtualizarAsync(int id, GeneroAtualizarDto dto)
		{
			var genero = await _context.Generos.FindAsync(id);
			
			if (genero == null) 
			{
				throw new KeyNotFoundException("Gênero não encontrado.");
			}

			if(!string.IsNullOrWhiteSpace(dto.Nome))
			{
				var existe = await _context.Generos.AnyAsync(g => g.Nome == dto.Nome && g.Id != id);
				if (existe)
				{
					throw new ArgumentException("Gênero já cadastrado.");
				}
				genero.Nome = dto.Nome;
			}

			await _context.SaveChangesAsync();
			return _mapper.Map<GeneroExibicaoDto>(genero);
		}
		
		// Remove um gênero por Id
		public async Task DeletarGeneroAsync(int id)
		{
			var genero = await _context.Generos.FindAsync(id);

			if (genero == null)
			{
				throw new KeyNotFoundException("Gênero não encontrado.");
			}

			_context.Generos.Remove(genero);
			await _context.SaveChangesAsync();
		}
	}
}
