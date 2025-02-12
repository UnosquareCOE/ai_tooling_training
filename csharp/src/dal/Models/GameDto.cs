namespace dal.Models;

public class Game
{
    public Guid Id { get; set; }
    public int RemainingGuesses { get; set; }
    public string? Word { get; set; }
    public string? UnmaskedWord { get; set; }
    public List<string> IncorrectGuesses { get; set; } = [];
    public required string Status { get; set; }
}