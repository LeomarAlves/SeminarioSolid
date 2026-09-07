var notificador = new NotificadorConsole();
var processador = new ProcessadorDePedido(notificador);

var pedido = new Pedido
{
    Cliente = "Tio Bob",
    Valor = 350.90m
};

bool sucesso = processador.Processar(pedido);

public class Pedido
{
    public required string Cliente {get; set; }
    public decimal Valor { get; set; }

    public bool EhValido()
    {
        return Valor > 0 && !string.IsNullOrWhiteSpace(Cliente);
    }
}

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

public class ProcessadorDePedido
{
    private readonly INotificador _notificador;

    public ProcessadorDePedido(INotificador notificador)
    {
        _notificador = notificador;
    }

    public bool Processar(Pedido pedido)
    {
        if(!pedido.EhValido())
        {
            return false;
        }

        _notificador.EnviarMensagem(pedido.Cliente, "Seu pedido foi processado com sucesso!");
        return true;
    }
}
