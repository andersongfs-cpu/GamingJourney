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
			// Erro de autenticação 401 unauthorized
			// Erro 400 BadRequest
			context.Response.StatusCode = exception switch
			{
				UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
				_ => (int)HttpStatusCode.BadRequest
			};

			var response = new { message = exception.Message };
			var json = JsonSerializer.Serialize(response);

			return context.Response.WriteAsync(json);
		}
	}
}
