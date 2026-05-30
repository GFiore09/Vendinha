using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using VendinhaUnimar.Enums;
using VendinhaUnimar.Models;

namespace VendinhaUnimar.Services
{
    public class DividaService
    {
        private static string connectionString =
            "Server=(localdb)\\mssqllocaldb;" +
            "Database=vendinha;" +
            "Trusted_Connection=True;";

       
        public bool Criar(Divida divida, out List<ValidationResult> listaErros)
        {
            if (Validar(divida, out listaErros) == false)
            {
                return false;
            }

            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            // Verificar se o cliente existe
            var verificarCliente = new SqlCommand(
                "SELECT COUNT(1) FROM Clientes WHERE Id = @ClienteId", conexao);
            verificarCliente.Parameters.Add("@ClienteId", SqlDbType.Int).Value = divida.ClienteId;
            var clienteExiste = (int)verificarCliente.ExecuteScalar();

            if (clienteExiste == 0)
            {
                listaErros.Add(new ValidationResult("Cliente não encontrado", new[] { "ClienteId" }));
                Console.WriteLine("ClienteId: Cliente não encontrado");
                return false;
            }

            // Verificar se já tem dívida aberta
            var verificarDivida = new SqlCommand(
                "SELECT COUNT(1) FROM Dividas WHERE ClienteId = @ClienteId AND Situacao = 'Aberta'",
                conexao);
            verificarDivida.Parameters.Add("@ClienteId", SqlDbType.Int).Value = divida.ClienteId;
            var dividasAbertas = (int)verificarDivida.ExecuteScalar();

            if (dividasAbertas > 0)
            {
                listaErros.Add(new ValidationResult(
                    "O cliente já possui uma dívida em aberto",
                    new[] { "ClienteId" }));
                Console.WriteLine("ClienteId: O cliente já possui uma dívida em aberto");
                return false;
            }

            var comando =
                "INSERT INTO Dividas (ClienteId, Valor, Situacao, DataCriacao, DataPagamento) " +
                "VALUES (@ClienteId, @Valor, @Situacao, @DataCriacao, NULL)";

            var sqlCommand = new SqlCommand(comando, conexao);
            sqlCommand.Parameters.Add("@ClienteId", SqlDbType.Int).Value = divida.ClienteId;
            sqlCommand.Parameters.Add("@Valor", SqlDbType.Decimal).Value = divida.Valor;
            sqlCommand.Parameters.Add("@Situacao", SqlDbType.VarChar).Value = "Aberta";
            sqlCommand.Parameters.Add("@DataCriacao", SqlDbType.DateTime2).Value = DateTime.Now;

            sqlCommand.ExecuteNonQuery();
            return true;
        }

        public bool Validar(Divida divida, out List<ValidationResult> listaErros)
        {
            var contexto = new ValidationContext(divida);
            var erros = new List<ValidationResult>();
            listaErros = erros;

            var objetoValido = Validator.TryValidateObject(divida, contexto, erros, true);

            if (divida.ClienteId <= 0)
            {
                erros.Add(new ValidationResult("ClienteId inválido", new[] { "ClienteId" }));
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

        public List<Divida> ListarPorCliente(int clienteId)
        {
            var sqlCommand =
                "SELECT Id, ClienteId, Valor, Situacao, DataCriacao, DataPagamento " +
                "FROM Dividas " +
                "WHERE ClienteId = @ClienteId " +
                "ORDER BY DataCriacao DESC";

            var conexao = new SqlConnection(connectionString);
            conexao.Open();
            var sqlConsulta = new SqlCommand(sqlCommand, conexao);
            sqlConsulta.Parameters.Add("@ClienteId", SqlDbType.Int).Value = clienteId;

            var leitor = sqlConsulta.ExecuteReader();
            var lista = new List<Divida>();

            while (leitor.Read())
            {
                var divida = new Divida
                {
                    Id = leitor.GetInt32(0),
                    ClienteId = leitor.GetInt32(1),
                    Valor = leitor.GetDecimal(2),
                    Situacao = leitor.GetString(3) == "Paga"
                        ? SituacaoDivida.Paga
                        : SituacaoDivida.Aberta,
                    DataCriacao = leitor.GetDateTime(4),
                    DataPagamento = leitor.IsDBNull(5) ? null : leitor.GetDateTime(5)
                };
                lista.Add(divida);
            }

            return lista;
        }

        public Divida BuscaPorId(int id)
        {
            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var sqlCommand = new SqlCommand(
                "SELECT Id, ClienteId, Valor, Situacao, DataCriacao, DataPagamento FROM Dividas WHERE Id = @Id",
                conexao);
            sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var leitor = sqlCommand.ExecuteReader();
            if (!leitor.Read())
            {
                return null;
            }

            return new Divida
            {
                Id = leitor.GetInt32(0),
                ClienteId = leitor.GetInt32(1),
                Valor = leitor.GetDecimal(2),
                Situacao = leitor.GetString(3) == "Paga"
                    ? SituacaoDivida.Paga
                    : SituacaoDivida.Aberta,
                DataCriacao = leitor.GetDateTime(4),
                DataPagamento = leitor.IsDBNull(5) ? null : leitor.GetDateTime(5)
            };
        }

        public bool MarcarComoPaga(int id)
        {
            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var comando = new SqlCommand(
                "UPDATE Dividas SET Situacao = 'Paga', DataPagamento = @DataPagamento " +
                "WHERE Id = @Id AND Situacao = 'Aberta'",
                conexao);
            comando.Parameters.Add("@DataPagamento", SqlDbType.DateTime2).Value = DateTime.Now;
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            return comando.ExecuteNonQuery() > 0;
        }

        public bool Excluir(int id)
        {
            var conexao = new SqlConnection(connectionString);
            conexao.Open();

            var comando = new SqlCommand("DELETE FROM Dividas WHERE Id = @Id", conexao);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            return comando.ExecuteNonQuery() > 0;
        }
    }
}
