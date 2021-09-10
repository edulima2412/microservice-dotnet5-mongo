using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Catalog.Api.Repositories;
using Catalog.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Catalog
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      // Serializar tipos de dados do mongo
      BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
      BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
      var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

      //Adicionar provider do mongo
      services.AddSingleton<IMongoClient>(ServiceProvider =>
      {
        return new MongoClient(mongoDbSettings.ConnectionString);
      });

      services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();

      services.AddControllers(options =>
      {
        options.SuppressAsyncSuffixInActionNames = false;
      });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Api", Version = "v1" });
      });

      services.AddHealthChecks()
        .AddMongoDb(
          mongoDbSettings.ConnectionString,
          name: "mongodb",
          timeout: TimeSpan.FromSeconds(3),
          tags: new[] { "ready" });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog Api v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
          Predicate = (check) => check.Tags.Contains("ready"),
          ResponseWriter = async (context, report) =>
          {
            var result = JsonSerializer.Serialize(
              new
              {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                  name = entry.Key,
                  status = entry.Value.Status.ToString(),
                  except = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                  durations = entry.Value.Duration.ToString()
                })
              }
            );

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
          }
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
          Predicate = (_) => false
        });
      });
    }
  }
}
