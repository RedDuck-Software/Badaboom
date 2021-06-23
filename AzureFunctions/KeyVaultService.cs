using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class KeyVaultService
    {
        public async Task<string> GetSecretValue(string keyName)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secretBundle = await keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("keyvault") + keyName).ConfigureAwait(false);
            
            var secret = secretBundle.Value;
            return secret;
        }
    }
}