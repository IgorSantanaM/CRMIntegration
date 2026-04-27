using CRMIntegration.Domain.Clients.Events;
using CRMIntegration.Domain.Core.Exceptions;
using CRMIntegration.Domain.Core.Model;
using System.Text.RegularExpressions;

namespace CRMIntegration.Domain.Clients
{
    public class Client : Entity<Guid>, IAggregateRoot
    {
        public string IdVoll { get; private set; } = string.Empty;
        public int IdCobMais { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Whatsapp { get; private set; } = string.Empty;
        public string CPFCNPJ { get; private set; } = string.Empty;
        public int? IdTelefoneCobMais { get; private set; }
        public bool Acionavel { get; private set; } = true;
        public bool Ativo { get; private set; } = true;
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUltimoAcionamento { get; private set; }
        public DateTime? DataSincronizacaoVoll { get; private set; }
        public string? Email { get; private set; }

        private Client() { }

        public Client(int idCobMais,
            string nome,
            string whatsapp,
            string cpfCnpj,
            int? idTelefoneCobMais = null,
            string? email = null)
        {
            ValidateIdCobMais(idCobMais);
            ValidateName(nome);
            ValidateWhatsApp(whatsapp);
            ValidateCPFCNPJ(cpfCnpj);

            if (!string.IsNullOrWhiteSpace(email))
                ValidateEmail(email);

            Id = Guid.NewGuid();
            IdCobMais = idCobMais;
            Nome = nome.Trim();
            Whatsapp = ClearPhoneNumber(whatsapp);
            CPFCNPJ = LimparCPFCNPJ(cpfCnpj);
            IdTelefoneCobMais = idTelefoneCobMais;
            Email = email?.Trim();
            Acionavel = true;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;

            AddDomainEvent(new ClientCreatedEvent(
                Id,
                IdCobMais,
                Nome,
                Whatsapp,
                DataCriacao
            ));
        }

        public void SynchronizeWithVoll(string idVoll)
        {
            if (string.IsNullOrWhiteSpace(idVoll))
                throw new DomainException("O ID da Voll é obrigatório.");

            if (!string.IsNullOrEmpty(IdVoll) && IdVoll != idVoll)
                throw new DomainException("Este cliente já está sincronizado com outro ID na Voll.");

            IdVoll = idVoll;
            DataSincronizacaoVoll = DateTime.UtcNow;

            AddDomainEvent(new ClientSynchronizedVollEvent(
                Id,
                IdCobMais,
                idVoll,
                DataSincronizacaoVoll.Value
            ));
        }

        public void MarkAsTriggered()
        {
            if (!Acionavel)
                throw new DomainException("Este cliente já foi marcado como não acionável.");

            if (!Ativo)
                throw new DomainException("Não é possível acionar um cliente inativo.");

            DataUltimoAcionamento = DateTime.UtcNow;

            AddDomainEvent(new ClientTriggeredEvent(
                Id,
                IdCobMais,
                Whatsapp,
                DataUltimoAcionamento.Value
            ));
        }

        public void MarkAsNonActionable()
        {
            if (!Acionavel)
                return; 

            Acionavel = false;

            AddDomainEvent(new ClientMarkedAsNonActionableEvent(
                Id,
                IdCobMais,
                Whatsapp,
                DateTime.UtcNow
            ));
        }

        public void MarkAsActionable()
        {
            if (Acionavel)
                return; 

            if (!Ativo)
                throw new DomainException("Não é possível marcar como acionável um cliente inativo.");

            Acionavel = true;

            AddDomainEvent(new ClientMarkedAsActionableEvent(
                Id,
                IdCobMais,
                Whatsapp,
                DateTime.UtcNow
            ));
        }

        public void Inactivate(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentNullException(nameof(motivo));

            if (!Ativo)
                throw new DomainException("O cliente já está inativo.");

            Ativo = false;
            Acionavel = false;

            AddDomainEvent(new ClientInactivatedEvent(
                Id,
                IdCobMais,
                motivo,
                DateTime.UtcNow
            ));
        }

        public void Activate()
        {
            if (Ativo)
                throw new DomainException("O cliente já está ativo.");

            Ativo = true;
            Acionavel = true;

            AddDomainEvent(new ClientActivatedEvent(
                Id,
                IdCobMais,
                DateTime.UtcNow
            ));
        }

        public void UpdateEmail(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
                ValidateEmail(email);

            var emailAnterior = Email;
            Email = email?.Trim();

            if (emailAnterior != Email)
            {
                AddDomainEvent(new ClientEmailUpdatedEvent(
                    Id,
                    emailAnterior,
                    Email,
                    DateTime.UtcNow
                ));
            }
        }

        public void UpdateName(string nome)
        {
            ValidateName(nome);

            var nomeAnterior = Nome;
            Nome = nome.Trim();

            if (nomeAnterior != Nome)
            {
                AddDomainEvent(new ClientNameUpdatedEvent(
                    Id,
                    nomeAnterior,
                    Nome,
                    DateTime.UtcNow
                ));
            }
        }

        public void UpdateWhatsApp(string whatsapp, int? idTelefoneCobMais = null)
        {
            ValidateWhatsApp(whatsapp);

            var whatsappAnterior = Whatsapp;
            Whatsapp = ClearPhoneNumber(whatsapp);

            if (idTelefoneCobMais.HasValue)
                IdTelefoneCobMais = idTelefoneCobMais;

            if (whatsappAnterior != Whatsapp)
            {
                IdVoll = string.Empty;
                DataSincronizacaoVoll = null;

                AddDomainEvent(new ClientWhatsAppUpdatedEvent(
                    Id,
                    whatsappAnterior,
                    Whatsapp,
                    DateTime.UtcNow
                ));
            }
        }

        public bool CanBeActionable() => Acionavel && Ativo;

        public bool IsSynchronizedWithVoll() => !string.IsNullOrEmpty(IdVoll);

        public TimeSpan? TimeSinceLastAction() =>
            DataUltimoAcionamento.HasValue
                ? DateTime.UtcNow - DataUltimoAcionamento.Value
                : null;

        public bool WasRecentlyActioned(int daysToConsiderRecent = 7) =>
            DataUltimoAcionamento.HasValue &&
            DateTime.UtcNow - DataUltimoAcionamento.Value <= TimeSpan.FromDays(daysToConsiderRecent);

        public bool NeedsResynchronizationWithVoll(int daysToExpire = 30) =>
            !DataSincronizacaoVoll.HasValue ||
            DateTime.UtcNow - DataSincronizacaoVoll.Value > TimeSpan.FromDays(daysToExpire);

        #region Validations
        private static void ValidateIdCobMais(int id)
        {
            if (id <= 0)
                throw new DomainException("O ID do CobMais deve ser maior que zero.");
        }

        private static void ValidateName(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do cliente é obrigatório.");

            if (nome.Trim().Length < 3)
                throw new DomainException("O nome do cliente deve ter no mínimo 3 caracteres.");

            if (nome.Length > 200)
                throw new DomainException("O nome do cliente deve ter no máximo 200 caracteres.");
        }

        private static void ValidateWhatsApp(string whatsapp)
        {
            if (string.IsNullOrWhiteSpace(whatsapp))
                throw new DomainException("O número de WhatsApp é obrigatório.");

            var numero = ClearPhoneNumber(whatsapp);

            if (!Regex.IsMatch(numero, @"^55\d{10,11}$"))
                throw new DomainException("O número de WhatsApp deve estar no formato brasileiro válido (Ex: 5511987654321).");

            if (numero.Length < 12 || numero.Length > 13)
                throw new DomainException("O número de WhatsApp deve ter entre 12 e 13 dígitos (incluindo código do país).");
        }

        private static void ValidateCPFCNPJ(string cpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                throw new DomainException("O CPF/CNPJ é obrigatório.");

            var documento = LimparCPFCNPJ(cpfCnpj);

            if (documento.Length != 11 && documento.Length != 14)
                throw new DomainException("O CPF deve ter 11 dígitos ou o CNPJ deve ter 14 dígitos.");

            if (documento.Length == 11 && !ValidateCPF(documento))
                throw new DomainException("CPF inválido.");

            if (documento.Length == 14 && !ValidateCNPJ(documento))
                throw new DomainException("CNPJ inválido.");
        }

        private static void ValidateEmail(string email)
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new DomainException("O email informado é inválido.");

            if (email.Length > 100)
                throw new DomainException("O email deve ter no máximo 100 caracteres.");
        }

        private static string ClearPhoneNumber(string numero) =>
            Regex.Replace(numero, @"[^\d]", "");

        private static string LimparCPFCNPJ(string cpfCnpj) =>
            Regex.Replace(cpfCnpj, @"[^\d]", "");

        private static bool ValidateCPF(string cpf)
        {
            if (cpf.All(c => c == cpf[0])) return false; 

            var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        private static bool ValidateCNPJ(string cnpj)
        {
            if (cnpj.All(c => c == cnpj[0])) return false; 

            var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }
        #endregion
    }
}
