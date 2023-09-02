using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using C2IEDM.Web.Persistence;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Interfaces;
using C2IEDM.Web.Infrastructure.Security;

namespace C2IEDM.Web.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
        });

        // This section is for running locally
        services.AddDbContext<DataContext>(opt =>
        {
            //opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            //opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        // This section is for deploying to Heroku
        /*
        services.AddDbContext<DataContext>(options =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string connStr;

            // Depending on if in development or production, use either Heroku-provided
            // connection string, or development connection string from env var.
            if (env == "Development")
            {
                // Use connection string from file.
                connStr = config.GetConnectionString("DefaultConnection");
            }
            else
            {
                // Use connection string provided at runtime by Heroku.
                var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl.Replace("postgres://", string.Empty);
                var pgUserPass = connUrl.Split("@")[0];
                var pgHostPortDb = connUrl.Split("@")[1];
                var pgHostPort = pgHostPortDb.Split("/")[0];
                var pgDb = pgHostPortDb.Split("/")[1];
                var pgUser = pgUserPass.Split(":")[0];
                var pgPass = pgUserPass.Split(":")[1];
                var pgHost = pgHostPort.Split(":")[0];
                var pgPort = pgHostPort.Split(":")[1];

                connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
            }

            // Whether the connection string came from the local development configuration file
            // or from the environment variable from Heroku, use it to set up your DbContext.
            options.UseNpgsql(connStr);
        });
        */

        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000");
            });
        });

        services.AddControllers().AddNewtonsoftJson(opt =>
            opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);
        services.AddControllers().AddNewtonsoftJson(opt =>
            opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        // Jeg har været nødt til at udkommentere dette indtl videre, da det giver fejlen JsonError: No data input at 1:1
        // ChatGPT siger, at det kan skyldes noget middleware, og jeg mistænker, at det relaterer sig til det med at du
        // jo opererer med pagination. I PR opererer du jo ikke med NewtonSoftJson, og der er der ikke umiddelbart nogen
        // problemer. I C2IEDM er du umiddelbart nødt til det at bruge NewtonSoftJson, så du kan operere polymorfisk med
        // typehierarkier, så det virker som om der er et clash af en art

        //services.AddControllers().AddNewtonsoftJson(options =>
        //{
        //    var ebbesJsonResolver = new EbbesContractResolver();
        //    //ebbesJsonResolver.IgnoreProperty(typeof(LinePoint), "LineId");
        //    //ebbesJsonResolver.IgnoreProperty(typeof(LinePoint), "PointId");
        //    //ebbesJsonResolver.IgnoreProperty(typeof(LinePoint), "LinePointIndex");
        //    //ebbesJsonResolver.IgnoreProperty(typeof(Point), "Id");
        //    //ebbesJsonResolver.IgnoreProperty(typeof(Point), "LinePoints");

        //    options.SerializerSettings.ContractResolver = ebbesJsonResolver;
        //});

        services.AddMediatR(assemblies: typeof(Application.People.List.Handler).Assembly);
        //services.AddMediatR(assemblies: typeof(Application.Locations.List.Handler).Assembly);
        services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
        services.AddScoped<IUserAccessor, UserAccessor>();

        return services;
    }
}