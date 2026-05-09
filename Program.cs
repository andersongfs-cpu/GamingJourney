using AutoMapper;
using GamingJourney.Data;
using GamingJourney.Mappings;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using System.Reflection;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Rate Limiting
builder.Services.AddRateLimiter(options =>
{
	// Define o código de status quando o limite é excedido (429 Too many requests)
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	options.AddPolicy("RegistroPolicy", httpContext =>
		RateLimitPartition.GetFixedWindowLimiter(
			partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
			factory: _ => new FixedWindowRateLimiterOptions
			{
				PermitLimit = 10,                  // Máximo de requisições (10)
				Window = TimeSpan.FromMinutes(1),  // Janela de tempo entre requisições (1min)				
				QueueLimit = 0                     // Não deixa requisições na fila				
			}));
});

// JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
	x.RequireHttpsMetadata = false;
	x.SaveToken = true;
	x.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});

// AutoMapper
builder.Services.AddSingleton(new MapperConfiguration(cfg =>
{
	cfg.AddProfile<MappingProfile>();
}).CreateMapper());

// Services - AddScope - Inejeção de dependência
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<GeneroService>();
builder.Services.AddScoped<JogoService>();
builder.Services.AddScoped<PlataformaService>();
builder.Services.AddScoped<UsuarioJogoService>();

// Swagger
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// Transforma Enums em strings
		options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{ 
		Title = "GamingJourney API", 
		Version = "v1",
		Description = "API para gerenciamento de biblioteca de jogos."
	});

	// Lógica para descrições (XML)
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	if (File.Exists(xmlPath))
	{
		c.IncludeXmlComments(xmlPath);
	}

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Digite: Bearer {seu token}"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[]{}
		}
	});
});

// Banco de dados:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Política de CORS(Compartilhamento de Recursos entre Origens)
builder.Services.AddCors(options =>
{
	options.AddPolicy("FreeAccess", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// Configura para usar o swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "GamingJourney API v1");	
	c.RoutePrefix = string.Empty;	
});


app.UseMiddleware<GamingJourney.Middlewares.ExceptionMiddleware>();
app.UseRateLimiter();
app.UseCors("FreeAccess"); // UseCors
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Roda migrations automaticamente ao subir
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var db = services.GetRequiredService<AppDbContext>();
		db.Database.Migrate();
	}
	catch (Exception ex)
	{
		// Se o banco demorar a acordar e der timeout, o erro é capturado aqui e não quebra a API.
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "Ocorreu um erro ao rodar as migrations. O banco de dados pode estar indisponível no momento da inicialização.");
	}
}

app.MapControllers();
app.Run();