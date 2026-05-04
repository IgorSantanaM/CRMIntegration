using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Application.Features.Campaigns.DTOs
{
    internal sealed record CsvContactDto(
        string Nome,
        string Cpf,
        string TipoCobranca,
        string Contrato,
        int DiasAtraso,
        string Status,
        string Equipe,
        string UltimoOperador
    );
}
