using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Infra.Services.BemChat
{
    public class BemChatOptions
    {
        /// <summary>x
        /// URL base da instância BemChat do cliente.
        /// Formato: https://DOMINIO_BEMCHAT.api.bemchat.com.br
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Token de autenticação obtido em Configurações > API no painel BemChat.
        /// Enviado via header: Authorization: Bearer {Token}
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// ID da conexão WhatsApp padrão cadastrada no BemChat.
        /// Pode ser sobrescrito por requisição.
        /// </summary>
        public int? DefaultWhatsAppId { get; set; }

        /// <summary>
        /// Delay padrão em milissegundos entre envios no modo bulk.
        /// Padrão: 3000ms. Recomenda-se mínimo de 2000ms para evitar banimento.
        /// </summary>
        public int DefaultDelayBetweenSendsMs { get; set; } = 3000;
    }
}
