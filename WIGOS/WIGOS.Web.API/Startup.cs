﻿using WIGOS.Web.Application.People;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WIGOS.Web.API.Extensions;
using WIGOS.Web.API.Middleware;

namespace WIGOS.Web.API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(config =>
                {
                    config.RegisterValidatorsFromAssemblyContaining<Create>();
                });
            services.AddApplicationServices(_config);
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            // app.UseXContentTypeOptions();
            // app.UseReferrerPolicy(opt => opt.NoReferrer());
            // app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            // app.UseXfo(opt => opt.Deny());
            // app.UseCsp(opt => opt
            //     .BlockAllMixedContent()
            //     .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com"))
            //     .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
            //     .FormActions(s => s.Self())
            //     .FrameAncestors(s => s.Self())
            //     .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com"))
            //     .ScriptSources(s => s.Self())
            // );

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }
            // else
            // {
            //     app.Use(async (context, next) =>
            //     {
            //         context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
            //         await next.Invoke();
            //     });
            // }

            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}