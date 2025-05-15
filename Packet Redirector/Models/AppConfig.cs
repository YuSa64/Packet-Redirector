namespace PacketRedirector.Models
{
    public class AppConfig
    {
        public string ReceiveProtocol { get; set; } = "UDP";
        public string ReceiveIP { get; set; } = "127.0.0.1";
        public int ReceivePort { get; set; } = 39539;

        public string SendProtocol { get; set; } = "UDP";
        public string SendIP { get; set; } = "127.0.0.1";
        public int SendPort { get; set; } = 39539;

        public string Language { get; set; } = "en";
    }
}