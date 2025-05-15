namespace PacketRedirector.Models;

public class RedirectSettings
{
    public string ReceiveProtocol { get; set; } = "UDP";
    public string ReceiveIP { get; set; } = "127.0.0.1";
    public int ReceivePort { get; set; } = 39539;

    public string SendProtocol { get; set; } = "UDP";
    public string TargetIP { get; set; } = "127.0.0.1";
    public int TargetPort { get; set; } = 39539;
}