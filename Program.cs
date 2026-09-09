// ============================================================================
// PRINCÍPIO KISS (Keep It Simple, Stupid)
// Instanciação direta e clara dos componentes.
// ============================================================================

// Graças ao DIP podemos alternar facilmente entre:
// INotificador notificador = new NotificadorConsole(); ou INotificador notificador = new NotificadorEmail();
// Ou ainda implementar uma nova forma de notificação no futuro (ex: sms) sem alterar a classe ProcessadorPedido
INotificador notificador = new NotificadorEmail();

var geradorDeRecibo = new GeradorDeRecibo();
var processador = new ProcessadorDePedido(notificador, geradorDeRecibo);

var pedido = new Pedido
{
    Cliente = "Tio Bob",
    Valor = 100.00m
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

// ============================================================================
// PRINCÍPIO SRP - Entidade de Domínio
// Responsabilidade única: regras de validação e estado financeiro do pedido.
// ============================================================================
public class Pedido
{
    public required string Cliente { get; set; }
    public decimal Valor { get; set; }

    public bool EhValido()
    {
        return Valor > 0 && !string.IsNullOrWhiteSpace(Cliente);
    }

    public decimal AplicarTaxaEntrega()
    {
        if (Valor <= 199.99m)
        {
            Valor += 19.99m;
        }

        return Valor;
    }
}

// ============================================================================
// PRINCÍPIO SRP - Serviço de Formatação
// Responsabilidade única: formatação de comprovantes/recibos textuais.
// ============================================================================
public class GeradorDeRecibo
{
    public string Gerar(Pedido pedido)
    {
        return $"RECIBO-{pedido.Cliente.ToUpper()}-{pedido.Valor:F2}";
    }
}
// ============================================================================
// PRINCÍPIO DIP - Abstração
// ============================================================================
public interface INotificador
{
    void EnviarMensagem(string destinatario, string mensagem);
}

// ============================================================================
// PRINCÍPIO DIP - Implementações Concretas
// Demonstram a substituição de canais sem alterar o ProcessadorDePedido.
// ============================================================================
public class NotificadorConsole : INotificador
{
    public void EnviarMensagem(string destinatario, string mensagem)
    {
        Console.WriteLine($"[Console] Para {destinatario}: {mensagem}");
    }
}

public class NotificadorEmail : INotificador
{
    public void EnviarMensagem(string destinatario, string mensagem)
    {
        Console.WriteLine($"[E-mail Enviado] Para: {destinatario.ToLower().Replace(" ", ".")}@solid.com | Conteúdo: {mensagem}");
    }
}

// ============================================================================
// SRP + DIP: Orquestrador
// Depende apenas da interface INotificador e delega tarefas específicas.
// ============================================================================
public class ProcessadorDePedido
{
    private readonly INotificador _notificador;
    private readonly GeradorDeRecibo _geradorDeRecibo;

    public ProcessadorDePedido(INotificador notificador, GeradorDeRecibo geradorDeRecibo)
    {
        _notificador = notificador;
        _geradorDeRecibo = geradorDeRecibo;
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

        // Delegação para a classe especialista em recibos (SRP)
        string recibo = _geradorDeRecibo.Gerar(pedido);

        _notificador.EnviarMensagem(
            pedido.Cliente,
            $"Pedido processado com sucesso! Protocolo: {recibo} | Total: R$ {pedido.Valor:F2}"
        );

        return new ResultadoProcessamento
        {
            Sucesso = true,
            Mensagem = "Pedido processado com êxito."
        };
    }
}
