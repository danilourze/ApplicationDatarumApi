using AplicationDatariumApi.Domain.Interface;
using AplicationDatariumApi.Infra;
using AplicationDatariumApi.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IPortfolioService, PortfolioService>();

builder.Services.AddHttpClient<IEconomicIndicatorService, BcbSgsService>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();