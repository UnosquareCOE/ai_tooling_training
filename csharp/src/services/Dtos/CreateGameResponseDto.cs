namespace services.Dtos;

public class CreateGameResponseDto
{
    public Guid GameId { get; set; }
    public string? MaskedWord { get; set; }
    public int AttemptsRemaining { get; set; }
}