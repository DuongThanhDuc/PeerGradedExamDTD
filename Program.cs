using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Http;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication().AddCookie();
        builder.Services.AddAuthorization();

        builder.Services.AddSingleton<UserServices>();


        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

        app.UseExceptionHandler("/error");
        app.UseHttpLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) =>
       {
           await next();

           if (context.Response.StatusCode >= 400)
           {
               Console.WriteLine($"[SECURITY] Path: {context.Request.Path}, Status: {context.Response.StatusCode}");
           }
       });

        app.Use(async (context, next) =>
       {
           if (context.Request.Query["secure"] != "true")
           {
               context.Response.StatusCode = 400;
               await context.Response.WriteAsync("Simulated HTTPS Required");
               return;
           }
           await next();
       });

        bool IsValidInput(string? input) =>
         !string.IsNullOrEmpty(input) &&
         Regex.IsMatch(input, "^[a-zA-Z0-9 ]+$") &&
         !input.Contains("<script>");

        app.Use(async (context, next) =>
        {
            var input = context.Request.Query["input"].ToString();

            if (!string.IsNullOrEmpty(input) && !IsValidInput(input))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid Input");
                return;
            }
            await next();
        });

        app.Use(async (context, next) =>
      {
          if (context.Request.Path == "/unauthorized")
          {
              context.Response.StatusCode = 401;
              await context.Response.WriteAsync("Unauthorized Access");
              return;
          }
          await next();
      });

        app.Use(async (context, next) =>
              {
                  var isAuthenticated = context.Request.Query["authenticated"] == "true";

                  if (!isAuthenticated)
                  {
                      context.Response.StatusCode = 403;
                      await context.Response.WriteAsync("Access Denied");
                      return;
                  }

                  context.Response.Cookies.Append("Auth", "true", new CookieOptions
                  {
                      Secure = true,
                      HttpOnly = true
                  });

                  await next();
              });

        app.Use(async (context, next) =>
 {
     await Task.Delay(100);
     await context.Response.WriteAsync("Processed Asynchronously\n");
     await next();
 });
 

        app.Use(async (context, next) =>
        {
            var start = DateTime.UtcNow;
            await next();
            var duration = DateTime.UtcNow - start;

            Console.WriteLine($"[Request Duration] {duration.TotalMilliseconds} ms");
        });



    }
}