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
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "GamingJourney API", Version = "v1" });
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<GamingJourney.Middlewares.ExceptionMiddleware>();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Roda migrations automaticamente ao subir
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

app.MapControllers();
app.Run();