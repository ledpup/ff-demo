using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Fabric;

namespace VDS.CourseAdminMastering.WebApi
{
    public class Startup
    {
        private string env { get; }
        public Startup(IConfiguration configuration, StatelessServiceContext serviceContext)
        {
            Configuration = configuration;

            //Bring service fabric configuration back to .net core
            var config = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            env = config.Settings.Sections["Environment"].Parameters["ASPNETCORE_ENVIRONMENT"].Value;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Application insight
            services.AddApplicationInsightsTelemetry(Configuration);

            // Application healthcheck
            services.AddHealthChecks();

            // Convert C# pascal case attributes to camel case 
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });

            // Config filter
            services.AddMvcCore(
                    
                    )
                    .AddAuthorization()
                    .AddApiExplorer();

            // Collect errors from AbstractValidator in handlers
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //Enfore to trigger IPipelineBehavior
                options.SuppressModelStateInvalidFilter = true;
            });

            // DI config for MediatR
            services.AddMvc();

            // SwaggerUI config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VDS CourseAdminMastering api", Version = "v1" });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    poly => poly.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddSingleton(typeof(IDisposable), typeof(SimpleDisposableClass));

            services.AddFeatureManagement();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (env == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-AU")
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VDS CourseAdmin api");
                c.RoutePrefix = env == "Development" ? string.Empty : "courseadmin";
                c.DefaultModelsExpandDepth(-1);
            });

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });

            app.UseAzureAppConfiguration();
        }
    }
}