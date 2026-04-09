using AutoMapper;
using GamingJourney.Data;
using GamingJourney.DTOs;
using GamingJourney.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingJourney.Services
{
	public class PlataformaService
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public PlataformaService(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Lista plataformas por nome
		public async Task<List<PlataformaExibicaoDto>> ExibirTodosAsync(string? nome)
		{
			var query = _context.Plataformas.AsQueryable();
			if (!string.IsNullOrWhiteSpace(nome))
			{
				query = query.Where(p => p.Nome.Contains(nome));
			}

			var plataformas = await query.ToListAsync();
			return _mapper.Map<List<PlataformaExibicaoDto>>(plataformas);
		}

		// Lista plataformas por Id
		public async Task<PlataformaExibicaoDto?> ExibirTodosIdAsync(int id)
		{
			var plataforma = await _context.Plataformas.FindAsync(id);
			if (plataforma == null) return null;

			return _mapper.Map<PlataformaExibicaoDto>(plataforma);
		}

		// Adiciona uma nova plataforma
		public async Task<PlataformaResponseDto> RegistrarAsync(PlataformaRegistroDto dto)
		{
			var plataformaExiste = await _context.Plataformas.AnyAsync(p => p.Nome == dto.Nome);
			if (plataformaExiste) throw new Exception("Plataforma já cadastrada.");

			var plataforma = _mapper.Map<Plataforma>(dto);

			_context.Plataformas.Add(plataforma);
			await _context.SaveChangesAsync();

			return _mapper.Map<PlataformaResponseDto>(plataforma);
		}

		// Edita/Put plataforma por Id
		public async Task<PlataformaExibicaoDto?> AtualizarAsync(int id, PlataformaAtualizarDto dto)
		{
			var plataforma = await _context.Plataformas.FindAsync(id);
			if (plataforma == null) return null;

			if (!string.IsNullOrWhiteSpace(dto.Nome))
			{
				var plataformaExiste = await _context.Plataformas.AnyAsync(p => p.Nome == dto.Nome && p.Id != id);
				if (plataformaExiste) throw new Exception("Plataforma já cadastrada.");

				plataforma.Nome = dto.Nome;
			}

			if (!string.IsNullOrEmpty(dto.LogoUrl))
			{
				plataforma.LogoUrl = dto.LogoUrl;
			}

			await _context.SaveChangesAsync();
			return _mapper.Map<PlataformaExibicaoDto>(plataforma);
		}

		// Remove plataforma do BD
		public async Task<bool?> DeletarPlataformaAsync(int id)
		{
			var plataforma = await _context.Plataformas.FindAsync(id);
			if (plataforma == null) return null;

			_context.Plataformas.Remove(plataforma);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
