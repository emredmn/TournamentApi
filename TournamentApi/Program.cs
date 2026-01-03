using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tournament.db"));

// 2. JWT Auth Ayarlarý
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123456789!!!")); // Bunu appsettings'den almalýsýn
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// 3. GraphQL Kurulumu
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization() // [Authorize] attribute'u için gerekli
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.Run();