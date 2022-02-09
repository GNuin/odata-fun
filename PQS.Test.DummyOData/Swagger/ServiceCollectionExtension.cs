using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Swagger
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configura swagger gen 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="appName"></param>
        /// <returns></returns>

        public static IServiceCollection AddSwaggerGenExtended(this IServiceCollection services,string appName)
        {
            services.AddSwaggerGen(c =>
            {

                
                //TODO: ESTO DEBERIA SER PARAMETRIZADO/ VERSIONAMOS LAS APIS?
                c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = appName });

                // Predicado de inclusion a swagger
                c.DocInclusionPredicate((name, api) =>
                {
                    // excluye todo lo que empieza con odata
                    var include = !api.RelativePath.Contains("odata");
                    return include;

                });

                // definie el operation id default
                c.CustomOperationIds(c =>
                {
                    return c.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });

                // Assign scope requirements to operations based on AuthorizeAttribute
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                // filtro para que el swagger genere bien los uploads de archivo
                c.OperationFilter<SingleFileOperationFilter>();

                // filtro para que el swagger genere bien los uploads de multiples archivos
                c.OperationFilter<MultiFileOperationFilter>();

                c.DescribeAllParametersInCamelCase();
            });

            return services;

        }

    }
}
