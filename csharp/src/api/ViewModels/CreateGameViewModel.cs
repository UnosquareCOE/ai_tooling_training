namespace api.ViewModels
{
    public class CreateGameViewModel
    {
        public Guid GameId { get; set; }
        public string MaskedWord { get; set; }
        public int AttemptsRemaining { get; set; }
    }
}