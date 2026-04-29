using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Infra.Services.CobMais
{
    public class CobMaisOptions
    {
        public const string Section = "CobMais";

        public string BaseUrl { get; set; } = "https://api.cobmais.com.br";

        /// <summary>Ocp-Apim-Subscription-Key /consulta API</summary>
        public string ConsultaApiKey { get; set; } = string.Empty;

        /// <summary>Ocp-Apim-Subscription-Key /cobranca API</summary>
        public string CobrancaApiKey { get; set; } = string.Empty;

        /// <summary>Event type registered in CobMais when campaign is dispatched</summary>
        public string CampaignEventType { get; set; } = "WHATSAPP_CAMPANHA";

        /// <summary>Event channel type (e.g. WHATSAPP)</summary>
        public string EventChannelType { get; set; } = "WHATSAPP";
    }
}
