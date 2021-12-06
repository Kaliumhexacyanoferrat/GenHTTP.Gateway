using System.Net.Http;
using System.Threading.Tasks;

namespace GenHTTP.Gateway.Tests
{

    public static class TestExtensions
    {

        public static Task<string> GetContent(this HttpResponseMessage response) => response.Content.ReadAsStringAsync();

    }

}
