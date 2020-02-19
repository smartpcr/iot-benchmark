// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Allocation.cs" company="Microsoft Corporation">
//   Copyright (c) 2020 Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common.KeyVault
{
    public class VaultSettings
    {
        public string VaultName { get; set; }
        public string ClientId { get; set; }
        public string ClientCertFile { get; set; }
        public string ClientSecretFile { get; set; }
        public string VaultUrl => $"https://{VaultName}.vault.azure.net";
    }
}