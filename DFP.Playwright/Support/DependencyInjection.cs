using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web;

namespace DFP.Playwright.Support
{
    public static class DependencyInjection
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            services.AddScoped<TestContext>();

            services.AddScoped<LoginPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "";
                return new LoginPage(tc.Page!, baseUrl);
            });

            services.AddScoped<DashboardPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new DashboardPage(tc.Page!);
            });

            services.AddScoped<ShipmentPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new ShipmentPage(tc.Page!);
            });

            services.AddScoped<ReportsPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new ReportsPage(tc.Page!);
            });

            return services;
        }
    }
}

