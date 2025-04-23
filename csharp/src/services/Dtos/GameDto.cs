using System.Text.Json.Serialization;
using services.Constants;

namespace services.Dtos;

public class GameDto
{
    public int RemainingGuesses { get; set; }
    public string? Word { get; set; }

    [JsonIgnore] public string? UnmaskedWord { get; set; }

    public List<string> IncorrectGuesses { get; set; } = [];
    public required string Status { get; set; }
}