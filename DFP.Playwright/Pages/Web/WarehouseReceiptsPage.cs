using Microsoft.Playwright;
using System;
using System.Linq;
using System.Threading.Tasks;
using DFP.Playwright.Pages.Web.BasePages;

namespace DFP.Playwright.Pages.Web
{
    public sealed class WarehouseReceiptsPage : BasePage
    {
        public WarehouseReceiptsPage(IPage page) : base(page)
        {
        }

        private static readonly string[] WarehouseMenuSelectors =
        {
            "role=button[name='Warehouse']",
            "role=link[name='Warehouse']",
            "text=Warehouse"
        };

        private static readonly string[] WarehouseReceiptsSelectors =
        {
            "role=link[name='Warehouse Receipts']",
            "text=Warehouse Receipts"
        };

        private static readonly string[] TableViewSelectors =
        {
            "role=button[name='Table View']",
            "text=Table View"
        };

        public async Task GoToWarehouseReceiptsAsync()
        {
            var warehouse = await FindLocatorAsync(WarehouseMenuSelectors, timeoutMs: 15000);
            await warehouse.ClickAsync();

            var receipts = await FindLocatorAsync(WarehouseReceiptsSelectors, timeoutMs: 15000);
            await receipts.ClickAsync();
        }

        public async Task SelectTableViewAsync()
        {
            var tableView = await FindLocatorAsync(TableViewSelectors, timeoutMs: 15000);
            await tableView.ClickAsync();
        }

        public async Task VerifyCustomFieldsAsync()
        {
            var customFieldsEnv = Environment.GetEnvironmentVariable("WAREHOUSE_RECEIPT_CUSTOM_FIELDS") ?? "";
            var customFields = customFieldsEnv
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToList();

            if (customFields.Count == 0)
            {
                // Fallback: look for any header or label containing "Custom"
                var locator = Page.Locator("text=/custom/i");
                if (await locator.CountAsync() == 0)
                    throw new InvalidOperationException("No custom fields specified. Set WAREHOUSE_RECEIPT_CUSTOM_FIELDS or add selectors.");
                return;
            }

            foreach (var field in customFields)
            {
                var locator = Page.Locator($"text=\"{field}\"");
                if (await locator.CountAsync() == 0)
                    throw new InvalidOperationException($"Custom field not found in UI: {field}");
            }
        }
    }
}
