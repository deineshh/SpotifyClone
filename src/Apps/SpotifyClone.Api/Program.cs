using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SpotifyClone.Accounts.Infrastructure.Auth;
using SpotifyClone.Accounts.Infrastructure.DependencyInjection;
using SpotifyClone.Accounts.Infrastructure.Persistence.Accounts.Database;
using SpotifyClone.Accounts.Infrastructure.Persistence.Identity.Database;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.DependencyInjection;
using SpotifyClone.Shared.BuildingBlocks.Infrastructure.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddBuildingBlocks(builder.Configuration);
builder.Services.AddAccountsModule(builder.Configuration);

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options
    => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),

        ClockSkew = TimeSpan.Zero
    });

builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddCors(options =>
options.AddPolicy(name: "DevCors",
        policy => policy.WithOrigins(
            "https://localhost:3000",
            "http://localhost:3000",
            "http://localhost:8080")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

WebApplication app = builder.Build();

app.MapStaticAssets();

app.UseRouting();

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.UseCors("DevCors");

if (app.Environment.IsDevelopment())
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        IServiceProvider services = scope.ServiceProvider;

        AccountsAppDbContext accountsDb = services.GetRequiredService<AccountsAppDbContext>();
        await accountsDb.Database.MigrateAsync();

        IdentityAppDbContext identityDb = services.GetRequiredService<IdentityAppDbContext>();
        await identityDb.Database.MigrateAsync();
    }

    app.UseHttpsRedirection();

    app.UseDeveloperExceptionPage();

    app.MapOpenApi();

    app.MapScalarApiReference(options
        => options
            .WithTitle("SpotifyClone API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
    );
}

await app.RunAsync();
