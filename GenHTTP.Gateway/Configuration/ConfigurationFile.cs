using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenHTTP.Gateway.Configuration
{

    public static class ConfigurationFile
    {

        public static GatewayConfiguration Load(string file)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                        .Build();

            return deserializer.Deserialize<GatewayConfiguration>(File.ReadAllText(file));
        }

    }

}
