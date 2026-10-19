using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace CrudKronixForDevs.Middlewares;

/// <summary>
/// Converte exceções não tratadas em respostas HTTP padronizadas.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa o middleware com o próximo componente do pipeline e um logger.
    /// </summary>
    /// <param name="next">Próximo componente que deve processar a requisição.</param>
    /// <param name="logger">Logger usado para diagnóstico interno.</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executa o próximo componente e trata falhas que não foram tratadas pela aplicação.
    /// </summary>
    /// <param name="context">Contexto HTTP atual.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            _logger.LogWarning(
                exception,
                "Validação rejeitada para {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await EscreverRespostaAsync(context, HttpStatusCode.BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Erro não tratado durante {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await EscreverRespostaAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    /// <summary>
    /// Escreve um corpo JSON padronizado sem expor detalhes internos da exceção.
    /// </summary>
    /// <param name="context">Contexto HTTP que receberá a resposta.</param>
    /// <param name="statusCode">Status HTTP que será devolvido.</param>
    /// <param name="detail">Mensagem segura para o consumidor da API.</param>
    private static async Task EscreverRespostaAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "about:blank",
            title = statusCode == HttpStatusCode.BadRequest ? "Requisição inválida" : "Erro interno",
            status = (int)statusCode,
            detail,
            traceId = context.TraceIdentifier
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, problem);
    }
}
