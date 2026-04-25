using System.Net;
using System.Text.Json;

namespace GamingJourney.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				// Segue o fluxo normal
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			
			// Decide o status code			
			context.Response.StatusCode = exception switch
			{
				UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // Erro de autenticação 401 unauthorized
				KeyNotFoundException => (int)HttpStatusCode.NotFound, // Erro de não encontrado 404 NotFound
				ArgumentException => (int)HttpStatusCode.BadRequest, // Erro de argumento inválido 400 BadRequest
				_ => (int)HttpStatusCode.InternalServerError // Erro desconhecido 500
			};

			var response = new { message = exception.Message };
			var json = JsonSerializer.Serialize(response);

			return context.Response.WriteAsync(json);
		}
	}
}
