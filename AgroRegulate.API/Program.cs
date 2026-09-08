using System.Text;
using AgroRegulate.Application.Interfaces;
using AgroRegulate.Infrastructure.Persistence;
using AgroRegulate.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller Servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Swagger'a JWT Yetkilendirme Deste�i Ekleme
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgroRegulate API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. �rnek: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
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

// 3. JWT Authentication Yap�land�rmas�
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// 4. MediatR Kayd�
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AgroRegulate.Application.Features.Allocations.Commands.CreateAllocationRequestCommand).Assembly));

// 5. Altyap� Servisleri
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// 6. PostgreSQL Veritaban� Servisi
builder.Services.AddDbContext<AgroDbContext>(options =>
{
    // Izole test modu: PostgreSQL yerine bellek ici veritabani (kurulum gerekmez)
    if (string.Equals(builder.Configuration["Database:Provider"], "InMemory", StringComparison.OrdinalIgnoreCase))
    {
        options.UseInMemoryDatabase("AgroRegulateDb");
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<AgroDbContext>());

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Statik frontend dosyalar�n� sunma deste�i
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// S�ralama �nemli: Authentication -> Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 7. Otomatik Migration ve Seed Data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AgroDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

app.Run();