namespace Game.Services.Dto
{
    public class MakeGuessDto
    {
        public string MaskedWord { get; set; }
        public int AttemptsRemaining { get; set; }
        public List<string> Guesses { get; set; }
        public string Status { get; set; }
    }
}