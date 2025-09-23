namespace Payment.API.Config;

public class ApiConfig
{
    
    public string BankAppHost { get; set; }
    public int BankAppPort { get; set; }
    public string BankAppUrl => $"http://{BankAppHost}:{BankAppPort}";
    
    public string GatewayHost { get; set; }
    public int GatewayPort { get; set; }
    
    public string GatewayUrl => $"http://{GatewayHost}:{GatewayPort}";
    
    
}