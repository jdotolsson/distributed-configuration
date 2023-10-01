using JESoft.AspNet.Configuration.DependencyInjection;
using JESoft.Catalog.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add Azure App Configuration
builder.Configuration.AddCustomConfiguration();
builder.Services.AddOptionsValidationRecursivly<AppSettings>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }