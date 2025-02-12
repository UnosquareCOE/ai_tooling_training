namespace Game.Services.Dto
{
    public class GameDto
    {
        public Guid GameId { get; set; }
        public string Word { get; set; }
        public string UnmaskedWord { get; set; }
        public int RemainingGuesses { get; set; }
        public string Status { get; set; }
        public List<string> IncorrectGuesses { get; set; }
    }
}