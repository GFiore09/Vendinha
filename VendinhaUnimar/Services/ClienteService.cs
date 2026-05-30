using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using VendinhaUnimar.Models;
using VendinhaUnimar.Utils;

namespace VendinhaUnimar.Services
{
    public class ClienteService
    {
        private static string connectionString =
            "Server=(localdb)\\mssqllocaldb;" +
            "Database=vendinha;" +
            "Trusted_Connection=True;";

        
        public bool Criar(Cliente cliente, out List<ValidationResult> listaErros)
        {
            if (Validar(cliente, out listaErros) == false)
            {
                return false;
            }

            var cpf = DocumentoUtils.SomenteNumeros(cliente.Cpf);

            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            
            var verificar = new SqlCommand("SELECT COUNT(1) FROM Clientes WHERE Cpf = @Cpf", conexao);
            verificar.Parameters.Add("@Cpf", SqlDbType.VarChar).Value = cpf;
            var existe = (int)verificar.ExecuteScalar();

            if (existe > 0)
            {
                listaErros.Add(new ValidationResult("Já existe um cliente com este CPF", new[] { "Cpf" }));
                Console.WriteLine("Cpf: Já existe um cliente com este CPF");
                return false;
            }

            var comando =
                "INSERT INTO Clientes (NomeCompleto, Cpf, DataNascimento, Email) " +
                "VALUES (@NomeCompleto, @Cpf, @DataNascimento, @Email)";

            var sqlCommand = new SqlCommand(comando, conexao);
            sqlCommand.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar).Value = cliente.NomeCompleto;
            sqlCommand.Parameters.Add("@Cpf", SqlDbType.VarChar).Value = cpf;
            sqlCommand.Parameters.Add("@DataNascimento", SqlDbType.Date).Value = cliente.DataNascimento;
            sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value =
                (object)cliente.Email ?? DBNull.Value;

            sqlCommand.ExecuteNonQuery();
            return true;
        }

        public bool Validar(Cliente cliente, out List<ValidationResult> listaErros)
        {
            var contexto = new ValidationContext(cliente);
            var erros = new List<ValidationResult>();
            listaErros = erros;

            var objetoValido = Validator.TryValidateObject(cliente, contexto, erros, true);

            if (!DocumentoUtils.CpfValido(cliente.Cpf))
            {
                erros.Add(new ValidationResult("CPF inválido", new[] { "Cpf" }));
                objetoValido = false;
            }

            if (!string.IsNullOrWhiteSpace(cliente.Email) && !cliente.Email.Contains("@"))
            {
                erros.Add(new ValidationResult("E-mail inválido", new[] { "Email" }));
                objetoValido = false;
            }

            if (!objetoValido)
            {
                foreach (var erro in erros)
                {
                    Console.WriteLine("{0}: {1}",
                        erro.MemberNames.FirstOrDefault() ?? "Erro",
                        erro.ErrorMessage);
                }
            }

            return objetoValido;
        }

        public List<Cliente> Listar()
        {
            var sqlCommand =
                "SELECT Id, NomeCompleto, Cpf, DataNascimento, Email " +
                "FROM Clientes " +
                "ORDER BY NomeCompleto ASC";

            var conexao = new SqlConnection(connectionString);
            conexao.Open();
            var sqlConsulta = new SqlCommand(sqlCommand, conexao);
            var leitor = sqlConsulta.ExecuteReader();

            var lista = new List<Cliente>();
            while (leitor.Read())
            {
                var cliente = new Cliente
                {
                    Id = leitor.GetInt32(0),
                    NomeCompleto = leitor.GetString(1),
                    Cpf = leitor.GetString(2),
                    DataNascimento = leitor.GetDateTime(3),
                    Email = leitor.IsDBNull(4) ? null : leitor.GetString(4)
                };
                lista.Add(cliente);
            }

            return lista;
        }

        public List<Cliente> Listar(string pesquisa)
        {
            
            var resultado = Listar()
                .Where(item =>
                    item.NomeCompleto.Contains(pesquisa, StringComparison.OrdinalIgnoreCase) ||
                    item.Cpf.Contains(pesquisa) ||
                    (item.Email != null && item.Email.Contains(pesquisa, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(item => item.NomeCompleto);

            return resultado.ToList();
        }

        public Cliente BuscaPorId(int id)
        {
            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var sqlCommand = new SqlCommand(
                "SELECT Id, NomeCompleto, Cpf, DataNascimento, Email FROM Clientes WHERE Id = @Id",
                conexao);
            sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var leitor = sqlCommand.ExecuteReader();
            if (!leitor.Read())
            {
                return null;
            }

            return new Cliente
            {
                Id = leitor.GetInt32(0),
                NomeCompleto = leitor.GetString(1),
                Cpf = leitor.GetString(2),
                DataNascimento = leitor.GetDateTime(3),
                Email = leitor.IsDBNull(4) ? null : leitor.GetString(4)
            };
        }

        public bool Atualizar(int id, string novoNome, string novoEmail, out List<ValidationResult> listaErros)
        {
            listaErros = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(novoNome))
            {
                listaErros.Add(new ValidationResult("Nome é obrigatório", new[] { "NomeCompleto" }));
                Console.WriteLine("NomeCompleto: Nome é obrigatório");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(novoEmail) && !novoEmail.Contains("@"))
            {
                listaErros.Add(new ValidationResult("E-mail inválido", new[] { "Email" }));
                Console.WriteLine("Email: E-mail inválido");
                return false;
            }

            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var comando = new SqlCommand(
                "UPDATE Clientes SET NomeCompleto = @NomeCompleto, Email = @Email WHERE Id = @Id",
                conexao);
            comando.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar).Value = novoNome;
            comando.Parameters.Add("@Email", SqlDbType.VarChar).Value =
                string.IsNullOrWhiteSpace(novoEmail) ? (object)DBNull.Value : novoEmail.ToLower();
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var linhas = comando.ExecuteNonQuery();
            return linhas > 0;
        }

        public bool Excluir(int id)
        {
            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var comando = new SqlCommand("DELETE FROM Clientes WHERE Id = @Id", conexao);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            return comando.ExecuteNonQuery() > 0;
        }
    }
}
