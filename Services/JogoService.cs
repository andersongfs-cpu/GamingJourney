using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Services
{
	public class JogoService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;
		public JogoService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Lista os jogos por Nome		
		public async Task<List<JogoExibicaoDto>> ExibirTodosAsync(string? titulo, string? genero, string? plataforma)
		{
			// .Include para incluir as listas Generos e Plataformas
			var query = _context.Jogos
			.Include(j => j.Generos)
			.Include(j => j.Plataformas)
			.AsQueryable();

			if (!string.IsNullOrWhiteSpace(titulo))
			{
				query = query.Where(j => j.Titulo.Contains(titulo));
			}

			// Procure se dentro da lista de gêneros existe ALGUÉM com esse nome
			if (!string.IsNullOrWhiteSpace(genero))
			{
				query = query.Where(j => j.Generos.Any(g => g.Nome.Contains(genero)));
			}

			// Procure se dentro da lista de plataformas existe ALGUÉM com esse nome
			if (!string.IsNullOrWhiteSpace(plataforma))
			{
				query = query.Where(j => j.Plataformas.Any(p => p.Nome.Contains(plataforma)));
			}

			var listaJogos = await query.ToListAsync();
			return _mapper.Map<List<JogoExibicaoDto>>(listaJogos);
		}

		// Lista os jogos por Id
		public async Task<JogoExibicaoDto?> ExibirTodosIdAsync(int id)
		{
			var jogo = await _context.Jogos.FindAsync(id);
			if (jogo == null) return null;
			
			return _mapper.Map<JogoExibicaoDto>(jogo);
		}

		// Registra um novo Jogo
		public async Task<JogoResponseDto> RegistrarAsync(JogoRegistroDto dto)
		{
			var jogoExiste = await _context.Jogos.AnyAsync(j => j.Titulo == dto.Titulo);
			if (jogoExiste)
			{
				throw new Exception("Jogo já cadastrado.");
			}

			var jogo = _mapper.Map<Jogo>(dto);

			// Busca e vincula os gêneros
			if (dto.GenerosIds != null && dto.GenerosIds.Any())
			{
				jogo.Generos = await _context.Generos
					.Where(g => dto.GenerosIds.Contains(g.Id))
					.ToListAsync();
			}

			// Busca e vincula as plataformas
			if (dto.PlataformasIds != null && dto.PlataformasIds.Any())
			{
				jogo.Plataformas = await _context.Plataformas
				.Where(p => dto.PlataformasIds.Contains(p.Id))
				.ToListAsync();
			}

			_context.Jogos.Add(jogo);

			await _context.SaveChangesAsync();
			return _mapper.Map<JogoResponseDto>(jogo);
		}
	}


}
