using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System.Text.RegularExpressions;

public class Program
{
    private const string InputValidationPattern = "^[a-zA-Z0-9 ]+$";
    private const string DangerousScript = "<script>";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services);

        var app = builder.Build();

        ConfigurePipeline(app);

        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpLogging();

        services.AddSingleton<UserServices>();
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            c.RoutePrefix = string.Empty; // Swagger UI at root
        });

        app.UseExceptionHandler("/error");
        app.UseHttpLogging();

        app.Use(async (context, next) =>
        {
            await next();

            if (context.Response.StatusCode >= 400)
            {
                Console.WriteLine($"[SECURITY] Path: {context.Request.Path}, Status: {context.Response.StatusCode}");
            }
        });

        app.UseRouting();
        app.MapControllers();
    }
}
