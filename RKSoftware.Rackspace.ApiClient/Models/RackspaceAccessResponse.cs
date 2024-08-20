namespace RKSoftware.Rackspace.ApiClient;

internal sealed class RackspaceAccessResponse
{
    public Access? Access { get; set; }
}

internal sealed class Access
{
    public Token? Token { get; set; }

    public List<ServiceCatalog>? ServiceCatalog { get; set; }

    public User? User { get; set; }
}

internal sealed class Token
{
    public DateTime? Expires { get; set; }

    public string? Id { get; set; }

    public Tenant? Tenant { get; set; }
}

internal sealed class Tenant
{
    public string? Id { get; set; }

    public string? Name { get; set; }
}

internal sealed class User
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public List<Role>? Roles { get; set; }
}

internal sealed class Role
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? TenantId { get; set; }
}

internal sealed class ServiceCatalog
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public List<Endpoint>? Endpoints { get; set; }
}

internal sealed class Endpoint
{
    public string? TenantId { get; set; }

    public string? PublicURL { get; set; }

    public required string Region { get; set; }
}