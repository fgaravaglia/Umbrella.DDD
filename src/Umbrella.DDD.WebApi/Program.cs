using System;
using System.Diagnostics;
using Serilog;
using Serilog.Configuration;
using Umbrella.DDD.WebApi;
using Microsoft.Extensions.DependencyInjection;

//Specify the activity id for tracing based on OpenTelemetry standard
Activity.DefaultIdFormat = ActivityIdFormat.W3C;

// The initial "bootstrap" logger is able to log errors during start-up.
// It's completely replaced by the logger configured in `UseSerilog()` below,
// once configuration and dependency-injection have both been set up successfully.
Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddServices(builder.Configuration, Log.Logger);
builder.Host.UseSerilog();
// generate Application
var app = builder.Build();
Log.Logger.Information("Middleware configuration started...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Logger.Information("Middleware configuration completed. Application running...");
app.Run();
