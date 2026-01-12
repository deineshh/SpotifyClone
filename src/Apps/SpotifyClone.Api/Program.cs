using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:Issuer"];
        options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:Audience"];
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!));
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
            "http://localhost:8080")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

WebApplication app = builder.Build();

app.MapStaticAssets();

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
else
{
    app.UseExceptionHandler(errorApp =>
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            IExceptionHandlerFeature? error = context.Features.Get<IExceptionHandlerFeature>();
            if (error?.Error is not null)
            {
                ILogger<Program> logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(error.Error, "Global unhandled exception");

                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
            }
        }));
}

await app.RunAsync();
