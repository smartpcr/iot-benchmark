// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Allocation.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Common.KeyVault
{
    public static class KeyVaultBuilder
    {
        public static IServiceCollection AddKeyVault(this IServiceCollection services, IConfiguration configuration)
        {
            var vaultSettings = new VaultSettings();
            configuration.Bind(nameof(VaultSettings), vaultSettings);
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger<VaultSettings>();
            logger?.LogInformation($"retrieving vault settings: vaultName={vaultSettings.VaultName}");
            
            KeyVaultClient.AuthenticationCallback callback = async (authority, resource, scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                if (!string.IsNullOrEmpty(vaultSettings.ClientSecretFile))
                {
                    var clientSecretFile = GetSecretOrCertFile(vaultSettings.ClientSecretFile, logger);
                    var clientSecret = File.ReadAllText(clientSecretFile);

                    var credential = new ClientCredential(vaultSettings.ClientId, clientSecret);
                    var result = await authContext.AcquireTokenAsync(resource, credential);
                    if (result == null)
                        throw new InvalidOperationException("Failed to obtain the JWT token");

                    return result.AccessToken;
                }
                else
                {
                    var clientCertFile = GetSecretOrCertFile(vaultSettings.ClientCertFile, logger);
                    var certificate = new X509Certificate2(clientCertFile);

                    Console.WriteLine($"Authenticate client {vaultSettings.ClientId} with cert: {certificate.Thumbprint}");
                    var clientCred = new ClientAssertionCertificate(vaultSettings.ClientId, certificate);

                    Console.WriteLine($"Authenticating...");
                    var result = await authContext.AcquireTokenAsync(resource, clientCred);

                    if (result == null)
                        throw new InvalidOperationException("Failed to obtain the JWT token");

                    return result.AccessToken;
                }

            };
            var kvClient = new KeyVaultClient(callback);
            services.AddSingleton<IKeyVaultClient>(kvClient);

            return services;
        }

        /// <summary>
        /// fallback: secretFile --> ~/.secrets/secretFile --> /tmp/.secrets/secretFile
        /// </summary>
        /// <param name="secretOrCertFile"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string GetSecretOrCertFile(string secretOrCertFile, ILogger logger)
        {
            logger?.LogInformation($"checking secret/cert file {secretOrCertFile}");
            if (!File.Exists(secretOrCertFile))
            {
                var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                secretOrCertFile = Path.Combine(homeFolder, ".secrets", secretOrCertFile);
                logger?.LogInformation($"checking secret/cert file {secretOrCertFile}");

                if (!File.Exists(secretOrCertFile))
                {
                    secretOrCertFile = Path.Combine("/tmp/.secrets", secretOrCertFile);
                    logger?.LogInformation($"checking secret/cert file {secretOrCertFile}");
                }
            }
            logger?.LogInformation($"Using secret/cert file {secretOrCertFile}");
            if (!File.Exists(secretOrCertFile))
            {
                throw new System.Exception($"unable to find client secret/cert file: {secretOrCertFile}");
            }

            return secretOrCertFile;
        }
    }
}