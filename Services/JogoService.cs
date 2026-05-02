using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
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
		public async Task<JogoResponseDto> ExibirPorIdAsync(int id)
		{
			var jogo = await _context.Jogos
			.Include(j => j.Generos)
			.Include(j => j.Plataformas)
			.FirstOrDefaultAsync(j => j.Id == id);

			if (jogo == null)
			{
				throw new KeyNotFoundException($"Jogo com ID {id} não encontrado.");
			}

			return _mapper.Map<JogoResponseDto>(jogo);
		}

		// Registra um novo Jogo
		public async Task<JogoResponseDto> RegistrarAsync(JogoRegistroDto dto)
		{
			// Verifica se jogo já existe no Bando de Dados
			var jogoExiste = await _context.Jogos.AnyAsync(j => j.Titulo == dto.Titulo);
			if (jogoExiste)
			{
				throw new ArgumentException("Jogo já cadastrado.");
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

		// Edita/Atualiza um jogo
		public async Task<JogoExibicaoDto> AtualizarAsync(int id, JogoAtualizarDto dto)
		{
			var jogo = await _context.Jogos
			.Include(j => j.Generos)
			.Include(j => j.Plataformas)
			.FirstOrDefaultAsync(j => j.Id == id);

			if (jogo == null)
			{
				throw new KeyNotFoundException($"Jogo com ID {id} não encontrado.");
			}

			if (!string.IsNullOrWhiteSpace(dto.Titulo))
			{
				var existe = await _context.Jogos.AnyAsync(j => j.Titulo == dto.Titulo && j.Id != id);
				if (existe)
				{
					throw new ArgumentException("Titulo de jogo já existente");
				}
				jogo.Titulo = dto.Titulo;
			}

			if (!string.IsNullOrWhiteSpace(dto.CapaUrl))
			{
				jogo.CapaUrl = dto.CapaUrl;
			}

			if (dto.GenerosIds != null)
			{
				jogo.Generos = await _context.Generos
					.Where(j => dto.GenerosIds.Contains(j.Id))
					.ToListAsync();
			}

			if (dto.PlataformasIds != null)
			{
				jogo.Plataformas = await _context.Plataformas
					.Where(j => dto.PlataformasIds.Contains(j.Id))
					.ToListAsync();
			}

			await _context.SaveChangesAsync();
			return _mapper.Map<JogoExibicaoDto>(jogo);
		}

		// Exclui um jogo do BD por Id
		public async Task DeletarJogoAsync(int id)
		{
			var jogo = await _context.Jogos.FindAsync(id);

			if (jogo == null) throw new KeyNotFoundException("Jogo não encontrado");

			_context.Jogos.Remove(jogo);
			await _context.SaveChangesAsync();
		}
	}
}
