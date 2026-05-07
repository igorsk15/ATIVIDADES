using System;
using System.Collections.Generic;
using System.Linq; // Necessário para usar FirstOrDefault

namespace ControleAcesso
{
    // Níveis de acesso do sistema
    public enum NivelAcesso
    {
        visitante = 1,
        Funcionario = 2,
        Administrador = 3
    }

    // Classe de usuário
    public class Usuario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Senha { get; set; }
        public NivelAcesso Nivel { get; set; }
    }

    // Classe de área protegida
    public class Area
    {
        public int Id { get; set; }
        public string? NomeArea { get; set; }
        public NivelAcesso NivelMinimo { get; set; }
    }

    // Registro de entrada em áreas
    public class RegistroAcesso
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public string? NomeUsuario { get; set; }

        public int AreaId { get; set; }
        public string? Area { get; set; }

        // Data e hora do acesso
        public DateTime DataHora { get; set; } = DateTime.Now;

        // Define se foi permitido ou não
        public bool Autorizado { get; set; }
    }

    // Sistema principal de controle
    public class SistemaControle
    {
        // Histórico de acessos
        public List<RegistroAcesso> Historico { get; set; }
            = new List<RegistroAcesso>();

        // Verifica se o usuário pode acessar a área
        public bool ProcessarAcesso(Usuario usuario, Area area)
        {
            bool podeAcessar =
                (int)usuario.Nivel >= (int)area.NivelMinimo;

            // Cria novo registro de acesso
            var novoLog = new RegistroAcesso
            {
                Id = Historico.Count + 1,

                UsuarioId = usuario.Id,
                NomeUsuario = usuario.Nome,

                AreaId = area.Id,
                Area = area.NomeArea,

                DataHora = DateTime.Now,
                Autorizado = podeAcessar
            };

            // Salva no histórico
            Historico.Add(novoLog);

            return podeAcessar;
        }
    }

    class Program
    {
        // Lista de usuários cadastrados
        static List<Usuario> BancoUsuarios =
            new List<Usuario>();

        // Histórico geral do sistema
        static List<RegistroAcesso> HistoricoGeral =
            new List<RegistroAcesso>();

        static void Main()
        {
            // Tela de cadastro
            TelaCadastro();

            // Tela de login
            Usuario? logado = TelaLogin();

            // Verifica se login foi feito
            if (logado != null)
            {
                Console.Clear();

                Console.WriteLine(
                    $"Bem-vindo, {logado.Nome}!"
                );

                // Apenas administrador pode ver histórico
                if (logado.Nivel == NivelAcesso.Administrador)
                {
                    SimularMovimentacoes(logado);
                    ExibirHistorico();
                }
                else
                {
                    Console.WriteLine(
                        "\nVocê não tem permissão de Administrador para ver o histórico."
                    );
                }
            }

            Console.WriteLine(
                "\nPressione qualquer tecla para sair..."
            );

            Console.ReadKey();
        }

        // Cadastro de novo usuário
        static void TelaCadastro()
        {
            Console.WriteLine("-=- CADASTRO DE USUÁRIO -=-");

            Usuario novo = new Usuario();

            // Gera ID automaticamente
            novo.Id = BancoUsuarios.Count + 1;

            Console.Write("Nome: ");
            novo.Nome = Console.ReadLine();

            Console.Write("CPF: ");
            novo.CPF = Console.ReadLine();

            Console.Write("Senha: ");
            novo.Senha = Console.ReadLine();

            Console.WriteLine(
                "Nível (1-Visitante, 2-Funcionario, 3-Administrador): "
            );

            // Converte nível digitado
            if (int.TryParse(Console.ReadLine(), out int nivel))
            {
                novo.Nivel = (NivelAcesso)nivel;
            }
            else
            {
                // Define visitante como padrão
                novo.Nivel = NivelAcesso.visitante;
            }

            // Salva usuário
            BancoUsuarios.Add(novo);

            Console.WriteLine(
                "\nUsuário cadastrado com sucesso!\n"
            );
        }

        // Tela de login
        static Usuario? TelaLogin()
        {
            Console.WriteLine("-=- LOGIN -=-");

            Console.Write("CPF: ");
            string? id = Console.ReadLine();

            Console.Write("Senha: ");
            string? pass = Console.ReadLine();

            // Procura usuário no banco
            Usuario? user = BancoUsuarios.FirstOrDefault(
                u => u.CPF == id && u.Senha == pass
            );

            // Verifica se encontrou
            if (user == null)
            {
                Console.WriteLine(
                    "\nErro: Usuário ou senha inválidos."
                );

                return null;
            }

            return user;
        }

        // Exibe histórico de acessos
        static void ExibirHistorico()
        {
            Console.WriteLine(
                "\n-=- HISTÓRICO DE MOVIMENTAÇÃO -=-"
            );

            // Verifica se existe histórico
            if (HistoricoGeral.Count == 0)
            {
                Console.WriteLine(
                    "Nenhum registro encontrado."
                );
            }
            else
            {
                foreach (var log in HistoricoGeral)
                {
                    string status =
                        log.Autorizado
                        ? "AUTORIZADO"
                        : "NEGADO";

                    Console.WriteLine(
                        $"[{log.DataHora:dd/MM/yyyy HH:mm:ss}] " +
                        $"Usuário: {log.NomeUsuario} | " +
                        $"Área: {log.Area} | " +
                        $"Status: {status}"
                    );
                }
            }
        }

        // Simula acessos ao sistema
        static void SimularMovimentacoes(Usuario user)
        {
            HistoricoGeral.Add(
                new RegistroAcesso
                {
                    NomeUsuario = user.Nome, //Puxa o nome de quem logou
                    Area = "Data Center",
                    DataHora = DateTime.Now.AddMinutes(-10),
                    Autorizado = false
                }
            );

            HistoricoGeral.Add(
                new RegistroAcesso
                {
                    NomeUsuario = user.Nome, //Puxa o nome de quem logou
                    Area = "Data Center",
                    DataHora = DateTime.Now.AddMinutes(-5),
                    Autorizado = true
                }
            );
        }
    }
}
