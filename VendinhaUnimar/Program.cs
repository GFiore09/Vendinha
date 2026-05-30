using VendinhaUnimar.Models;
using VendinhaUnimar.Services;

var clienteService = new ClienteService();
var dividaService = new DividaService();

while (true)
{
    Console.ReadKey();
    Console.Clear();
    Console.WriteLine("=== VENDINHA ===");
    Console.WriteLine("1 - Cadastrar cliente");
    Console.WriteLine("2 - Listar clientes");
    Console.WriteLine("3 - Buscar cliente por ID");
    Console.WriteLine("4 - Atualizar cliente");
    Console.WriteLine("5 - Excluir cliente");
    Console.WriteLine("10 - Pesquisar clientes");
    Console.WriteLine("---");
    Console.WriteLine("6 - Cadastrar dívida");
    Console.WriteLine("7 - Listar dívidas por cliente");
    Console.WriteLine("8 - Marcar dívida como paga");
    Console.WriteLine("9 - Excluir dívida");
    Console.WriteLine("================");
    Console.WriteLine("Digite a opcao:");

    int opcao;
    try
    {
        opcao = int.Parse(Console.ReadLine());
    }
    catch (Exception)
    {
        Console.WriteLine("Opção inválida");
        continue;
    }

    if (opcao == 1)
    {
        Console.WriteLine("Digite o nome completo:");
        var nomeCompleto = Console.ReadLine();

        Console.WriteLine("Digite o CPF (somente números ou com pontuação):");
        var cpf = Console.ReadLine();

        Console.WriteLine("Digite a data de nascimento (dd/MM/yyyy):");
        var dataNascimento = DateTime.Parse(Console.ReadLine());

        Console.WriteLine("Digite o email (opcional, pressione Enter para pular):");
        var email = Console.ReadLine();

        var cliente = new Cliente
        {
            NomeCompleto = nomeCompleto,
            Cpf = cpf,
            DataNascimento = dataNascimento,
            Email = string.IsNullOrWhiteSpace(email) ? null : email
        };

        var sucesso = clienteService.Criar(cliente, out _);

        if (sucesso)
        {
            Console.WriteLine("Cliente cadastrado com sucesso!");
            cliente.PrintDados();
        }
        else
        {
            Console.WriteLine("Erro durante o cadastro do cliente!");
        }
    }
    else if (opcao == 2)
    {
        var clientes = clienteService.Listar();
        if (clientes.Count == 0)
        {
            Console.WriteLine("Nenhum cliente cadastrado.");
        }
        else
        {
            foreach (var item in clientes)
            {
                item.PrintDados();
                Console.WriteLine("==================");
            }
        }
    }
    else if (opcao == 3)
    {
        Console.WriteLine("Digite o ID do cliente:");
        var id = int.Parse(Console.ReadLine());

        var clienteEncontrado = clienteService.BuscaPorId(id);
        if (clienteEncontrado == null)
        {
            Console.WriteLine("Cliente não encontrado.");
        }
        else
        {
            clienteEncontrado.PrintDados();
        }
    }
    else if (opcao == 4)
    {
        Console.WriteLine("Digite o ID do cliente:");
        var id = int.Parse(Console.ReadLine());

        var clienteEncontrado = clienteService.BuscaPorId(id);
        if (clienteEncontrado == null)
        {
            Console.WriteLine("Cliente não encontrado.");
        }
        else
        {
            clienteEncontrado.PrintDados();
            Console.WriteLine("Digite o novo nome completo:");
            var novoNome = Console.ReadLine();
            Console.WriteLine("Digite o novo email (opcional, pressione Enter para limpar):");
            var novoEmail = Console.ReadLine();

            var sucesso = clienteService.Atualizar(id, novoNome, novoEmail, out _);
            Console.WriteLine(sucesso ? "Cliente atualizado com sucesso!" : "Erro ao atualizar cliente.");
        }
    }
    else if (opcao == 5)
    {
        Console.WriteLine("Digite o ID do cliente:");
        var id = int.Parse(Console.ReadLine());

        var sucesso = clienteService.Excluir(id);
        Console.WriteLine(sucesso ? "Cliente excluído com sucesso!" : "Cliente não encontrado.");
    }
    // LINQ - Language Integrated Query
    else if (opcao == 10)
    {
        Console.Write("Digite a pesquisa (nome, CPF ou email): ");
        var pesquisa = Console.ReadLine();
        var resultado = clienteService.Listar(pesquisa);

        if (resultado.Count == 0)
        {
            Console.WriteLine("Nenhum cliente encontrado.");
        }
        else
        {
            foreach (var item in resultado)
            {
                item.PrintDados();
                Console.WriteLine("==================");
            }
        }
    }
    else if (opcao == 6)
    {
        Console.WriteLine("Digite o ID do cliente:");
        var clienteId = int.Parse(Console.ReadLine());

        Console.WriteLine("Digite o valor da dívida:");
        var valor = decimal.Parse(Console.ReadLine());

        var divida = new Divida
        {
            ClienteId = clienteId,
            Valor = valor
        };

        var sucesso = dividaService.Criar(divida, out _);

        if (sucesso)
        {
            Console.WriteLine("Dívida cadastrada com sucesso!");
        }
        else
        {
            Console.WriteLine("Erro durante o cadastro da dívida!");
        }
    }
    else if (opcao == 7)
    {
        Console.WriteLine("Digite o ID do cliente:");
        var clienteId = int.Parse(Console.ReadLine());

        var dividas = dividaService.ListarPorCliente(clienteId);

        if (dividas.Count == 0)
        {
            Console.WriteLine("Nenhuma dívida encontrada para este cliente.");
        }
        else
        {
            foreach (var item in dividas)
            {
                item.PrintDados();
                Console.WriteLine("==================");
            }
        }
    }
    else if (opcao == 8)
    {
        Console.WriteLine("Digite o ID da dívida:");
        var id = int.Parse(Console.ReadLine());

        var sucesso = dividaService.MarcarComoPaga(id);
        Console.WriteLine(sucesso ? "Dívida marcada como paga!" : "Dívida não encontrada ou já paga.");
    }
    else if (opcao == 9)
    {
        Console.WriteLine("Digite o ID da dívida:");
        var id = int.Parse(Console.ReadLine());

        var sucesso = dividaService.Excluir(id);
        Console.WriteLine(sucesso ? "Dívida excluída com sucesso!" : "Dívida não encontrada.");
    }
}
