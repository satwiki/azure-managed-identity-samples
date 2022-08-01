using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace FnApp_ManagedIdentity_Test
{
    public class Function1
    {
        public Function1(IConfiguration configuration)
        {

        }

        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                // Get credential through managed identity to access key vault 
                string clientId = Environment.GetEnvironmentVariable("ManagedIdentityClientId");
                string keyVaultUri = Environment.GetEnvironmentVariable("VaultUri");
                string secretName = Environment.GetEnvironmentVariable("SecretName");

                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = clientId });

                // Initialize key vault client 

                var keyVaultSecretClient = new SecretClient(new Uri(keyVaultUri), credential);

                // Fetch the secret by name 
                var secretValue = keyVaultSecretClient.GetSecret(secretName);

                log.LogInformation($"Value of secret {secretName} fetched from key vault is: " + secretValue.Value.Value.ToString());
            }
            catch (Exception ex)
            {
                log.LogError($"Error ocurred {ex.Message}");
            }
        }
    }
}
