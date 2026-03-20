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

            services.AddScoped<ShipmentHubPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new ShipmentHubPage(tc.Page!);
            });

            services.AddScoped<WarehouseReceiptPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new WarehouseReceiptPage(tc.Page!);
            });

            services.AddScoped<UsersHubPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new UsersHubPage(tc.Page!);
            });

            services.AddScoped<QuotationPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new QuotationPage(tc.Page!);
            });

            services.AddScoped<QuotationsHubPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new QuotationsHubPage(tc.Page!);
            });

            services.AddScoped<PurchaseOrderPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new PurchaseOrderPage(tc.Page!);
            });

            services.AddScoped<MailingRulesHubPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new MailingRulesHubPage(tc.Page!);
            });

            services.AddScoped<HomeHubPage>(sp =>
            {
                var tc = sp.GetRequiredService<TestContext>();
                return new HomeHubPage(tc.Page!);
            });


            return services;
        }
    }
}

