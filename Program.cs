using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TB2_aws.Data;
using TB2_aws.Models;
using TB2_aws.Service;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

static string RequiredEnvironment(string name)
{
    var value = Environment.GetEnvironmentVariable(name);
    return string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"{name} nao configurado.")
        : value;
}

var jwtSecret = RequiredEnvironment("JWT_SECRET");
if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("JWT_SECRET precisa ter pelo menos 32 bytes.");
}

var jwtIssuer = RequiredEnvironment("JWT_ISSUER");
var jwtAudience = RequiredEnvironment("JWT_AUDIENCE");
var connectionBuilder = new Npgsql.NpgsqlConnectionStringBuilder
{
    Host = RequiredEnvironment("DB_HOST"),
    Port = int.Parse(RequiredEnvironment("DB_PORT")),
    Database = RequiredEnvironment("DB_NAME"),
    Username = RequiredEnvironment("DB_USER")
};
connectionBuilder["password"] = RequiredEnvironment("DB_PASSWORD");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionBuilder.ConnectionString));
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TB2-aws API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe apenas o token JWT."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await SeedInitialUserAsync(app);
await app.RunAsync();

static async Task SeedInitialUserAsync(WebApplication app)
{
    var name = Environment.GetEnvironmentVariable("ADMIN_NAME");
    var email = Environment.GetEnvironmentVariable("ADMIN_EMAIL")?.Trim().ToLowerInvariant();
    var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

    if (string.IsNullOrWhiteSpace(name) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

    if (await context.Usuarios.AnyAsync(usuario => usuario.Email == email))
    {
        return;
    }

    var usuario = new Usuario
    {
        Nome = name.Trim(),
        Email = email,
        SenhaHash = string.Empty
    };
    usuario.SenhaHash = hasher.HashPassword(usuario, password);
    context.Usuarios.Add(usuario);
    await context.SaveChangesAsync();
}
