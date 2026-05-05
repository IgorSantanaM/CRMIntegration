using CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaign;
using CRMIntegration.Application.Features.Campaigns.DTOs;
using CRMIntegration.Domain.Campaings;
using CRMIntegration.Domain.Core.Data;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Infra.Services.Voll;
using CRMIntegration.Services.CobMais;
using CRMIntegration.Services.CobMais.DTOs;
using CRMIntegration.Services.CobMais.DTOs.Requests;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CRMIntegration.Application.Features.Campaigns.Commands.TriggerCampaignFromCsv
{
    public class TriggerCampaignFromCsvCommandHandler(
         IPublishEndpoint publishEndpoint,
         ICobMaisService cobMaisService,
         ICampaignRepository campaignRepository,
         IUnitOfwork unitOfWork,
         VollOptions vollOptions,
         ILogger<TriggerCampaignFromCsvCommandHandler> logger)
         : IRequestHandler<TriggerCampaignFromCsvCommand, TriggerCampaignFromCsvResponse>
    {
        public async Task<TriggerCampaignFromCsvResponse> Handle(
            TriggerCampaignFromCsvCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await campaignRepository.ExistsWithNameTodayAsync(
                request.TemplateName, cancellationToken);

            if (exists)
                throw new DomainException(
                    "Campaign already triggered today for this template.");

            var (parsed, skipped) = await ParseCsvAsync(request.CsvFile, cancellationToken);

            logger.LogInformation(
                "[CSV] Parsed {Total} rows. Valid={Valid} Skipped={Skipped}",
                parsed.Count + skipped, parsed.Count, skipped);

            if (parsed.Count == 0)
                throw new DomainException(
                    "CSV contains no valid contacts after parsing.");

            var cpfSet = parsed
                .Select(c => NormalizeCpf(c.Cpf))
                .Where(cpf => !string.IsNullOrEmpty(cpf))
                .ToHashSet();

            logger.LogInformation(
                "[CobMais] Fetching actionable contacts for {Count} CPFs.", cpfSet.Count);

            var cobMaisContacts = await cobMaisService.GetActionableContactsAsync(
                new GetActionableContactsRequest(null, null),
                cancellationToken);

            var cobMaisIndex = cobMaisContacts
                .Where(c => !string.IsNullOrEmpty(c.CpfCnpj))
                .GroupBy(c => NormalizeCpf(c.CpfCnpj))
                .ToDictionary(g => g.Key, g => g.First());

            var finalContacts = new List<(CsvContactDto Csv, ActionableContactDto CobMais)>();
            var unmatchedCount = 0;

            foreach (var csvRow in parsed)
            {
                var normalizedCpf = NormalizeCpf(csvRow.Cpf);

                if (!cobMaisIndex.TryGetValue(normalizedCpf, out var cobMaisContact))
                {
                    logger.LogWarning(
                        "[CSV] CPF {Cpf} not found in CobMais actionable contacts. Skipping.",
                        csvRow.Cpf);
                    unmatchedCount++;
                    continue;
                }

                finalContacts.Add((csvRow, cobMaisContact));
            }

            logger.LogInformation(
                "[CSV] Matched {Matched} contacts. Unmatched={Unmatched}",
                finalContacts.Count, unmatchedCount);

            if (finalContacts.Count == 0)
                throw new DomainException(
                    "No CSV contacts could be matched against CobMais actionable contacts.");

            var campaign = new Campaign(
                nome: $"CSV_{request.TemplateName}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                template: request.TemplateName,
                channelIdVoll: vollOptions.ChannelId,
                dataDisparo: DateTime.UtcNow,
                totalContatos: finalContacts.Count
            );

            campaign.StartProcessing();

            await campaignRepository.AddAsync(campaign);

            logger.LogInformation(
                "[Campaign] Created {CampaignId} with {Total} contacts.",
                campaign.Id, finalContacts.Count);

            foreach (var (csv, cobMais) in finalContacts)
            {
                await publishEndpoint.Publish<SendContactMessageCommand>(
                    new SendContactMessageCommand(
                        CampaignId: campaign.Id,
                        CorrelationId: Guid.NewGuid(),
                        IdPessoa: cobMais.IdPessoa,
                        CpfCnpj: cobMais.CpfCnpj,
                        Nome: cobMais.Nome,
                        PhoneNumber: cobMais.PhoneNumber,
                        PhoneId: cobMais.PhoneId,
                        TemplateName: request.TemplateName
                    ), cancellationToken);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "[Campaign] All {Count} messages published. CampaignId={CampaignId}",
                finalContacts.Count, campaign.Id);

            return new TriggerCampaignFromCsvResponse(
                CampaignId: campaign.Id,
                TotalContacts: finalContacts.Count,
                Skipped: skipped + unmatchedCount,
                Message: $"Campaign triggered. {finalContacts.Count} messages queued."
            );
        }

        private static async Task<(List<CsvContactDto> Valid, int Skipped)> ParseCsvAsync(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            var valid = new List<CsvContactDto>();
            var skipped = 0;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
            {
                line = line.TrimStart(',').Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    skipped++;
                    continue;
                }

                var cols = line.Split(',');

                if (cols.Length < 8)
                {
                    skipped++;
                    continue;
                }

                var nome = cols[0].Trim();
                var cpf = cols[1].Trim();
                var tipoCobranca = cols[2].Trim().ToUpperInvariant();
                var contrato = cols[3].Trim();
                var diasAtrasoRaw = cols[4].Trim();
                var status = cols[5].Trim().ToUpperInvariant();
                var equipe = cols[6].Trim();
                var ultimoOperador = cols[7].Trim();

                if (string.IsNullOrWhiteSpace(cpf))
                {
                    skipped++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(nome))
                {
                    skipped++;
                    continue;
                }

                if (status != "ANDAMENTO" && status != "NOVO")
                {
                    skipped++;
                    continue;
                }

                if (!int.TryParse(diasAtrasoRaw, out var diasAtraso))
                    diasAtraso = 0;

                valid.Add(new CsvContactDto(
                    Nome: nome,
                    Cpf: cpf,
                    TipoCobranca: tipoCobranca,
                    Contrato: contrato,
                    DiasAtraso: diasAtraso,
                    Status: status,
                    Equipe: equipe,
                    UltimoOperador: ultimoOperador
                ));
            }

            return (valid, skipped);
        }

        private static string NormalizeCpf(string cpf) =>
            new(cpf.Where(char.IsDigit).ToArray());
    }
}
