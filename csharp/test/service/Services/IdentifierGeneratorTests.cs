using System;
using service.services;

namespace service.tests.Services;

public class IdentifierGeneratorTests
{

    [Fact]
    public void GenerateIdentifier_ShouldReturnGUID()
    {
        // Arrange
        var service = new IdentifierGeneratorService();

        // Act
        var identifier = service.RetrieveIdentifier();

        // Assert
        Assert.IsType<Guid>(identifier);
    }


    [Fact]
    public void GenerateIdentifier_ShouldReturnUniqueIdentifier()
    {
        // Arrange
        var service = new IdentifierGeneratorService();

        // Act
        var identifier1 = service.RetrieveIdentifier();
        var identifier2 = service.RetrieveIdentifier();

        // Assert
        Assert.NotEqual(identifier1, identifier2);
    }

}
