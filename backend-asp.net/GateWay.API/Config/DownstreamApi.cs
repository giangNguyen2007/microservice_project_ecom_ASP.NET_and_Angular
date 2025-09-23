namespace GateWay.API.Config;

public class DownstreamApi
{
    public string AuthApiScheme { get; set; }
    public string AuthApiHost { get; set; }
    public int AuthApiPort { get; set; }
    
    public string ProductApiScheme { get; set; }
    public string ProductApiHost { get; set; }
    public int ProductApiPort { get; set; }
    
    public string OrderApiScheme { get; set; }
    public string OrderApiHost { get; set; }
    public int OrderApiPort { get; set; }
    
    public string PaymentApiScheme { get; set; }
    public string PaymentApiHost { get; set; }
    public int PaymentApiPort { get; set; }
    
}