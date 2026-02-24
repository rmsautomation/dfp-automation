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

            var envPath = Path.Combine(
                AppContext.BaseDirectory,
                "Config",
                $".env.{env}"
            );

            if (!File.Exists(envPath))
                throw new FileNotFoundException($".env file not found: {envPath}");

            Env.Load(envPath);

            Console.WriteLine($"Environment loaded: {envPath}");
            Console.WriteLine($"BASE_URL = {Environment.GetEnvironmentVariable("BASE_URL")}");
        }
    }
}

