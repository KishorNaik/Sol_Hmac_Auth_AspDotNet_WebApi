using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using Sol_Demos.Controllers;
using Sol_Demos.Extensions.Middlewares;
using Sol_Demos.Extensions.Services;
using Sol_Demos.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ITraceIdService, TraceIdService>();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<DataResponseFactory>();

builder.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

// Serilog Middleware
app.MapSeriLogs();
//app.UseTraceIdResponseMiddleware();

app.Run();