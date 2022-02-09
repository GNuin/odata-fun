using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using PQS.Test.DummyOData.Models;
using PQS.Test.DummyOData.Swagger;
using PQS.Test.DummyOData.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace PQS.Test.DummyOData
{
    public class Startup
    {
        private const string APP_NAME = "PQS.Test.DummyOData";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services

                 .AddControllers(options =>
                 {
                     //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                 })
                .AddOData(opt =>
                {
                    opt
                    .EnableQueryFeatures(100)
                    .AddRouteComponents("odata", EdmModelBuilder.GetEdmModel());

                    // agrega odata y las operaciones permitidas
                    opt.QuerySettings.EnableCount = true;

                });

            services.AddSwaggerGenExtended(APP_NAME);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
            });

            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PQS.Test.DummyOData v1"));
                
            }
            
            app.UseHttpsRedirection();

            app.UseODataQueryRequest();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
               
            });
        }


        
    }
}
