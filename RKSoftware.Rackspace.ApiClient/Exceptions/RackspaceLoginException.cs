namespace RKSoftware.Rackspace.ApiClient;

public class RackspaceLoginException : Exception
{
    public RackspaceLoginException(string message) : base(message)
    {
    }

    public RackspaceLoginException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RackspaceLoginException(int eventId) : base($"{eventId}")
    {
    }

    public RackspaceLoginException()
    {
    }
}
