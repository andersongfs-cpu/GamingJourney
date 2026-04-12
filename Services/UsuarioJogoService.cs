using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
	}
}
