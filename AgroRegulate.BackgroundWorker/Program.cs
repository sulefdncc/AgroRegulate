using AgroRegulate.Application.Interfaces;
using AgroRegulate.BackgroundWorker;
using AgroRegulate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Veritabaný Servis Kaydý (API ile ayný In-Memory veritabanýný simüle eder)
builder.Services.AddDbContext<AgroDbContext>(options =>
    options.UseInMemoryDatabase("AgroRegulateDb"));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<AgroDbContext>());

// Background Service Kaydý
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();