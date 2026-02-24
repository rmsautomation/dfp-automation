using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFP.Playwright.Support
{
    public class TestContext
    {
        public IPlaywright? Playwright { get; set; }
        public IBrowser? Browser { get; set; }
        public IBrowserContext? Context { get; set; }
        public IPage? Page { get; set; }
        public Dictionary<string, object> Data { get; } = new();
    }
}

