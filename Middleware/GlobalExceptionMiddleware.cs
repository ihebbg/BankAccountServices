using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BankAccountServices.Middleware
{
	public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);  // Exécute la requête suivante dans le pipeline
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur est survenue.");
				await HandleExceptionAsync(context, ex);
			}
		}
		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			HttpStatusCode statusCode;
			string message;

			switch (exception)
			{
				case ArgumentException argEx:
					statusCode = HttpStatusCode.BadRequest;
					message = argEx.Message;
					break;

				case KeyNotFoundException keyNotFoundEx:
					statusCode = HttpStatusCode.NotFound;
					message = keyNotFoundEx.Message;
					break;

				case UnauthorizedAccessException unauthorizedEx:
					statusCode = HttpStatusCode.Unauthorized;
					message = unauthorizedEx.Message;
					break;

				case InvalidOperationException invalidOpEx:
					statusCode = HttpStatusCode.Conflict;
					message = invalidOpEx.Message;
					break;

				case TimeoutException timeoutEx:
					statusCode = HttpStatusCode.RequestTimeout;
					message = timeoutEx.Message;
					break;
			
				case MySqlException mySqlEx:
					statusCode = HttpStatusCode.InternalServerError;
					message = $"Erreur base de données MySQL : {mySqlEx.Message}";
					break;

				case DbUpdateConcurrencyException concurrencyEx:
					statusCode = HttpStatusCode.Conflict;
					message = "Conflit de concurrence détecté lors de la mise à jour des données.";
					break;

				case DbUpdateException dbUpdateEx:
					statusCode = HttpStatusCode.InternalServerError;
					message = $"Erreur lors de la mise à jour de la base de données : {dbUpdateEx.Message}";
					break;

				default:
					statusCode = HttpStatusCode.InternalServerError;
					message =exception.Message;
					break;
			}
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			var result = JsonSerializer.Serialize(new { error = message });
			return context.Response.WriteAsync(result);
		}
	}
}
