using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Services.BemChat.DTOs.Requests
{
    public record SendTextMessageRequest(
          /// <summary>
          /// Número do destinatário com DDI. Ex: 5567999990001
          /// </summary>
          string Number,

          /// <summary>
          /// Texto da mensagem a ser enviada.
          /// </summary>
          string Body,

          /// <summary>
          /// (Opcional) ID da conta WhatsApp cadastrada no BemChat.
          /// Quando nulo, usa a conexão padrão da instância.
          /// </summary>
          int? WhatsAppId = null
      );
}
