using AplicationDatariumApi.Domain.Interface;
using AplicationDatariumApi.Infra;
using AplicationDatariumApi.Service;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IPortfolioService, PortfolioService>();

builder.Services.AddHttpClient<IEconomicIndicatorService, BcbSgsService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();