using System.Diagnostics;

namespace CSharpApp.Api.Middleware
{
    public sealed class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Middleware> _logger;

        public Middleware(RequestDelegate next, ILogger<Middleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    context.TraceIdentifier);
            }
        }
    }
}
