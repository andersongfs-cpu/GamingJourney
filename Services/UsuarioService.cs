using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Services
{
	public class UsuarioService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public UsuarioService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<UsuarioResponseDto> RegistrarAsync(UsuarioRegistroDto dto)
		{
			// Verifica se Email já existe
			var emailExiste = await _context.Usuarios.AnyAsync(e => e.Email == dto.Email);
			
			if (emailExiste)
			{
				throw new Exception("Email já cadastrado.");
			}

			// Mapeia DTO para a entidade
			var usuario = _mapper.Map<Usuario>(dto);

			// Hash da senha
			usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

			// Token de confirmação de Email
			usuario.TokenConfirmacao = Guid.NewGuid().ToString();

			_context.Usuarios.Add(usuario);
			await _context.SaveChangesAsync();

			return _mapper.Map<UsuarioResponseDto>(usuario);
		}
	}
}
