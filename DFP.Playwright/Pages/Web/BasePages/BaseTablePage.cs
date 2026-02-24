using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFP.Playwright.Pages.Web.BasePages
{
    public abstract class BaseTablePage : BasePage
    {

        public BaseTablePage(IPage page) : base(page) { }
        public async Task<bool> IsRecordVisibleAsync(string row) => await GetElementByTextAsync(row).First.IsVisibleAsync();
    }
}

