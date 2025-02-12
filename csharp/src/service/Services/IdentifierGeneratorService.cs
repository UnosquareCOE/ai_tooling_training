using service.interfaces;

namespace service.services;

public class IdentifierGeneratorService : IIdentifierGenerator
{
    public Guid RetrieveIdentifier()
    {
        return Guid.NewGuid();
    }
}