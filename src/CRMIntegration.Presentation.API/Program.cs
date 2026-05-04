using CRMIntegration.Infra.CrossCutting;
using CRMIntegration.Infra.Data.Contexts;
using CRMIntegration.Presentation.API.Endpoints.Internal;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

services.AddOpenApi();

services.RegisterApplication(builder.Configuration);
services.RegisterServices(builder.Configuration);
services.RegisterInfrastructure(builder.Configuration);
services.AddMassTransitConfiguration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseCors();

app.UseEndpoints<Program>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CRMIntegrationContext>();

    await dbContext.Database.MigrateAsync();
}

app.MapGet("/health", () => Results.Ok("API IS RUNNING!"));
//app.UseHttpsRedirection();

app.Run();
