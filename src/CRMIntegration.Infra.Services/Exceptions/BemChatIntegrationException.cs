namespace CRMIntegration.Infra.Services.Exceptions
{
    /// <summary>
    /// Exceção específica da integração com o BemChat.
    /// Equivalente ao VollIntegrationException do projeto original.
    /// </summary>
    public class BemChatIntegrationException : Exception
    {
        public int StatusCode { get; }

        public BemChatIntegrationException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
