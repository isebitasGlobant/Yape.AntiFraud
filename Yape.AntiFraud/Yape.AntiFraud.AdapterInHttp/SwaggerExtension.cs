using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Yape.AntiFraud.AdapterInHttp
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerGenCustomized(this IServiceCollection services, string appName)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                    {
                        Title = $"{appName} {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated ? $"{appName} {description.ApiVersion} is marked as deprecated. Please consider using a newer version." : string.Empty
                    });
                }

                options.DescribeAllParametersInCamelCase();
            
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            return services;
        }

        // Make available all versions of the API in the ListBox
        public static IApplicationBuilder AllowSwaggerToListApiVersions(this WebApplication app, string appName)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"{description.ApiVersion} ({appName})");
                }
                options.DocExpansion(DocExpansion.Full);
            });

            

            return app;
        }

        public static IApplicationBuilder AddXMLDocumentation(this WebApplication app)
        {
            var xmlFile = $"{app.GetType().Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers.Clear();
                    swaggerDoc.Servers.Add(new OpenApiServer { Url = "https://localhost:5001" });
                });
            });
            return app;
        }
    }
}
