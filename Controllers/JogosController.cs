using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamingJourney.Controllers
{
	[Route("Api/[controller]")]
	[ApiController]
	public class JogosController : Controller
	{
		private readonly JogoService _jogoService;
		private readonly GeneroService _generoService;
		private readonly PlataformaService _plataformaService;

		public JogosController(JogoService jogoService, GeneroService generoService, PlataformaService plataformaService)
		{
			_jogoService = jogoService;
			_generoService = generoService;
			_plataformaService = plataformaService;
		}

		// Lista jogos cadastrados por nome
		[HttpGet]
		public async Task<ActionResult<List<JogoExibicaoDto>>> ExibirJogos(
		[FromQuery] string? titulo,
		[FromQuery] string? genero,
		[FromQuery] string? plataforma)
		{
			var jogos = await _jogoService.ExibirTodosAsync(titulo, genero, plataforma);
			return Ok(jogos);
		}

		// Registra um novo jogo
		[HttpPost("registrar")]
		public async Task<IActionResult> Registrar(JogoRegistroDto dto)
		{
			var jogo = await _jogoService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(Registrar), jogo);
		}
	}
}
