namespace CrudKronixForDevs.Configurations;

/// <summary>
/// Configurações usadas para emitir e validar tokens JWT.
/// Os valores sensíveis devem ser fornecidos por User Secrets ou variáveis de ambiente.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Nome da seção de configuração no appsettings.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Chave usada para assinar os tokens.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Emissor esperado para os tokens.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiência esperada para os tokens.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de validade do token em minutos.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
