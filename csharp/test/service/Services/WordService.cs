using NSubstitute;
using Service.Constants;

namespace service.services;

public class WordServiceTests
{
    [Fact]
    public void RetrieveWord_OfflineMode_ReturnsWordFromList()
    {
        // Arrange
        Environment.SetEnvironmentVariable(EnvironmentVariables.UseOfflineMode, Connectivity.Offline);
        var wordService = new WordService();

        // Act
        var result = wordService.RetrieveWord();

        // Assert
        Assert.Contains(result, new[] { "banana", "canine", "unosquare", "airport" });
    }
}
