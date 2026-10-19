using System.Text;
using CrudKronixForDevs.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace CrudKronixForDevs.Tests.Middlewares;

/// <summary>
/// Testa a padronização de erros feita pelo middleware global.
/// </summary>
public class ExceptionHandlingMiddlewareTests
{
    /// <summary>
    /// Confirma que exceções internas retornam 500 sem expor a mensagem original.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_DeveOcultarDetalhes_QuandoOcorreExcecaoNaoTratada()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("segredo interno"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var responseBody = new MemoryStream();
        var context = new DefaultHttpContext();
        context.Response.Body = responseBody;

        await middleware.InvokeAsync(context);

        responseBody.Position = 0;
        var response = await new StreamReader(responseBody, Encoding.UTF8).ReadToEndAsync();
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("erro interno", response, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("segredo interno", response, StringComparison.OrdinalIgnoreCase);
    }
}
