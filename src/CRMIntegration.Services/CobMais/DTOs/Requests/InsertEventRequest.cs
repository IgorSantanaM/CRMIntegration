namespace CRMIntegration.Services.CobMais.DTOs.Requests
{
    public record InsertEventRequest(string CodigoCliente,
        string? Contrato,
        string DataEvento,
        string TipoEvento,
        string DescricaoEvento,
        string? TipoCanal,
        int? Identificacao);
}
