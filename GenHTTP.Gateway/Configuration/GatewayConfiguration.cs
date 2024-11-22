namespace GenHTTP.Gateway.Configuration;

public class GatewayConfiguration
{

    public bool? EnableQuic { get; set; }

    public Dictionary<string, HostConfiguration>? Hosts { get; set; }
        
}