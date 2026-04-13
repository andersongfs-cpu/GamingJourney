using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Services
{
	public class UsuarioJogoService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public UsuarioJogoService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		// Usuário adiciona jogo a sua coleção
		public async Task<UsuarioJogo?> AdicionarJogoAColecaoAsync(int usuarioId, UsuarioJogoColecaoDto dto)
		{
			// Verifica se o jogo existe
			var jogoExiste = await _context.Jogos.AnyAsync(j => j.Id == dto.JogoId);
			if (!jogoExiste)
			{
				throw new Exception("Jogo não existe.");
			}

			// Verifica se o usuário já tem o jogo registrado
			var jaPossui = await _context.UsuariosJogos
				.AnyAsync(j => j.UsuarioId == usuarioId && j.JogoId == dto.JogoId);
			if (jaPossui)
			{
				throw new Exception("Usuário já possui jogo na coleção.");
			}

			var novaEntrada = new UsuarioJogo
			{
				UsuarioId = usuarioId,
				JogoId = dto.JogoId,
				Nota = dto.Nota,
				Status = dto.Status
			};

			_context.UsuariosJogos.Add(novaEntrada);
			await _context.SaveChangesAsync();
			return novaEntrada;
		}

		// Lista/Get os jogos na lista do usuário
		public async Task<List<UsuarioJogoExibicaoDto>> ExibirListaDeJogosAsync(
		int usuarioId, string? nomeJogo, string? generoJogo, decimal? nota, string? plataforma, StatusJogo? status)
		{
			// Segurança - Verifica Id do usuário
			var query = _context.UsuariosJogos.AsQueryable().Where(u => u.UsuarioId == usuarioId);

			// Filtra por nome do jogo
			if (!string.IsNullOrEmpty(nomeJogo))
			{
				query = query.Where(n => n.Jogo.Titulo.Contains(nomeJogo));
			}

			// Filtra por genero do jogo (Muitos para Muitos)
			if (!string.IsNullOrWhiteSpace(generoJogo))
			{
				query = query.Where(g => g.Jogo.Generos.Any(ug => ug.Nome.Contains(generoJogo)));
			}

			// Filtra por nota
			if (nota.HasValue)
			{
				query = query.Where(n => n.Nota == nota);
			}

			// Filtra por plataforma (Muitos para Muitos)
			if (!string.IsNullOrWhiteSpace(plataforma))
			{
				query = query.Where(p => p.Jogo.Plataformas.Any(up => up.Nome.Contains(plataforma)));
			}

			// Filtra pelo status do jogo
			if (status.HasValue)
			{
				query = query.Where(uj => uj.Status == status);
			}

			// Ordena Lista em ordem alfabética
			query = query.OrderBy(j => j.Jogo.Titulo);

			var listaJogos = await query
			.Select(j => new UsuarioJogoExibicaoDto
			{
				JogoNome = j.Jogo.Titulo,
				Nota = j.Nota,
				Plataforma = string.Join(", ", j.Jogo.Plataformas.Select(p => p.Nome)),
				Genero = string.Join(", ", j.Jogo.Generos.Select(g => g.Nome)),
				Status = j.Status,
				CapaUrl = "https://localhost:7064/" + j.Jogo.CapaUrl
			})			
			.ToListAsync();

			return listaJogos;
		}
	}
}
