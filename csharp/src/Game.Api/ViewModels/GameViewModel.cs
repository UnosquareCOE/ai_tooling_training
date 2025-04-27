using System.Text.Json.Serialization;

namespace api.ViewModels;

public class GameViewModel
{
    public int RemainingGuesses { get; set; }
    public string? Word { get; set; }

    [JsonIgnore] public string? UnmaskedWord { get; set; }

    public List<string> IncorrectGuesses { get; set; } = [];
    public string? Status { get; set; }
}