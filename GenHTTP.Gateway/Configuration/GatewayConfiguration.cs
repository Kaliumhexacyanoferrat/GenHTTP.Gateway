namespace GenHTTP.Gateway.Configuration;

public class GatewayConfiguration
{

    public string? Engine { get; set; }

    public PortConfiguration? Ports { get; set; }

    public List<string>? Protocols { get; set; }

    public Dictionary<string, HostConfiguration>? Hosts { get; set; }
        
}
