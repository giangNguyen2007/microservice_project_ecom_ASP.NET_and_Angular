namespace GateWay.API.Config;

public class GatewayConfig
{
    public String Host { get; set; }
    public int Port { get; set; }
    public string Url => $"http://{Host}:{Port}";
}