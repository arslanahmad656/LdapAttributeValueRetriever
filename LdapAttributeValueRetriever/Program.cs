using LdapAttributeValueRetriever.Worker;
using LdapAttributeValueRetriever.Worker.Models;
using Microsoft.Extensions.Configuration;

try
{
    var config = GetConfiguration();
    var connectionInfo = GetConnection(config);
    var request = GetRequest(config);

    using var retriever = new Retriever(connectionInfo, request);
    var res = retriever.GetValues(out var success);

    Console.WriteLine($"Total {res.Count} values.");
    Console.WriteLine(string.Join(Environment.NewLine, res));
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
	throw;
}


static IConfiguration GetConfiguration()
    => new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

static ConnectionInfo GetConnection(IConfiguration configuration)
    => configuration.GetSection("LdapConnectionInfo").Get<ConnectionInfo>() ?? throw new Exception("Could not bind the settings.");

static Request GetRequest(IConfiguration configuration)
    => configuration.GetSection("RequestParams").Get<Request>() ?? throw new Exception("Could not bind the settings.");