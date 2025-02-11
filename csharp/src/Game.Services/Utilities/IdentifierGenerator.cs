using Game.Services.Interfaces;

namespace Game.Services.Utilities;
public class IdentifierGenerator : IIdentifierGenerator
{
    public Guid RetrieveIdentifier()
    {
        return Guid.NewGuid();
    }
}