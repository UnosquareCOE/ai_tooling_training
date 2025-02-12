using service.interfaces;

namespace service.services;

public class IdentifierGenerator : IIdentifierGenerator
{
    public Guid RetrieveIdentifier()
    {
        return Guid.NewGuid();
    }
}