public class Pedido
{
    public string Cliente {get; set; }
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
        Console.WriteLine($"[Notificação] Para {destinatario}: {mensagem}")
    }
}

public class ProcessadorDePedido
{
    private readonly INotificador _notificador;

    public ProcessadorDePedido(INotificador notificador)
    {
        -notificador = notificador;
    }
}

