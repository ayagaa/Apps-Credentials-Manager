using Microsoft.Extensions.Configuration;

namespace CredentialsManager
{
    public class Supplier
    {
        public IConfiguration Configuration { get; }

        public Supplier(string credentialsFile = null, string credentialsFolder = null)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

            // Option A: JSON file
            if (!string.IsNullOrWhiteSpace(credentialsFile))
                builder.AddJsonFile(
                    path: Path.IsPathRooted(credentialsFile)
                        ? credentialsFile
                        : Path.Combine(credentialsFolder ?? ".", credentialsFile),
                    optional: false, reloadOnChange: false
                );

            // Option B: Environment variables
            builder.AddEnvironmentVariables();

            // Optional: Azure Key Vault, AWS Secrets Manager, etc.

            Configuration = builder.Build();
        }

        public string GetSecret(string key) =>
        Configuration[key] ??
        throw new InvalidOperationException($"Secret '{key}' not found.");

        /// <summary>
        /// Bind a section (or the entire file) to a POCO, Dictionary, list, etc.
        /// If sectionKey is null or empty, binds root.
        /// </summary>
        public T GetSecretObject<T>(string sectionKey = null)
        {
            if (string.IsNullOrEmpty(sectionKey))
                return Configuration.Get<T>();

            var section = Configuration.GetSection(sectionKey);
            if (!section.Exists())
                throw new InvalidOperationException($"Configuration section '{sectionKey}' not found.");

            return section.Get<T>();
        }
    }
}
