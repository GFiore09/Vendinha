using System.ComponentModel.DataAnnotations;
using VendinhaUnimar.Enums;

namespace VendinhaUnimar.Models
{
    public class Divida
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        public SituacaoDivida Situacao { get; set; } = SituacaoDivida.Aberta;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataPagamento { get; set; }

        public void PrintDados()
        {
            Console.WriteLine("Id: {0}", Id);
            Console.WriteLine("ClienteId: {0}", ClienteId);
            Console.WriteLine("Valor: R$ {0:F2}", Valor);
            Console.WriteLine("Situacao: {0}", Situacao);
            Console.WriteLine("DataCriacao: {0}", DataCriacao.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("DataPagamento: {0}", DataPagamento.HasValue
                ? DataPagamento.Value.ToString("dd/MM/yyyy HH:mm")
                : "(não paga)");
        }
    }
}
