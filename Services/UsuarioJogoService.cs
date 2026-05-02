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
		public async Task<UsuarioJogo> AdicionarJogoAColecaoAsync(int usuarioId, UsuarioJogoColecaoDto dto)
		{
			// Verifica se o jogo existe
			var jogoExiste = await _context.Jogos.AnyAsync(j => j.Id == dto.JogoId);
			if (!jogoExiste)
			{
				throw new ArgumentException("Jogo não existe.");
			}

			// Verifica se o usuário já tem o jogo registrado
			var jaPossui = await _context.UsuariosJogos
				.AnyAsync(j => j.UsuarioId == usuarioId && j.JogoId == dto.JogoId);
			if (jaPossui)
			{
				throw new ArgumentException("Usuário já possui jogo na coleção.");
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
				CapaUrl = j.Jogo.CapaUrl
			})			
			.ToListAsync();

			return listaJogos;
		}

		// Deleta jogo da lista do usuário
		public async Task DeletarJogoAsync(int usuarioId, int? jogoId, string? nomeJogo)
		{
			// Segurança - Verifica Id do usuário.
			var query = _context.UsuariosJogos.AsQueryable().Where(u => u.UsuarioId == usuarioId);

			// Variável começa como nula
			UsuarioJogo? delJogo = null;

			// Busca jogo na lista do usuário por Id
			if (jogoId.HasValue)
			{
				delJogo = await query.FirstOrDefaultAsync(uj => uj.JogoId == jogoId);
			}
			// Busca jogo na lista por nome caso Id não tenha sido inserido
			else if (!string.IsNullOrWhiteSpace(nomeJogo))
			{
				delJogo = await query.FirstOrDefaultAsync(uj => uj.Jogo.Titulo.ToLower() == nomeJogo.ToLower());
			}

			// Retorna null caso nenhum valor seja inserido ou jogo não encontrado
			if (delJogo == null)
			{
				throw new KeyNotFoundException("Jogo não encontrado em sua lista");
			}

			_context.UsuariosJogos.Remove(delJogo);
			await _context.SaveChangesAsync();
		}

		// Edita/Put Jogo
		public async Task<UsuarioJogoExibicaoDto> AtualizarJogoAsync(int usuarioId, int? jogoId, string? nomeJogo, UsuarioJogoAtualizarDto editDto)
		{
			// Segurança - Verifica Id do usuário
			var query = _context.UsuariosJogos
				.Include(uj => uj.Jogo) // Trás todos os dados do Objeto Jogo
				.Where(u => u.UsuarioId == usuarioId) // Apenas o que pertence ao usuário dono do Token
				.AsQueryable();

			// Variável começa como nula
			UsuarioJogo? editJogo = null;

			// Busca jogo na lista do usuário por Id
			if (jogoId.HasValue)
			{
				editJogo = await query.FirstOrDefaultAsync(uj => uj.JogoId == jogoId);
			}
			// Busca jogo na lista poor nome caso Id não tenha sido inserido
			else if (!string.IsNullOrEmpty(nomeJogo))
			{
				editJogo = await query.FirstOrDefaultAsync(uj => uj.Jogo.Titulo.ToLower() == nomeJogo.ToLower());
			}

			// Retorna null caso nenhum valor seja inserido ou jogo não encontrado
			if (editJogo == null)
			{
				throw new KeyNotFoundException("Jogo não encontrado em sua lista");
			}

			// Se Status tem novo valor inserido, propriedade é atualizada
			if (editDto.Status.HasValue)
			{
				editJogo.Status = editDto.Status.Value;
			}

			// Se Nota tem novo valor inserido, propriedade é atualizada
			if (editDto.Nota.HasValue)
			{
				editJogo.Nota = editDto.Nota.Value;
			}

			await _context.SaveChangesAsync();

			return new UsuarioJogoExibicaoDto
			{
				JogoNome = editJogo.Jogo?.Titulo,
				Nota = editJogo.Nota,
				Status = editJogo.Status,
				CapaUrl = editJogo.Jogo?.CapaUrl,
				Genero = editJogo.Jogo?.Generos?.FirstOrDefault()?.Nome,
				Plataforma = editJogo.Jogo?.Plataformas?.FirstOrDefault()?.Nome
			};
		}
	}
}
