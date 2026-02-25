using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetEnv;
using System.Threading.Tasks;
using System.Collections;

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

            var osEnvSnapshot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var key = entry.Key?.ToString();
                var value = entry.Value?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(key))
                    osEnvSnapshot[key] = value;
            }

            Env.Load(envPath);
            ResolveEnvironmentAliases(osEnvSnapshot);

            Console.WriteLine($"Environment loaded: {envPath}");
            Console.WriteLine($"BASE_URL = {Environment.GetEnvironmentVariable("BASE_URL")}");
        }

        private static void ResolveEnvironmentAliases(Dictionary<string, string> osEnvSnapshot)
        {
            // Pass 1: resolve values like Env.DFP_PASSWORD from the OS environment snapshot
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var key = entry.Key?.ToString();
                var value = entry.Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!value.StartsWith("Env.", StringComparison.OrdinalIgnoreCase))
                    continue;

                var sourceKey = value.Substring(4).Trim();
                if (string.IsNullOrWhiteSpace(sourceKey))
                    throw new InvalidOperationException($"Invalid Env alias for {key}. Expected format Env.VAR_NAME.");

                if (!osEnvSnapshot.TryGetValue(sourceKey, out var sourceValue) || string.IsNullOrWhiteSpace(sourceValue))
                    throw new InvalidOperationException($"Environment variable '{sourceKey}' not found for alias '{key}'.");

                Environment.SetEnvironmentVariable(key, sourceValue);
            }

            // Pass 2: resolve simple references like CORRECT_PASSWORD=DFP_PASSWORD
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                var key = entry.Key?.ToString();
                var value = entry.Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    continue;

                if (string.Equals(key, value, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!IsSimpleVariableName(value))
                    continue;

                var sourceValue = Environment.GetEnvironmentVariable(value);
                if (string.IsNullOrWhiteSpace(sourceValue))
                    continue;

                Environment.SetEnvironmentVariable(key, sourceValue);
            }
        }

        private static bool IsSimpleVariableName(string value)
        {
            foreach (var ch in value)
            {
                if (!(char.IsLetterOrDigit(ch) || ch == '_'))
                    return false;
            }
            return value.Length > 0;
        }
    }
}

