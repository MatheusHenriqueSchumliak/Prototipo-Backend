using System.Text.Json;
using System.Text.Json.Serialization;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Exceptions;

namespace PrototipoBackEnd.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Cliente desistiu; não há a quem responder.
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                var (status, message) = Mapear(ex);

                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Erro não tratado em {Metodo} {Caminho}", context.Request.Method, context.Request.Path);

                context.Response.Clear();
                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(message), JsonOptions));
            }
        }

        // ArgumentException vem das invariantes do domínio (ex.: e-mail inválido) e é erro de entrada do cliente.
        // A mensagem de 500 é fixa de propósito: nada de stack trace ou detalhe interno na resposta.
        private static (int Status, string Message) Mapear(Exception ex) => ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
            ConflictException => (StatusCodes.Status409Conflict, ex.Message),
            ValidationException => (StatusCodes.Status400BadRequest, ex.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Credenciais inválidas."),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno. Tente novamente mais tarde.")
        };
    }
}
