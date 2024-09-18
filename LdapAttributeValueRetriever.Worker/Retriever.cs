using LdapAttributeValueRetriever.Worker.Models;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace LdapAttributeValueRetriever.Worker;

public class Retriever : IDisposable
{
    private readonly LdapConnection connection;
    private readonly ConnectionInfo connectionInfo;
    private readonly Request requestInfo;

    public Retriever(ConnectionInfo connectionInfo, Request requestInfo)
    {
        this.connectionInfo = connectionInfo;
        this.requestInfo = requestInfo;
        
        var identifier = new LdapDirectoryIdentifier(connectionInfo.Server);
        var auth = GetAuth();
        var connection = new LdapConnection(identifier, auth.Credentials, auth.AuthType);
        connection.Bind();  // test it!

        this.connection = connection;
    }

    public List<string> GetValues(out bool success)
    {
        success = false;
        var values = new List<string>();
        var request = new SearchRequest(requestInfo.BaseDN, requestInfo.Filter, (SearchScope)requestInfo.SearchScope, requestInfo.AttributeName);

        var response = (SearchResponse)connection.SendRequest(request);
        if (response.Entries.Count == 0)
        {
            return values;
        }

        var entry = response.Entries[0];
        if (!entry.Attributes.Contains(requestInfo.AttributeName))
        {
            return values;
        }

        foreach (DirectoryAttribute attribute in entry.Attributes.Values)
        {
            foreach (object value in attribute)
            {
                if (value is byte[] bytes)
                {
                    values.Add(Encoding.ASCII.GetString(bytes));
                }
                else
                {
                    values.Add(value.ToString() ?? string.Empty);
                }
            }
        }

        success = true;
        return values;
    }

    public void Dispose() => this.connection.Dispose();

    private (AuthType AuthType, NetworkCredential? Credentials) GetAuth()
        => connectionInfo.AuthenticationType switch
        {
            AuthenticationType.Anonymous => (AuthType.Anonymous, null),
            AuthenticationType.Negotiate => (AuthType.Negotiate, null),
            AuthenticationType.Basic => (AuthType.Basic, new NetworkCredential(connectionInfo.Username, connectionInfo.Password)),
            _ => throw new NotSupportedException("Authentication type not supported."),
        };
}
