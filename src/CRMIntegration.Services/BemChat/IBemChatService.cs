using CRMIntegration.Services.BemChat.DTOs.Requests;
using CRMIntegration.Services.BemChat.DTOs.Responses;

namespace CRMIntegration.Services.BemChat
{
    public interface IBemChatService
    {
        /// <summary>
        /// [POST /api/messages/send]
        /// Envia uma mensagem de texto simples para um único destinatário.
        /// </summary>
        Task<BemChatSendMessageResponse> SendTextMessageAsync(
            SendTextMessageRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// [POST /api/messages/send  (multipart/form-data)]
        /// Envia mensagem com mídia (imagem, documento, áudio, vídeo).
        /// </summary>
        Task<BemChatSendMessageResponse> SendMediaMessageAsync(
            SendMediaMessageRequest request,
            CancellationToken cancellationToken = default);
    }
}
