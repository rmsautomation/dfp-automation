using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetEnv;
using System.Threading.Tasks;

namespace DFP.Playwright.Support
{
    public static class EnvLoader
    {
        public static void LoadEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ENV") ?? "local";

            var baseDir = AppContext.BaseDirectory;
            var envPath = Path.Combine(baseDir, "Config", $".env.{env}");
            if (!File.Exists(envPath))
            {
                var fallback = Path.Combine(baseDir, "Config", ".env.local");
                if (File.Exists(fallback))
                {
                    envPath = fallback;
                }
            }

            if (!File.Exists(envPath))
                throw new FileNotFoundException($".env file not found: {envPath}");

            Env.Load(envPath);

            Console.WriteLine($"Environment loaded: {envPath}");
            Console.WriteLine($"BASE_URL = {Environment.GetEnvironmentVariable("BASE_URL")}");
        }
    }
}

