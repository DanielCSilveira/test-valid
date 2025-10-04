using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Api.Configuration;
using Api.middleware;
using Infra;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:ClientId"];
        Console.WriteLine($"Auth: {options.Authority}");
        Console.WriteLine($"Audience: {options.Audience}");
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        options.TokenValidationParameters.RoleClaimType = "roles";


        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

// 1. Adicionar o serviço CORS no container
builder.Services.AddCors(options =>
{
    
    options.AddPolicy("CorsPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader() 
                  .AllowAnyMethod() 
                  .AllowCredentials();
        });
});

// Autorização
builder.Services.AddAuthorization();





// Controllers
builder.Services.AddControllers();

// DI
builder.Services.AddDependencys();

// Swagger com suporte a JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Valid API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT sem o prefixo 'Bearer '."
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Healthchecks
builder.Services.AddHealthChecks();

//dbcontext.
//fix to timestampbehavior on postgree
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

//Tratativa Simplificada de excecoes
app.UseGlobalExceptionHandler();

app.MapControllers();
app.MapHealthChecks("/health");




app.Run();

public partial class Program { }
