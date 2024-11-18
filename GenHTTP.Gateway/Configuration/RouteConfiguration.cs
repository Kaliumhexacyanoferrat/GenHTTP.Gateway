namespace GenHTTP.Gateway.Configuration;

public class RouteConfiguration
{

    public Dictionary<string, RouteConfiguration>? Routes { get; set; }

    public string? Destination { get; set; }

    public string? Listing { get; set; }
        
    public string? Location { get; set; }

    public ContentConfiguration? Content { get; set; }

}