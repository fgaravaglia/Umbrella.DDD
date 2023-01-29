using Serilog;
using Serilog.Configuration;
using System.Diagnostics;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

