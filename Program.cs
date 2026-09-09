// ============================================================================
// PRINCÍPIO KISS (Keep It Simple, Stupid)
// O fluxo principal é direto e sem complexidade desnecessária.
// Montamos e conectamos os objetos de forma transparente e manual.
// ============================================================================
var notificador = new NotificadorConsole();
var processador = new ProcessadorDePedido(notificador);

var pedido = new Pedido
{
    Cliente = "Tio Bob",
    Valor = 200.00m
};

ResultadoProcessamento resultado = processador.Processar(pedido);

if (!resultado.Sucesso)
{
    Console.WriteLine($"[Erro] {resultado.Mensagem}");
}

public class ResultadoProcessamento
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

// ==============================================================================
// SRP (Single Responsibility Principle)
// Responsabilidade única: Representar os dados do pedido e validar suas regras.
// Ela não se preocupa com gravação em banco, telas ou envio de notificações.
// ==============================================================================
public class Pedido
{
    public required string Cliente {get; set; }
    public decimal Valor { get; set; }

    public bool EhValido()
    {
        return Valor > 0 && !string.IsNullOrWhiteSpace(Cliente);
    }

    public decimal AplicarTaxaEntrega()
    {
        if(Valor <= 199.99m)
        {
            Valor += 19.99m;
        }

        return Valor;
    }
}

// ============================================================================
// DIP (Dependency Inversion Principle) - A Abstração (Contrato)
// Módulos de alto nível e de baixo nível dependem desta interface,
// e não de implementações concretas diretas.
// ============================================================================
public interface INotificador
{
    void EnviarMensagem(string destinatario, string mensagem);
}

public class NotificadorConsole : INotificador
{
    public void EnviarMensagem(string destinatario, string mensagem)
    {
        Console.WriteLine($"[Notificação] Para {destinatario}: {mensagem}");
    }
}

// =================================================================================
//   SRP + DIP: O Serviço de Alto Nível
// - SRP: Sua única responsabilidade é orquestrar o processamento do pedido.
// - DIP: Recebe a abstração (INotificador) via construtor (Injeção de Dependência),
//   ficando totalmente desacoplado de como a notificação é enviada.
// =================================================================================
public class ProcessadorDePedido
{
    private readonly INotificador _notificador;

    public ProcessadorDePedido(INotificador notificador)
    {
        _notificador = notificador;
    }

   public ResultadoProcessamento Processar(Pedido pedido)
    {
        if (!pedido.EhValido())
        {
            return new ResultadoProcessamento
            {
                Sucesso = false,
                Mensagem = "Falha ao processar: dados do pedido são inválidos."
            };
        }

        pedido.AplicarTaxaEntrega();

        _notificador.EnviarMensagem(pedido.Cliente, $"Seu pedido foi processado com sucesso! Valor final: R$ {pedido.Valor:F2}");

        
        return new ResultadoProcessamento
        {
            Sucesso = true,
            Mensagem = "Pedido processado e taxa calculada com êxito."
        };
    }
}
