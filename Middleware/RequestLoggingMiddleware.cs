using System.Diagnostics;
using Serilog;
using Serilog.Events;

namespace BankAccountServices.Middleware
{
	public class RequestLoggingMiddleware(RequestDelegate next)
	{
		private readonly RequestDelegate _next = next;

		public async Task Invoke(HttpContext context)
		{
			var stopwatch = Stopwatch.StartNew();

			await _next(context);

			stopwatch.Stop();

			var level = context.Response.StatusCode switch
			{
				>= 500 => LogEventLevel.Error,
				>= 400 => LogEventLevel.Warning,
				_ => LogEventLevel.Information
			};

			Log.Write(
				level,
				"HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
				context.Request.Method,
				context.Request.Path.Value,
				context.Response.StatusCode,
				stopwatch.ElapsedMilliseconds);
		}
	}
}
