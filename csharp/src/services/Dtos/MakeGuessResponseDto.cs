namespace services.Dtos;
    
    public class MakeGuessResponseDto
    {
        public string? MaskedWord { get; set; }
        public int AttemptsRemaining { get; set; }
        public List<string> Guesses { get; set; } = [];
        public required string Status { get; set; }
    }