using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CrudKronixForDevs.Configurations;

/// <summary>
/// Adiciona a configuração de autenticação Bearer ao documento OpenAPI.
/// Isso permite que o Scalar exiba o botão Authorize e reutilize o token JWT.
/// </summary>
public sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Registra o esquema Bearer e aplica a exigência de segurança às operações documentadas.
    /// </summary>
    /// <param name="document">Documento OpenAPI que será alterado.</param>
    /// <param name="context">Contexto da geração do documento.</param>
    /// <param name="cancellationToken">Token para cancelar a geração.</param>
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        var bearerSchemeExists = authenticationSchemes.Any(scheme =>
            scheme.Name == Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme);

        if (!bearerSchemeExists)
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        };

        foreach (var path in document.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}
