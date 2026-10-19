using System.Text;
using CrudKronixForDevs.Configurations;
using CrudKronixForDevs.Data;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Middlewares;
using CrudKronixForDevs.Repositories;
using CrudKronixForDevs.Repositories.Interfaces;
using CrudKronixForDevs.Services;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers para que a aplicação consiga mapear as rotas HTTP.
builder.Services.AddControllers();
// Registra o OpenAPI e declara o esquema Bearer para que o Scalar disponibilize o Authorize.
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// O banco InMemory facilita o aprendizado, mas perde os dados quando a aplicação é encerrada.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CrudKronixForDevsDb"));

// Registra os Repositories e Services por abstrações, permitindo trocar implementações e testá-las com mocks.
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMedicamentoService, MedicamentoService>();
builder.Services.AddScoped<IAnexoService, AnexoService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Limita requisições multipart para evitar uploads maiores que o limite definido pela API.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// Carrega as configurações do JWT a partir de appsettings, User Secrets ou variáveis de ambiente.
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Key com pelo menos 32 caracteres usando User Secrets ou variável de ambiente.");
}

// Configura a validação de assinatura, emissor, audiência e validade dos tokens recebidos.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Habilita a autorização para que endpoints possam exigir um usuário autenticado com [Authorize].
builder.Services.AddAuthorization();

var app = builder.Build();

// Cria apenas o usuário de desenvolvimento configurado por segredo local ou variável de ambiente.
await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);

// Configura o pipeline HTTP apenas depois que todos os serviços foram registrados.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Centraliza o tratamento de exceções antes dos demais componentes do pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// A autenticação deve ocorrer antes da autorização para preencher o usuário da requisição.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
