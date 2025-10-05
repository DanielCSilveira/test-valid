namespace Api.middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Npgsql.PostgresException dbEx) when (IsDuplicateException(dbEx))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = "Registro já existe." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado");
                // For POC purposes, return 400 Bad Request for all unexpected errors.
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = "Ocorreu um erro inesperado." });
            }
        }

        private bool IsDuplicateException(Npgsql.PostgresException ex)
        {
            return ex.Message.Contains("duplicate") == true
                || ex.Message.Contains("UNIQUE constraint") == true;
        }
    }


}
