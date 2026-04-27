using CRMIntegration.Domain.Campaings.Enum;
using CRMIntegration.Domain.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMIntegration.Domain.Campaings
{
    public class Campaign : Entity<Guid>, IAggregateRoot
    {
        public string IdCampanhaVoll { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public DateTime DataDisparo { get; set; }
        public CampaignStatus Status { get; set; } = default;
        public int TotalContatos { get; set; } = 0;
        public string ChannelIdVoll { get; set; } = string.Empty;
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public int TotalEnviados { get; set; } = 0;
        public int TotalEntregues { get; set; } = 0;
        public int TotalFalhas { get; set; } = 0;
        public int TotalLidos { get; set; } = 0;
    }
}
