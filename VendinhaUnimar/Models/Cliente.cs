using System.ComponentModel.DataAnnotations;

namespace VendinhaUnimar.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, MinimumLength = 5)]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos")]
        public string Cpf { get; set; }

        public DateTime DataNascimento { get; set; }

        [Range(0, 150)]
        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var totalAnos = hoje.Year - DataNascimento.Year;
                var aniversarioAnoAtual = DataNascimento.AddYears(totalAnos);

                if (aniversarioAnoAtual > hoje)
                {
                    totalAnos--;
                }

                return totalAnos;
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = string.IsNullOrWhiteSpace(value) ? null : value.ToLower(); }
        }

        public virtual void PrintDados()
        {
            Console.WriteLine("Id: {0}", Id);
            Console.WriteLine("Nome: {0}", NomeCompleto);
            Console.WriteLine("CPF: {0}", Cpf);
            Console.WriteLine("DataNascimento: {0}", DataNascimento.ToShortDateString());
            Console.WriteLine("Idade: {0}", Idade);
            Console.WriteLine("Email: {0}", Email ?? "(não informado)");
        }
    }
}
