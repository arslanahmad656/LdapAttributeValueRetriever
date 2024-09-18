namespace LdapAttributeValueRetriever.Worker.Models;

public record ConnectionInfo(string Server, string Username, string Password, AuthenticationType AuthenticationType);