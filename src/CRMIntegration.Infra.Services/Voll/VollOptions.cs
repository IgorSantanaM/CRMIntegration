using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Infra.Services.Voll
{
    public class VollOptions
    {
        public const string Section = "Voll";

        /// <summary>Voll domain (e.g. https://app.voll.chat)</summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>API-KEY header value</summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>Default channel ID for campaigns</summary>
        public string ChannelId { get; set; } = string.Empty;

        /// <summary>Custom field ID for CobMais pessoa ID</summary>
        public string CustomFieldCobMaisId { get; set; } = "cf_cobmais_id";

        /// <summary>Custom field ID for CPF/CNPJ</summary>
        public string CustomFieldCpfCnpj { get; set; } = "cf_cpfcnpj";
    }
}
