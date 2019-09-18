using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace RazorRender
{
    public static class RazorRenderCollectionExtensions
    {
        public static void AddRazorRenderService(this IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IRazorRenderService, RazorRenderService>();
        }

        public static void AddRazorRenderService(this IServiceCollection services, string path)
        {
            AddRazorRenderService(services);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                var fileProvider = new PhysicalFileProvider(path);
                options.FileProviders.Add(fileProvider);
            });
        }

        public static void AddRazorRenderService(this IServiceCollection services, List<string> paths)
        {
            AddRazorRenderService(services);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                foreach (var item in paths)
                {
                    var fileProvider = new PhysicalFileProvider(item);
                    options.FileProviders.Add(fileProvider);
                }
            });
        }
    }
}
