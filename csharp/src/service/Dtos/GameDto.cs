namespace service.dtos;

public class GameDto
{
    public Guid Id { get; set; }
    public string Word { get; set; }
    public string MaskedWord { get; set; }
    public List<string> Guesses { get; set; }
    public int GuessesRemaining { get; set; }
    public string Status { get; set; }
}