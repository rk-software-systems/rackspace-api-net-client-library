namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceRequestException : Exception
{
    public RackspaceRequestException(string message) : base(message)
    {
    }

    public RackspaceRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RackspaceRequestException(int eventId): base($"{eventId}")
    {
    }

    public RackspaceRequestException()
    {
    }
}
