using api.Constants;

namespace api.ViewModels;
    
    public class MakeGuessResponseViewModel
    {
        public string? MaskedWord { get; set; }
        public int AttemptsRemaining { get; set; }
        public List<string> Guesses { get; set; } = [];
        public string Status { get; set; }
    }