namespace LdapAttributeValueRetriever.Worker.Models;

public record Request(string Filter, string AttributeName, string BaseDN, AttributeSearchScope SearchScope);