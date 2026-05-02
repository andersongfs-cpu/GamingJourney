using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GamingJourney.Services
{
	public class UsuarioService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public UsuarioService(AppDbContext context, IMapper mapper, IConfiguration configuration)
		{
			_context = context;
			_mapper = mapper;
			_configuration = configuration;
		}

		// Registra um novo usuário
		public async Task<UsuarioResponseDto> RegistrarAsync(UsuarioRegistroDto dto)
		{
			// Verifica se Email já existe
			var emailExiste = await _context.Usuarios.AnyAsync(e => e.Email == dto.Email);

			if (emailExiste)
			{
				throw new ArgumentException("Email já cadastrado.");
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

		// Login
		public async Task<string> LoginAsync(UsuarioLoginDto dto)
		{
			var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

			if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
			{
				throw new ArgumentException("Email ou senha inválidos");
			}

			return GerarToken(usuario);
		}

		// Gerar token
		private string GerarToken(Usuario usuario)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]!);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
					new Claim(ClaimTypes.Name, usuario.Nome),
					new Claim(ClaimTypes.Email, usuario.Email),
					new Claim(ClaimTypes.Role, usuario.Cargo.ToString())
				}),
				Expires = DateTime.UtcNow.AddHours(8),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		// Lista usuários por Nome e Email
		public async Task<List<UsuarioExibicaoDto>> GetTodosAsync(string? nome, string? email)
		{
			var query = _context.Usuarios.AsQueryable();

			if (!string.IsNullOrWhiteSpace(nome))
			{
				query = query.Where(q => q.Nome.Contains(nome));
			}

			if (!string.IsNullOrWhiteSpace(email))
			{
				query = query.Where(q => q.Email.Contains(email));
			}

			var usuario = await query.ToListAsync();

			return _mapper.Map<List<UsuarioExibicaoDto>>(usuario);
		}

		// Lista usuários por Id
		public async Task<UsuarioResponseDto> GetPorId(int id)
		{
			var usuario = await _context.Usuarios.FindAsync(id);
			
			if (usuario == null)
			{
				throw new KeyNotFoundException($"Usuário de ID {id} não encontrado.");
			}

			return _mapper.Map<UsuarioResponseDto>(usuario);
		}

		// Edita/Put usuários por Id
		public async Task<UsuarioResponseDto> AtualizarAsync(int id, UsuarioAtualizarDto editDto)
		{
			var usuario = await _context.Usuarios.FindAsync(id);
			if (usuario == null)
			{
				throw new KeyNotFoundException($"Usuário de ID {id} não encontrado.");
			}

			if (!string.IsNullOrWhiteSpace(editDto.Nome))
			{
				usuario.Nome = editDto.Nome;
			}

			if (!string.IsNullOrWhiteSpace(editDto.Email) && editDto.Email != usuario.Email)
			{
				var existeEmail = await _context.Usuarios.AnyAsync(e => e.Email == editDto.Email);
				if (existeEmail)
				{
					throw new ArgumentException("Email já cadastrado.");
				}
				usuario.Email = editDto.Email;
				usuario.EmailConfirmado = false;
				usuario.TokenConfirmacao = Guid.NewGuid().ToString();
			}

			if (!string.IsNullOrWhiteSpace(editDto.Senha))
			{
				usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(editDto.Senha);
			}

			if (editDto.DtNasc.HasValue)
			{
				usuario.DtNasc = editDto.DtNasc.Value;
			}
			await _context.SaveChangesAsync();
			return _mapper.Map<UsuarioResponseDto>(usuario);
		}

		// Deleta usuário do BD
		public async Task DelUsuario(int id)
		{
			var usuario = await _context.Usuarios.FindAsync(id);
			if (usuario == null)
			{
				throw new KeyNotFoundException($"Usuário de ID{id} não encontrado.");
			}

			_context.Usuarios.Remove(usuario);
			await _context.SaveChangesAsync();
		}
	}
}
