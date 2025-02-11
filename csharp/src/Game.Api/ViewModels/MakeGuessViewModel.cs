namespace api.ViewModels;

public class MakeGuessViewModel
{
    public string? MaskedWord { get; set; }
    public int AttemptsRemaining { get; set; }
    public List<string> Guesses { get; set; } = new();
    public string? Status { get; set; }
}